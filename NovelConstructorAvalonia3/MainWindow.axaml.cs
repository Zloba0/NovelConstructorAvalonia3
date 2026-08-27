using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using System.Linq;
using static NovelConstructorAvalonia3.Controls;
using Avalonia;
using System;
using System.IO;

namespace NovelConstructorAvalonia3;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DragDrop.AddDragOverHandler(controlCanvas, ControlCanvas_DragOver);
        DragDrop.AddDropHandler(controlCanvas, ControlCanvas_Drop);

        iconImage.PointerPressed += IconImage_PointerPressed;
        textFileImage.PointerPressed += TextFileImage_PointerPressed;
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

        container.Width = 200;
        container.Height = 150;

        Canvas.SetLeft(
            container,
            dropPosition.X - container.Width / 2);

        Canvas.SetTop(
            container,
            dropPosition.Y - container.Height / 2);

        controlCanvas.Children.Add(container);
    }

    private void CreateEmptyTextControl(Point dropPosition)
    {
        ConstructorTextBox textBox =
            new ConstructorTextBox();

        ConstructorControlContainer container =
            new ConstructorControlContainer(textBox);

        container.Width = 300;
        container.Height = 200;

        Canvas.SetLeft(
            container,
            dropPosition.X - container.Width / 2);

        Canvas.SetTop(
            container,
            dropPosition.Y - container.Height / 2);

        controlCanvas.Children.Add(container);
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

        container.Width = 200;
        container.Height = 150;

        Canvas.SetLeft(
            container,
            dropPosition.X - container.Width / 2);

        Canvas.SetTop(
            container,
            dropPosition.Y - container.Height / 2);

        controlCanvas.Children.Add(container);
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

        Canvas.SetLeft(
            container,
            dropPosition.X - container.Width / 2);

        Canvas.SetTop(
            container,
            dropPosition.Y - container.Height / 2);

        controlCanvas.Children.Add(container);
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