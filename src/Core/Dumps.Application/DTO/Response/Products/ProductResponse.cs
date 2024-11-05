namespace Dumps.Application.DTO.Response.Products;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string CodeTitle { get; set; }
    public string Description { get; set; }
    public long Price { get; set; }
    public int Discount { get; set; } = 0;
}
