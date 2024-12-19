using BookApp.Enums;
using BookApp.Utils;
using BookApp.ViewModels;

namespace BookApp;

public partial class MainPage : AppPage
{
    #region Members
    
    private readonly MainPageViewModel ViewModel;
    
    #endregion
    
    #region Properties
    
    public override PageName Name
    {
        get { return PageName.Main; }
    }
    
    #endregion
    
    int count = 0;

    public MainPage(Intent intent)
        : base(intent)
    {
        InitializeComponent();
        BindingContext = ViewModel = new MainPageViewModel(intent);
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