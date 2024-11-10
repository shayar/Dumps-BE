using Microsoft.AspNetCore.Http;

namespace Dumps.Application.DTO.Request.Products
{
    public class CreateProductRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public long Price { get; set; }
        public string CodeTitle { get; set; }
        public int Discount { get; set; } = 0;
        public IFormFile PdfFile { get; set; }  // PDF File for upload
    }
}
