namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlSerializer = new XmlSerializer(
                typeof(BookImportXmlInputModel[]),
                new XmlRootAttribute("Books"));
            var books =
                (BookImportXmlInputModel[])xmlSerializer.Deserialize(
                    new StringReader(xmlString));

            foreach (var xmlBook in books)
            {
                if (!IsValid(xmlBook) || xmlBook.Pages <50 || xmlBook.Pages>5000)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }
                bool parsedDate = DateTime.TryParseExact(
                    xmlBook.PublishedOn, "MM/dd/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date); ;
                if (!parsedDate)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }
                var book = new Book
                {
                    Name = xmlBook.Name,
                    Genre = Enum.Parse<Genre>(xmlBook.Genre),
                    Price = xmlBook.Price,
                    Pages = xmlBook.Pages,
                    PublishedOn = date

                };
                context.Books.Add(book);
                sb.AppendLine($"Successfully imported book {book.Name} for {book.Price:f2}.");
                
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var authorDTOs = JsonConvert.DeserializeObject<InportJsonAuthorsInputModel[]>(jsonString);
            List<Author> authors = new List<Author>();
            
            foreach (var authorDTO in authorDTOs)
            {
                if (!IsValid(authorDTO))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }
                if (authors.Any(a=>a.Email == authorDTO.Email))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }
                var author = new Author
                {
                    FirstName = authorDTO.FirstName,
                    LastName = authorDTO.LastName,
                    Email = authorDTO.Email,
                    Phone = authorDTO.Phone,

                };

                foreach (var bookDTO in authorDTO.Books)
                {
                    if (!bookDTO.BookId.HasValue)
                    {
                        continue;
                    }

                    var book = context.Books.FirstOrDefault(b => b.Id == bookDTO.BookId);
                    if (book == null)
                    {
                        continue;
                    }
                    author.AuthorsBooks.Add(new AuthorBook
                    {
                        Author = author,
                        Book = book
                    });
                    
                }
                if (author.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }
                foreach (var bookDto in authorDTO.Books)
                {
                    var book = context.Books.FirstOrDefault(b => b.Id == bookDto.BookId);
                    
                    
                }
                authors.Add(author);
                sb.AppendLine($"Successfully imported author - {author.FirstName + ' ' + author.LastName} with {author.AuthorsBooks.Count()} books.");

            }
            context.Authors.AddRange(authors);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}