using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System;
using System.IO;
using System.Linq;
using static NovelConstructorAvalonia3.Controls;
using static NovelConstructorAvalonia3.Previews;

namespace NovelConstructorAvalonia3;

public partial class MainWindow : Window
{
    private readonly ModelsIn.Slide currentSlide =
    new ModelsIn.Slide();

    private ModelsIn.ControlLayer? activeLayer;
    public MainWindow()
    {
        InitializeComponent();
        InitializeFirstLayer();

        DragDrop.AddDragOverHandler(controlCanvas, ControlCanvas_DragOver);
        DragDrop.AddDropHandler(controlCanvas, ControlCanvas_Drop);

        iconImage.PointerPressed += IconImage_PointerPressed;
        textFileImage.PointerPressed += TextFileImage_PointerPressed;

        AddHandler(
            KeyDownEvent,
            MainWindow_KeyDown,
            Avalonia.Interactivity.RoutingStrategies.Tunnel);

    }
    private void InitializeFirstLayer()
    {
        ModelsIn.ControlLayer layer =
            new ModelsIn.ControlLayer();

        currentSlide.Layers.Add(layer);
        currentSlide.ActiveLayer = layer;

        activeLayer = layer;
    }
    private void AddControlToActiveLayer(
        ConstructorControlContainer container)
    {
        controlCanvas.Children.Add(container);

        if (activeLayer == null)
            return;

        activeLayer.Controls.Add(container);
    }
    private void MainWindow_KeyDown(
        object? sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
            return;

        Controls.ConstructorControlContainer? container =
            Controls.ConstructorControlContainer.selectedContainer;

        if (container == null)
            return;

        container.DeleteControl();

        e.Handled = true;
    }
    private void ControlCanvas_DragOver(
        object? sender,
        DragEventArgs e)
    {
        DataFormat pictureFormat =
            DataFormat.CreateStringPlatformFormat(
                "application/novelconstructor-picture");

        DataFormat textFormat =
            DataFormat.CreateStringPlatformFormat(
                "application/novelconstructor-text");

        if (e.DataTransfer.Formats.Contains(DataFormat.File) ||
            e.DataTransfer.Formats.Contains(pictureFormat) ||
            e.DataTransfer.Formats.Contains(textFormat))
        {
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
            return;
        }

        e.DragEffects = DragDropEffects.None;
        e.Handled = true;
    }
    private async void ControlCanvas_Drop(
        object? sender,
        DragEventArgs e)
    {
        Point dropPosition = e.GetPosition(controlCanvas);

        DataFormat pictureFormat =
            DataFormat.CreateStringPlatformFormat(
                "application/novelconstructor-picture");

        DataFormat textFormat =
            DataFormat.CreateStringPlatformFormat(
                "application/novelconstructor-text");

        // Пустой контрол изображения из панели инструментов
        if (e.DataTransfer.Formats.Contains(pictureFormat))
        {
            CreateEmptyImageControl(dropPosition);
            e.Handled = true;
            return;
        }

        // Пустой текстовый контрол из панели инструментов
        if (e.DataTransfer.Formats.Contains(textFormat))
        {
            CreateEmptyTextControl(dropPosition);
            e.Handled = true;
            return;
        }

        // Файл из Проводника
        if (e.DataTransfer.Formats.Contains(DataFormat.File))
        {
            IStorageItem[]? files = e.DataTransfer.TryGetFiles();

            if (files == null)
                return;

            foreach (IStorageItem file in files)
            {
                if (file.Path == null)
                    continue;

                string filePath = file.Path.LocalPath;

                if (IsImageFile(filePath))
                {
                    CreateImageControl(filePath, dropPosition);
                }
                else if (IsTextFile(filePath))
                {
                    await CreateTextControl(filePath, dropPosition);
                }
            }

            e.Handled = true;
        }
    }
    private bool IsImageFile(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension == ".png" ||
               extension == ".jpg" ||
               extension == ".jpeg" ||
               extension == ".bmp" ||
               extension == ".gif" ||
               extension == ".webp";
    }

    private bool IsTextFile(string filePath)
    {
        string extension =
            Path.GetExtension(filePath).ToLowerInvariant();

        return extension == ".txt" ||
               extension == ".rtf" ||
               extension == ".docx";
    }

