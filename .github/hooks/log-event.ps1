# log-event.ps1 — Appends agent lifecycle events to agent_log.txt for auditing/debugging.
# Invoked by agent_hook.json on UserPromptSubmit and PreToolUse events.
# Receives a JSON payload from the agent on stdin.

param(
    [string]$EventName = "Unknown"
)

$logFile = Join-Path $PSScriptRoot "agent_log.txt"

# Read and parse the JSON payload
$rawInput = [Console]::In.ReadToEnd()
$detail = ""
if (-not [string]::IsNullOrWhiteSpace($rawInput)) {
    try {
        $parsed = $rawInput | ConvertFrom-Json
        if ($EventName -eq "UserPromptSubmit") {
            $detail = $parsed.prompt
        } elseif ($EventName -eq "PreToolUse") {
            $detail = $parsed.tool_name
        }
    } catch { }
}

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$entry = "$timestamp  [$EventName]  $detail"

Add-Content -Path $logFile -Value $entry

# Exit 0 = allow the agent to continue normally
exit 0
