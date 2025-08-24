# Log Lady (.NET) Document Pipeline

This project is a modular .NET/C# rewrite of the `pdf-magic-monkey` pipeline, designed for high-volume PDF/TIFF OCR processing and document classification using ABBYY FineReader Engine (FRE) SDK.

## Pipeline Overview

1. **File Intake:** Watches `PDF_IN` for new files, moves them to `PDF_working`.
2. **Pre-processing:** (Optional, ABBYY handles most) Orientation, grayscale, denoise, etc.
3. **OCR:** ABBYY FRE SDK extracts text.
4. **Classification:** Simple rules-based classifier (extendable).
5. **Final Storage:** Moves finished files to `PDF_final` and updates SQLite DB.

## Project Structure

- **LogLady.sln:** Solution file
- **LogLady.Main:** Console app (entry point)
- **LogLady.Processing:** Processing pipeline (class library)
- **LogLady.Data:** Data access (class library)
- **appsettings.json:** Configurable paths/settings
- **.gitignore:** Standard .NET ignores

## Directory Layout

```
C:\PDF-Processing\
├── PDF_IN\
├── PDF_working\
├── PDF_final\
└── debug_imgs\
```

## Quickstart

1. Open in Visual Studio or `dotnet` CLI.
2. Restore and build solution.
3. Update `appsettings.json` with your actual paths and ABBYY license info.
4. Place PDFs in `PDF_IN` to begin processing.

## Configuration

See `appsettings.json` for all paths and ABBYY FRE options.

## Database

SQLite with schema (see Data/DatabaseService.cs).

---

**This is a starter scaffold. Integrate ABBYY .NET SDK in OcrEngine.cs after installing FRE SDK.**
