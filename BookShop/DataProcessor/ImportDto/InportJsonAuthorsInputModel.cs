using BookShop.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class InportJsonAuthorsInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^(\d{3})\-(\d{3})\-(\d{4})$")]
        public string Phone { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public InportJsonBookInputModel[] Books { get; set; }
    }

   
    /*{
"FirstName": "K",
"LastName": "Tribbeck",
"Phone": "808-944-5051",
"Email": "btribbeck0@last.fm",
"Books": [
 {
   "Id": 79
 },
 {
   "Id": 40
 }
]
},
*/
}
