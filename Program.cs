using ASPNET.Classes;

// ── Helper ──────────────────────────────────────────────────────────────────
static TodoItem MakeTodo(string title, bool completed = false) => new TodoItem
{
    Title = title,
    IsCompleted = completed,
    DueDate = DateTime.Now.AddDays(new Random().Next(1, 30))
};

// ── Class 1: Matematika ──────────────────────────────────────────────────────
var math = new Class
{
    Name = "Matematika",
    Description = "Analiza i linearna algebra",
    ColorHex = "#4A90D9",
    IsUrgent = false
};
math.SetTag("egzakt", Priority.Medium);

var l1 = math.AddLesson("Derivacije", "Osnove diferencijskog racuna");
l1.TodoItems.AddRange(new[] {
    MakeTodo("Procitati poglavlje o derivacijama", true),
    MakeTodo("Rijesiti zadatke 1-10"),
    MakeTodo("Napraviti sazetak pravila deriviranja")
});

var l2 = math.AddLesson("Integrali", "Odredeni i neodredeni integral");
l2.TodoItems.AddRange(new[] {
    MakeTodo("Pogledati video predavanje", true),
    MakeTodo("Vjezbati metodu supstitucije", true),
    MakeTodo("Izraditi tablicu integracijskih formula")
});

var l3 = math.AddLesson("Matrice", "Operacije s matricama");
l3.TodoItems.AddRange(new[] {
    MakeTodo("Nauciti mnozenje matrica"),
    MakeTodo("Rijesiti sustav jednadzbi Gaussovom metodom"),
    MakeTodo("Ponoviti vlastite vrijednosti")
});

// ── Class 2: Programiranje ───────────────────────────────────────────────────
var prog = new Class
{
    Name = "Programiranje",
    Description = "C# i .NET ekosustav",
    ColorHex = "#7B68EE",
    IsUrgent = true
};
prog.SetTag("dev", Priority.High);

var l4 = prog.AddLesson("LINQ", "Upiti nad kolekcijama");
l4.TodoItems.AddRange(new[] {
    MakeTodo("Prouciti Where, Select, GroupBy", true),
    MakeTodo("Napisati primjere LINQ upita"),
    MakeTodo("Usporediti LINQ i SQL sintaksu", true)
});

var l5 = prog.AddLesson("Asinkrono programiranje", "async/await i Task");
l5.TodoItems.AddRange(new[] {
    MakeTodo("Razumjeti razliku Task vs Thread"),
    MakeTodo("Napraviti async HTTP poziv"),
    MakeTodo("Obraditi iznimke u async metodi")
});

var l6 = prog.AddLesson("Entity Framework", "ORM i migracije");
l6.TodoItems.AddRange(new[] {
    MakeTodo("Postaviti DbContext", true),
    MakeTodo("Kreirati prvu migraciju", true),
    MakeTodo("Implementirati repozitorij obrazac", true)
});

// ── Class 3: Engleski jezik ──────────────────────────────────────────────────
var eng = new Class
{
    Name = "Engleski jezik",
    Description = "Akademski engleski i konverzacija",
    ColorHex = "#50C878",
    IsUrgent = false
};
eng.SetTag("jezik", Priority.Low);

var l7 = eng.AddLesson("Gramatika", "Vremena i kondicionali");
l7.TodoItems.AddRange(new[] {
    MakeTodo("Ponoviti present perfect", true),
    MakeTodo("Vjezbati conditional sentences"),
    MakeTodo("Napraviti kviz iz gramatike")
});

var l8 = eng.AddLesson("Vokabular", "Akademske i strucne rijeci");
l8.TodoItems.AddRange(new[] {
    MakeTodo("Nauciti 20 novih rijeci", true),
    MakeTodo("Napisati recenice s novim rijecima", true),
    MakeTodo("Flashcard ponavljanje")
});

var l9 = eng.AddLesson("Pisanje eseja", "Struktura i argumentacija");
l9.TodoItems.AddRange(new[] {
    MakeTodo("Procitati primjere eseja"),
    MakeTodo("Napisati uvodni odlomak"),
    MakeTodo("Dobiti povratnu informaciju od profesora")
});

