using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace shop_file_upload_.common.PDF;

public class PDFService : IPDFService
{
    public byte[] GeneratePDF<T>(IEnumerable<T> items, List<PDFColumn<T>> columns, PDFOptions options)
    {
        return Document.Create(file => 
        {
            file.Page(page =>
            {
                // تنظیمات
                page.Size(PageSizes.A4);
                page.Margin(options.Margin);
                page.DefaultTextStyle(
                    f => f.FontFamily(options.FontFamily).FontSize(options.FontSize)
                    );

                // header
                page.Header().ContentFromRightToLeft().Column(h =>
                {
                    if (!string.IsNullOrWhiteSpace(options.logoPath))
                    {
                        h.Item().AlignCenter().Height(70).Image(options.logoPath);
                    }
                    h.Item().AlignCenter().Text(options.Title).Bold().FontSize(18);
                });

                // content
                page.Content().ContentFromRightToLeft().Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                foreach (var item in columns)
                                {
                                    c.RelativeColumn(item.Width);
                                }
                            });
                            table.Header(tableHeader =>
                            {
                                foreach (var item in columns)
                                {
                                    tableHeader.Cell().Background(options.HeaderBackgroundColor).Padding(5).Border(1).AlignCenter().Text(item.Header);
                                }
                            });
                            foreach (var record in items)
                            {
                                foreach(var item in columns)
                                {
                                    table.Cell().Border(1).Padding(5).AlignCenter().Text(item.Value(record));
                                }
                            }
                        });
                    }
                    );
                // footer
                if (options.ShowFooter)
                {
                    page.Footer().PaddingTop(9).Row(row =>
                    {
                        if (options.ShowPrintDate)
                        {
                            row.RelativeItem().AlignLeft().Text($"تاریخ چاپ : {DateTime.Now:yyyy/MM/dd}");
                        }
                        if (options.ShowPageNumber)
                        {
                            row.RelativeItem().AlignRight().Text(text =>
                            {
                                text.Span("صفحه ");
                                text.CurrentPageNumber();
                                text.Span("از ");
                                text.TotalPages();
                            });
                        }
                    });
                }
            });            
        }
        ).GeneratePdf();
    }
}