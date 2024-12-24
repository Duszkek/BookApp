using BookApp.Base;
using BookApp.Enums;
using BookApp.Utils;
using BookApp.ViewModels;

namespace BookApp.Views;

public partial class AddUserView
    : AppPage
{
    #region Members

    private readonly AddUserViewModel ViewModel;

    #endregion

    #region Properties

    public override PageName Name => PageName.AddUser;

    #endregion
    
    #region Ctor

    public AddUserView(Intent intent)
        : base(intent)
    {
        InitializeComponent();
        BindingContext = ViewModel = new AddUserViewModel(intent);
    }
    
    #endregion
}