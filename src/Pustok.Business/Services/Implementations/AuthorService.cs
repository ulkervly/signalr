using Pustok.Data.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pustok.Core.Models;
using Pustok.Core.Repositories.Interfaces;
using Pustok.Business.Services.Interfaces;
using static Pustok.Core.Repositories.Interfaces.IAuthorIRepository;

namespace Pustok.Business.Services.Implementations
{
    public class AuhtorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuhtorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        public async Task CreateAsync(Author entity)
        {
            await _authorRepository.CreateAsync(entity);
            await _authorRepository.CommitAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _authorRepository.GetByIdAsync(x => x.Id == id && x.IsDeleted == false);
            if (entity is null) throw new NullReferenceException();

            _authorRepository.Delete(entity);
            await _authorRepository.CommitAsync();
        }

        public async Task<List<Author>> GetAllAsync()
        {
            return await _authorRepository.GetAllAsync();
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            var entity = await _authorRepository.GetByIdAsync(x => x.Id == id && x.IsDeleted == false);
            if (entity is null) throw new NullReferenceException();
            return entity;
        }

        public async Task UpdateAsync(Author author)
        {
            var existEntity = await _authorRepository.GetByIdAsync(x => x.Id == author.Id && x.IsDeleted == false);

            if (_authorRepository.Table.Any(x => x.FullName == author.FullName && existEntity.Id != author.Id))
                throw new NullReferenceException();

            existEntity.FullName = author.FullName;
            await _authorRepository.CommitAsync();
        }
    }
}
