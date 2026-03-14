# Copilot Instructions — PhotoStax AI Curator

## Project Overview

PhotoStax AI Curator is a .NET 10 MAUI application that uses OpenAI GPT-4o Vision to analyze scanned photo stacks (from Epson FastFoto scanners) and enrich their metadata with AI-discovered information: OCR text, people, places, events, dates, locations, moods, and more. Enriched metadata is written back to the photostax library (XMP + sidecar files).

## Architecture

This is a **4-layer clean architecture** — respect the dependency flow:

```
App (MAUI Views + ViewModels)
  ↓ depends on
Services (AI pipeline, review queue, enrichment orchestration)
  ↓ depends on
Domain (models, interfaces, configuration)
  ↑ implemented by
Infrastructure (photostax adapter, HTTP clients)
```

**Rules:**
- **Domain** has zero dependencies on other project layers. It defines interfaces and models only.
- **Services** depends on Domain only. It contains business logic and AI orchestration.
- **Infrastructure** implements Domain interfaces. It depends on Domain and Services.
- **App** is the composition root. It wires DI and contains MAUI views/viewmodels.
- Never add a reference from a lower layer to a higher layer (e.g., Domain must never reference Services).

## Technology Stack

| Component | Technology |
|-----------|-----------|
| Framework | .NET 10 (LTS) |
| UI | .NET MAUI (Windows, macOS, Android, iOS) |
| MVVM | CommunityToolkit.Mvvm (source generators) |
| AI | OpenAI GPT-4o Vision (swappable via `IAiVisionService`) |
| Photo Library | Photostax NuGet 0.1.5 (wraps Rust FFI) |
| Testing | xUnit, Moq, coverlet |
| Package Management | Central Package Management (Directory.Packages.props) |

## Coding Standards

### Naming

- **Classes**: PascalCase, `sealed` unless inheritance is explicitly needed
- **Interfaces**: `I` prefix (e.g., `IPhotostaxAdapter`)
- **Private fields**: `_camelCase` with underscore prefix
- **Constants**: PascalCase
- **Async methods**: `Async` suffix (e.g., `AnalyzeAsync`)
- **Test classes**: `{ClassName}Tests`
- **Test methods**: `{Behavior}_{Expectation}` (e.g., `Categorize_HighConfidence_ReturnsHigh`)
- **Namespaces**: mirror folder structure (e.g., `PhotostaxAiCurator.Services.AI`)

### Style

