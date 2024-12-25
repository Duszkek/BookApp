using BookApp.Enums;
using BookApp.Models;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookApp.ViewModels;

public partial class BookDetailsViewModel
    : ObservableObject
{
    #region Members
    
    private Intent Intent { get; set; }
    
    #endregion
    
    #region Properties

    [ObservableProperty]
    private BookModel bookModel;
    
    [ObservableProperty]
    private bool statusChanged;

    [ObservableProperty]
    private bool isBookSavedCurrently;
    
    #endregion
    
    #region Ctor

    public BookDetailsViewModel(Intent intent)
    {
        Intent = intent;
        BookModel = Intent.GetValue<BookModel>(IntentName.SelectedBook);
        IsBookSavedCurrently = BookModel.IsSaved;
    }
    
    #endregion
    
    #region Methods
    
    [RelayCommand]
    public async Task ChangeBookSaveStatus()
    {
        IsBookSavedCurrently = !IsBookSavedCurrently;
        StatusChanged = IsBookSavedCurrently != BookModel.IsSaved;
    }
    
    [RelayCommand]
    public async Task Save()
    {
        if (IsBookSavedCurrently != BookModel.IsSaved)
        {
            Intent.AddValue(IntentName.ChangedBook, BookModel);
        }
        
        await ApplicationNavigator.GoBack();
    }
    
    #endregion
}