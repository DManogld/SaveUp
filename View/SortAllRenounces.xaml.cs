using SaveUp.ViewModels;
namespace SaveUp.View;

public partial class SortAllRenouncesPage : ContentPage
{
    public SortAllRenouncesPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RenouncesViewModel viewModel)
        {
            viewModel.UpdateRenouncesList();
        }
    }

}