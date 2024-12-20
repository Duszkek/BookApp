using BookApp.Enums;
using BookApp.Utils;
using BookApp.ViewModels;

namespace BookApp.Views;

public partial class LoginView : AppPage
{
    #region Members
    
    private readonly LoginViewModel ViewModel;
    
    #endregion
    
    #region Properties
    
    public override PageName Name
    {
        get { return PageName.Main; }
    }
    
    #endregion
    
    int count = 0;

    public LoginView(Intent intent)
        : base(intent)
    {
        InitializeComponent();
        BindingContext = ViewModel = new LoginViewModel(intent);
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);

        ViewModel.OnButtonClicked();
    }
}