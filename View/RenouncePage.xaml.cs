namespace SaveUp.View;

public partial class RenouncePage : ContentPage
{
	public RenouncePage()
	{
		InitializeComponent();
	}

    private void Preis_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Double.TryParse(e.NewTextValue, out double value))
        {
            if (value < 0)
            {
                Preis.Text = e.OldTextValue;
            }
        }
        else
        {
            Preis.Text = e.OldTextValue;
        }
    }
}