using Application.DTOs.ComentDTOs;

namespace Application.DTOs.ProductsDTOs;

public record ProductWithPaginatedCommentsDto : ProductResponseDTO
{
    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
    public int TotalCommentsCount { get; set; }
    public int CommentsPageNumber { get; set; }
    public int CommentsPageSize { get; set; }
    public int CommentsTotalPages { get; set; }
}