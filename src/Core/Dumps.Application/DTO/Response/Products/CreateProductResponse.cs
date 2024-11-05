namespace Dumps.Application.DTO.Response.Products
{
    public class CreateProductResponse
    {
        public bool Succeeded { get; set; }
        public Guid ProductId { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
