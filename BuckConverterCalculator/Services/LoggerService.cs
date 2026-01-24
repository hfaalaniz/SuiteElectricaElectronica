using System;
using System.IO;
using System.Text;

namespace BuckConverterCalculator.Services
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public class LoggerService
    {
        private static LoggerService _instance;
        private static readonly object _lock = new object();
        private readonly string _logFilePath;
        private readonly bool _consoleOutput;

        private LoggerService()
        {
            // Crear carpeta Logs si no existe
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Nombre del archivo con fecha
            string fileName = $"BuckConverter_{DateTime.Now:yyyyMMdd}.log";
            _logFilePath = Path.Combine(logDirectory, fileName);
            _consoleOutput = true;

            // Log inicial
            Log(LogLevel.Info, "=== Logger Service Initialized ===");
            Log(LogLevel.Info, $"Log file: {_logFilePath}");
        }

        public static LoggerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoggerService();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Debug(string message, string source = null)
        {
            Log(LogLevel.Debug, message, source);
        }

        public void Info(string message, string source = null)
        {
            Log(LogLevel.Info, message, source);
        }

        public void Warning(string message, string source = null)
        {
            Log(LogLevel.Warning, message, source);
        }

        public void Error(string message, string source = null, Exception ex = null)
        {
            string fullMessage = message;
            if (ex != null)
            {
                fullMessage += $"\nException: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
            }
            Log(LogLevel.Error, fullMessage, source);
        }

        private void Log(LogLevel level, string message, string source = null)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ");
                sb.Append($"[{level.ToString().ToUpper()}] ");

                if (!string.IsNullOrEmpty(source))
                {
                    sb.Append($"[{source}] ");
                }

                sb.Append(message);

                string logEntry = sb.ToString();

                // Escribir a archivo
                lock (_lock)
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }

                // Escribir a consola si está habilitado
                if (_consoleOutput)
                {
                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = GetColorForLevel(level);
                    Console.WriteLine(logEntry);
                    Console.ForegroundColor = originalColor;
                }
            }
            catch (Exception ex)
            {
                // Si falla el logging, al menos intentar escribir a consola
                Console.WriteLine($"Logger error: {ex.Message}");
            }
        }

        private ConsoleColor GetColorForLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return ConsoleColor.Gray;
                case LogLevel.Info:
                    return ConsoleColor.White;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.White;
            }
        }

        public void LogDivider()
        {
            Log(LogLevel.Info, new string('-', 80));
        }

        public string GetLogFilePath()
        {
            return _logFilePath;
        }

        public void OpenLogFile()
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    System.Diagnostics.Process.Start("notepad.exe", _logFilePath);
                }
            }
            catch (Exception ex)
            {
                Error("Failed to open log file", "LoggerService", ex);
            }
        }
    }
}