namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var jsonDepartments = JsonConvert.DeserializeObject<IEnumerable<DepartmentCellsInputModelcs>>(jsonString);
            var sb = new StringBuilder();

            foreach (var jsonDepartment in jsonDepartments)
            {
                if (!IsValid(jsonDepartment) || !jsonDepartment.Cells.All(IsValid) || jsonDepartment.Cells.Count() == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var department = new Department
                {
                    Name = jsonDepartment.Name,
                    Cells = jsonDepartment.Cells.Select(x => new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    }).ToList()
                };
                context.Departments.Add(department);
                
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var jsonPrisoners = JsonConvert.DeserializeObject<IEnumerable<PrisonerMailsInputModel>>(jsonString);
            foreach (var jsonPrisoner in jsonPrisoners)
            {
                if (!IsValid(jsonPrisoner) || !(jsonPrisoner.Mails.All(IsValid)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                //dd/MM/yyyy
                var isValidReleaseDate = DateTime.TryParseExact(
                    jsonPrisoner.ReleaseDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime releaseDate);

                var incarcerationDate = DateTime.ParseExact(
                    jsonPrisoner.IncarcerationDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);
                var prisoner = new Prisoner
                {
                    FullName = jsonPrisoner.FullName,
                    Nickname = jsonPrisoner.Nickname,
                    Age = jsonPrisoner.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = isValidReleaseDate ? (DateTime?)releaseDate : null,
                    Bail = jsonPrisoner.Bail,
                    CellId = jsonPrisoner.CellId,
                    Mails = jsonPrisoner.Mails.Select(x => new Mail
                    {
                        Description = x.Description,
                        Sender = x.Sender,
                        Address = x.Address
                    }).ToList()

                };
                context.Prisoners.Add(prisoner);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
                context.SaveChanges();
            
            }
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlOfficers = XmlConverter.Deserializer<OfficersPrisonersXmlInputModel>(xmlString, "Officers");

            foreach (var xmlOfficer in xmlOfficers)
            {
                if (!IsValid(xmlOfficer))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var officer = new Officer
                {
                    FullName = xmlOfficer.FullName,
                    Salary = xmlOfficer.Money,
                    Position = Enum.Parse<Position>(xmlOfficer.Position),
                    Weapon = Enum.Parse<Weapon>(xmlOfficer.Weapon),
                    DepartmentId = xmlOfficer.DepartmentId,
                    OfficerPrisoners = xmlOfficer.Prisoners.Select(x => new OfficerPrisoner
                    {
                        PrisonerId = x.Id
                    }).ToArray()

                };
                context.Officers.Add(officer);
                context.SaveChanges();
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");

            }
           
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}