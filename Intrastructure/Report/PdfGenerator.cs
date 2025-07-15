using Domain.Interfaces;
using Intrastructure.Repositories;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Intrastructure.Report
{
    public class PdfGenerator : IPdfGenerator
    {

        public byte[] GenerateProductReport(List<Product> products)
        {
            var svgContent = IconHelper.LoadSvgContent("neonnova");
            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.Background().Background("#ffffff");

                    // Encabezado
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Reporte de Productos")
                                .FontSize(18).FontColor("#6a5bff").SemiBold();
                        });

                        row.ConstantItem(150).Column(col =>
                        {
                            col.Item().AlignCenter()
                                .Width(120).Height(60)
                                .Svg(svgContent);
                        });
                    });

                    // Contenido principal
                    page.Content().Column(content =>
                    {
                        content.Item().PaddingVertical(10)
                            .LineHorizontal(1).LineColor("#6a5bff");

                        content.Item().PaddingTop(10).Table(table =>
                        {
                            // Definir columnas
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(40);   // ID
                                cols.RelativeColumn(3);    // Nombre
                                cols.RelativeColumn(2);    // Precio
                                cols.ConstantColumn(50);   // Stock
                                cols.RelativeColumn(3);    // Categoría Nombre
                            });

                            // Encabezado
                            table.Header(header =>
                            {
                                void HeaderCell(string text) => header.Cell()
                                    .Background("#6a5bff").PaddingVertical(5).PaddingHorizontal(3)
                                    .Text(text).FontColor("#ffffff").Bold().AlignCenter();

                                HeaderCell("ID");
                                HeaderCell("Nombre");
                                HeaderCell("Precio");
                                HeaderCell("Stock");
                                HeaderCell("Categoría");
                            });

                            // Filas
                            foreach (var p in products)
                            {
                                table.Cell().Padding(5).AlignCenter().Text(p.Id.ToString());
                                table.Cell().Padding(5).AlignLeft().Text(p.Name);
                                table.Cell().Padding(5).AlignRight().Text($"S/ {p.Price:0.00}");
                                table.Cell().Padding(5).AlignCenter().Text(p.Stock.ToString());
                                table.Cell().Padding(5).AlignCenter().Text(p.Category?.Name?? "Categoría no disponible"); // Manejo de null
                            }
                        });
                    });

                    // Pie de página
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().PaddingBottom(5).Text("RUC: 20602345678").FontSize(10).FontColor("#6a5bff");
                            col.Item().PaddingBottom(5).Text("Dirección: Av. Larco 782, Lima").FontSize(10);
                            col.Item().PaddingBottom(5).Text("Teléfono: +51 951 890 315 | jhon3122@gmail.com").FontSize(10);
                        });

                        row.ConstantItem(150).Column(col =>
                        {
                            col.Item().PaddingTop(10).Text("Fecha de emisión:")
                                .FontSize(10).Bold().FontColor("#6a5bff");

                            col.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                                .FontSize(12).Bold();
                        });
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
    }