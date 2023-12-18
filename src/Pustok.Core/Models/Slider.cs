using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pustok.Core.Models
{
    public class Slider : BaseEntity
    {
        [Required]
        [StringLength(maximumLength: 50)]
        public string Title { get; set; }
        [Required]
        [StringLength(maximumLength: 250)]
        public string Description { get; set; }
        [StringLength(maximumLength: 100)]
        public string? Image { get; set; }
        public string RedirectUrl { get; set; }
        public string ButtonText { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
