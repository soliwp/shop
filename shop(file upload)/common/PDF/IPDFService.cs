namespace shop_file_upload_.common.PDF;
public interface IPDFService
{
    byte[] GeneratePDF<T>(IEnumerable<T> items , List<PDFColumn<T>> columns , PDFOptions options);
}
