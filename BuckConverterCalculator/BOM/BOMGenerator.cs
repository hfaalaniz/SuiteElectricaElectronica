using BuckConverterCalculator.Database;
using BuckConverterCalculator.SchematicEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuckConverterCalculator.BOM
{
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
                // Analizar codigo comentado si se desea incluir nombre del proyecto
                //ProjectName = document.ProjectName,
                GeneratedDate = DateTime.Now,
                Items = new List<BOMItem>()
            };

            // Agrupar componentes por tipo y valor
            var componentGroups = document.Components
                .Where(c => !(c is WireComponent) && !(c is NodeComponent) && !(c is LabelComponent))
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

            if (component is ResistorComponent r)
                key += "_" + r.Value + "_" + r.Power;
            else if (component is CapacitorComponent c)
                key += "_" + c.Capacitance + "_" + c.VoltageRating;
            else if (component is InductorComponent l)
                key += "_" + l.Value + "_" + l.Isat;
            else if (component is DiodeComponent d)
                key += "_" + d.PartNumber;
            else if (component is MosfetComponent m)
                key += "_" + m.PartNumber;
            else if (component is ICComponent ic)
                key += "_" + ic.PartNumber;

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

            // Extraer información específica del tipo de componente
            if (component is ResistorComponent resistor)
            {
                item.Value = resistor.Value;
                item.Description = $"Resistor {resistor.Value} {resistor.Power} {resistor.Tolerance}";
                item.Package = "0805"; // Default, debería venir del componente
            }
            else if (component is CapacitorComponent capacitor)
            {
                item.Value = capacitor.Capacitance;
                item.Description = $"Capacitor {capacitor.Capacitance} {capacitor.VoltageRating} {capacitor.CapacitorType}";
                item.Package = "0805";
            }
            else if (component is InductorComponent inductor)
            {
                item.Value = inductor.Value;
                item.Description = $"Inductor {inductor.Value} {inductor.Isat}";
                item.PartNumber = "TBD";
            }
            else if (component is DiodeComponent diode)
            {
                item.PartNumber = diode.PartNumber;
                item.Description = $"Diode {diode.PartNumber}";
            }
            else if (component is MosfetComponent mosfet)
            {
                item.PartNumber = mosfet.PartNumber;
                item.Description = $"MOSFET {mosfet.ChannelType} {mosfet.VDS}";
            }
            else if (component is ICComponent ic)
            {
                item.PartNumber = ic.PartNumber;
                item.Description = $"IC {ic.PartNumber}";
                // Analizar codigo para descripción más detallada
                //item.Description = ic.Description;
            }

            return item;
        }

        public void ExportToCSV(BillOfMaterials bom, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                // Header
                writer.WriteLine("Item,Quantity,Designators,Type,Value,Description,Manufacturer,Part Number,Package,Unit Price,Total Price,Supplier,Stock,Datasheet");

                // Items
                foreach (var item in bom.Items)
                {
                    writer.WriteLine($"{item.ItemNumber}," +
                                   $"{item.Quantity}," +
                                   $"\"{item.Designators}\"," +
                                   $"{item.Type}," +
                                   $"{item.Value}," +
                                   $"\"{item.Description}\"," +
                                   $"{item.Manufacturer}," +
                                   $"{item.PartNumber}," +
                                   $"{item.Package}," +
                                   $"${item.UnitPrice:F2}," +
                                   $"${item.TotalPrice:F2}," +
                                   $"{item.Supplier}," +
                                   $"{item.Stock}," +
                                   $"{item.DatasheetURL}");
                }

                // Summary
                writer.WriteLine();
                writer.WriteLine($"Total Unique Components:,{bom.TotalUniqueComponents}");
                writer.WriteLine($"Total Components:,{bom.TotalComponents}");
                writer.WriteLine($"Total Cost:,${bom.TotalCost:F2}");
                writer.WriteLine($"Generated:,{bom.GeneratedDate}");
            }
        }

        public void ExportToExcel(BillOfMaterials bom, string filename)
        {
            // Requiere ClosedXML o EPPlus NuGet package
            // Ejemplo con EPPlus:

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("BOM");

                // Headers
                worksheet.Cells[1, 1].Value = "Item";
                worksheet.Cells[1, 2].Value = "Quantity";
                worksheet.Cells[1, 3].Value = "Designators";
                worksheet.Cells[1, 4].Value = "Type";
                worksheet.Cells[1, 5].Value = "Value";
                worksheet.Cells[1, 6].Value = "Description";
                worksheet.Cells[1, 7].Value = "Manufacturer";
                worksheet.Cells[1, 8].Value = "Part Number";
                worksheet.Cells[1, 9].Value = "Package";
                worksheet.Cells[1, 10].Value = "Unit Price";
                worksheet.Cells[1, 11].Value = "Total Price";
                worksheet.Cells[1, 12].Value = "Supplier";

                // Formato de headers
                using (var range = worksheet.Cells[1, 1, 1, 12])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Data
                int row = 2;
                foreach (var item in bom.Items)
                {
                    worksheet.Cells[row, 1].Value = item.ItemNumber;
                    worksheet.Cells[row, 2].Value = item.Quantity;
                    worksheet.Cells[row, 3].Value = item.Designators;
                    worksheet.Cells[row, 4].Value = item.Type;
                    worksheet.Cells[row, 5].Value = item.Value;
                    worksheet.Cells[row, 6].Value = item.Description;
                    worksheet.Cells[row, 7].Value = item.Manufacturer;
                    worksheet.Cells[row, 8].Value = item.PartNumber;
                    worksheet.Cells[row, 9].Value = item.Package;
                    worksheet.Cells[row, 10].Value = item.UnitPrice;
                    worksheet.Cells[row, 11].Value = item.TotalPrice;
                    worksheet.Cells[row, 12].Value = item.Supplier;

                    row++;
                }

                // Summary
                row += 2;
                worksheet.Cells[row, 1].Value = "Total Unique Components:";
                worksheet.Cells[row, 2].Value = bom.TotalUniqueComponents;
                worksheet.Cells[row + 1, 1].Value = "Total Components:";
                worksheet.Cells[row + 1, 2].Value = bom.TotalComponents;
                worksheet.Cells[row + 2, 1].Value = "Total Cost:";
                worksheet.Cells[row + 2, 2].Value = bom.TotalCost;
                worksheet.Cells[row + 2, 2].Style.Numberformat.Format = "$#,##0.00";

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Save
                package.SaveAs(new FileInfo(filename));
            }
        }
    }

    public class BillOfMaterials
    {
        public string ProjectName { get; set; }
        public DateTime GeneratedDate { get; set; }
        public List<BOMItem> Items { get; set; }
        public int TotalUniqueComponents { get; set; }
        public int TotalComponents { get; set; }
        public double TotalCost { get; set; }
    }

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
    }
}