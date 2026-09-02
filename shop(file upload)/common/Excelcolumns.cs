namespace shop_file_upload_.common
{
    public class Excelcolumns<T>
    {
        public string columnTitle { get; set; } = string.Empty;
        public Func<T , object?> value { get; set; }  // for example = p => p.name  - p is T and object is p.name
    }
}
