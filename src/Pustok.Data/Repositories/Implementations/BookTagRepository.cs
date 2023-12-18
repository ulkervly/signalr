using Pustok.Core.Repositories.Interfaces;
using Pustok.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pustok.Core.Models;

namespace Pustok.Data.Repositories.Implementations
{
    public class BookTagRepository : GenericRepository<BookTag>, IBookTagsRepository
    {
        public BookTagRepository(PustokContext context) : base(context)
        {
        }
    }
}
