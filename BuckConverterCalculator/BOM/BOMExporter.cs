using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuckConverterCalculator.BOM
{
    /// <summary>
    /// Exportador de BOM a múltiples formatos
    /// </summary>
    public class BOMExporter
    {
        /// <summary>
        /// Exporta BOM a formato CSV
        /// </summary>
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
                writer.WriteLine($"Project:,{bom.ProjectName}");
                writer.WriteLine($"Generated:,{bom.GeneratedDate:yyyy-MM-dd HH:mm:ss}");
            }
        }

        /// <summary>
        /// Exporta BOM a formato Excel usando EPPlus
        /// </summary>
        public void ExportToExcel(BillOfMaterials bom, string filename)
        {
            // Requiere EPPlus NuGet package
            // Install-Package EPPlus

            try
            {
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("BOM");

                    // Título del proyecto
                    worksheet.Cells[1, 1].Value = bom.ProjectName;
                    worksheet.Cells[1, 1].Style.Font.Size = 16;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;

                    worksheet.Cells[2, 1].Value = $"Generated: {bom.GeneratedDate:yyyy-MM-dd HH:mm:ss}";
                    worksheet.Cells[2, 1].Style.Font.Italic = true;

                    // Headers
                    int headerRow = 4;
                    worksheet.Cells[headerRow, 1].Value = "Item";
                    worksheet.Cells[headerRow, 2].Value = "Quantity";
                    worksheet.Cells[headerRow, 3].Value = "Designators";
                    worksheet.Cells[headerRow, 4].Value = "Type";
                    worksheet.Cells[headerRow, 5].Value = "Value";
                    worksheet.Cells[headerRow, 6].Value = "Description";
                    worksheet.Cells[headerRow, 7].Value = "Manufacturer";
                    worksheet.Cells[headerRow, 8].Value = "Part Number";
                    worksheet.Cells[headerRow, 9].Value = "Package";
                    worksheet.Cells[headerRow, 10].Value = "Unit Price";
                    worksheet.Cells[headerRow, 11].Value = "Total Price";
                    worksheet.Cells[headerRow, 12].Value = "Supplier";
                    worksheet.Cells[headerRow, 13].Value = "Stock";
                    worksheet.Cells[headerRow, 14].Value = "Datasheet";

                    // Formato de headers
                    using (var range = worksheet.Cells[headerRow, 1, headerRow, 14])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                        range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    // Data
                    int row = headerRow + 1;
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
                        worksheet.Cells[row, 13].Value = item.Stock;

                        // Hyperlink para datasheet
                        if (!string.IsNullOrEmpty(item.DatasheetURL))
                        {
                            worksheet.Cells[row, 14].Hyperlink = new Uri(item.DatasheetURL);
                            worksheet.Cells[row, 14].Value = "Datasheet";
                            worksheet.Cells[row, 14].Style.Font.UnderLine = true;
                            worksheet.Cells[row, 14].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        }

                        row++;
                    }

                    // Formato de precios
                    worksheet.Cells[headerRow + 1, 10, row - 1, 11].Style.Numberformat.Format = "$#,##0.00";

                    // Summary
                    row += 2;
                    worksheet.Cells[row, 1].Value = "Total Unique Components:";
                    worksheet.Cells[row, 2].Value = bom.TotalUniqueComponents;
                    worksheet.Cells[row, 1].Style.Font.Bold = true;

                    row++;
                    worksheet.Cells[row, 1].Value = "Total Components:";
                    worksheet.Cells[row, 2].Value = bom.TotalComponents;
                    worksheet.Cells[row, 1].Style.Font.Bold = true;

                    row++;
                    worksheet.Cells[row, 1].Value = "Total Cost:";
                    worksheet.Cells[row, 2].Value = bom.TotalCost;
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "$#,##0.00";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.Font.Size = 12;
                    worksheet.Cells[row, 2].Style.Font.Bold = true;
                    worksheet.Cells[row, 2].Style.Font.Size = 12;

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Freeze header row
                    worksheet.View.FreezePanes(headerRow + 1, 1);

                    // Save
                    var file = new FileInfo(filename);
                    package.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to Excel: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exporta BOM a formato HTML
        /// </summary>
        public void ExportToHTML(BillOfMaterials bom, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine("<html>");
                writer.WriteLine("<head>");
                writer.WriteLine($"<title>BOM - {bom.ProjectName}</title>");
                writer.WriteLine("<style>");
                writer.WriteLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                writer.WriteLine("h1 { color: #333; }");
                writer.WriteLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
                writer.WriteLine("th { background-color: #4F81BD; color: white; padding: 10px; text-align: left; }");
                writer.WriteLine("td { border: 1px solid #ddd; padding: 8px; }");
                writer.WriteLine("tr:nth-child(even) { background-color: #f2f2f2; }");
                writer.WriteLine(".summary { margin-top: 30px; font-size: 14px; }");
                writer.WriteLine(".total { font-weight: bold; font-size: 16px; }");
                writer.WriteLine("</style>");
                writer.WriteLine("</head>");
                writer.WriteLine("<body>");

                writer.WriteLine($"<h1>Bill of Materials - {bom.ProjectName}</h1>");
                writer.WriteLine($"<p>Generated: {bom.GeneratedDate:yyyy-MM-dd HH:mm:ss}</p>");

                writer.WriteLine("<table>");
                writer.WriteLine("<tr>");
                writer.WriteLine("<th>Item</th><th>Qty</th><th>Designators</th><th>Type</th><th>Value</th>");
                writer.WriteLine("<th>Description</th><th>Manufacturer</th><th>Part Number</th><th>Package</th>");
                writer.WriteLine("<th>Unit Price</th><th>Total Price</th><th>Supplier</th><th>Datasheet</th>");
                writer.WriteLine("</tr>");

                foreach (var item in bom.Items)
                {
                    writer.WriteLine("<tr>");
                    writer.WriteLine($"<td>{item.ItemNumber}</td>");
                    writer.WriteLine($"<td>{item.Quantity}</td>");
                    writer.WriteLine($"<td>{item.Designators}</td>");
                    writer.WriteLine($"<td>{item.Type}</td>");
                    writer.WriteLine($"<td>{item.Value}</td>");
                    writer.WriteLine($"<td>{item.Description}</td>");
                    writer.WriteLine($"<td>{item.Manufacturer}</td>");
                    writer.WriteLine($"<td>{item.PartNumber}</td>");
                    writer.WriteLine($"<td>{item.Package}</td>");
                    writer.WriteLine($"<td>${item.UnitPrice:F2}</td>");
                    writer.WriteLine($"<td>${item.TotalPrice:F2}</td>");
                    writer.WriteLine($"<td>{item.Supplier}</td>");

                    if (!string.IsNullOrEmpty(item.DatasheetURL))
                        writer.WriteLine($"<td><a href='{item.DatasheetURL}' target='_blank'>PDF</a></td>");
                    else
                        writer.WriteLine("<td>-</td>");

                    writer.WriteLine("</tr>");
                }

                writer.WriteLine("</table>");

                writer.WriteLine("<div class='summary'>");
                writer.WriteLine($"<p>Total Unique Components: <strong>{bom.TotalUniqueComponents}</strong></p>");
                writer.WriteLine($"<p>Total Components: <strong>{bom.TotalComponents}</strong></p>");
                writer.WriteLine($"<p class='total'>Total Cost: <strong>${bom.TotalCost:F2}</strong></p>");
                writer.WriteLine("</div>");

                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }

        /// <summary>
        /// Exporta BOM a formato de texto plano
        /// </summary>
        public void ExportToText(BillOfMaterials bom, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine("==================================================");
                writer.WriteLine($"  BILL OF MATERIALS - {bom.ProjectName}");
                writer.WriteLine("==================================================");
                writer.WriteLine($"Generated: {bom.GeneratedDate:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();

                // Calcular anchos de columna
                int[] widths = { 5, 5, 15, 10, 12, 30, 15, 20 };

                // Header
                writer.WriteLine(PadRight("Item", widths[0]) +
                               PadRight("Qty", widths[1]) +
                               PadRight("Designators", widths[2]) +
                               PadRight("Type", widths[3]) +
                               PadRight("Value", widths[4]) +
                               PadRight("Description", widths[5]) +
                               PadRight("Part Number", widths[6]) +
                               PadRight("Price", widths[7]));

                writer.WriteLine(new string('-', widths.Sum()));

                // Items
                foreach (var item in bom.Items)
                {
                    writer.WriteLine(PadRight(item.ItemNumber.ToString(), widths[0]) +
                                   PadRight(item.Quantity.ToString(), widths[1]) +
                                   PadRight(item.Designators, widths[2]) +
                                   PadRight(item.Type, widths[3]) +
                                   PadRight(item.Value, widths[4]) +
                                   PadRight(Truncate(item.Description, widths[5]), widths[5]) +
                                   PadRight(item.PartNumber, widths[6]) +
                                   PadRight($"${item.TotalPrice:F2}", widths[7]));
                }

                writer.WriteLine(new string('-', widths.Sum()));

                // Summary
                writer.WriteLine();
                writer.WriteLine($"Total Unique Components: {bom.TotalUniqueComponents}");
                writer.WriteLine($"Total Components: {bom.TotalComponents}");
                writer.WriteLine($"TOTAL COST: ${bom.TotalCost:F2}");
            }
        }

        private string PadRight(string text, int width)
        {
            if (text == null) text = "";
            if (text.Length > width) return text.Substring(0, width);
            return text.PadRight(width);
        }

        private string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Length <= maxLength) return text;
            return text.Substring(0, maxLength - 3) + "...";
        }
    }
}