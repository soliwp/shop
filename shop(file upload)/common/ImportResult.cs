namespace shop_file_upload_.common;

public class ImportResult<T>
{
    public List<importRowResult<T>> rows { get; set; } = new();
    public int SuccessRows { get; set; }
    public int failedRows { get; set; }
}