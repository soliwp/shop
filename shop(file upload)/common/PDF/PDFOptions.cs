using QuestPDF.Helpers;

namespace shop_file_upload_.common.PDF;

public class PDFOptions
{
    public string Title {  get; set; } = string.Empty;
    public float FontSize { get; set; } = 12;
    public float Margin { get; set; } = 20;
    public string FontFamily { get; set; } = "BNazanin_0";
    public bool ShowFooter { get; set; }
    public bool ShowPageNumber { get; set; }
    public bool ShowPrintDate { get; set; }
    public string? logoPath { get; set; }
    public string HeaderBackgroundColor { get; set; } = Colors.Grey.Lighten2;
    public string HeaderTextColor { get; set; } = Colors.Black;
}