using OfficeOpenXml;

namespace shop_file_upload_.common
{
    public class ExcelService : IExcelService
    {
        public async Task<byte[]> ExportAsync<T>(List<T> data, List<Excelcolumns<T>> columns, string sheetName)
        {
            ExcelPackage.License.SetNonCommercialPersonal("soliwp");
            using var excel = new ExcelPackage();
            var sheet = excel.Workbook.Worksheets.Add(sheetName);
            for (int i = 0; i < columns.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = columns[i].columnTitle;
            }
            int row = 2;
            foreach (var item in data)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    sheet.Cells[row , i+1].Value = columns[i].value(item);
                }
                row++;
            }
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            return await excel.GetAsByteArrayAsync();
        }
        public async Task<List<excelRow>> ReadExcelAsync(IFormFile file)
        {
            ExcelPackage.License.SetNonCommercialPersonal("soliwp");
            var stream = new MemoryStream();
            var result = new List<excelRow>();
            await file.CopyToAsync(stream);
            using var excel = new ExcelPackage(stream);
            var sheet = excel.Workbook.Worksheets.First();
            var rowsCount = sheet.Dimension.Rows;
            var columnsCount = sheet.Dimension.Columns;

            for (int row = 2; row <= rowsCount; row++)
            { 
                var excelRow = new excelRow() { rowNumber = row};
                for(int column = 1;  column <= columnsCount; column++)
                {
                    excelRow.values.Add(sheet.Cells[row,column].Text.Trim());
                }
                result.Add(excelRow);
            }
            return result;
        }
    }
}
