namespace Application.DTOs.ProductsDTOs;

public class ProductWithCommentsPaginatedResponseDto
{
    public IEnumerable<ProductsWithCommentsDto> Products { get; set; }
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}