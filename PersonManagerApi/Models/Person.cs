using System.ComponentModel.DataAnnotations;

namespace PersonManagerApi.Models
{
    public class Person
    {
        public Guid Id { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }
    }
}
