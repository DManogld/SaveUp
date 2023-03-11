namespace SaveUp.View;

public partial class AllRenoucesPage : ContentPage
{
	public AllRenoucesPage()
	{
		InitializeComponent();
	}
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        notesCollection.SelectedItem = null;
    }
}