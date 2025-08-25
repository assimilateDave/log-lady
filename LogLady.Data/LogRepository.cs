using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace LogLady.Data
{
    /// <summary>
    /// Repository for document processing logs using MySQL backend.
    /// </summary>
    public class LogRepository
    {
        private readonly DatabaseService _databaseService;

        public LogRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        /// <summary>
        /// Saves a log message to the MySQL database.
        /// </summary>
        /// <param name="message">The log message to save</param>
        /// <param name="logLevel">Log level (e.g., INFO, ERROR, DEBUG)</param>
        /// <param name="component">Component that generated the log</param>
        /// <param name="documentId">Optional document ID if log is related to a specific document</param>
        public async Task SaveLogAsync(string message, string logLevel = "INFO", string component = "LogLady", int? documentId = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            using (var connection = _databaseService.GetConnection())
            {
                await connection.OpenAsync();

                var insertSql = @"
                    INSERT INTO processing_logs (document_id, log_level, message, timestamp, component)
                    VALUES (@documentId, @logLevel, @message, @timestamp, @component)";

                using (var command = new MySqlCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@documentId", documentId.HasValue ? (object)documentId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@logLevel", logLevel);
                    command.Parameters.AddWithValue("@message", message);
                    command.Parameters.AddWithValue("@timestamp", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@component", component);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Legacy synchronous method for backward compatibility.
        /// Saves a log message to the database.
        /// </summary>
        /// <param name="message">The log message to save</param>
        public void SaveLog(string message)
        {
            // Use async method synchronously for backward compatibility
            SaveLogAsync(message).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Saves document information to the database.
        /// </summary>
        /// <param name="filename">Original filename</param>
        /// <param name="filePath">Full path to the processed file</param>
        /// <param name="fileSize">File size in bytes</param>
        /// <param name="classification">Document classification</param>
        /// <param name="ocrText">Extracted OCR text</param>
        /// <returns>The ID of the inserted document record</returns>
        public async Task<int> SaveDocumentAsync(string filename, string filePath, long fileSize, string classification = null, string ocrText = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename cannot be null or empty", nameof(filename));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            using (var connection = _databaseService.GetConnection())
            {
                await connection.OpenAsync();

                var insertSql = @"
                    INSERT INTO documents (filename, file_path, processed_date, file_size, classification, ocr_text, status)
                    VALUES (@filename, @filePath, @processedDate, @fileSize, @classification, @ocrText, @status)
                    ON DUPLICATE KEY UPDATE
                        processed_date = @processedDate,
                        file_size = @fileSize,
                        classification = @classification,
                        ocr_text = @ocrText,
                        status = @status";

                using (var command = new MySqlCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@filename", filename);
                    command.Parameters.AddWithValue("@filePath", filePath);
                    command.Parameters.AddWithValue("@processedDate", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@fileSize", fileSize);
                    command.Parameters.AddWithValue("@classification", classification ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ocrText", ocrText ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@status", "processed");

                    await command.ExecuteNonQueryAsync();

                    // Get the inserted/updated ID
                    command.CommandText = "SELECT LAST_INSERT_ID()";
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }
    }
}