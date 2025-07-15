namespace Application.DTOs.UsersDTOs;

public class UserDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public required string Email { get; set; }
    public string? AvatarUrl { get; set; }
    public string InitialAvatar { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsGoogleAccount { get; set; }
}