using Pustok.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pustok.Business.Services.Interfaces
{
    public interface IGenreService
    {
        Task CreateAsync(Genre entity);
        Task Delete(int id);
        Task<Genre> GetByIdAsync(int id);
        Task<List<Genre>> GetAllAsync();
        Task UpdateAsync(Genre genre);
    }
}
