using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace LogLady.Data
{
    /// <summary>
    /// MySQL database service for Log Lady document processing system.
    /// Handles database connections and schema management.
    /// </summary>
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Tests the database connection and creates the database if it doesn't exist.
        /// </summary>
        public async Task InitializeDatabaseAsync()
        {
            try
            {
                // First connect without specifying database to create it if needed
                var builder = new MySqlConnectionStringBuilder(_connectionString);
                var databaseName = builder.Database;
                builder.Database = "";

                using (var connection = new MySqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync();
                    
                    // Create database if it doesn't exist
                    var createDbCommand = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{databaseName}`", connection);
                    await createDbCommand.ExecuteNonQueryAsync();
                }

                // Now connect to the specific database and create tables
                await CreateTablesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates the required tables if they don't exist.
        /// </summary>
        private async Task CreateTablesAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var createTablesScript = @"
                    CREATE TABLE IF NOT EXISTS documents (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        filename VARCHAR(500) NOT NULL,
                        file_path VARCHAR(1000) NOT NULL,
                        processed_date DATETIME NOT NULL,
                        file_size BIGINT,
                        classification VARCHAR(100),
                        ocr_text LONGTEXT,
                        status VARCHAR(50) NOT NULL DEFAULT 'processed',
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                        UNIQUE KEY unique_file_path (file_path)
                    );

                    CREATE TABLE IF NOT EXISTS processing_logs (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        document_id INT,
                        log_level VARCHAR(20) NOT NULL,
                        message TEXT NOT NULL,
                        timestamp DATETIME NOT NULL,
                        component VARCHAR(100),
                        FOREIGN KEY (document_id) REFERENCES documents(id) ON DELETE CASCADE
                    );";

                var command = new MySqlCommand(createTablesScript, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Tests if the database connection is working.
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a new database connection. Remember to dispose it after use.
        /// </summary>
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}