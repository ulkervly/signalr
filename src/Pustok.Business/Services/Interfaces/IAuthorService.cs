using Pustok.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pustok.Business.Services.Interfaces
{
    public interface IAuthorService
    {
        Task CreateAsync(Author entity);
        Task Delete(int id);
        Task<Author> GetByIdAsync(int id);
        Task<List<Author>> GetAllAsync();
        Task UpdateAsync(Author entity);
    }
}
