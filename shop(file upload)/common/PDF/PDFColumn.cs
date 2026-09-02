namespace shop_file_upload_.common.PDF;
public class PDFColumn<T>
{
    public string Header { get; set; } = string.Empty;
    public Func<T, string> Value { get; set; }
    public int Width { get; set; } = 1;
}
