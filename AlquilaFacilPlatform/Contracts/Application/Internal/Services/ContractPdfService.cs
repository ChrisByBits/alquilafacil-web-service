using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AlquilaFacilPlatform.Contracts.Application.Internal.Services;

public interface IContractPdfService
{
    byte[] GeneratePdf(ContractInstance contract);
}

public class ContractPdfService : IContractPdfService
{
    public ContractPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GeneratePdf(ContractInstance contract)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, contract));
                page.Content().Element(c => ComposeContent(c, contract));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, ContractInstance contract)
    {
        container.Column(column =>
        {
            column.Item().AlignCenter().Text("CONTRATO DE ARRENDAMIENTO")
                .FontSize(16).Bold().FontColor(Colors.Blue.Darken3);

            column.Item().Height(10);

            column.Item().AlignCenter().Text($"Contrato N° {contract.Id}")
                .FontSize(10).FontColor(Colors.Grey.Darken1);

            column.Item().Height(5);

            column.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken3);

            column.Item().Height(15);
        });
    }

    private void ComposeContent(IContainer container, ContractInstance contract)
    {
        container.Column(column =>
        {
            // Contract content
            var lines = contract.GeneratedContent.Split('\n');
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    column.Item().Height(10);
                }
                else if (line.Trim().StartsWith("CLAUSULA") || line.Trim().StartsWith("PRIMERA") ||
                         line.Trim().StartsWith("SEGUNDA") || line.Trim().StartsWith("TERCERA") ||
                         line.Trim().StartsWith("CUARTA") || line.Trim().StartsWith("QUINTA") ||
                         line.Trim().StartsWith("SEXTA") || line.Trim().StartsWith("SEPTIMA") ||
                         line.Trim().StartsWith("COMPARECEN") || line.Trim().StartsWith("CONTRATO"))
                {
                    column.Item().Height(5);
                    column.Item().Text(line).Bold();
                    column.Item().Height(3);
                }
                else
                {
                    column.Item().Text(line).LineHeight(1.4f);
                }
            }

            column.Item().Height(30);

            // Signature section
            column.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
            column.Item().Height(20);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("ARRENDADOR").Bold();
                    col.Item().Height(60);
                    col.Item().AlignCenter().LineHorizontal(1).LineColor(Colors.Black);
                    col.Item().Height(5);
                    col.Item().AlignCenter().Text("Firma").FontSize(9);
                });

                row.ConstantItem(50);

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("ARRENDATARIO").Bold();
                    col.Item().Height(10);

                    // Insert signature image if available
                    if (!string.IsNullOrEmpty(contract.TenantSignature))
                    {
                        try
                        {
                            // Check if it's a base64 data URL
                            var base64Data = contract.TenantSignature;
                            if (base64Data.Contains(","))
                            {
                                base64Data = base64Data.Split(',')[1];
                            }

                            var imageBytes = Convert.FromBase64String(base64Data);
                            col.Item().Height(50).AlignCenter().Image(imageBytes).FitArea();
                        }
                        catch
                        {
                            col.Item().Height(50);
                        }
                    }
                    else
                    {
                        col.Item().Height(50);
                    }

                    col.Item().AlignCenter().LineHorizontal(1).LineColor(Colors.Black);
                    col.Item().Height(5);
                    col.Item().AlignCenter().Text("Firma").FontSize(9);
                });
            });

            column.Item().Height(20);

            // Contract metadata
            if (contract.SignedAt.HasValue)
            {
                column.Item().AlignCenter()
                    .Text($"Documento firmado digitalmente el {contract.SignedAt.Value:dd/MM/yyyy HH:mm}")
                    .FontSize(9).FontColor(Colors.Grey.Darken1);
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
            column.Item().Height(5);
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("AlquilaFacil - ").FontSize(8).FontColor(Colors.Grey.Medium);
                    text.Span("Plataforma de Alquiler de Espacios").FontSize(8).FontColor(Colors.Grey.Medium);
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Pagina ").FontSize(8);
                    text.CurrentPageNumber().FontSize(8);
                    text.Span(" de ").FontSize(8);
                    text.TotalPages().FontSize(8);
                });
            });
        });
    }
}
