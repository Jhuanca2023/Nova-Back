using Application.DTOs.ComentDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class ProductCommentService : IProductCommentService
    {
        private readonly IProductCommentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<Users> _userManager;

        public ProductCommentService(
            IProductCommentRepository repository,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IProductRepository productRepository,
            UserManager<Users> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _productRepository = productRepository;
            _userManager = userManager;
        }


        public async Task<CommentDto> AddCommentAsync(int productId, CreateCommentDto dto)
        {
            var user = await _currentUserService.GetUser();
            if (user == null)
                throw new InvalidOperationException("Usuario no autenticado.");

            var userId = user.Id;

            if (await _repository.ExistsByProductAndUserAsync(productId, userId))
                throw new InvalidOperationException("Ya has comentado este producto.");

            var comment = new ProductComment
            {
                ProductId = productId,
                UserId = userId,
                Comment = dto.Comment,
                Rating = dto.Rating,
                Date = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(comment);
            await _productRepository.UpdateProductPunctuationAsync(productId);

            var commentDto = _mapper.Map<CommentDto>(created);
            commentDto.AvatarUrl = await GetUserAvatarUrlAsync(userId);

            return commentDto;
        }

        public async Task<CommentDto> UpdateCommentAsync(int commentId, UpdateCommentDto dto)
        {
            var user = await _currentUserService.GetUser();
            if (user == null)
                throw new InvalidOperationException("Usuario no autenticado.");

            var userId = user.Id;

            var comment = await _repository.GetByIdAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("No existe tu comentario para actualizar.");

            if (comment.UserId != userId)
                throw new KeyNotFoundException("No existe tu comentario para actualizar.");

            _mapper.Map(dto, comment);
            comment.Date = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(comment);

            var commentDto = _mapper.Map<CommentDto>(updated);
            commentDto.AvatarUrl = await GetUserAvatarUrlAsync(comment.UserId);

            return commentDto;
        }
        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _repository.GetByIdAsync(id);
            if (comment == null)
                throw new KeyNotFoundException("No se encontró el comentario con ese Id.");

            var user = await _currentUserService.GetUser();
            if (user == null)
                throw new UnauthorizedAccessException("Usuario no autenticado.");

            // Verificar que el comentario pertenece al usuario autenticado
            if (comment.UserId != user.Id)
                throw new UnauthorizedAccessException("No tienes permisos para eliminar este comentario.");

            // Si pasa todas las validaciones, eliminar el comentario
            if (!await _repository.DeleteAsync(id))
                throw new KeyNotFoundException("No se pudo eliminar el comentario.");
        }

        public async Task<bool> HasUserCommentedAsync(int productId, string userId)
        {
            return await _repository.ExistsByProductAndUserAsync(productId, userId);
        }

        public async Task<List<CommentDto>> GetCommentsByProductIdAsync(int productId)
        {
            var comments = await _repository.GetCommentsByProductIdAsync(productId);
            var commentDtos = _mapper.Map<List<CommentDto>>(comments);

            foreach (var dto in commentDtos)
            {
                dto.AvatarUrl = await GetUserAvatarUrlAsync(dto.UserId);
            }

            return commentDtos;
        }
        private async Task<string> GetUserAvatarUrlAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var claims = await _userManager.GetClaimsAsync(user);
            var pictureClaim = claims.FirstOrDefault(c => c.Type == "picture");

            if (!string.IsNullOrEmpty(pictureClaim?.Value))
            {
                // Para URLs de Google, devolver la URL directa
                if (pictureClaim.Value.Contains("googleusercontent.com") || pictureClaim.Value.Contains("ggpht.com"))
                {
                    return pictureClaim.Value;
                }
                else
                {
                    // Para otras URLs, usar el proxy
                    return $"/api/proxy/image?url={Uri.EscapeDataString(pictureClaim.Value)}";
                }
            }
            return null;
        }
        public async Task<PagedResult<CommentDto>> GetPaginatedCommentsByProductIdAsync(int productId, int pageNumber, int pageSize)
        {
            var pagedComments = await _repository.GetPaginatedCommentsByProductIdAsync(productId, pageNumber, pageSize);
            var commentDtos = _mapper.Map<List<CommentDto>>(pagedComments.Items);

            // Añadir URLs de avatares para cada comentario
            foreach (var dto in commentDtos)
            {
                dto.AvatarUrl = await GetUserAvatarUrlAsync(dto.UserId);
            }

            return new PagedResult<CommentDto>
            {
                Items = commentDtos,
                PageNumber = pagedComments.PageNumber,
                PageSize = pagedComments.PageSize,
                TotalCount = pagedComments.TotalCount,
                TotalPages = pagedComments.TotalPages
            };
        }
    }
}

