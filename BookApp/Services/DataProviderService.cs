using System.Text;
using BookApp.Database;
using BookApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using BookApp.DTO;
using BookApp.Models;
using BookApp.Structs;
using BookApp.Utils;

namespace BookApp.Services;

public class DataProviderService
{
    #region Const
    
    private readonly HttpClient HttpClient;
    private readonly AppDbContext DbContext;
    private readonly DTOMapper Mapper;
    
    #region Google Book API

    private readonly string MainApiUrlFormat = "https://www.googleapis.com/books/v1/volumes?q=";
    private readonly string InTitleSuffixFormat = "+intitle:{0}";
    private readonly string InAuthorSuffixFormat = "+inauthor:{0}";
    private readonly string ApiUrlSuffixFormat = "&maxResults={0}&startIndex={1}";
    private readonly string KeySuffix = "&key={0}";
    
    private readonly string apiKey = "";
    
    #endregion

    #endregion
    
    #region Ctor
    
    public DataProviderService(AppDbContext dbContext)
    {
        DbContext = dbContext;
        HttpClient = new HttpClient();
        Mapper = new DTOMapper();
    }

    #endregion
    
    #region Methods
    
    #region Add

    public async Task<UserModel> CreateUserAsync(string username)
    {
        User user = new User { Name = username };
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        return new UserModel(user);
    }
    
    public async Task SaveBookLocallyForUserAsync(int idUser, BookModel bookModel)
    {
        User? user = await DbContext.Users.FindAsync(idUser);

        if (user == null)
        {
            throw new NullReferenceException(nameof(user));
        }
    
        Book? book = DbContext.Books.FirstOrDefault(x => x.ApiId == bookModel.ApiId);

        if (book is null)
        {
            book = new Book
            {
                ApiId = bookModel.ApiId,
                Title = bookModel.Title,
                Authors = bookModel.Authors,
                Description = bookModel.Description,
                PublishedDate = bookModel.PublishedDate,
                Thumbnail = bookModel.Thumbnail,
                SmallThumbnail = bookModel.SmallThumbnail,
                PageCount = bookModel.PageCount,
            };
    
            DbContext.Books.Add(book);
            await DbContext.SaveChangesAsync();
        }

        bool isBookAssignedToUser =
            DbContext.UserReadBooks.Any(x => x.UserId == user.IdUser && x.BookId == book.IdBook);

        if (!isBookAssignedToUser)
        {
            UserReadBooks userReadBook = new UserReadBooks
            {
                UserId = user.IdUser,
                BookId = book.IdBook
            };
    
            DbContext.UserReadBooks.Add(userReadBook);
            await DbContext.SaveChangesAsync();
        }
    }
    
    #endregion
    
    #region Delete

    public async Task<bool> DeleteUserAsync(int userId)
    {
        User userToDelete = DbContext.Users.FirstOrDefault(x => x.IdUser == userId);
        
        if (userToDelete is null)
        {
            throw new ArgumentNullException($"No user found with id {userId}");
        }
        
        DbContext.Users.Remove(userToDelete);
        await DbContext.SaveChangesAsync();

        List<Book> unassignedBooks = await DbContext.Books
            .Where(book => !DbContext.UserReadBooks.Any(urb => urb.BookId == book.IdBook))
            .ToListAsync();

        if (unassignedBooks.Any())
        {
            DbContext.Books.RemoveRange(unassignedBooks);
            await DbContext.SaveChangesAsync();
        }
        
        return true;
    }

    public async Task DeleteBookForUserAsync(int idUser, BookModel bookModel)
    {
        if (!bookModel.IsSaved)
        {
            return;
        }
        
        bool userExists = DbContext.Users.FirstOrDefault(x => x.IdUser == idUser) != null;
        
        if (!userExists)
        {
            throw new ArgumentNullException($"No user found with id {idUser}");
        }
        
        Book? book = DbContext.Books.FirstOrDefault(x => x.ApiId == bookModel.ApiId);
        
        if (book is null)
        {
            throw new ArgumentNullException($"No book found with api id {book.ApiId}");
        }
        
        UserReadBooks? userReadBook = DbContext.UserReadBooks.FirstOrDefault(x => x.UserId == idUser && x.BookId == book.IdBook);

        if (userReadBook is null)
        {
            throw new ArgumentNullException($"No book found with api id {book.ApiId} for user with id {idUser}");
        }
        
        DbContext.UserReadBooks.Remove(userReadBook);
        await DbContext.SaveChangesAsync();
        
        bool isBookValid = DbContext.UserReadBooks.Any(x => x.BookId == book.IdBook);
        if (!isBookValid)
        {
            DbContext.Books.Remove(book);
        }
        
        await DbContext.SaveChangesAsync();
    }
    
