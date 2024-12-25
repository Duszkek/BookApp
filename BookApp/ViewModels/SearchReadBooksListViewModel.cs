using System.Collections.ObjectModel;
using BookApp.Base;
using BookApp.Entities;
using BookApp.Enums;
using BookApp.Models;
using BookApp.Structs;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookApp.ViewModels;

public partial class SearchReadBooksListViewModel
    : PaginationListViewModel
{
    #region Properties

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
        :base(intent)
    {
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
    
    #region Override
    
    public override void Refresh()
    {
        CurrentPaginationIndex = 0;
        EmptyListMessage = "No elements found - change search parameters and try again or add more books";
        BookList.Clear();
        CounterIsVisible = false;
    }
    
    public override async Task ItemTapped(object selectedBook)
    {
        if (selectedBook is BookModel bookModel && BookList.Contains(bookModel))
        {
            Intent.AddValue(IntentName.SelectedBook, bookModel);
            await ApplicationNavigator.GoToPage(Intent, NavigationWizardStep.BookDetails);
        }
    }
    
    public override async Task GetValueFromIntent(Intent intent)
    {
        if (Intent.HasValue(IntentName.ChangedBook))
        {
            BookModel bookChanged = Intent.GetAndPopValue<BookModel>(IntentName.ChangedBook);
            if (bookChanged != null)
            {
                await DeleteBook(bookChanged);
            }
        }
    }
    
    #endregion
    
    #region RelayCommands
    
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

    #region Override
    
        public override async Task PerformSearch()
    {
        Refresh();

        ListIsVisible = true;
        OnPropertyChanged(nameof(ListIsVisible));
        
        IsLoading = true;
        
        DbResponseStruct dbResponseStruct = await MauiProgram.DataProviderService.GetBookModelListForUserAsync(MauiProgram.CurrentUser.UserId, TitleQuery.Trim(), AuthorQuery.Trim(), CurrentPaginationIndex);
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
    
    public override async Task LoadMoreBooks()
    {
        if (StopLoadingData)
        {
            return;
        }
        
        IsLoading = true;
        DbResponseStruct dbResponseStruct = await MauiProgram.DataProviderService.GetBookModelListForUserAsync(MauiProgram.CurrentUser.UserId, TitleQuery.Trim(), AuthorQuery.Trim(), CurrentPaginationIndex);
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
    
    
    #endregion
    
    #endregion
    
    #endregion
}