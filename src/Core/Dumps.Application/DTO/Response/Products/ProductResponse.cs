namespace Dumps.Application.DTO.Response.Products;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string CodeTitle { get; set; }
    public string Description { get; set; }
    public long Price { get; set; }
    public int Discount { get; set; } = 0;
    public ProductVersionResponse? CurrentVersion { get; set; } // Include current version details

}

public class ProductVersionResponse
{
    public Guid Id { get; set; }
    public float VersionNumber { get; set; }
    public string PdfUrl { get; set; }
    public string FileName { get; set; }
}