    #endregion
    
    #region Get

    public async Task<bool> CheckIfBookIsSavedForUser(int idUser, string bookApiId)
    {
        Book? book = DbContext.Books.FirstOrDefault(x => x.ApiId == bookApiId);

        if (book is null)
        {
            return false;
        }
        
        bool bookIsAssigned = DbContext.UserReadBooks.Any(x => x.UserId == idUser && x.BookId == book.IdBook);

        return bookIsAssigned;
    }

    public async Task<List<UserModel>> GetUserModelListAsync()
    {
        List<UserModel> userModels = [];
        List<User> users = await DbContext.Users.ToListAsync();

        if (users is null || users.Count == 0)
        {
            return userModels;
        }
        
        foreach (User user in users)
        {
            int bookCount = await DbContext.UserReadBooks.CountAsync(x => x.UserId == user.IdUser);
            userModels.Add(new UserModel(user, bookCount));
        }
        
        return userModels;
    }

    public async Task<DbResponseStruct> GetBookModelListForUserAsync(int idUser, string? bookTitle = null, string? bookAuthor = null, int paginationOffest = 0)
    {
        bool userExists = await DbContext.Users.AnyAsync(x => x.IdUser == idUser);

        if (!userExists)
        {
            throw new ArgumentNullException($"No user found with id {idUser}");
        }

        int[] userBooksIds = await DbContext.UserReadBooks.Where(x => x.UserId == idUser).Select(x => x.BookId).ToArrayAsync();

        if (userBooksIds.Length == 0)
        {
            return new DbResponseStruct() { ItemsFound = 0, Books = [] };
        }

        IQueryable<Book> userBooksQuery = DbContext.Books.Where(x => userBooksIds.Contains(x.IdBook));

        if (!string.IsNullOrEmpty(bookTitle))
        {
            string pattern = $"%{bookTitle}%";
            userBooksQuery = userBooksQuery.Where(x => EF.Functions.Like(x.Title, pattern));
        }

        if (!string.IsNullOrEmpty(bookAuthor))
        {
            string pattern = $"%{bookAuthor}%";
            userBooksQuery = userBooksQuery.Where(x => EF.Functions.Like(x.Authors, pattern));
        }
        
        List<Book> paginatedBooks = userBooksQuery.Skip(paginationOffest).Take(Constants.MaxApiResults).ToList();

        return new DbResponseStruct() { ItemsFound = userBooksQuery.Count(), Books = paginatedBooks.Select(book => new BookModel(book)).ToList() };
    }

    #endregion
    
    #region Google Books API
    
    public async Task<ApiResponseStruct> SearchBooksAsync(string titleQuery, string authorQuery, int pagination)
    {
        try
        {
            string url = BuildGoogleBooksApiUrl(titleQuery, authorQuery, pagination);

            HttpResponseMessage response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                return new ApiResponseStruct() { ItemsFound = 0, Books = new List<BookModel>() };
            }

            DtoBookApiResponse? apiResponse = JsonSerializer.Deserialize<DtoBookApiResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse?.Items == null || !apiResponse.Items.Any())
            {
                return new ApiResponseStruct() { ItemsFound = apiResponse?.TotalItems ?? 0, Books = new List<BookModel>() };
            }

            List<BookModel> booksList = apiResponse.Items
                .Select(dtoBook => Mapper.Get<BookModel, DtoBook>(dtoBook))
                .ToList();

            return new ApiResponseStruct() { ItemsFound = apiResponse.TotalItems, Books = booksList };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during book search: {ex.Message}");

            return new ApiResponseStruct() { ItemsFound = 0, Books = new List<BookModel>() };
        }
    }
    
    #endregion
    public async Task InitializeDatabaseAsync()
    {
        await DbContext.InitializeDatabaseAsync();
    }
    
    private string BuildGoogleBooksApiUrl(string title, string author, int startIndex)
    {
        StringBuilder queryBuilder = new StringBuilder(MainApiUrlFormat);

        if (!string.IsNullOrWhiteSpace(title))
        {
            queryBuilder.AppendFormat(InTitleSuffixFormat, Uri.EscapeDataString(title));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            queryBuilder.AppendFormat(InAuthorSuffixFormat, Uri.EscapeDataString(author));
        }

        queryBuilder.AppendFormat(ApiUrlSuffixFormat, Constants.MaxApiResults, startIndex);
        
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            queryBuilder.AppendFormat(KeySuffix, apiKey);
        }

        return queryBuilder.ToString();
    }
    
    #endregion
}