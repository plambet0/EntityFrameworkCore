﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class PrisonerMailsInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string FullName { get; set; }
        [Required]
        [RegularExpression("The [A-Z]{1}[a-z]+")]
        public string Nickname { get; set; }
        [Range(18,65)]
        public int Age { get; set; }
        
        public string IncarcerationDate { get; set; }
        
        public string ReleaseDate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Bail { get; set; }

        public int? CellId { get; set; }

        public IEnumerable<MailInputModel> Mails { get; set; }
    }

    public class MailInputModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Sender { get; set; }
        [Required]
        [RegularExpression(@"^([A-z0-9\s]+ str.)$")]
        public string Address { get; set; }
    }
    /*{
"FullName": "",
"Nickname": "The Wallaby",
"Age": 32,
"IncarcerationDate": "29/03/1957",
"ReleaseDate": "27/03/2006",
"Bail": null,
"CellId": 5,
"Mails": [
 {
   "Description": "Invalid FullName",
   "Sender": "Invalid Sender",
   "Address": "No Address"
 },
*/
}
