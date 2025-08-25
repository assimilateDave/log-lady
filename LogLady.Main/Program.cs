using System;
using System.IO;
using System.Threading.Tasks;
using LogLady.Data;
using Newtonsoft.Json;

namespace LogLady.Main
{
    /// <summary>
    /// Configuration model for appsettings.json
    /// </summary>
    public class AppSettings
    {
        public string PdfInPath { get; set; }
        public string PdfWorkingPath { get; set; }
        public string PdfFinalPath { get; set; }
        public string DebugImgPath { get; set; }
        public string ConnectionString { get; set; }
        public ABBYYSettingsConfig ABBYYSettings { get; set; }
    }

    public class ABBYYSettingsConfig
    {
        public string LicensePath { get; set; }
        public string RecognitionLanguages { get; set; }
        public string Profile { get; set; }
        public object OtherOptions { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to LogLady - MySQL Edition!");
            Console.WriteLine("=====================================");

            try
            {
                // Load configuration
                var settings = LoadConfiguration();
                if (settings == null)
                {
                    Console.WriteLine("Failed to load appsettings.json");
                    return;
                }

                Console.WriteLine($"Loaded configuration:");
                Console.WriteLine($"  PDF Input Path: {settings.PdfInPath}");
                Console.WriteLine($"  PDF Working Path: {settings.PdfWorkingPath}");
                Console.WriteLine($"  PDF Final Path: {settings.PdfFinalPath}");
                Console.WriteLine($"  Connection String: {MaskConnectionString(settings.ConnectionString)}");
                Console.WriteLine();

                // Initialize database service
                Console.WriteLine("Initializing MySQL database service...");
                var databaseService = new DatabaseService(settings.ConnectionString);
                
                // Test database connection
                Console.WriteLine("Testing database connection...");
                bool isConnected = await databaseService.TestConnectionAsync();
                
                if (isConnected)
                {
                    Console.WriteLine("✓ Database connection successful!");
                    
                    // Initialize database schema
                    Console.WriteLine("Initializing database schema...");
                    await databaseService.InitializeDatabaseAsync();
                    Console.WriteLine("✓ Database schema initialized!");

                    // Test logging functionality
                    Console.WriteLine("Testing log repository...");
                    var logRepo = new LogRepository(databaseService);
                    await logRepo.SaveLogAsync("LogLady application started successfully", "INFO", "LogLady.Main");
                    Console.WriteLine("✓ Log saved to database!");

                    // Test document saving (example)
                    Console.WriteLine("Testing document repository...");
                    var docId = await logRepo.SaveDocumentAsync(
                        "sample.pdf", 
                        "/PDF_final/sample.pdf", 
                        1024000, 
                        "test_document", 
                        "Sample OCR text content"
                    );
                    Console.WriteLine($"✓ Sample document saved with ID: {docId}");
                    
                    Console.WriteLine();
                    Console.WriteLine("MySQL migration completed successfully!");
                    Console.WriteLine("The application is ready to process documents with MySQL backend.");
                }
                else
                {
                    Console.WriteLine("✗ Database connection failed!");
                    Console.WriteLine("Please check your MySQL server and connection string in appsettings.json");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                Console.WriteLine();
                Console.WriteLine("Common issues:");
                Console.WriteLine("- MySQL server is not running");
                Console.WriteLine("- Database credentials are incorrect");
                Console.WriteLine("- Database 'loglady' doesn't exist (will be created automatically)");
                Console.WriteLine("- Firewall blocking connection to MySQL");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static AppSettings LoadConfiguration()
        {
            try
            {
                var configPath = "appsettings.json";
                if (!File.Exists(configPath))
                {
                    Console.WriteLine($"Configuration file not found: {configPath}");
                    return null;
                }

                var jsonContent = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<AppSettings>(jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                return null;
            }
        }

        private static string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return "[Not Set]";

            // Simple password masking for display
            return connectionString.Contains("Password=") 
                ? connectionString.Substring(0, connectionString.IndexOf("Password=") + 9) + "***;"
                : connectionString;
        }
    }
}