using System.ComponentModel.DataAnnotations;

namespace IbisWorld_Web.Models
{
    public class TermDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }
        public string? Definition { get; set; }
    }
}
