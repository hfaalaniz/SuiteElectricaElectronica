using System;
using System.Diagnostics;
using System.IO;

namespace BuckConverterCalculator
{
    public static class DebugLogger
    {
        private static string logFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"TransformerCalc_Log_{DateTime.Now:yyyyMMdd}.txt"
        );

        public static void Log(string categoria, string mensaje, params object[] args)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] [{categoria}] {string.Format(mensaje, args)}";
            Debug.WriteLine(logEntry);

            try
            {
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch { /* Evitar errores en logging */ }
        }

        public static void LogCalculation(string nombre, double valor, string unidad = "")
        {
            Log("CALC", $"{nombre} = {valor:F6} {unidad}");
        }

        public static void LogError(string mensaje, Exception ex = null)
        {
            Log("ERROR", mensaje);
            if (ex != null)
                Log("ERROR", $"Excepción: {ex.Message}\n{ex.StackTrace}");
        }

        public static string GetLogPath() => logFilePath;
    }
}