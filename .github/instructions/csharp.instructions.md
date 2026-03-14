---
applyTo: "src/**/*.cs"
---

# C# Source Code Instructions

## Layer boundaries are strict
- Domain: models and interfaces only, zero project dependencies
- Services: business logic, depends on Domain only
- Infrastructure: implements Domain interfaces
- App: composition root, MAUI views and viewmodels

## Required patterns
- Use `sealed class` unless inheritance is explicitly designed for
- Use file-scoped namespaces (`namespace Foo;`)
- Use constructor dependency injection with `_camelCase` private readonly fields
- All public types and members must have `/// <summary>` XML documentation
- Async methods must accept `CancellationToken` and use the `Async` suffix
- Use `IOptions<T>` for configuration, never raw strings
- Use structured logging: `_logger.LogInformation("Processing {Id}", id)` — not string interpolation

## Code style
- `var` when type is apparent; explicit type otherwise
- Collection initialization with `[]` (e.g., `List<string> items = [];`)
- Prefer pattern matching (`is`, `switch` expressions) over type checks/casts
- Braces on own line (Allman style), optional for single-line `if`/`foreach`
- Group related properties with `// --- Category ---` comment headers in models
