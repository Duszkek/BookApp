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

    private async void SearchBooksList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
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
    
    #endregion
}