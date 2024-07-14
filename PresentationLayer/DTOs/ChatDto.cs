namespace PresentationLayer.DTOs
{
    public record ChatDto
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string AuthorName { get; set; } //UserId
    }
}
