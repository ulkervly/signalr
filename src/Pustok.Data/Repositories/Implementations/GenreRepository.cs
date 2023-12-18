using Pustok.Core.Models;
using Pustok.Core.Repositories.Interfaces;
using Pustok.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pustok.Data.Repositories.Implementations
{
    public class GenreRepository : GenericRepository<Genre>, IGenreRepository
    {
        public GenreRepository(PustokContext context) : base(context)
        {
        }
    }
}
