using BookApp.Base;
using BookApp.Enums;
using BookApp.Utils;
using BookApp.ViewModels;

namespace BookApp.Views;

public partial class SelectBookListTypeView
    : AppPage
{
    #region Members

    private readonly SelectBookListTypeViewModel ViewModel;

    #endregion

    #region Properties

    public override PageName Name => PageName.SelectBookListType;

    #endregion
    
    #region Ctor

    public SelectBookListTypeView(Intent intent)
        : base(intent)
    {
        InitializeComponent();
        BindingContext = ViewModel = new SelectBookListTypeViewModel(intent);
    }
    
    #endregion
}