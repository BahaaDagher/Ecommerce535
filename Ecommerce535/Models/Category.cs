using Ecommerce535.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce535.Models
{
    public class Category
    {
        public int Id { get; set; }
        //[Required]
        //[MinLength(2)]
        //[MaxLength(20)]
        [MinMaxCustomeLengthAttribute(2 ,20)]
        public string  Name { get; set; }
        public string? Description  { get; set; }
        public bool Status{ get; set; }
    }
}
