---
applyTo: "tests/**/*.cs"
---

# Test Code Instructions

## Framework and conventions
- Use xUnit (`[Fact]` and `[Theory]`) — not NUnit or MSTest
- Use Moq for mocking interfaces
- Test class naming: `{ClassUnderTest}Tests`
- Test method naming: `{Method}_{Scenario}_{Expected}` (e.g., `Parse_ValidJson_ReturnsResult`)
- No XML documentation required in test files

## Structure
- Mirror the source folder structure under `tests/{Project}.Tests/`
- One test class per source class
- Use Arrange-Act-Assert pattern with blank lines separating each section
- Shared test config comes from `tests/Directory.Build.props` — don't duplicate IsPackable or xunit Using

## What to test
- All domain models: default values, edge cases, validation
- All service methods: happy path, error paths, boundary conditions
- Mock all dependencies via interfaces — never use real HTTP or file I/O
- No tests that require API keys or network access — mock `IAiVisionService`

## What not to test
- MAUI views/viewmodels (UI tests are separate concern)
- Auto-generated code from CommunityToolkit.Mvvm
- Simple DTOs with no logic
