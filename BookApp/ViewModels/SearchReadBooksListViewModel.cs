using System.Collections.ObjectModel;
using BookApp.Entities;
using BookApp.Enums;
using BookApp.Models;
using BookApp.Structs;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookApp.ViewModels;

public partial class SearchReadBooksListViewModel
    : ObservableObject
{
    #region Members
    
    private int CurrentPaginationIndex;
    private bool StopLoadingData;
    
    #endregion
    
    #region Properties
    
    private Intent Intent { get; }
    
    public bool AnyBookIsVisible => BookList.Count > 0;
    
    public ObservableCollection<BookModel> BookList { get; set; } = [];
    
    public BookModel? SelectedItem { get; set; }

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string titleQuery;
    
    [ObservableProperty]
    private string authorQuery;

    [ObservableProperty]
    private bool listIsVisible;
    
    [ObservableProperty]
    private string emptyListMessage;

    [ObservableProperty]
    private int booksFoundCounter;

    [ObservableProperty]
    private bool counterIsVisible;

    public int BooksReadCounter
    {
        get
        {
            return MauiProgram.CurrentUser.BooksCount;
        }
    }

    #endregion
    
    #region Ctor

    public SearchReadBooksListViewModel(Intent intent)
    {
        Intent = intent;
        EmptyListMessage = "You have no read books saved! In order to add new books go to the book list menu, find your books and save them!";
    }
    
    #endregion
    
    #region Methods

    public async Task LoadDataAsync()
    {
        ListIsVisible = true;
        OnPropertyChanged(nameof(ListIsVisible));
        
        IsLoading = true;
        DbResponseStruct dbResponseStruct = await MauiProgram.DataProviderService.GetBookModelListForUserAsync(MauiProgram.CurrentUser.UserId);
        if (dbResponseStruct.Books.Count < Constants.MaxApiResults)
        {
            StopLoadingData = true;
        }
        
        CurrentPaginationIndex = dbResponseStruct.Books.Count;
        BooksFoundCounter = MauiProgram.CurrentUser.BooksCount;
        
        foreach (BookModel book in dbResponseStruct.Books)
        {
            BookList.Add(book);
        }

        ListIsVisible = BookList.Count > 0;
        IsLoading = false;
        CounterIsVisible = ListIsVisible && !IsLoading;

        OnPropertyChanged(nameof(BookList));
        OnPropertyChanged(nameof(BooksFoundCounter));
        OnPropertyChanged(nameof(CounterIsVisible));
        OnPropertyChanged(nameof(AnyBookIsVisible));
        OnPropertyChanged(nameof(ListIsVisible));
    }
    
    public void Refresh()
    {
        CurrentPaginationIndex = 0;
        EmptyListMessage = "No elements found - change search parameters and try again or add more books";
        BookList.Clear();
        CounterIsVisible = false;
    }
    
    public async Task ItemTapped(object selectedBook)
    {
        if (selectedBook is BookModel bookModel && BookList.Contains(bookModel))
        {
            Intent.AddValue(IntentName.SelectedBook, bookModel);
            await ApplicationNavigator.GoToPage(Intent, NavigationWizardStep.BookDetails);
        }
    }
    
    #region RelayCommands
    
    [RelayCommand]
    public async Task PerformSearch()
    {
        Refresh();

        ListIsVisible = true;
        OnPropertyChanged(nameof(ListIsVisible));
        
        IsLoading = true;
        
        DbResponseStruct dbResponseStruct = await MauiProgram.DataProviderService.GetBookModelListForUserAsync(MauiProgram.CurrentUser.UserId, TitleQuery, AuthorQuery, CurrentPaginationIndex);
        BooksFoundCounter = dbResponseStruct.ItemsFound;
        if (dbResponseStruct.Books.Count < Constants.MaxApiResults)
        {
            StopLoadingData = true;
        }
        
        CurrentPaginationIndex = dbResponseStruct.Books.Count;
        foreach (BookModel book in dbResponseStruct.Books)
        {
            BookList.Add(book);
        }

        ListIsVisible = BookList.Count > 0;
        IsLoading = false;
        CounterIsVisible = ListIsVisible && !IsLoading;

        OnPropertyChanged(nameof(BookList));
        OnPropertyChanged(nameof(BooksFoundCounter));
        OnPropertyChanged(nameof(CounterIsVisible));
        OnPropertyChanged(nameof(AnyBookIsVisible));
        OnPropertyChanged(nameof(ListIsVisible));
    }
    
    [RelayCommand]
    public async Task LoadMoreBooks()
    {
        if (StopLoadingData)
        {
            return;
        }
        
        IsLoading = true;
        DbResponseStruct dbResponseStruct = await MauiProgram.DataProviderService.GetBookModelListForUserAsync(MauiProgram.CurrentUser.UserId, TitleQuery, AuthorQuery, CurrentPaginationIndex);
        BooksFoundCounter = dbResponseStruct.ItemsFound;

        CurrentPaginationIndex += dbResponseStruct.Books.Count;

        if (dbResponseStruct.Books.Count < Constants.MaxApiResults)
        {
            StopLoadingData = true;
        }
        
        foreach (BookModel book in dbResponseStruct.Books)
        {
            BookList.Add(book);
        }
        
        OnPropertyChanged(nameof(BookList));
        OnPropertyChanged(nameof(AnyBookIsVisible));
        IsLoading = false;
    }
    
    [RelayCommand]
    public async Task DeleteBook(BookModel book)
    {
        await MauiProgram.DataProviderService.DeleteBookForUserAsync(MauiProgram.CurrentUser.UserId, book);
        int bookIndex = BookList.IndexOf(book);
        BookList[bookIndex].IsSaved = false;
        MauiProgram.CurrentUser.BooksCount -= 1;
        BooksFoundCounter -= 1;
        BookList.Remove(book);
        OnPropertyChanged(nameof(BookList));
        OnPropertyChanged(nameof(BooksReadCounter));
    }

    #endregion
    
    #endregion
}