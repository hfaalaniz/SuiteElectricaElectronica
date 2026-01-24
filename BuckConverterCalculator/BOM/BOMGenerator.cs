using System;
using System.Collections.Generic;
using System.Linq;
using BuckConverterCalculator.Database;
using BuckConverterCalculator.SchematicEditor;

namespace BuckConverterCalculator.BOM
{
    /// <summary>
    /// Generador de Bill of Materials desde el esquemático
    /// </summary>
    public class BOMGenerator
    {
        private SchematicDocument document;
        private ComponentDatabase database;

        public BOMGenerator(SchematicDocument doc, ComponentDatabase db)
        {
            this.document = doc;
            this.database = db;
        }

        public BillOfMaterials GenerateBOM()
        {
            var bom = new BillOfMaterials
            {
                ProjectName = document.ProjectName ?? "Buck Converter Design",
                GeneratedDate = DateTime.Now,
                Items = new List<BOMItem>()
            };

            // Agrupar componentes por tipo y valor
            var componentGroups = document.Components
                .Where(c => !(c.GetType().Name.Contains("Wire")) &&
                           !(c.GetType().Name.Contains("Node")) &&
                           !(c.GetType().Name.Contains("Label")))
                .GroupBy(c => GetComponentKey(c))
                .Select(g => new
                {
                    Key = g.Key,
                    Components = g.ToList(),
                    Quantity = g.Count()
                });

            int itemNumber = 1;

            foreach (var group in componentGroups)
            {
                var component = group.Components.First();
                var bomItem = CreateBOMItem(component, group.Quantity, itemNumber++);

                // Buscar en base de datos para obtener info comercial
                if (!string.IsNullOrEmpty(bomItem.PartNumber))
                {
                    var dbComponent = database.SearchComponents(new ComponentSearchCriteria
                    {
                        PartNumber = bomItem.PartNumber
                    }).FirstOrDefault();

                    if (dbComponent != null)
                    {
                        bomItem.Manufacturer = dbComponent.Manufacturer;
                        bomItem.UnitPrice = dbComponent.UnitPrice;
                        bomItem.TotalPrice = dbComponent.UnitPrice * bomItem.Quantity;
                        bomItem.Supplier = dbComponent.Supplier;
                        bomItem.Stock = dbComponent.Stock;
                        bomItem.DatasheetURL = dbComponent.DatasheetURL;
                    }
                }

                // Referencias (designators)
                bomItem.Designators = string.Join(", ", group.Components.Select(c => c.Name));

                bom.Items.Add(bomItem);
            }

            // Calcular totales
            bom.TotalUniqueComponents = bom.Items.Count;
            bom.TotalComponents = bom.Items.Sum(i => i.Quantity);
            bom.TotalCost = bom.Items.Sum(i => i.TotalPrice);

            return bom;
        }

        private string GetComponentKey(SchematicComponent component)
        {
            // Clave única basada en tipo y valor principal
            string key = component.Type.ToString();

            // Usar reflection para obtener propiedades comunes
            var valueProperty = component.GetType().GetProperty("Value");
            if (valueProperty != null)
            {
                var value = valueProperty.GetValue(component);
                if (value != null)
                    key += "_" + value.ToString();
            }

            var partNumberProperty = component.GetType().GetProperty("PartNumber");
            if (partNumberProperty != null)
            {
                var partNumber = partNumberProperty.GetValue(component);
                if (partNumber != null)
                    key += "_" + partNumber.ToString();
            }

            return key;
        }

        private BOMItem CreateBOMItem(SchematicComponent component, int quantity, int itemNumber)
        {
            var item = new BOMItem
            {
                ItemNumber = itemNumber,
                Quantity = quantity,
                Type = component.Type.ToString()
            };

            // Usar reflection para extraer propiedades
            var valueProperty = component.GetType().GetProperty("Value");
            if (valueProperty != null)
            {
                var value = valueProperty.GetValue(component);
                item.Value = value?.ToString() ?? "";
            }

            var partNumberProperty = component.GetType().GetProperty("PartNumber");
            if (partNumberProperty != null)
            {
                var partNumber = partNumberProperty.GetValue(component);
                item.PartNumber = partNumber?.ToString() ?? "";
            }

            var descriptionProperty = component.GetType().GetProperty("Description");
            if (descriptionProperty != null)
            {
                var description = descriptionProperty.GetValue(component);
                item.Description = description?.ToString() ?? "";
            }
            else
            {
                // Generar descripción básica
                item.Description = $"{component.Type} {item.Value}";
            }

            item.Package = "TBD"; // Default

            return item;
        }
    }

    /// <summary>
    /// Clase principal de BOM - ÚNICA DEFINICIÓN
    /// </summary>
    [Serializable]
    public class BillOfMaterials
    {
        public string ProjectName { get; set; }
        public DateTime GeneratedDate { get; set; }
        public List<BOMItem> Items { get; set; }
        public int TotalUniqueComponents { get; set; }
        public int TotalComponents { get; set; }
        public double TotalCost { get; set; }
    }

    /// <summary>
    /// Item individual del BOM - ÚNICA DEFINICIÓN
    /// </summary>
    [Serializable]
    public class BOMItem
    {
        public int ItemNumber { get; set; }
        public int Quantity { get; set; }
        public string Designators { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public string Package { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public string Supplier { get; set; }
        public int Stock { get; set; }
        public string DatasheetURL { get; set; }
        public string Notes { get; set; }
    }
}