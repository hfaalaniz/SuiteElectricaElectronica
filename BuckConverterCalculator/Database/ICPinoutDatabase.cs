// ============================================================================
// SISTEMA DE BASE DE DATOS DE PINOUT EXTENSIBLE
// Agregar esta clase en un nuevo archivo: ICPinoutDatabase.cs
// ============================================================================

//using BuckConverterCalculator.Logging;
using BuckConverterCalculator.SchematicEditor;
using BuckConverterCalculator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BuckConverterCalculator.Database
{
    /// <summary>
    /// Definición de un componente en la base de datos de pinout
    /// </summary>
    public class ICDefinition
    {
        public string PartNumber { get; set; }
        public List<string> Aliases { get; set; }
        public string Type { get; set; }
        public string Manufacturer { get; set; }
        public int PinCount { get; set; }
        public string Description { get; set; }
        public List<PinDefinition> Pinout { get; set; }
    }

    public class PinDefinition
    {
        public int Pin { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    /// <summary>
    /// Base de datos de pinout para circuitos integrados
    /// </summary>
    public class ICPinoutDatabase
    {
        private static ICPinoutDatabase _instance;
        private List<ICDefinition> _components;
        private string _databasePath;

        public static ICPinoutDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ICPinoutDatabase();
                }
                return _instance;
            }
        }

        private ICPinoutDatabase()
        {
            // Buscar la base de datos en varios lugares
            _databasePath = FindDatabaseFile();
            LoadDatabase();
        }

        private string FindDatabaseFile()
        {
            // Lugares donde buscar el archivo
            var searchPaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IC_Pinout_Database.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "IC_Pinout_Database.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "IC_Pinout_Database.json"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "BuckConverter", "IC_Pinout_Database.json")
            };

            foreach (var path in searchPaths)
            {
                if (File.Exists(path))
                {
                    LoggerService.Instance.Info($"IC Pinout Database found at: {path}", "ICPinoutDB");
                    return path;
                }
            }

            LoggerService.Instance.Warning("IC Pinout Database not found, will use built-in definitions only", "ICPinoutDB");
            return null;
        }

        private void LoadDatabase()
        {
            _components = new List<ICDefinition>();

            if (!string.IsNullOrEmpty(_databasePath) && File.Exists(_databasePath))
            {
                try
                {
                    string json = File.ReadAllText(_databasePath);
                    _components = JsonSerializer.Deserialize<List<ICDefinition>>(json);
                    LoggerService.Instance.Info($"Loaded {_components.Count} IC definitions from database", "ICPinoutDB");
                }
                catch (Exception ex)
                {
                    LoggerService.Instance.Error($"Error loading IC database: {ex.Message}", "ICPinoutDB", ex);
                }
            }

            // Agregar definiciones hardcodeadas como fallback
            AddBuiltInDefinitions();
        }

        /// <summary>
        /// Agrega definiciones básicas hardcodeadas como fallback
        /// </summary>
        private void AddBuiltInDefinitions()
        {
            // Solo agregar si no están ya en el archivo JSON
            var builtIn = new List<ICDefinition>
            {
                // TL494 - PWM Controller 16 pines
                new ICDefinition
                {
                    PartNumber = "TL494",
                    Aliases = new List<string> { "TL494CN", "TL494IN" },
                    Type = "PWM Controller",
                    Manufacturer = "Texas Instruments",
                    PinCount = 16,
                    Description = "PWM Controller",
                    Pinout = new List<PinDefinition>
                    {
                        new PinDefinition { Pin = 1, Name = "1IN+", Type = "Input" },
                        new PinDefinition { Pin = 2, Name = "1IN-", Type = "Input" },
                        new PinDefinition { Pin = 3, Name = "FB", Type = "Input" },
                        new PinDefinition { Pin = 4, Name = "DTC", Type = "Input" },
                        new PinDefinition { Pin = 5, Name = "CT", Type = "Input" },
                        new PinDefinition { Pin = 6, Name = "RT", Type = "Input" },
                        new PinDefinition { Pin = 7, Name = "GND", Type = "Ground" },
                        new PinDefinition { Pin = 8, Name = "C1", Type = "Output" },
                        new PinDefinition { Pin = 9, Name = "E1", Type = "Output" },
                        new PinDefinition { Pin = 10, Name = "E2", Type = "Output" },
                        new PinDefinition { Pin = 11, Name = "C2", Type = "Output" },
                        new PinDefinition { Pin = 12, Name = "VCC", Type = "Power" },
                        new PinDefinition { Pin = 13, Name = "OUT", Type = "Output" },
                        new PinDefinition { Pin = 14, Name = "REF", Type = "Output" },
                        new PinDefinition { Pin = 15, Name = "2IN-", Type = "Input" },
                        new PinDefinition { Pin = 16, Name = "2IN+", Type = "Input" }
                    }
                }
            };

            foreach (var def in builtIn)
            {
                if (!_components.Any(c => c.PartNumber.Equals(def.PartNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    _components.Add(def);
                }
            }
        }

        /// <summary>
        /// Busca un componente por part number (con soporte para aliases)
        /// </summary>
        public ICDefinition FindComponent(string partNumber)
        {
            if (string.IsNullOrEmpty(partNumber))
                return null;

            var normalized = partNumber.ToUpper().Replace("-", "").Replace(" ", "").Trim();

            // Buscar coincidencia exacta
            var exact = _components.FirstOrDefault(c =>
                c.PartNumber.Replace("-", "").Replace(" ", "").Equals(normalized, StringComparison.OrdinalIgnoreCase));

            if (exact != null)
            {
                LoggerService.Instance.Info($"✅ Found exact match for {partNumber}: {exact.PartNumber} ({exact.PinCount} pins)", "ICPinoutDB");
                return exact;
            }

            // Buscar en aliases
            var aliasMatch = _components.FirstOrDefault(c =>
                c.Aliases != null && c.Aliases.Any(a =>
                    a.Replace("-", "").Replace(" ", "").Equals(normalized, StringComparison.OrdinalIgnoreCase)));

            if (aliasMatch != null)
            {
                LoggerService.Instance.Info($"✅ Found alias match for {partNumber}: {aliasMatch.PartNumber} ({aliasMatch.PinCount} pins)", "ICPinoutDB");
                return aliasMatch;
            }

            // Buscar coincidencia parcial (contiene)
            var partial = _components.FirstOrDefault(c =>
                normalized.Contains(c.PartNumber.Replace("-", "").Replace(" ", ""), StringComparison.OrdinalIgnoreCase) ||
                c.PartNumber.Replace("-", "").Replace(" ", "").Contains(normalized, StringComparison.OrdinalIgnoreCase));

            if (partial != null)
            {
                LoggerService.Instance.Info($"✅ Found partial match for {partNumber}: {partial.PartNumber} ({partial.PinCount} pins)", "ICPinoutDB");
                return partial;
            }

            LoggerService.Instance.Warning($"⚠️ No match found in database for: {partNumber}", "ICPinoutDB");
            return null;
        }

        /// <summary>
        /// Convierte PinDefinition a ComponentPin
        /// </summary>
        public List<ComponentPin> GetPins(ICDefinition definition)
        {
            if (definition?.Pinout == null)
                return null;

            var pins = new List<ComponentPin>();

            foreach (var pinDef in definition.Pinout)
            {
                var pinType = ParsePinType(pinDef.Type);
                pins.Add(new ComponentPin(pinDef.Pin, pinDef.Name, pinType, System.Drawing.Point.Empty));
            }

            return pins;
        }

        private PinType ParsePinType(string type)
        {
            if (string.IsNullOrEmpty(type))
                return PinType.Bidirectional;

            switch (type.ToUpper())
            {
                case "INPUT":
                    return PinType.Input;
                case "OUTPUT":
                    return PinType.Output;
                case "BIDIRECTIONAL":
                    return PinType.Bidirectional;
                case "POWER":
                    return PinType.Power;
                case "GROUND":
                    return PinType.Ground;
                case "NOCONNECT":
                    return PinType.NoConnect;
                case "PASSIVE":
                    return PinType.Passive;
                default:
                    return PinType.Bidirectional;
            }
        }

        /// <summary>
        /// Agrega un nuevo componente a la base de datos (y opcionalmente guarda)
        /// </summary>
        public void AddComponent(ICDefinition definition, bool saveToFile = true)
        {
            if (definition == null)
                return;

            // Evitar duplicados
            var existing = FindComponent(definition.PartNumber);
            if (existing != null)
            {
                LoggerService.Instance.Warning($"Component {definition.PartNumber} already exists in database", "ICPinoutDB");
                return;
            }

            _components.Add(definition);
            LoggerService.Instance.Info($"Added {definition.PartNumber} to database ({definition.PinCount} pins)", "ICPinoutDB");

            if (saveToFile)
            {
                SaveDatabase();
            }
        }

        /// <summary>
        /// Guarda la base de datos al archivo JSON
        /// </summary>
        public void SaveDatabase()
        {
            if (string.IsNullOrEmpty(_databasePath))
            {
                // Crear archivo en la carpeta de la aplicación
                _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IC_Pinout_Database.json");
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(_components, options);
                File.WriteAllText(_databasePath, json);
                LoggerService.Instance.Info($"Database saved to: {_databasePath}", "ICPinoutDB");
            }
            catch (Exception ex)
            {
                LoggerService.Instance.Error($"Error saving database: {ex.Message}", "ICPinoutDB", ex);
            }
        }

        /// <summary>
        /// Obtiene estadísticas de la base de datos
        /// </summary>
        public (int Total, Dictionary<string, int> ByType) GetStatistics()
        {
            var byType = _components
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            return (_components.Count, byType);
        }
    }
}