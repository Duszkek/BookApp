using BookApp.Base;
using BookApp.Enums;
using BookApp.Utils;
using BookApp.ViewModels;

namespace BookApp.Views;

public partial class SearchBooksListView
    : AppPage
{
    #region Members
    
    private readonly SearchBooksListViewModel ViewModel;
    
    #endregion
    
    #region Properties
    
    public override PageName Name => PageName.SearchBooksList;

    #endregion
    
    #region Ctor

    public SearchBooksListView(Intent intent)
        : base(intent)
    {
        InitializeComponent();
        BindingContext = ViewModel = new SearchBooksListViewModel(intent);
    }
    
    #endregion
    
    #region Methods

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel?.GetValueFromIntent(Intent);
    }
    
    private async void SearchBooksList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
        {
            return;
        }
        
        if (e.CurrentSelection?.Count > 0)
        {
            await ViewModel?.ItemTapped(e.CurrentSelection.FirstOrDefault());
        }
    
        if (sender is CollectionView userCollectionView)
        {
            userCollectionView.SelectedItem = null;
        }
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