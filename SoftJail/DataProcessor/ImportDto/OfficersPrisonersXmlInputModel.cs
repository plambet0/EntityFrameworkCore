using SoftJail.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class OfficersPrisonersXmlInputModel
    {
        [Required]
        [XmlElement("Name")]
        [MinLength(3)]
        [MaxLength(30)]
        public string FullName { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Money { get; set; }
        [EnumDataType(typeof(Position))]
        [XmlElement("Position")]
        public string Position { get; set; }
        [EnumDataType(typeof(Weapon))]
        [XmlElement("Weapon")]
        public string Weapon { get; set; }

        public int DepartmentId { get; set; }
        [XmlArray("Prisoners")]
        public PrisonerXmlInputModel[] Prisoners { get; set; }
    }
    [XmlType("Prisoner")]
    public class PrisonerXmlInputModel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
    /*<Officer>
<Name>Paddy Weiner</Name>
<Money>2854.56</Money>
<Position>Guard</Position>
<Weapon>ChainRifle</Weapon>
<DepartmentId>3</DepartmentId>
<Prisoners>
 <Prisoner id="4" />
 <Prisoner id="1" />
*/
}
