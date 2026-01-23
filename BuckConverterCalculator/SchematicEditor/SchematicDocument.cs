using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml;

namespace BuckConverterCalculator.SchematicEditor
{
    /// <summary>
    /// Documento del esquemático completo
    /// </summary>
    [Serializable]
    public class SchematicDocument
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public string Version { get; set; }

        [JsonProperty]
        public DateTime CreatedDate { get; set; }

        [JsonProperty]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty]
        public int CanvasWidth { get; set; }

        [JsonProperty]
        public int CanvasHeight { get; set; }

        [JsonProperty]
        public List<SchematicComponent> Components { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool IsDirty { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string FilePath { get; set; }

        public SchematicDocument()
        {
            Name = "Untitled Schematic";
            Description = "";
            Version = "1.0";
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            CanvasWidth = 2400;
            CanvasHeight = 1400;
            Components = new List<SchematicComponent>();
            IsDirty = false;
        }

        /// <summary>
        /// Add component to document
        /// </summary>
        public void AddComponent(SchematicComponent component)
        {
            Components.Add(component);
            IsDirty = true;
            ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Remove component from document
        /// </summary>
        public void RemoveComponent(SchematicComponent component)
        {
            Components.Remove(component);
            IsDirty = true;
            ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Get selected components
        /// </summary>
        public List<SchematicComponent> GetSelectedComponents()
        {
            return Components.Where(c => c.IsSelected).ToList();
        }

        /// <summary>
        /// Clear all selections
        /// </summary>
        public void ClearSelection()
        {
            foreach (var component in Components)
            {
                component.IsSelected = false;
            }
        }

        /// <summary>
        /// Select component at point
        /// </summary>
        public SchematicComponent SelectComponentAt(System.Drawing.Point point, bool addToSelection = false)
        {
            if (!addToSelection)
            {
                ClearSelection();
            }

            // Buscar en orden inverso (los últimos agregados están arriba)
            for (int i = Components.Count - 1; i >= 0; i--)
            {
                if (Components[i].HitTest(point))
                {
                    Components[i].IsSelected = true;
                    return Components[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Delete selected components
        /// </summary>
        public void DeleteSelected()
        {
            Components.RemoveAll(c => c.IsSelected);
            IsDirty = true;
            ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Duplicate selected components
        /// </summary>
        public void DuplicateSelected()
        {
            var selected = GetSelectedComponents();
            ClearSelection();

            foreach (var component in selected)
            {
                var clone = component.Clone();
                clone.IsSelected = true;
                Components.Add(clone);
            }

            IsDirty = true;
            ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Move selected components
        /// </summary>
        public void MoveSelected(int deltaX, int deltaY)
        {
            foreach (var component in Components.Where(c => c.IsSelected))
            {
                component.Move(deltaX, deltaY);
            }
            IsDirty = true;
            ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Save to JSON file
        /// </summary>
        public void SaveToFile(string filePath)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Converters = new List<Newtonsoft.Json.JsonConverter>
                    {
                        new StringEnumConverter()
                    }
                };

                string json = JsonConvert.SerializeObject(this, settings);
                File.WriteAllText(filePath, json);

                FilePath = filePath;
                IsDirty = false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving schematic: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Load from JSON file
        /// </summary>
        public static SchematicDocument LoadFromFile(string filePath)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Converters = new List<Newtonsoft.Json.JsonConverter>
                    {
                        new StringEnumConverter()
                    }
                };

                string json = File.ReadAllText(filePath);
                var document = JsonConvert.DeserializeObject<SchematicDocument>(json, settings);

                document.FilePath = filePath;
                document.IsDirty = false;

                return document;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading schematic: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Export to JSON string
        /// </summary>
        public string ExportToJson()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Newtonsoft.Json.Formatting.Indented,
                Converters = new List<Newtonsoft.Json.JsonConverter>
                {
                    new StringEnumConverter()
                }
            };

            return JsonConvert.SerializeObject(this, settings);
        }

        /// <summary>
        /// Import from JSON string
        /// </summary>
        public static SchematicDocument ImportFromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<Newtonsoft.Json.JsonConverter>
                {
                    new StringEnumConverter()
                }
            };

            return JsonConvert.DeserializeObject<SchematicDocument>(json, settings);
        }

        /// <summary>
        /// Get component count by type
        /// </summary>
        public Dictionary<ComponentType, int> GetComponentStats()
        {
            var stats = new Dictionary<ComponentType, int>();

            foreach (var component in Components)
            {
                if (stats.ContainsKey(component.Type))
                {
                    stats[component.Type]++;
                }
                else
                {
                    stats[component.Type] = 1;
                }
            }

            return stats;
        }

        /// <summary>
        /// Find component by ID
        /// </summary>
        public SchematicComponent FindComponentById(string id)
        {
            return Components.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Find components by name
        /// </summary>
        public List<SchematicComponent> FindComponentsByName(string name)
        {
            return Components.Where(c => c.Name.Contains(name)).ToList();
        }

        /// <summary>
        /// Get next available component name
        /// </summary>
        public string GetNextComponentName(ComponentType type)
        {
            string prefix = GetComponentPrefix(type);
            int maxNumber = 0;

            foreach (var component in Components.Where(c => c.Type == type))
            {
                if (component.Name.StartsWith(prefix))
                {
                    string numberPart = component.Name.Substring(prefix.Length);
                    if (int.TryParse(numberPart, out int number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }
            }

            return prefix + (maxNumber + 1);
        }

        private string GetComponentPrefix(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.Resistor: return "R";
                case ComponentType.Capacitor: return "C";
                case ComponentType.Inductor: return "L";
                case ComponentType.Diode: return "D";
                case ComponentType.Mosfet: return "Q";
                case ComponentType.IC: return "U";
                case ComponentType.Fuse: return "F";
                case ComponentType.Terminal: return "J";
                case ComponentType.Ground: return "GND";
                case ComponentType.VccSupply: return "VCC";
                case ComponentType.Wire: return "W";
                case ComponentType.Node: return "N";
                case ComponentType.Label: return "TXT";
                default: return "X";
            }
        }

        /// <summary>
        /// Validate document
        /// </summary>
        public List<string> Validate()
        {
            var errors = new List<string>();

            // Check for duplicate names
            var nameGroups = Components.GroupBy(c => c.Name).Where(g => g.Count() > 1);
            foreach (var group in nameGroups)
            {
                errors.Add($"Duplicate component name: {group.Key}");
            }

            // Check for components outside canvas
            foreach (var component in Components)
            {
                if (component.X < 0 || component.X > CanvasWidth ||
                    component.Y < 0 || component.Y > CanvasHeight)
                {
                    errors.Add($"Component {component.Name} is outside canvas bounds");
                }
            }

            return errors;
        }
    }
}