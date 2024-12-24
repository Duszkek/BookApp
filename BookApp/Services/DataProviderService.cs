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
    private readonly string GoogleBookApiUrlStringFormat = "https://www.googleapis.com/books/v1/volumes?q={0}&key={1}";
    private readonly string GoogleBookApiUrlNoKeyStringFormat = "https://www.googleapis.com/books/v1/volumes?q={0}";
    private readonly string apiKey = ""; 

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

    public async Task SaveBookLocallyForUserAsync(User user, BookModel bookModel)
    {
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
        
        return true;
    }

    public async Task DeleteBookForUserAsync(User user, BookModel bookModel)
    {
        if (!bookModel.IsSaved)
        {
            return;
        }
        
        bool userExists = DbContext.Users.FirstOrDefault(x => x.IdUser == user.IdUser) != null;
        
        if (userExists)
        {
            throw new ArgumentNullException($"No user found with id {user.IdUser}");
        }
        
        Book? book = DbContext.Books.FirstOrDefault(x => x.ApiId == bookModel.ApiId);
        
        if (book is null)
        {
            throw new ArgumentNullException($"No book found with api id {book.ApiId}");
        }
        
        UserReadBooks? userReadBook = DbContext.UserReadBooks.FirstOrDefault(x => x.UserId == user.IdUser && x.BookId == book.IdBook);

        if (userReadBook is null)
        {
            throw new ArgumentNullException($"No book found with api id {book.ApiId} for user with id {user.IdUser}");
        }
        
        DbContext.UserReadBooks.Remove(userReadBook);
        
        bool isBookValid = DbContext.UserReadBooks.Any(x => x.BookId == book.IdBook);
        if (!isBookValid)
        {
            DbContext.Books.Remove(book);
        }
        
        await DbContext.SaveChangesAsync();
    }
    
    #endregion
    
    #region Get
    
    public async Task<List<UserReadBooks>> GetSampleDataAsync()
    {
        List<UserReadBooks> sampleData = await DbContext.UserReadBooks.ToListAsync();
        return sampleData;
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
    
    #endregion
    
    public async Task<ApiResponseStruct> SearchBooksAsync(string query)
    {
        string url = !string.IsNullOrEmpty(apiKey) ? string.Format(GoogleBookApiUrlStringFormat, Uri.EscapeDataString(query), apiKey) : string.Format(GoogleBookApiUrlNoKeyStringFormat, Uri.EscapeDataString(query));
        HttpResponseMessage response = await HttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string jsonResponse = await response.Content.ReadAsStringAsync();
        DtoBookApiResponse? apiResponse = JsonSerializer.Deserialize<DtoBookApiResponse>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        List<BookModel> booksList = [];

        if (apiResponse is null)
        {
            return new ApiResponseStruct() {ItemsFound = 0, Books = []} ;
        }

        foreach (DtoBook dtoBook in apiResponse.Items)
        {
            BookModel book = Mapper.Get<BookModel, DtoBook>(dtoBook);
            booksList.Add(book);
        }
        return new ApiResponseStruct() { ItemsFound = apiResponse.TotalItems, Books = booksList };
    }
    
    public async Task InitializeDatabaseAsync()
    {
        await DbContext.InitializeDatabaseAsync();
    }
    
    #endregion
}