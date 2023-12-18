using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pustok.Core.Models
{
    public class BookImage:BaseEntity
    {
        public string ImageUrl { get; set; }
        public bool? IsPoster { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
