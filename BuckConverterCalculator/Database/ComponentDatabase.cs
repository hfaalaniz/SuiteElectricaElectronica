using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BuckConverterCalculator.Database
{
    public class ComponentDatabase
    {
        private List<ElectronicComponent> components;
        private string databasePath = "ComponentDatabase.json";

        public ComponentDatabase()
        {
            LoadDatabase();
        }

        public void LoadDatabase()
        {
            try
            {
                if (System.IO.File.Exists(databasePath))
                {
                    string json = System.IO.File.ReadAllText(databasePath);
                    components = JsonConvert.DeserializeObject<List<ElectronicComponent>>(json);
                }
                else
                {
                    components = GetDefaultComponents();
                    SaveDatabase();
                }
            }
            catch
            {
                components = GetDefaultComponents();
            }
        }

        public void SaveDatabase()
        {
            string json = JsonConvert.SerializeObject(components, Formatting.Indented);
            System.IO.File.WriteAllText(databasePath, json);
        }

        public List<ElectronicComponent> SearchComponents(ComponentSearchCriteria criteria)
        {
            var query = components.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.Type))
                query = query.Where(c => c.Type == criteria.Type);

            if (!string.IsNullOrEmpty(criteria.Manufacturer))
                query = query.Where(c => c.Manufacturer.Contains(criteria.Manufacturer));

            if (!string.IsNullOrEmpty(criteria.PartNumber))
                query = query.Where(c => c.PartNumber.Contains(criteria.PartNumber));

            // Filtros específicos por tipo
            if (criteria.Type == "MOSFET")
            {
                if (criteria.MinVoltage > 0)
                    query = query.Where(c => ParseDouble(c.GetSpec("VDS")) >= criteria.MinVoltage);

                if (criteria.MinCurrent > 0)
                    query = query.Where(c => ParseDouble(c.GetSpec("ID")) >= criteria.MinCurrent);

                if (criteria.MaxRDSon > 0)
                    query = query.Where(c => ParseDouble(c.GetSpec("RDSon")) <= criteria.MaxRDSon);
            }
            else if (criteria.Type == "Inductor")
            {
                if (criteria.MinInductance > 0)
                    query = query.Where(c => ParseDouble(c.GetSpec("Inductance")) >= criteria.MinInductance);

                if (criteria.MinCurrent > 0)
                    query = query.Where(c => ParseDouble(c.GetSpec("Isat")) >= criteria.MinCurrent);
            }
            else if (criteria.Type == "Capacitor")
            {
                if (criteria.MinCapacitance > 0)
                    query = query.Where(c => ParseDouble(c.GetSpec("Capacitance")) >= criteria.MinCapacitance);

                if (criteria.MinVoltage > 0)
                    query = query.Where(c => ParseDouble(c.GetSpec("Voltage")) >= criteria.MinVoltage);
            }

            // Ordenar por precio si se solicita
            if (criteria.SortByPrice)
                query = query.OrderBy(c => c.UnitPrice);

            return query.ToList();
        }

        private double ParseDouble(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            // Eliminar unidades y parsear
            value = value.Replace("V", "").Replace("A", "").Replace("Ω", "")
                        .Replace("H", "").Replace("F", "").Replace("m", "");

            if (double.TryParse(value, out double result))
                return result;

            return 0;
        }

        private List<ElectronicComponent> GetDefaultComponents()
        {
            return new List<ElectronicComponent>
            {
                // MOSFETs
                new ElectronicComponent
                {
                    Type = "MOSFET",
                    Manufacturer = "Infineon",
                    PartNumber = "IPD90N04S4L-05",
                    Description = "N-Channel MOSFET 40V 90A",
                    Specifications = new Dictionary<string, string>
                    {
                        { "VDS", "40V" },
                        { "ID", "90A" },
                        { "RDSon", "5mΩ" },
                        { "Package", "TO-252" }
                    },
                    UnitPrice = 1.25,
                    Stock = 5000,
                    DatasheetURL = "https://www.infineon.com/..."
                },
                
                // Inductores
                new ElectronicComponent
                {
                    Type = "Inductor",
                    Manufacturer = "Würth Elektronik",
                    PartNumber = "744355147",
                    Description = "Power Inductor 47µH 5A",
                    Specifications = new Dictionary<string, string>
                    {
                        { "Inductance", "47µH" },
                        { "Isat", "5A" },
                        { "DCR", "20mΩ" },
                        { "Package", "SMD" }
                    },
                    UnitPrice = 0.85,
                    Stock = 10000,
                    DatasheetURL = "https://www.we-online.com/..."
                },
                
                // Capacitores
                new ElectronicComponent
                {
                    Type = "Capacitor",
                    Manufacturer = "Murata",
                    PartNumber = "GRM32ER71H106KA12L",
                    Description = "Ceramic Capacitor 10µF 50V X7R",
                    Specifications = new Dictionary<string, string>
                    {
                        { "Capacitance", "10µF" },
                        { "Voltage", "50V" },
                        { "Tolerance", "10%" },
                        { "Package", "1210" }
                    },
                    UnitPrice = 0.35,
                    Stock = 25000,
                    DatasheetURL = "https://www.murata.com/..."
                },
                
                // ICs
                new ElectronicComponent
                {
                    Type = "IC",
                    Manufacturer = "Texas Instruments",
                    PartNumber = "TPS54560",
                    Description = "Buck Converter 5A 60V",
                    Specifications = new Dictionary<string, string>
                    {
                        { "Vin", "4.5-60V" },
                        { "Iout", "5A" },
                        { "Frequency", "100-2500kHz" },
                        { "Package", "SOIC-8" }
                    },
                    UnitPrice = 2.50,
                    Stock = 3000,
                    DatasheetURL = "https://www.ti.com/..."
                }
            };
        }

        public void AddComponent(ElectronicComponent component)
        {
            components.Add(component);
            SaveDatabase();
        }

        public void UpdateComponent(ElectronicComponent component)
        {
            var existing = components.FirstOrDefault(c => c.PartNumber == component.PartNumber);
            if (existing != null)
            {
                int index = components.IndexOf(existing);
                components[index] = component;
                SaveDatabase();
            }
        }

        public void DeleteComponent(string partNumber)
        {
            components.RemoveAll(c => c.PartNumber == partNumber);
            SaveDatabase();
        }
    }

    public class ElectronicComponent
    {
        public string Type { get; set; }
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
        public double UnitPrice { get; set; }
        public int Stock { get; set; }
        public string DatasheetURL { get; set; }
        public string Supplier { get; set; } = "DigiKey";
        public DateTime LastUpdated { get; set; } = DateTime.Now;


        public ElectronicComponent()
        {
            Specifications = new Dictionary<string, string>();
            LastUpdated = DateTime.Now;
        }

        public string GetSpec(string key)
        {
            if (Specifications != null && Specifications.ContainsKey(key))
                return Specifications[key];
            return "";
        }
    }

    public class ComponentSearchCriteria
    {
        public string Type { get; set; }
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }

        // Criterios eléctricos
        public double MinVoltage { get; set; }
        public double MinCurrent { get; set; }
        public double MaxRDSon { get; set; }
        public double MinInductance { get; set; }
        public double MinCapacitance { get; set; }

        // Criterios de compra
        public bool SortByPrice { get; set; }
        public bool InStockOnly { get; set; }
        public double MaxPrice { get; set; }
    }
}