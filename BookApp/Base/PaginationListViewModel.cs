using System.Collections.ObjectModel;
using BookApp.Models;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookApp.Base;

public partial class PaginationListViewModel
    : ObservableObject
{
    #region Properties
    
    public int CurrentPaginationIndex { get; set; }
    public bool StopLoadingData { get; set; }
    public Intent Intent { get; }
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

    public PaginationListViewModel(Intent intent)
    {
        Intent = intent;
        TitleQuery = string.Empty;
        AuthorQuery = string.Empty;
    }
    
    #endregion
    
    #region Methods
    
    public virtual void Refresh()
    {
    }
    
    public virtual async Task GetValueFromIntent(Intent intent)
    {
    }
    
    public virtual async Task ItemTapped(object objectTapped)
    {
    }

    [RelayCommand]
    public virtual async Task LoadMoreBooks()
    {
    }
    
    [RelayCommand]
    public virtual async Task PerformSearch()
    {
    }
    
    #endregion
}