- File-scoped namespaces (`namespace Foo;` not `namespace Foo { }`)
- Use `var` when the type is apparent from the right side; spell out the type otherwise
- Collection initialization with `[]` syntax (C# 12)
- Expression-bodied members for single-line implementations
- Braces on their own line (Allman style)
- Prefer pattern matching over `is`/`as` with null checks

### Documentation

- XML doc comments (`/// <summary>`) on all public types and members
- Single-line `<summary>` for simple members; multi-line for complex behavior
- No XML docs required in test files
- Use `// ---` comment headers to group related properties in models (see `AiAnalysisResult.cs`)
- Only add inline comments when the code needs clarification; don't comment obvious logic

### Dependency Injection

- All services are registered via DI extension methods (e.g., `AddServices()`, `AddInfrastructure()`)
- Use constructor injection exclusively — no service locator pattern
- Register implementations as interfaces (e.g., `AddSingleton<IAiVisionService, OpenAiVisionService>()`)
- Use `IOptions<T>` for configuration, never raw config strings

### Async Patterns

- All I/O-bound operations must be async with `CancellationToken` support
- Use `SemaphoreSlim` for concurrency limiting (see `EnrichmentPipeline.RunBatchAsync`)
- Always pass `CancellationToken` through the call chain
- Use `IProgress<T>` for reporting progress to the UI

### Error Handling

- Let exceptions bubble up to the ViewModel layer for user-facing error display
- Use structured logging (`ILogger<T>`) at decision points and error boundaries
- Never swallow exceptions silently — at minimum, log them

## MAUI / UI Conventions

- Use MVVM pattern exclusively — no code-behind logic except navigation
- ViewModels use `[ObservableProperty]` and `[RelayCommand]` from CommunityToolkit.Mvvm
- ViewModels are `partial class` (required for source generators)
- Views bind to ViewModels via `x:DataType` for compiled bindings
- Use `Shell` navigation with `ShellContent` route registration
- Use `Border` instead of `Frame` (Frame is obsolete in .NET 10)

## AI Integration Design

The AI layer is intentionally thin and provider-swappable:

- `IAiVisionService` is the abstraction — implement it for any AI provider
- `OpenAiVisionService` is the current implementation (GPT-4o Vision)
- Single multimodal API call per stack: front + back images sent together
- Response is structured JSON parsed by `AiResponseParser`
- Confidence-based routing: ≥85% auto-approve, 50-85% human review, <50% flagged
- Thresholds are configurable via `AiCuratorOptions`

## Metadata Strategy

PhotoStax uses a layered metadata approach:
- **EXIF tags**: read-only (camera data from scan)
- **XMP tags**: read/write (standard photo metadata)
- **Custom tags** (XMP sidecar): read/write (AI-enriched fields like `ocr_front`, `ocr_back`, `people`, `places`, `events`, `mood`, `scene`, etc.)

When writing metadata back, map `AiAnalysisResult` fields to both standard XMP tags and custom photostax sidecar tags.

## Testing Guidelines

- Every service and domain model should have unit tests
- Tests live in `tests/{ProjectName}.Tests/` mirroring the source structure
- Use Moq for mocking interfaces in service tests
- Test naming: `{Method}_{Scenario}_{ExpectedResult}`
- No integration tests that require real API keys in CI — mock the AI service
- Test projects inherit shared config from `tests/Directory.Build.props`

## Package Management

- All NuGet package versions are centralized in `Directory.Packages.props` at the repo root
- Individual `.csproj` files reference packages without versions
- Shared properties (TargetFramework, Nullable, etc.) come from `Directory.Build.props`
- Test-specific shared properties come from `tests/Directory.Build.props`
- When adding a new package, add the version to `Directory.Packages.props` first

## Git & CI Conventions

- Branch protection: PRs required for `main`, status checks must pass
- CI: build + test on ubuntu/windows/macos, MAUI build on Windows only
- Commit messages: imperative mood, descriptive body
- Dependabot: weekly NuGet + GitHub Actions updates
- Always include `Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>` when Copilot helps

## File Organization

```
photostax-ai-curator/
├── Directory.Build.props          # Shared project properties
├── Directory.Packages.props       # Central package versions
├── .editorconfig                  # Coding style enforcement
├── src/
│   ├── PhotostaxAiCurator.Domain/
│   │   ├── Configuration/         # AiCuratorOptions
│   │   ├── Interfaces/            # IAiVisionService, IPhotostaxAdapter, etc.
│   │   └── Models/                # AiAnalysisResult, StackInfo, ReviewItem, etc.
│   ├── PhotostaxAiCurator.Services/
│   │   ├── AI/                    # OpenAiVisionService, AiResponseParser, prompts
│   │   ├── Pipeline/              # EnrichmentPipeline
│   │   └── Review/                # ReviewQueueService
│   ├── PhotostaxAiCurator.Infrastructure/
│   │   └── Photostax/             # PhotostaxAdapter (wraps NuGet package)
│   └── PhotostaxAiCurator.App/
│       ├── Views/                 # XAML pages
│       ├── ViewModels/            # MVVM ViewModels
│       └── Resources/             # Icons, fonts, splash screen
└── tests/
    ├── Directory.Build.props      # Shared test properties
    ├── PhotostaxAiCurator.Domain.Tests/
    └── PhotostaxAiCurator.Services.Tests/
```

## Common Pitfalls

- MAUI workload is not supported on Linux — CI must target Windows/macOS for MAUI builds
- `TargetFramework` in Directory.Build.props must be cleared in MAUI csproj (`<TargetFramework />`) since it uses `TargetFrameworks` (plural)
- CommunityToolkit.Mvvm source generators produce command properties that MAUI compiled bindings can't see at compile time — the MAUIG2045 warnings are expected and harmless
- Photostax `Scan()` is lazy (no file I/O) — call `LoadMetadata()` or `GetStackWithMetadata()` to get EXIF/XMP data
- OpenAI responses may include markdown code fences around JSON — `AiResponseParser` strips them
