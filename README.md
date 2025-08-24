# Log Lady (.NET) Document Pipeline

This project is a modular .NET/C# rewrite of the `pdf-magic-monkey` pipeline, designed for high-volume PDF/TIFF OCR processing and document classification using ABBYY FineReader Engine (FRE) SDK.

## Pipeline Overview

1. **File Intake:** Watches `PDF_IN` for new files, moves them to `PDF_working`.
2. **Pre-processing:** (Optional, ABBYY handles most) Orientation, grayscale, denoise, etc.
3. **OCR:** ABBYY FRE SDK extracts text.
4. **Classification:** Simple rules-based classifier (extendable).
5. **Final Storage:** Moves finished files to `PDF_final` and updates MySQL database.

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
```

## Quickstart

1. **Setup MySQL Database:**
   - Install MySQL Server 8.0 or higher
   - Create a database named `loglady` (or update ConnectionString in appsettings.json)
   - Optionally run `schema.sql` to create tables (they will be created automatically on first run)

2. **Configure Application:**
   - Open in Visual Studio or `dotnet` CLI
   - Restore and build solution
   - Update `appsettings.json` with your actual paths, ABBYY license info, and MySQL connection string

3. **Run Application:**
   - Place PDFs in `PDF_IN` to begin processing

## Configuration

See `appsettings.json` for all configuration options:

- **File Paths:** Configure input, working, and output directories
- **ABBYY Settings:** License path and OCR configuration
- **MySQL Connection:** Update `ConnectionString` with your MySQL server details:
  ```json
  "ConnectionString": "Server=localhost;Database=loglady;User=your_username;Password=your_password;"
  ```

## Database

**MySQL** with automatic schema creation (see `Data/DatabaseService.cs` and `schema.sql`).

### Database Setup

1. **Install MySQL:** Download and install MySQL Server 8.0+
2. **Create Database:** `CREATE DATABASE loglady;`
3. **Configure Connection:** Update the `ConnectionString` in `appsettings.json`
4. **Schema Creation:** Tables are created automatically on first run, or run `schema.sql` manually

### Database Schema

The application uses two main tables:
- `documents`: Stores processed file information, OCR text, and classification results
- `processing_logs`: Stores detailed processing logs and system messages

See `schema.sql` for the complete database schema with indexes and comments.

---

**This is a starter scaffold. Integrate ABBYY .NET SDK in OcrEngine.cs after installing FRE SDK.**
