using Ecommerce535.Models;

namespace Ecommerce535.ViewModels
{
    public class PersonVM
    {
        public IEnumerable<Person> Persons { get; set; }
        public int Count  { get; set; }
    }
}
