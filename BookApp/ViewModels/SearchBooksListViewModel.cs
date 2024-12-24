using System.Collections.ObjectModel;
using BookApp.Entities;
using BookApp.Enums;
using BookApp.Models;
using BookApp.Structs;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookApp.ViewModels;

public partial class SearchBooksListViewModel
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

    #endregion
    
    #region Ctor

    public SearchBooksListViewModel(Intent intent)
    {
        Intent = intent;
        EmptyListMessage = "Use the search function above to find your books";
    }
    
    #endregion
    
    #region Methods

    public void Refresh()
    {
        CurrentPaginationIndex = 0;
        EmptyListMessage = "No elements found - change search parameters and try again";
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
        if (string.IsNullOrWhiteSpace(TitleQuery) && string.IsNullOrWhiteSpace(AuthorQuery))
        {
            return;
        }
        
        Refresh();

        ListIsVisible = true;
        OnPropertyChanged(nameof(ListIsVisible));
        
        IsLoading = true;
        ApiResponseStruct apiResponseStruct = await MauiProgram.DataProviderService.SearchBooksAsync(TitleQuery, AuthorQuery, CurrentPaginationIndex);
        CurrentPaginationIndex = apiResponseStruct.Books.Count;
        BooksFoundCounter = apiResponseStruct.ItemsFound;
        foreach (BookModel book in apiResponseStruct.Books)
        {
            if (await MauiProgram.DataProviderService.CheckIfBookIsSaved(book.ApiId))
            {
                book.IsSaved = true;
            }
            
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
        ApiResponseStruct apiResponseStruct = await MauiProgram.DataProviderService.SearchBooksAsync(TitleQuery, AuthorQuery, CurrentPaginationIndex);
        CurrentPaginationIndex += apiResponseStruct.Books.Count;

        if (apiResponseStruct.Books.Count < 40)
        {
            StopLoadingData = true;
        }
        
        foreach (BookModel book in apiResponseStruct.Books)
        {
            BookList.Add(book);
        }
        
        OnPropertyChanged(nameof(BookList));
        OnPropertyChanged(nameof(AnyBookIsVisible));
        IsLoading = false;
    }
    
    [RelayCommand]
    public async Task SwitchState(BookModel book)
    {
        if (!book.IsSaved)
        {
            await MauiProgram.DataProviderService.SaveBookLocallyForUserAsync(MauiProgram.CurrentUser.UserId, book);
            BookList[BookList.IndexOf(book)].IsSaved = true;
            MauiProgram.CurrentUser.BooksCount += 1;
            return;
        }
        
        await MauiProgram.DataProviderService.DeleteBookForUserAsync(MauiProgram.CurrentUser.UserId, book);
        BookList[BookList.IndexOf(book)].IsSaved = false;
        MauiProgram.CurrentUser.BooksCount -= 1;

        OnPropertyChanged(nameof(BookList));
    }
    
    [RelayCommand]
    private async Task SwitchBookLocalState()
    {
        await ApplicationNavigator.GoToPage(Intent, NavigationWizardStep.AddUser);
    }
    
    #endregion
    
    #endregion
}