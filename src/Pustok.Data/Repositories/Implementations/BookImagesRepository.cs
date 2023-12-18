using Pustok.Core.Repositories.Interfaces;
using Pustok.Core.Models;

using Pustok.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pustok.Core.Repositories.Interfaces.IBookImageRepository;

namespace Pustok.Data.Repositories.Implementations
{
    public class BookImagesRepository : GenericRepository<BookImage>, IBookImagesRepository
    {
        public BookImagesRepository(PustokContext context) : base(context)
        {
        }
    }
}
