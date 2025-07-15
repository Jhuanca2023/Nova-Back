using Application.DTOs.ComentDTOs;

namespace Application.DTOs.ProductsDTOs;

public record ProductsWithCommentsDto:ProductResponseDTO
{
    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
}