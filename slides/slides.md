---
theme: default
background: https://cover.sli.dev
title: .NET Usergroup Rhein/Ruhr
class: text-center
transition: slide-left
comark: true
duration: 35min
css: style.css
---

# Fehler sind keine Ausnahme

Inspiration aus anderen Sprachen und worauf wir uns als C#-Entwickler freuen können

---
layout: intro
---

# Moritz Freyburger

## Freelancer

- Backend-Developer (C#, TypeScript/JavaScript)
- Cloud Developer (AWS/Azure)
- DevOps Engineer (Terraform, Docker, CI/CD)
- Software-Nerd (Rust, Go, C/C++, F#, Embedded)

Web: https://www.freyburger.io/  
Mail: moritz@freyburger.io  
GitHub: https://github.com/FreyMo/

---
layout: intro
---

# Worum geht's heute?

- Fehlerbehandlung in C# ist suboptimal
  - Exceptions, `try/catch/finally`
  - `TryDo`
  - Nullable return value
- Wie machen andere Sprachen das?
  - C, Go, Rust
  - Spoiler: Fehler als Werte
- Wieso betrifft das C# Entwickler?
  - .NET 11 Preview 3

Slides: https://github.com/FreyMo/meetup-ratingen

<!--
- Exceptions de Facto Standard für Fehlerhandling in .NET
-->

---
layout: default
---

# Eine Zeile Code

```csharp [C#]
// Works just fine
var timestamp = new DateTimeOffset(2025, 2, 28, 0, 0, 0, TimeSpan.Zero);

// Do something with timestamp
```

<div v-click>

````md magic-move [Rust] {at:2}
```rust
let timestamp = chrono::NaiveDate::from_ymd_opt(2025, 2, 28);

// timestamp is **not** a NaiveDate 
```
```rust
match chrono::NaiveDate::from_ymd_opt(2025, 2, 29) {
    Some(timestamp) => {
        // Do something with timestamp
    }
    None => {
        // It turns out that 2025 is not a leap year
    }
};
```
````

</div>

<div v-click="3">

* Fehler sind implizit und versteckt
* Methodensignatur zeigt nicht an, dass Fehler möglich sind

</div>

<!--
- Sehr banales Beispiel -> selbst hier können Fehler passieren
- Bezug auf Exceptions
- Andere Sprachen erzwingen Fehlerbehandlung
-->

---

# Kontrollfluss

````md magic-move [C#]
```csharp
public void DoSomething()
{
    MightThrow();

    // Do something else
}
```
```csharp
public void Caller()
{
    // Do we catch here?

    DoSomething();
}

public void DoSomething()
{
    MightThrow();
}

public void MightThrow()
{
    // This is an implementation detail

    throw new Exception();
}
```
````

<div v-click="2">

* Implementierung von `MightThrow` ist nötig
* Gesamte Implementierung von `Caller` is relevant, nicht nur der Call auf `DoSomething`

</div>

---

## Wo fangen wir Exceptions?

```csharp [C#]
try {
    // your entire program
} catch (Exception e) {
    // ¯\_(ツ)_/¯
}
```

## Was machen wir mit Exceptions?

```csharp [C#]
try {
    // Do something
} catch (Exception e) {
    logger.LogError(e, "...");

    throw;

    // or even worse:
    // throw e;
}
```

---

# Sprache in der Sprache

* C# ist eine komplexe Sprache (mehr als 100 keywords)
  * `try`/`catch`/`finally`/`throw` existieren nur für Exceptions
  * Exceptions sind Fremdkörper, zweites Kontrollflusssystem
  * Fehlerbehandlung "bolted on"
* Jede Exception ist eine Bombe im Code
  * Klingt das zuverlässig?

---

# Noch ein Beispiel

````md magic-move [C#]
```csharp
public async Task<???> GetSomething(long id)
{
    var entity = await dbContext.Entities.FindAsync(id);

    if (entity is null)
    {
        // What do we do?
    }
}
```
```csharp
// Option A: Throw exception
public Task<Entity> GetSomething();

// Option B: Nullable object
public Task<Entity?> GetSomething();

// Option C: Result object
public Task<Result> GetSomething();
public record Result(bool wasSuccessful, Entity? entity);

// Option D: TryDo pattern
// Attention: Will not work for async methods
public Task<bool> TryGetSomething(long id, out Entity entity);
```
````

<div v-click>

* Kein Standard, viele Möglichkeiten (gut & schlecht)
* Fehlerbehandlung in C# ist ein Afterthought 

Fehlerbehandlung ist aufwändig. Fehlerbehandlung ist (fast) immer nötig. Fehler sind teurer, wenn sie später erkannt werden.

</div>

---

# Zusammenfassend

* Signatur versteckt erwartbares Verhalten
* Versteckter Kontrollfluss, Missbrauch von Kontrollfluss
* Fehlerhandling "bolted-on"
* Antipatterns wie log-and-throw, `throw ex;`, Verschlucken von Exceptions
- Performance im Fehlerfall
  - Stack Unwinding
  - handler search
  - stack trace capture

---

# Es ist nicht alles schlecht

- Sehr sauberer Happy-Path
  - Signatur bleibt "clean"
  - Vollständiger Abbruch möglich
- Stack Trace oft hilfreich
- Freiheit, fehler zu behandeln
- `IDisposable` und `finally`-Block

---
layout: cover
---

# Sprachen ohne Exceptions?

---

# C

```c [C]
typedef int ErrorCode;

#define ERR_OK      0
#define ERR_INVALID 1

ErrorCode populate_item(Item* item);

void do_something() {
  Item item;
  ErrorCode error = populate_item(&item);

  if (error == ERR_OK) {
    // item can be used here
  }
}
```

<div v-click>

* Simpel, aber gefährlich (wenig Compilerunterstützung, `int`-Konstanten, uninitialisierte Variablen)
* verbose

</div>

<!-- out -->

---
transition: fade
---

# C

```c [C]
Item* create_item() {
  Item* item = malloc(sizeof(Item));

  item->value = 5;

  return item;
}

void create_something() {
  Item* item = create_item();

  if (item != NULL) {
    // item can be used here
  }
}
```

<div v-click>

* Simpel, aber mindestens genauso gefährlich
* verbose

</div>

---

# Go

```go [Go]
func Open(name string) (file *File, err error)

func CopyFile(dstName, srcName string) (written int64, err error) {
  file, err := os.Open("filename.ext")
  if err != nil {
    return
  }

  // do something with the open *File f
}
```

<div v-click>

- simpel & effizient
* verbose

</div>

---

# Go (`defer`)

````md magic-move [Go]
```go
func CopyFile(dstName, srcName string) (written int64, err error) {
    src, err := os.Open(srcName)
    if err != nil {
        return
    }

    dst, err := os.Create(dstName)
    if err != nil {
        return
    }

    written, err = io.Copy(dst, src)
    dst.Close()
    src.Close()
    return
}
```
```go [Go]
func CopyFile(dstName, srcName string) (written int64, err error) {
    src, err := os.Open(srcName)
    if err != nil {
        return
    }
    defer src.Close()

    dst, err := os.Create(dstName)
    if err != nil {
        return
    }
    defer dst.Close()

    return io.Copy(dst, src)
}
```
````

<div v-click="2">

* Syntax vereinfacht Verhalten im Fehler
* Ähnlich `using`/`IDisposable`/`finally` in .NET

[Go docs](https://go.dev/blog/defer-panic-and-recover)

</div>

---
layout: center
---

# Rust

* kein `throw`/`try`/`catch`/`finally`
* kein `defer`/`using`
* kein GC

---
layout: fact
---

# Algebraic Data Types

---
transition: fade
layout: two-cols-header
---

# Verbundtypen

::left::

<div v-click>

## Produkttypen

- kombiniert alle Eigenschaften seiner Typen
- Menge aller Werte ist das **kartesische Produkt** der Menge der Komponenten
- Beispiele:
  - Tuple, Klassen, Structs, Records

<div style="position: absolute; bottom: 1.5rem; width: calc(50% - 5rem);">

```csharp [C#]
public record MyRecord
{
  public Fuzzy Fuzzy { get; init; } // 3    *
  public int X { get; init; }       // 2^32 *
  public int Y { get; init; }       // 2^32
}
```

</div>

</div>

::right::

<div v-click="2">

## Summentypen

* Auswahl einer seiner Eigenschaften
- Menge aller Werte ist die **Summe** der Menge der Komponenten
* vollständig zählbar (vs. Inheritance)
* Beispiele:
  - Enum (primitiv, nicht vollständig)

<div style="position: absolute; bottom: 1.5rem; width: calc(50% - 4rem);">

```csharp [C#]
public enum Fuzzy
{
  True,   // 1 +
  False,  // 1 +
  Maybe   // 1
}
```

</div>

</div>

---

# Rust: Enums on steroids

```rust [Rust]
pub enum Option<T> {
  Some(T), // Menge von T +
  None     // 1
}

pub enum Result<T, E> {
  Ok(T),   // Menge von T +
  Err(E)   // Menge von E
}
```

* Jeder Wert kann wiederum ein Summen- oder Produkttyp sein
* Compiler prüft, dass alle Werte berücksichtigt wurden
* Errors as values, Fehler Teil des Typsystems

---

# Rust

```rust [Rust]
let timestamp = chrono::NaiveDate::from_ymd_opt(2025, 2, 29);

// Provides a default value in case there is no value
let _ = timestamp.or(Some(chrono::NaiveDate::MAX));
let _ = timestamp.or_else(|| Some(chrono::NaiveDate::MAX));

// Maps the NaiveDate to another one, but only if the Option is Some
let _ = timestamp.and_then(|x| x.and_hms_opt(1, 2, 3));

// Treats an Option as an iterator with length 0 or 1
let _ = timestamp.filter(|x| x.leap_year());

// Evil
let _ = timestamp.unwrap();
```

* `impl` Block für `enum`s erlaubt Convenience-Funktionen
* `panic` für nicht auffangbare Fehler

---

````md magic-move [Rust]
```rust [Rust]
pub fn might_be_none() -> Option<String> {
    let timestamp = chrono::NaiveDate::from_ymd_opt(2025, 2, 29);

    match timestamp {
        None => None,
        Some(val) => {
            // ...
            Some(val.to_string())
        }
    }
}

pub fn might_fail() -> Result<String, chrono::ParseError> {
    let timestamp = chrono::NaiveDate::parse_from_str("2025-02-29", "%Y-%m-%d");

    match timestamp {
        Err(error) => Err(error),
        Ok(val) => {
            // ...
            Ok(val.to_string())
        }
    }
}
```
```rust [Rust]
pub fn might_be_none() -> Option<String> {
    // Returns early with None if timestamp is None
    let timestamp = chrono::NaiveDate::from_ymd_opt(2025, 2, 29)?;

    Some(timestamp.to_string())
}

pub fn might_fail() -> Result<String, chrono::ParseError> {
    // Returns early with Error if timestamp is None
    let timestamp = chrono::NaiveDate::parse_from_str("2025-02-29", "%Y-%m-%d")?;

    Ok(timestamp.to_string())
}
```
````

<div v-click="2">

* `?`-Operator als Syntactic Sugar
* Erlaubt early returns
* `impl From<T>` erlaubt Mapping von Fehlertypen (`anyhow`, `thiserror`)

</div>

---

# Rust

* Werte per Default auf dem Stack
  * Beim Verlassen des Scopes wird aufgeräumt
  * Speicher auf dem Heap wird per `Drop` trait gelöscht
  * GC nicht nötig
* `async/await` ist auch mit enums gelöst: `std::Future` ist ein Trait, der das `Poll` enum zurückgibt: 

```rust
pub enum Poll<T> {
    Ready(T),
    Pending
}
```

---
layout: fact
---

# Und C#?

---
layout: center
---

# Union Types

* .NET 11 Preview 3
* Vsl. C# 15 (November 2026)

---

# Union Types

```csharp [C#]
public union Pet(Cat, Dog);

public record class Cat(string Name);
public record class Dog(string Name);

Pet pet = new Cat("Whiskers");

Console.WriteLine(pet switch
{
    Cat c => $"Cat: {c.Name}",
    Dog d => $"Dog: {d.Name}",
});
```

---

# Union Types

* Composition over Inheritance
* Pattern Matching
* `struct` Type, kein `null` check
  * Custom: Kann auch als `class` implementiert werden
* `Value` ist ein `object?`
  * Custom: Kann als `object` implementiert werden. Boxing kann verhindert werden
* Methoden möglich (Achtung Namenskonflikt)
* Keine 'leeren' Einträge, instance object oder enum value/placeholder
  * Performance wird nicht so gut sein wie `int`, `TryDo` oder Rust enums
* Exhaustive. Andere [`closed`](https://devblogs.microsoft.com/dotnet/csharp-15-union-types/#related-proposals) types geplant: `enum`s und `class`/`struct`/`record`s

---

# Union Types

* Nicht nur für Fehler (geschlossenes Set)
* Unerwarteter Fehler: Exception
* Erwarteter Fehler: TryX, nullable, DU
* 3rd Party Alternativen gerade: [OneOf](https://github.com/mcintyre321/OneOf), [Optional](https://github.com/nlkl/Optional), [dotNext](https://github.com/dotnet/dotNext)
* F# als Alternative
  * Kompatibel mit C#
  * `Option`/`Result` out of the box
  * Discriminated Unions

```fsharp
type Shape =
    | Rectangle of width : float * length : float
    | Circle of radius : float
    | Prism of width : float * float * height : float
```

---

# Fazit

* Exceptions mit Vorsicht nutzen
* Alternative Fehlerbehandlung in C# nutzen
* Viele moderne Sprachen setzen auf Errors as Values
* Wird es Zeit mal andere Sprachen zu verwenden? Go, Rust, F#, OCaml?
* C# bekommt bald Union Types

---
layout: center
class: text-center
---

# Vielen Dank

Slides: https://github.com/FreyMo/meetup-ratingen
