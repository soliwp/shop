namespace shop_file_upload_.common
{
    public interface IExcelService
    {
        Task<byte[]> ExportAsync<T> (List<T> data, List<Excelcolumns<T>> columns , string sheetName);
        Task<List<excelRow>> ReadExcelAsync(IFormFile file);
    }
}
