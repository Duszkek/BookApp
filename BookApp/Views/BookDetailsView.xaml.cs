using BookApp.Base;
using BookApp.Enums;
using BookApp.Utils;
using BookApp.ViewModels;

namespace BookApp.Views;

public partial class BookDetailsView
    : AppPage
{
    #region Members

    private readonly BookDetailsViewModel ViewModel;

    #endregion

    #region Properties

    public override PageName Name => PageName.BookDetails;

    #endregion
    
    #region Ctor

    public BookDetailsView(Intent intent)
        : base(intent)
    {
        InitializeComponent();
        BindingContext = ViewModel = new BookDetailsViewModel(intent);
    }
    
    #endregion
    
    #region Methods
    
    private void Entry_OnCompleted(object? sender, EventArgs e)
    {
        if (sender is Entry entry)
        {
            entry.Unfocus();
        }
    }
    
    #endregion
}