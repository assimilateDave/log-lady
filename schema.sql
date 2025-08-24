-- Log Lady MySQL Database Schema
-- This schema supports document processing pipeline with OCR and classification

-- Create database (if running manually)
-- CREATE DATABASE IF NOT EXISTS loglady;
-- USE loglady;

-- Documents table - stores information about processed files
CREATE TABLE IF NOT EXISTS documents (
    id INT AUTO_INCREMENT PRIMARY KEY,
    filename VARCHAR(500) NOT NULL COMMENT 'Original filename of the document',
    file_path VARCHAR(1000) NOT NULL COMMENT 'Full path to the processed file',
    processed_date DATETIME NOT NULL COMMENT 'When the document was processed',
    file_size BIGINT COMMENT 'File size in bytes',
    classification VARCHAR(100) COMMENT 'Document classification result',
    ocr_text LONGTEXT COMMENT 'Extracted OCR text content',
    status VARCHAR(50) NOT NULL DEFAULT 'processed' COMMENT 'Processing status',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation timestamp',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Record update timestamp',
    UNIQUE KEY unique_file_path (file_path),
    INDEX idx_filename (filename),
    INDEX idx_processed_date (processed_date),
    INDEX idx_classification (classification),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Stores processed document information';

-- Processing logs table - stores detailed processing logs
CREATE TABLE IF NOT EXISTS processing_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    document_id INT COMMENT 'Reference to the document being processed (nullable for general logs)',
    log_level VARCHAR(20) NOT NULL COMMENT 'Log level: DEBUG, INFO, WARN, ERROR',
    message TEXT NOT NULL COMMENT 'Log message content',
    timestamp DATETIME NOT NULL COMMENT 'When the log entry was created',
    component VARCHAR(100) COMMENT 'Component or module that generated the log',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation timestamp',
    INDEX idx_timestamp (timestamp),
    INDEX idx_log_level (log_level),
    INDEX idx_component (component),
    INDEX idx_document_id (document_id),
    FOREIGN KEY (document_id) REFERENCES documents(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Stores processing and system logs';

-- Sample data for testing (optional - comment out for production)
-- INSERT INTO documents (filename, file_path, processed_date, file_size, classification, status)
-- VALUES 
--     ('sample.pdf', '/PDF_final/sample.pdf', NOW(), 1048576, 'invoice', 'processed'),
--     ('test.pdf', '/PDF_final/test.pdf', NOW(), 2097152, 'contract', 'processed');

-- INSERT INTO processing_logs (document_id, log_level, message, timestamp, component)
-- VALUES 
--     (1, 'INFO', 'Document processing started', NOW(), 'LogLady.Processing'),
--     (1, 'INFO', 'OCR extraction completed', NOW(), 'LogLady.OCR'),
--     (1, 'INFO', 'Document processing completed successfully', NOW(), 'LogLady.Processing');