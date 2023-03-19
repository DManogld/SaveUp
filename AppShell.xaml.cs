namespace SaveUp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(View.RenouncePage), typeof(View.RenouncePage));

    }
}
