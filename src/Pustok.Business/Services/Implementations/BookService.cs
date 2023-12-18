using Microsoft.AspNetCore.Hosting;
using Pustok.Business.Exceptions;
using Pustok.Business.Extensions;
using Pustok.Business.Services.Interfaces;
using Pustok.Core.Models;
using Pustok.Core.Repositories.Interfaces;
using Pustok.Data.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Pustok.Core.Repositories.Interfaces.IAuthorIRepository;
using static Pustok.Core.Repositories.Interfaces.IBookImageRepository;

namespace Pustok.Business.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IBookTagsRepository _bookTagsRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IBookImagesRepository _bookImagesRepository;

        public BookService(IBookRepository bookRepository,
                           IGenreRepository genreRepository,
                           IAuthorRepository authorRepository,
                           ITagRepository tagRepository,
                           IBookTagsRepository bookTagsRepository,
                           IWebHostEnvironment env,
                           IBookImagesRepository bookImagesRepository)

        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _authorRepository = authorRepository;
            _tagRepository = tagRepository;
            _bookTagsRepository = bookTagsRepository;
            _env = env;
            _bookImagesRepository = bookImagesRepository;
        }

        public async Task CreateAsync(Book entity)
        {
            if (!_genreRepository.Table.Any(x => x.Id == entity.GenreId))
            {
                throw new NotFoundException("GenreId", "Genre not found!");
            }

            if (!_authorRepository.Table.Any(x => x.Id == entity.AuthorId))
            {
                throw new NotFoundException("AuthorId", "Author not found!");
            }


            bool check = false;

            if (entity.TagIds != null)
            {
                foreach (var tagId in entity.TagIds)
                {
                    if (!_tagRepository.Table.Any(x => x.Id == tagId))
                    {
                        check = true;
                        break;
                    }
                }
            }

            if (check)
            {
                throw new NotFoundException("TagId", "Tag not found!");
            }
            else
            {
                if (entity.TagIds != null)
                {
                    foreach (var tagId in entity.TagIds)
                    {
                        BookTag bookTag = new BookTag
                        {
                            Book = entity,
                            TagId = tagId
                        };

                        await _bookTagsRepository.CreateAsync(bookTag);
                    }
                }
            }

            if (entity.BookPosterImageFile != null)
            {
                if (entity.BookPosterImageFile.ContentType != "image/jpeg" && entity.BookPosterImageFile.ContentType != "image/png")
                {
                    throw new InvalidImageContentException("BookPosterImageFile", "File must be .png or .jpeg (.jpg)");
                }
                if (entity.BookPosterImageFile.Length > 2097152)
                {
                    throw new InvalidImageContentException("BookPosterImageFile", "File size must be lower than 2mb!");
                }

                BookImage bookImage = new BookImage
                {
                    Book = entity,
                    ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/Books", entity.BookPosterImageFile),
                    IsPoster = true
                };

                await _bookImagesRepository.CreateAsync(bookImage);
            }

            if (entity.BookHoverImageFile != null)
            {
                if (entity.BookHoverImageFile.ContentType != "image/jpeg" && entity.BookHoverImageFile.ContentType != "image/png")
                {
                    throw new InvalidImageContentException("BookHoverImageFile", "File must be .png or .jpeg (.jpg)");
                }
                if (entity.BookHoverImageFile.Length > 2097152)
                {
                    throw new InvalidImageContentException("BookHoverImageFile", "File size must be lower than 2mb)");
                }

                BookImage bookImage = new BookImage
                {
                    Book = entity,
                    ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", entity.BookHoverImageFile),
                    IsPoster = false
                };

                await _bookImagesRepository.CreateAsync(bookImage);
            }


            if (entity.ImageFiles != null)
            {
                foreach (var imageFile in entity.ImageFiles)
                {
                    if (imageFile.ContentType != "image/jpeg" && imageFile.ContentType != "image/png")
                    {
                        throw new InvalidImageContentException("ImageFiles", "File must be .png or .jpeg (.jpg)");
                    }
                    if (imageFile.Length > 2097152)
                    {
                        throw new InvalidImageContentException("ImageFiles", "File size must be lower than 2mb)");
                    }

                    BookImage bookImage = new BookImage
                    {
                        Book = entity,
                        ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", imageFile),
                        IsPoster = null
                    };

                    await _bookImagesRepository.CreateAsync(bookImage);
                }
            }

            await _bookRepository.CreateAsync(entity);
            await _bookRepository.CommitAsync();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }



        public async Task<List<Book>> GetAllAsync()
        {
            return await _bookRepository.GetAllAsync(x => x.IsDeleted == false, "BookImages", "Author");
        }

        public async Task<List<Book>> GetAllAsync(Expression<Func<Book, bool>>? expression = null)
        {
            return await _bookRepository.GetAllAsync(expression, "BookImages", "Author");
        }

        public async Task<List<Book>> GetAllRelatedBooksAsync(Book book)
        {
            return await _bookRepository.GetAllAsync(x => x.GenreId == book.GenreId && !x.IsDeleted && x.Id != book.Id, "Author", "BookImages");
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            var entity = await _bookRepository.GetByIdAsync(x => x.Id == id && x.IsDeleted == false, "Author", "BookImages", "BookTags.Tag");

            if (entity is null) throw new NullReferenceException();

            return entity;
        }

        public async Task SoftDelete(int id)
        {
            var entity = await _bookRepository.GetByIdAsync(x => x.Id == id && x.IsDeleted == false);

            if (entity is null) throw new NullReferenceException();

            entity.IsDeleted = true;
            await _bookRepository.CommitAsync();
        }

        public Task UpdateAsync(Book entity)
        {
            throw new NotImplementedException();
        }
    }
}
