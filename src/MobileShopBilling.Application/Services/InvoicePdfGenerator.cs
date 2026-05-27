using MobileShopBilling.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MobileShopBilling.Application.Services;

public static class InvoicePdfGenerator
{
    public static byte[] Generate(Invoice invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(invoice.Tenant?.ShopName ?? "Mobile Shop")
                                .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                            c.Item().Text(invoice.Tenant?.Address ?? "");
                            c.Item().Text($"GST: {invoice.Tenant?.GstNumber ?? "N/A"}");
                            c.Item().Text($"Phone: {invoice.Tenant?.Phone ?? ""}");
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("TAX INVOICE").FontSize(16).Bold();
                            c.Item().Text($"Invoice #: {invoice.InvoiceNumber}").Bold();
                            c.Item().Text($"Date: {invoice.InvoiceDate:dd-MMM-yyyy}");
                            c.Item().Text($"Status: {invoice.Status}");
                        });
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                    if (invoice.Customer != null)
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Bill To:").Bold();
                                c.Item().Text(invoice.Customer.Name);
                                c.Item().Text(invoice.Customer.Phone);
                                if (!string.IsNullOrEmpty(invoice.Customer.Email))
                                    c.Item().Text(invoice.Customer.Email);
                                if (!string.IsNullOrEmpty(invoice.Customer.GstNumber))
                                    c.Item().Text($"GST: {invoice.Customer.GstNumber}");
                            });
                        });
                        col.Item().PaddingVertical(5);
                    }
                });

                page.Content().Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("#").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Product").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Qty").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Price").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Disc%").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Tax%").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .AlignRight().Text("Total").FontColor(Colors.White).Bold();
                        });

                        var idx = 1;
                        foreach (var item in invoice.Items)
                        {
                            var bg = idx % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                            table.Cell().Background(bg).Padding(4).Text($"{idx}");
                            table.Cell().Background(bg).Padding(4).Text(item.ProductName);
                            table.Cell().Background(bg).Padding(4).Text($"{item.Quantity}");
                            table.Cell().Background(bg).Padding(4).Text($"{item.UnitPrice:N2}");
                            table.Cell().Background(bg).Padding(4).Text($"{item.DiscountPercent:N1}%");
                            table.Cell().Background(bg).Padding(4).Text($"{item.TaxPercent:N1}%");
                            table.Cell().Background(bg).Padding(4).AlignRight().Text($"{item.TotalPrice:N2}");
                            idx++;
                        }
                    });

                    col.Item().PaddingTop(10).AlignRight().Width(250).Column(summary =>
                    {
                        summary.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Subtotal:");
                            row.ConstantItem(100).AlignRight().Text($"{invoice.SubTotal:N2}");
                        });
                        summary.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Discount:");
                            row.ConstantItem(100).AlignRight().Text($"-{invoice.DiscountAmount:N2}");
                        });
                        summary.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Tax (GST):");
                            row.ConstantItem(100).AlignRight().Text($"+{invoice.TaxAmount:N2}");
                        });
                        summary.Item().PaddingVertical(3).LineHorizontal(1);
                        summary.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Total:").Bold().FontSize(12);
                            row.ConstantItem(100).AlignRight()
                                .Text($"{invoice.TotalAmount:N2}").Bold().FontSize(12);
                        });
                    });

                    if (invoice.Payment != null)
                    {
                        col.Item().PaddingTop(15).Column(pay =>
                        {
                            pay.Item().Text("Payment Details:").Bold();
                            pay.Item().Text($"Mode: {invoice.Payment.PaymentMode}");
                            if (!string.IsNullOrEmpty(invoice.Payment.TransactionReference))
                                pay.Item().Text($"Reference: {invoice.Payment.TransactionReference}");
                        });
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Thank you for your purchase! ");
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}
