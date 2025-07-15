using Domain.Interfaces;
using Domain.Entities;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Intrastructure.Repositories;

namespace Intrastructure.Report
{
    public class PdfGeneratorUsers : IPdfGeneratorUsers
    {
        public byte[] GenerateProductReport(List<Users> users)
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

                    // Header
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Reporte de Usuarios")
                                .FontSize(18).FontColor("#6a5bff").SemiBold();
                        });

                        row.ConstantItem(150).Column(col =>
                        {
                            col.Item().AlignCenter()
                                .Width(120).Height(60)
                                .Svg(svgContent);
                        });
                    });

                    // Main content
                    page.Content().Column(content =>
                    {
                        content.Item().PaddingVertical(10)
                            .LineHorizontal(1).LineColor("#6a5bff");

                        content.Item().PaddingTop(10).Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);    // Nombre completo
                                cols.RelativeColumn(3);    // Email
                                cols.RelativeColumn(2);    // Último acceso
                                cols.ConstantColumn(60);   // Estado
                            });

                            // Header
                            table.Header(header =>
                            {
                                void HeaderCell(string text) => header.Cell()
                                    .Background("#6a5bff").PaddingVertical(5).PaddingHorizontal(3)
                                    .Text(text).FontColor("#ffffff").Bold().AlignCenter();

                                HeaderCell("Nombre Completo");
                                HeaderCell("Correo");
                                HeaderCell("Último Acceso");
                                HeaderCell("Estado");
                            });

                            // Rows
                            foreach (var user in users)
                            {
                                // Full name
                                table.Cell().Padding(5).AlignLeft()
                                    .Text($"{user.FirstName} {user.LastName}");

                                // Email
                                table.Cell().Padding(5).AlignLeft()
                                    .Text(user.Email);

                                // Last login
                                table.Cell().Padding(5).AlignCenter()
                                    .Text(user.LastLogin?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca");

                                // Status
                                table.Cell().Padding(5).AlignCenter()
                                    .Text(!user.LockoutEnabled || (user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow) 
                                        ? "Activo" 
                                        : "Inactivo");
                            }
                        });
                    });

                    // Footer
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