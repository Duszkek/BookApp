using System.Collections.ObjectModel;
using BookApp.Entities;
using BookApp.Enums;
using BookApp.Models;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookApp.ViewModels;

public partial class SelectBookListTypeViewModel
    : ObservableObject
{
    #region Members
    private Intent Intent { get; set; }
    
    #endregion
    
    #region Properties
    
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string bookCounterLabel;
    
    #endregion
    
    #region Ctor

    public SelectBookListTypeViewModel(Intent intent)
    {
        Intent = intent;
        Title = $"Welcome {MauiProgram.CurrentUser.Username}!";
    }
    
    #endregion
    
    #region Methods

    public void Refresh()
    {
        BookCounterLabel = $"Books read: {MauiProgram.CurrentUser.BooksCount}";
    }
    
    #region Relay Commands
    
    [RelayCommand]
    private async Task GoToSearchBooks()
    {
        await ApplicationNavigator.GoToPage(Intent, NavigationWizardStep.SearchBooksList);
    }
    
    [RelayCommand]
    private async Task GoToSavedBooks()
    {
    }
    
    [RelayCommand]
    private async Task Logout()
    {
        MauiProgram.CurrentUser = null;
        await ApplicationNavigator.GoBack();
    }
    
    #endregion
    
    #endregion
}