// ── Ispis ────────────────────────────────────────────────────────────────────
var classes = new[] { math, prog, eng };
const string line = "══════════════════════════════════════════";

foreach (var c in classes)
{
    var urgentLabel = c.IsUrgent ? "  [!] HITNO" : "";
    var tagInfo = c.Tag is not null
        ? $"{c.Tag.Name}  ({c.Tag.Priority}  {c.Tag.Priority?.ToColorHex()})"
        : "-";

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"\n╔{line}");
    Console.WriteLine($"║  {c.Name}{urgentLabel}");
    Console.ResetColor();
    Console.WriteLine($"║  Opis   : {c.Description}");
    Console.WriteLine($"║  Boja   : {c.ColorHex}");
    Console.WriteLine($"║  Tag    : {tagInfo}");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"╚{line}");
    Console.ResetColor();

    foreach (var lesson in c.Lessons)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  ┌─ Lekcija {lesson.OrderIndex + 1}: {lesson.Title}");
        Console.ResetColor();
        Console.WriteLine($"  │  Opis     : {lesson.Description}");
        Console.WriteLine($"  │  Napredak : {lesson.CompletionPercentage}%");
        Console.WriteLine($"  │  Zadaci   :");

        foreach (var todo in lesson.TodoItems)
        {
            Console.ForegroundColor = todo.IsCompleted ? ConsoleColor.Green : ConsoleColor.White;
            Console.WriteLine($"  │    {todo}");
            Console.ResetColor();
        }

        Console.WriteLine($"  └{'─'.ToString().PadRight(42, '─')}");
    }
}

// ── LINQ: Hitne klase ────────────────────────────────────────────────────────
var urgentClasses = classes.Where(c => c.IsUrgent).ToList();

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine($"\n╔{line}");
Console.WriteLine($"║  HITNE KLASE  ({urgentClasses.Count})");
Console.WriteLine($"╚{line}");
Console.ResetColor();

if (urgentClasses.Count == 0)
{
    Console.WriteLine("  Nema hitnih klasa.");
}
else
{
    foreach (var c in urgentClasses)
        Console.WriteLine($"  • {c.Name}");
}

// ── LINQ: Zakasnjeli zadaci ──────────────────────────────────────────────────
var overdueTodos = classes
    .SelectMany(c => c.Lessons.Select(l => new { Class = c, Lesson = l }))
    .SelectMany(x => x.Lesson.TodoItems.Select(t => new { x.Class, x.Lesson, Todo = t }))
    .Where(x => !x.Todo.IsCompleted && x.Todo.DueDate.HasValue && x.Todo.DueDate.Value < DateTime.Now)
    .OrderBy(x => x.Todo.DueDate)
    .ToList();

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine($"\n╔{line}");
Console.WriteLine($"║  ZAKASNJELI ZADACI  ({overdueTodos.Count})");
Console.WriteLine($"╚{line}");
Console.ResetColor();

if (overdueTodos.Count == 0)
{
    Console.WriteLine("  Nema zakasnjelih zadataka.");
}
else
{
    foreach (var x in overdueTodos)
        Console.WriteLine($"  • [{x.Class.Name} › {x.Lesson.Title}]  {x.Todo.Title}  (rok: {x.Todo.DueDate:dd.MM.yyyy})");
}

// ── LINQ: Top lekcije po broju neobavljenih zadataka ─────────────────────────
var topLessons = classes
    .SelectMany(c => c.Lessons.Select(l => new
    {
        ClassName  = c.Name,
        LessonName = l.Title,
        Pending    = l.TodoItems.Count(t => !t.IsCompleted)
    }))
    .Where(x => x.Pending > 0)
    .OrderByDescending(x => x.Pending)
    .Take(5)
    .ToList();

Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine($"\n╔{line}");
Console.WriteLine($"║  TOP LEKCIJE PO BROJU ZADATAKA  ({topLessons.Count})");
Console.WriteLine($"╚{line}");
Console.ResetColor();

if (topLessons.Count == 0)
{
    Console.WriteLine("  Sve lekcije su dovrsene.");
}
else
{
    foreach (var x in topLessons)
        Console.WriteLine($"  • {x.Pending,2} zadataka  [{x.ClassName} › {x.LessonName}]");
}

Console.WriteLine();
