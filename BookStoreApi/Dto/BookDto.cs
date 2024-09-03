namespace BookStoreApi.Dto
{
    public class BookDto
    {
        public string? Id { get; set; }
        public string BookName { get; set; } = null!;

        public decimal Price { get; set; }

        public string Category { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Content { get; set; } = null!;
    }
}