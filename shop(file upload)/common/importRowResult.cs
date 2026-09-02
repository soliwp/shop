namespace shop_file_upload_.common;

public class importRowResult<T>
{
    public int rowNumber { get;set; }
    public string Title { get;set; }
    public bool success { get;set; }
    public string message { get;set; }= string.Empty;
    public T data { get; set; }
}
