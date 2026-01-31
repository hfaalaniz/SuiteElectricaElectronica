using System;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            DebugLogger.Log("APP", "═══════════════════════════════════════════════════");
            DebugLogger.Log("APP", "Aplicación iniciada - Versión 2.0");
            DebugLogger.Log("APP", "Log guardado en: {0}", DebugLogger.GetLogPath());
            DebugLogger.Log("APP", "═══════════════════════════════════════════════════");

            Application.Run(new DemoForm());  // TransformadorCalculatorForm());

            DebugLogger.Log("APP", "Aplicación finalizada");

        }
    }
}