    private void CreateEmptyImageControl(Point dropPosition)
    {
        ConstructorPictureBox pictureBox =
            new ConstructorPictureBox();

        ConstructorControlContainer container =
            new ConstructorControlContainer(pictureBox);

        container.SetBackground(Brushes.LightBlue);

        container.Width = 200;
        container.Height = 150;

        Point controlPosition =
            GetControlPosition(
                dropPosition,
                container.Width,
                container.Height);

        Canvas.SetLeft(
            container,
            controlPosition.X);

        Canvas.SetTop(
            container,
            controlPosition.Y);

        AddControlToActiveLayer(container);
    }

    private void CreateEmptyTextControl(Point dropPosition)
    {
        ConstructorTextBox textBox =
            new ConstructorTextBox();

        ConstructorControlContainer container =
            new ConstructorControlContainer(textBox);

        container.Width = 300;
        container.Height = 200;

        Point controlPosition =
            GetControlPosition(
                dropPosition,
                container.Width,
                container.Height);

        Canvas.SetLeft(
            container,
            controlPosition.X);

        Canvas.SetTop(
            container,
            controlPosition.Y);

        AddControlToActiveLayer(container);
    }

    private async void IconImage_PointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        DataTransfer dataTransfer = new DataTransfer();

        DataTransferItem item = new DataTransferItem();

        item.Set(
            DataFormat.CreateStringPlatformFormat(
                "application/novelconstructor-picture"),
            "picture");

        dataTransfer.Add(item);

        await DragDrop.DoDragDropAsync(
            e,
            dataTransfer,
            DragDropEffects.Copy);
    }

    private async void TextFileImage_PointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        DataTransfer dataTransfer = new DataTransfer();

        DataTransferItem item = new DataTransferItem();

        item.Set(
            DataFormat.CreateStringPlatformFormat(
                "application/novelconstructor-text"),
            "text");

        dataTransfer.Add(item);

        await DragDrop.DoDragDropAsync(
            e,
            dataTransfer,
            DragDropEffects.Copy);
    }

    private void CreateImageControl(
    string filePath,
    Point dropPosition)
    {
        ConstructorPictureBox pictureBox = new ConstructorPictureBox();

        Bitmap bitmap = new Bitmap(filePath);

        pictureBox.Source = bitmap;

        ConstructorControlContainer container =
            new ConstructorControlContainer(pictureBox);

        container.SetBackground(Brushes.LightBlue);

        container.Width = 200;
        container.Height = 150;

        Point controlPosition =
             GetControlPosition(
                 dropPosition,
                 container.Width,
                 container.Height);

        Canvas.SetLeft(
            container,
            controlPosition.X);

        Canvas.SetTop(
            container,
            controlPosition.Y);

        AddControlToActiveLayer(container);
    }
    private async System.Threading.Tasks.Task CreateTextControl(
            string filePath,
            Point dropPosition)
    {
        ConstructorTextBox textBox =
            new ConstructorTextBox();

        await textBox.LoadFromFileAsync(filePath);

        ConstructorControlContainer container =
            new ConstructorControlContainer(textBox);

        container.Width = 300;
        container.Height = 200;

        Point controlPosition =
            GetControlPosition(
                dropPosition,
                container.Width,
                container.Height);

        Canvas.SetLeft(
            container,
            controlPosition.X);

        Canvas.SetTop(
            container,
            controlPosition.Y);

        AddControlToActiveLayer(container);
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
    private Point GetControlPosition(
        Point dropPosition,
        double controlWidth,
        double controlHeight)
    {
        double left =
            dropPosition.X - controlWidth / 2;

        double top =
            dropPosition.Y - controlHeight / 2;

        double maxLeft =
            Math.Max(
                0,
                controlCanvas.Bounds.Width - controlWidth);

        double maxTop =
            Math.Max(
                0,
                controlCanvas.Bounds.Height - controlHeight);

        left = Math.Clamp(
            left,
            0,
            maxLeft);

        top = Math.Clamp(
            top,
            0,
            maxTop);

        return new Point(left, top);
    }
}