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

public partial class SearchBooksListViewModel
    : PaginationListViewModel
{
    #region Ctor

    public SearchBooksListViewModel(Intent intent)
        : base(intent)
    {
        EmptyListMessage = "Use the search function above to find your books";
    }
    
    #endregion
    
    #region Methods
    
    #region Override

    public override async Task GetValueFromIntent(Intent intent)
    {
        if (Intent.HasValue(IntentName.ChangedBook))
        {
            BookModel bookChanged = Intent.GetAndPopValue<BookModel>(IntentName.ChangedBook);
            if (bookChanged != null)
            {
                await SwitchState(bookChanged);
            }
        }
    }

    public override void Refresh()
    {
        CurrentPaginationIndex = 0;
        EmptyListMessage = "No elements found - change search parameters and try again";
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
    
    #endregion
    
    #region RelayCommands
    
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
    
    #region Override
    
    public override async Task PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(TitleQuery) && string.IsNullOrWhiteSpace(AuthorQuery))
        {
            return;
        }
        
        Refresh();

        ListIsVisible = true;
        OnPropertyChanged(nameof(ListIsVisible));
        
        IsLoading = true;
        ApiResponseStruct apiResponseStruct = await MauiProgram.DataProviderService.SearchBooksAsync(TitleQuery.Trim(), AuthorQuery.Trim(), CurrentPaginationIndex);
        CurrentPaginationIndex = apiResponseStruct.Books.Count;
        BooksFoundCounter = apiResponseStruct.ItemsFound;
        foreach (BookModel book in apiResponseStruct.Books)
        {
            if (await MauiProgram.DataProviderService.CheckIfBookIsSavedForUser(MauiProgram.CurrentUser.UserId, book.ApiId))
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
    
    public override async Task LoadMoreBooks()
    {
        if (StopLoadingData)
        {
            return;
        }
        
        IsLoading = true;
        ApiResponseStruct apiResponseStruct = await MauiProgram.DataProviderService.SearchBooksAsync(TitleQuery.Trim(), AuthorQuery.Trim(), CurrentPaginationIndex);
        CurrentPaginationIndex += apiResponseStruct.Books.Count;

        if (apiResponseStruct.Books.Count < 40)
        {
            StopLoadingData = true;
        }
        
        foreach (BookModel book in apiResponseStruct.Books)
        {
            if (await MauiProgram.DataProviderService.CheckIfBookIsSavedForUser(MauiProgram.CurrentUser.UserId, book.ApiId))
            {
                book.IsSaved = true;
            }
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