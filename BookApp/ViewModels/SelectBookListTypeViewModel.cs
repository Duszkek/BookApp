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
        BookCounterLabel = $"Books read: {MauiProgram.CurrentUser.BooksCount}";
    }
    
    #endregion
    
    #region Methods
    
    [RelayCommand]
    private async Task GoToSearchBooks()
    {
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
}