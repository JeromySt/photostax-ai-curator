# PhotoStax AI Curator

An AI-powered photo metadata curator that scans [photostax](https://github.com/JeromySt/photostax) repositories, analyzes every photo (front and back) using OpenAI GPT-4o Vision, and enriches metadata with OCR text, people, places, events, dates, locations, and more.

## Features

- **AI-Powered Analysis** — GPT-4o Vision extracts OCR text, identifies people, recognizes places, detects events and holidays, estimates dates, and more — all from a single multimodal API call per photo stack
- **Confidence-Based Review** — High-confidence results are auto-approved; lower confidence items are queued for interactive human review
- **OCR Front & Back** — Reads handwriting, date stamps, and printed text from both the front and back of scanned photos
- **Location Recognition** — Identifies landmarks and places from visual cues and OCR text
- **Cross-Platform** — Built with .NET MAUI targeting Windows, macOS, Android, and iOS
- **Metadata Write-Back** — All enriched metadata is written back via photostax (XMP + sidecar), making it readable by any photo viewer or album software

## Architecture

```
Presentation (MAUI Views + ViewModels)
    ↓
Services (AI Pipeline, Review Queue, Enrichment)
    ↓
Domain (Models, Interfaces)
    ↓
Infrastructure (Photostax Adapter, OpenAI HTTP Client)
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) with MAUI workload
- OpenAI API key with GPT-4o Vision access

## Getting Started

1. Clone this repository
2. Set your OpenAI API key in the Settings page
3. Point the app at a folder containing FastFoto scans
4. Click "Start AI Analysis" and review the results

## Building

```bash
dotnet build
dotnet test
```

For Windows:
```bash
dotnet build -f net10.0-windows10.0.19041.0
```

## License

MIT
