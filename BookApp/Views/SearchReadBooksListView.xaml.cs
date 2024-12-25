using BookApp.Base;
using BookApp.Enums;
using BookApp.Utils;
using BookApp.ViewModels;

namespace BookApp.Views;

public partial class SearchReadBooksListView
    : AppPage
{
    #region Members
    
    private readonly SearchReadBooksListViewModel ViewModel;
    
    #endregion
    
    #region Properties
    
    public override PageName Name => PageName.SearchReadBooksList;

    #endregion
    
    #region Ctor

    public SearchReadBooksListView(Intent intent)
        : base(intent)
    {
        InitializeComponent();
        BindingContext = ViewModel = new SearchReadBooksListViewModel(intent);
    }
    
    #endregion
    
    #region Methods

    private async void SearchReadBooksList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection?.Count > 0)
        {
            await ViewModel?.ItemTapped(e.CurrentSelection.FirstOrDefault());
        }
    
        if (sender is CollectionView userCollectionView)
        {
            userCollectionView.SelectedItem = null;
        }
    }
    
    protected override async void OnAppearing()
    {
        if (!IsLoaded)
        {
            base.OnAppearing();
            await ViewModel?.LoadDataAsync();
            IsLoaded = true;
        }
        
        await ViewModel?.GetValueFromIntent(Intent);
    }
    
    private void Entry_OnCompleted(object? sender, EventArgs e)
    {
        if (sender is Entry entry)
        {
            entry.Unfocus();
        }
    }
    
    #endregion
}