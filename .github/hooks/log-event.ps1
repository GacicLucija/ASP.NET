# log-event.ps1 — Appends agent lifecycle events to agent_log.txt for auditing/debugging.
# Invoked by agent_hook.json on UserPromptSubmit, PreToolUse, and PostToolUse events.
# Receives a JSON payload from the agent on stdin.

param(
    [string]$EventName = "Unknown"
)

# Use $PSScriptRoot when available, fall back to the known absolute path
$hooksDir = if ($PSScriptRoot) { $PSScriptRoot } else { "C:\git\ASP.NET\.github\hooks" }
$logFile      = Join-Path $hooksDir "agent_log.txt"
$errorLogFile = Join-Path $hooksDir "agent_error_log.txt"

# Truncate a string to $max chars for readable log lines
function Truncate([string]$s, [int]$max = 300) {
    if (-not $s) { return "" }
    if ($s.Length -le $max) { return $s }
    return $s.Substring(0, $max) + " ...[truncated]"
}

# Extract the tool name from multiple possible property layouts
function Get-ToolName($parsed) {
    if ($parsed.PSObject.Properties['tool_name']) { return $parsed.tool_name }
    if ($parsed.PSObject.Properties['toolName'])  { return $parsed.toolName  }
    if ($parsed.PSObject.Properties['name'])       { return $parsed.name      }
    return $null
}

# Extract tool_input from multiple possible property layouts
function Get-ToolInput($parsed) {
    if ($parsed.PSObject.Properties['tool_input']) { return $parsed.tool_input }
    if ($parsed.PSObject.Properties['toolInput'])  { return $parsed.toolInput  }
    if ($parsed.PSObject.Properties['input'])      { return $parsed.input      }
    return $null
}

# Build a multi-line change summary for file-editing tools
function Get-ChangeDetail([string]$toolName, $toolInput) {
    if (-not $toolInput) { return "" }
    $lines = @()

    switch -Regex ($toolName) {
        "^create_file$" {
            $fp      = if ($toolInput.PSObject.Properties['file_path']) { $toolInput.file_path } else { $toolInput.filePath }
            $content = $toolInput.content
            if ($fp)      { $lines += "    File:    $fp" }
            if ($content) { $lines += "    Content: $(Truncate $content 400)" }
        }

        "^replace_string_in_file$" {
            $fp  = if ($toolInput.PSObject.Properties['file_path']) { $toolInput.file_path } else { $toolInput.filePath }
            $old = if ($toolInput.PSObject.Properties['old_string']) { $toolInput.old_string } else { $toolInput.oldString }
            $new = if ($toolInput.PSObject.Properties['new_string']) { $toolInput.new_string } else { $toolInput.newString }
            if ($fp)  { $lines += "    File:    $fp" }
            if ($old) { $lines += "    Before:  $(Truncate $old 250)" }
            if ($new) { $lines += "    After:   $(Truncate $new 250)" }
        }

        "^multi_replace_string_in_file$" {
            $replacements = $toolInput.replacements
            if ($replacements) {
                $i = 1
                foreach ($r in $replacements) {
                    $fp  = if ($r.PSObject.Properties['file_path']) { $r.file_path } else { $r.filePath }
                    $old = if ($r.PSObject.Properties['old_string']) { $r.old_string } else { $r.oldString }
                    $new = if ($r.PSObject.Properties['new_string']) { $r.new_string } else { $r.newString }
                    $lines += "    [$i] File:   $fp"
                    if ($old) { $lines += "        Before: $(Truncate $old 150)" }
                    if ($new) { $lines += "        After:  $(Truncate $new 150)" }
                    $i++
                }
            }
        }

        "^apply_patch$" {
            # try common property names for the diff/patch content
            $patch = $null
            foreach ($prop in @('patch','diff','content','input')) {
                if ($toolInput.PSObject.Properties[$prop]) { $patch = $toolInput.$prop; break }
            }
            if ($patch) { $lines += "    Patch:   $(Truncate $patch 600)" }
        }

        "^edit_notebook_file$" {
            $fp = if ($toolInput.PSObject.Properties['file_path']) { $toolInput.file_path } else { $toolInput.filePath }
            if ($fp) { $lines += "    File:    $fp" }
        }
    }

    if ($lines.Count -gt 0) { return "`n" + ($lines -join "`n") }
    return ""
}

try {
    $rawInput = [Console]::In.ReadToEnd()
    $detail   = ""

    if (-not [string]::IsNullOrWhiteSpace($rawInput)) {
        try {
            $parsed = $rawInput | ConvertFrom-Json

            if ($EventName -eq "UserPromptSubmit") {
                if ($parsed.PSObject.Properties['prompt'])  { $detail = $parsed.prompt  }
                elseif ($parsed.PSObject.Properties['message']) { $detail = $parsed.message }
                elseif ($parsed.PSObject.Properties['text'])    { $detail = $parsed.text    }
                else { $detail = "Prompt received (properties: $($parsed.PSObject.Properties.Name -join ', '))" }
            }
            elseif ($EventName -eq "PreToolUse") {
                $toolName = Get-ToolName $parsed
                if ($toolName) {
                    $toolInput    = Get-ToolInput $parsed
                    $changeDetail = Get-ChangeDetail $toolName $toolInput
                    $detail       = $toolName + $changeDetail
                } else {
                    $detail = "Tool invoked (properties: $($parsed.PSObject.Properties.Name -join ', '))"
                }
            }
            elseif ($EventName -eq "PostToolUse") {
                $toolName = Get-ToolName $parsed
                $outcome  = $null
                foreach ($prop in @('success','status','result','error')) {
                    if ($parsed.PSObject.Properties[$prop]) { $outcome = $parsed.$prop; break }
                }
                $outcomeStr = if ($null -ne $outcome) { " => $outcome" } else { "" }
                $detail = if ($toolName) { "$toolName$outcomeStr [completed]" } else { "tool completed$outcomeStr" }
            }
            else {
                $detail = "Event received"
            }
        } catch {
            $detail = "Error parsing JSON: $_"
            $errorTimestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
            $errorMsg = "$errorTimestamp  [$EventName]  Parse error: $_`nRaw input (first 500 chars): $($rawInput.Substring(0, [Math]::Min(500, $rawInput.Length)))"
            Add-Content -Path $errorLogFile -Value $errorMsg -ErrorAction SilentlyContinue
        }
    } else {
        $detail = "No input received"
    }

    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $entry     = "$timestamp  [$EventName]  $detail"

    Add-Content -Path $logFile -Value $entry -ErrorAction Stop

} catch {
    # Last-resort error logging — never let the hook crash the agent
    $errorTimestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $errorMsg       = "$errorTimestamp  [$EventName]  Script error: $_"
    Add-Content -Path $errorLogFile -Value $errorMsg -ErrorAction SilentlyContinue
}

# Exit 0 = allow the agent to continue normally
exit 0
