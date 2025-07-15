using Application.DTOs.ComentDTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IProductCommentService
{
    Task<CommentDto> AddCommentAsync(int productId, CreateCommentDto dto);
    Task<CommentDto> UpdateCommentAsync(int productId, UpdateCommentDto dto);
    Task DeleteCommentAsync(int id);
    Task<bool> HasUserCommentedAsync(int productId, string userId); 
    
    Task<List<CommentDto>> GetCommentsByProductIdAsync(int productId);
    Task<PagedResult<CommentDto>> GetPaginatedCommentsByProductIdAsync(int productId, int pageNumber, int pageSize);
    
}