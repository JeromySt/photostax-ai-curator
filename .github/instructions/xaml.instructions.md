---
applyTo: "src/PhotostaxAiCurator.App/**/*.xaml"
---

# XAML / MAUI UI Instructions

## MVVM pattern
- All logic belongs in ViewModels — XAML code-behind should only contain navigation or platform-specific code
- Use `x:DataType` on every page for compiled bindings
- Bind commands via `{Binding CommandName}` — CommunityToolkit.Mvvm generates them from `[RelayCommand]`

## Layout guidelines
- Use `Border` instead of `Frame` (Frame is obsolete in .NET 10 MAUI)
- Use `Grid` for complex layouts, `VerticalStackLayout`/`HorizontalStackLayout` for simple flows
- Use `CollectionView` for lists (not `ListView`)
- Define reusable styles in `App.xaml` or `Resources/Styles/`

## Shell navigation
- Register all pages as `ShellContent` routes in `AppShell.xaml`
- Use `Shell.GoToAsync` for programmatic navigation
- Don't use `FlyoutIcon` with empty values — omit the attribute entirely if no icon is available
