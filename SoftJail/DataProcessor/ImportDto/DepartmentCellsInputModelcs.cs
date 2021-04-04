﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentCellsInputModelcs
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }

        public IEnumerable<CellInputModel> Cells { get; set; }
    }

    public class CellInputModel
    {
        [Range(1,1000)]
        public int CellNumber { get; set; }
        [Required]
        public bool HasWindow { get; set; }
    }
    /*{
"Name": "",
"Cells": [
 {
   "CellNumber": 101,
   "HasWindow": true
 },
 {
   "CellNumber": 102,
   "HasWindow": false
 }
]
},
*/
}
