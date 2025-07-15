namespace Application.DTOs.ComentDTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }

        public string ProductName { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }

    }
}
