# CLAUDE.md

Guidance for AI assistants working in this repository.

## Overview

A small .NET console application that computes all prime numbers from 2 up to a
user-supplied number `N`. The sieve work is parallelized across cores, execution
time is measured and printed, and the user can optionally print the discovered
primes to the console. The program runs an interactive read-eval loop until the
user enters `-1` to exit.

## Layout

```
.
├── .gitignore                                  # Standard Visual Studio / .NET ignore rules
└── ComputePrimeNumbers/
    ├── ComputePrimeNumbers.sln                 # Solution file (open this in Visual Studio)
    └── ComputePrimeNumbers/
        ├── ComputePrimeNumbers.csproj          # Project file — Exe, targets netcoreapp3.1
        ├── Program.cs                          # Entry point + interactive console loop
        └── PrimeNumberGenerator.cs             # Prime computation + logging logic
```

The solution name, project name, and root namespace are all `ComputePrimeNumbers`.

## Key components

- **`Program`** (`Program.cs`) — Entry point. Holds a single static
  `PrimeNumberGenerator` instance. `Main` runs an input loop: prompts for a
  natural number `N`, validates with `int.TryParse`, times `CalculatePrimes(n)`
  with a `Stopwatch`, prints elapsed seconds, then calls `AskUserToPrintPrimes`.
  Entering `-1` (the `EXIT_CODE` constant) stops the loop; input `< 2` is ignored
  silently and the loop re-prompts.

- **`PrimeNumberGenerator`** (`PrimeNumberGenerator.cs`) — The computation engine.
  - `CalculatePrimes(int n)` — Resets the internal store and uses `Parallel.For`
    over `2..n` to trial-divide each candidate up to its square root. Primes are
    collected into a `ConcurrentBag<int>` (thread-safe, unordered).
  - `GetOrderedPrimeNumbers()` — Returns the bag sorted ascending as
    `List<int>`. Use this whenever ordered results are needed; the backing
    `PrimeNumbers` bag is private and unordered.
  - `LogPrimeNumbers()` — Prints the ordered primes to the console, or a
    "No prime numbers" message if none were computed.

## Conventions

- **Language/runtime:** C# targeting `netcoreapp3.1`. Keep the target framework in
  sync if you change `ComputePrimeNumbers.csproj`.
- **Concurrency:** Prime collection is parallel and therefore unordered. Never
  assume `PrimeNumbers` is sorted — always go through `GetOrderedPrimeNumbers()`.
  Any shared mutable state added later must be thread-safe (the code uses
  `ConcurrentBag<int>` for this reason).
- **Encapsulation:** `PrimeNumbers` is intentionally a private property exposed
  only via the `GetOrderedPrimeNumbers()` wrapper. Preserve this pattern — do not
  expose the raw bag.
- **Style:** Braces on their own lines (Allman style), `PascalCase` for methods
  and properties, `camelCase` for locals, XML `<summary>` doc comments on public
  methods. Match the existing style when editing.

## Build & run

`dotnet` is not preinstalled in every environment; install the .NET Core 3.1 SDK
(or open the solution in Visual Studio 2019+) before building.

```bash
# From the repository root
dotnet build   ComputePrimeNumbers/ComputePrimeNumbers.sln
dotnet run --project ComputePrimeNumbers/ComputePrimeNumbers/ComputePrimeNumbers.csproj
```

The app is interactive: type a number ≥ 2 to compute primes, answer `y`/`n` to
the print prompt, and enter `-1` to exit.

## Tests

There is currently **no test project**. If adding tests, create a separate test
project (e.g. xUnit/NUnit), add it to the solution, and target the prime logic in
`PrimeNumberGenerator` — keep computation logic out of `Program` so it stays
testable.

## Git workflow

- Active development branch: `claude/claude-md-docs-t8aqdi`.
- Commit with clear, descriptive messages and push with
  `git push -u origin <branch-name>`.
- Do not open a pull request unless explicitly asked.
