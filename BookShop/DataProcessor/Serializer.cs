namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors.Select(x => new
            {
                AuthorName = x.FirstName + " " + x.LastName,
                Books = x.AuthorsBooks.OrderByDescending(b => b.Book.Price).Select(b => new
                {
                    BookName = b.Book.Name,
                    BookPrice = b.Book.Price.ToString("f2")
                })
                .ToArray()

            })
            .ToArray()
            .OrderByDescending(x => x.Books.Count())
            .ThenBy(x => x.AuthorName)
            .ToArray();
            

            return JsonConvert.SerializeObject(authors, Formatting.Indented);
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books
                .Where(x => x.PublishedOn < date && x.Genre == Data.Models.Genre.Science)
                .ToArray()
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Take(10)
                .Select(b => new ExportBookDTO
                {
                    
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                    Pages = b.Pages
                })
            .ToArray();

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportBookDTO[]),
                    new XmlRootAttribute("Books"));
            var sw = new StringWriter();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            xmlSerializer.Serialize(sw, books, ns);
            return sw.ToString();
        }
    }
}