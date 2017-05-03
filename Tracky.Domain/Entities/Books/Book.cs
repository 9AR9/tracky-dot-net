namespace Tracky.Domain.Entities.Books
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }

        public int AuthorId { get; set; }           // Foreign key
        public Author Author { get; set; }          // Navigation property
    }
}