namespace Dumps.Application.DTO.Response.Products;

public class GetAllProductResponse
{
    public bool Succeeded { get; set; }

    public List<ProductResponse> Products { get; set; }

    public IEnumerable<string> Errors { get; set; }
}
