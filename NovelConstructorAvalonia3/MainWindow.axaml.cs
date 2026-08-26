using Avalonia.Controls;
using Avalonia.Platform;

namespace NovelConstructorAvalonia3;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    protected override void OnSizeChanged(Avalonia.Controls.SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        ResizePanel1();
    }
    private void ResizePanel1()
    {
        if (panel1Container == null || panel1 == null)
            return;

        Screen? screen = Screens.ScreenFromWindow(this);

        if (screen == null)
            return;

        double screenWidth = screen.Bounds.Width;
        double screenHeight = screen.Bounds.Height;

        if (screenWidth <= 0 || screenHeight <= 0)
            return;

        double proportion = screenHeight / screenWidth;

        double availableWidth = panel1Container.Bounds.Width;
        double availableHeight = panel1Container.Bounds.Height;

        if (availableWidth <= 0 || availableHeight <= 0)
            return;

        double panelWidth = availableWidth;
        double panelHeight = panelWidth * proportion;

        if (panelHeight > availableHeight)
        {
            panelHeight = availableHeight;
            panelWidth = panelHeight / proportion;
        }

        panel1.Width = panelWidth;
        panel1.Height = panelHeight;

        panel1.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        panel1.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
    }
}