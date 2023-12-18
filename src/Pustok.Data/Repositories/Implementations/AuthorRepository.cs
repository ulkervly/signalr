using Pustok.Core.Models;
using Pustok.Core.Repositories.Interfaces;
using Pustok.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pustok.Core.Repositories.Interfaces.IAuthorIRepository;

namespace Pustok.Data.Repositories.Implementations
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(PustokContext context) : base(context)
        {
        }
    }
}
