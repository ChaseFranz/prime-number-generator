# CLAUDE.md

Guidance for AI assistants working in this repository.

## Overview

A small .NET console application that computes all prime numbers from 2 up to a
user-supplied number `N` with a parallel segmented Sieve of Eratosthenes.
Execution time is measured and printed, and the user can optionally print the
discovered primes to the console. The program runs an interactive read-eval loop
until the user enters `-1` to exit.

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

- **`Program`** (`Program.cs`) — Entry point and all console I/O. Holds a single
  static `PrimeNumberGenerator` instance. `Main` runs an input loop: prompts for a
  natural number `N`, reads a line, and exits on end-of-input (a `null` read, e.g.
  EOF or exhausted piped input). It validates with `int.TryParse`; anything that
  is neither `>= 2` nor the `EXIT_CODE` constant (`-1`) prints a guidance message
  and re-prompts. Valid input is timed with a `Stopwatch`, elapsed seconds are
  printed, and the returned primes are passed to `AskUserToPrintPrimes` →
  `PrintPrimes`. Entering `-1` stops the loop.
  - Presentation lives here on purpose: `PrintPrimes(primes)` renders the result
    (or "No prime numbers"), keeping the computation engine free of `Console`
    dependencies so it stays unit-testable.

- **`PrimeNumberGenerator`** (`PrimeNumberGenerator.cs`) — The computation engine.
  Stateless and free of any `Console` use.
  - `CalculatePrimes(int n)` — Returns an `IReadOnlyList<int>` of the primes in
    `[2, n]` in ascending order (empty when `n < 2`). It runs a segmented Sieve of
    Eratosthenes: base primes up to `sqrt(n)` are found sequentially, then the rest
    of the range is sieved with `Parallel.For` over **disjoint** segments of a
    shared `bool[]`. Because no two threads write the same array slot, no locking
    is needed. The final ascending order falls out of the sieve — no post-sort.

## Conventions

- **Language/runtime:** C# targeting `netcoreapp3.1`. Keep the target framework in
  sync if you change `ComputePrimeNumbers.csproj`.
- **Concurrency:** Parallelism relies on each segment owning a disjoint slice of
  the shared `bool[]`, so there are no concurrent writes to the same slot and no
  locking. If you change the partitioning, preserve that disjointness — otherwise
  you reintroduce data races. Any other shared mutable state added later must be
  made thread-safe explicitly.
- **Separation of concerns:** `PrimeNumberGenerator` is pure computation with no
  `Console` calls; all I/O and prompting live in `Program`. Keep computation out
  of `Program` and I/O out of the generator so the prime logic stays testable.
- **Stateless engine:** `CalculatePrimes` returns its result rather than storing
  it on the instance. Don't reintroduce shared instance state for results.
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

Unit tests live in `ComputePrimeNumbers/ComputePrimeNumbers.Tests/` (xUnit,
`netcoreapp3.1`) and are registered in the solution. They cover
`PrimeNumberGenerator.CalculatePrimes`: below-2 inputs, small known prime sets,
ascending order, inclusive upper bound, prime-counting (`pi`) counts at 100/1000/
10000, a "every result is actually prime" check, and a cross-check against an
independent trial-division reference over a range that spans many parallel
segments.

```bash
# From the repository root
dotnet test ComputePrimeNumbers/ComputePrimeNumbers.sln
```

Keep computation logic in `PrimeNumberGenerator` (not `Program`) so it stays
testable, and add cases here when you change the algorithm.

## Git workflow

- Active development branch: `claude/claude-md-docs-t8aqdi`.
- Commit with clear, descriptive messages and push with
  `git push -u origin <branch-name>`.
- Do not open a pull request unless explicitly asked.
