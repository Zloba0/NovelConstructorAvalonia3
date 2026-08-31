using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using AvRichTextBox;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapePath = Avalonia.Controls.Shapes.Path;
using Word = DocumentFormat.OpenXml.Wordprocessing;

namespace NovelConstructorAvalonia3
{
    internal class Controls
    {
        public class ConstructorPictureBox : Image
        {
            public ConstructorPictureBox()
            {
                Stretch = Stretch.Fill;
            }
        }
        public class ConstructorTextBox : RichTextBox
        {
            public ConstructorTextBox()
            {
                Cursor = new Cursor(StandardCursorType.Ibeam);

                Padding = new Thickness(0);

                FlowDocument = new FlowDocument();

                RemoveDocumentPadding();
            }

            public void SetPlainText(string text)
            {
                FlowDocument.Selection.Text = text;
            }

            public async Task LoadFromFileAsync(string filePath)
            {
                string extension =
                    Path.GetExtension(filePath)
                        .ToLowerInvariant();

                switch (extension)
                {
                    case ".txt":
                        await LoadTextFileAsync(filePath);
                        break;

                    case ".rtf":
                        LoadRtfFile(filePath);
                        break;

                    case ".docx":
                        LoadDocxFile(filePath);
                        break;

                    default:
                        throw new NotSupportedException(
                            $"Формат '{extension}' не поддерживается.");
                }
            }

            private async Task LoadTextFileAsync(
                string filePath)
            {
                string text =
                    await File.ReadAllTextAsync(
                        filePath,
                        Encoding.UTF8);

                FlowDocument.Selection.Text = text;
            }

            private void LoadRtfFile(
                string filePath)
            {
                using FileStream stream =
                    new FileStream(
                        filePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read);

                FlowDocument.Selection.Load(
                    stream,
                    ContentDataFormat.Rtf);
            }

            private void LoadDocxFile(string filePath)
            {
                LoadWordDoc(filePath);

                FlowDocument.PagePadding =
                    new Thickness(0);

                foreach (AvRichTextBox.Block block
                    in FlowDocument.Blocks)
                {
                    block.Margin =
                        new Thickness(0);
                }
            }

            private void RemoveDocumentPadding()
            {
                FlowDocument.PagePadding =
                    new Thickness(0);
            }
        }
        public class ConstructorControlContainer : ContentControl
        {
            private const double ResizeHandleSize = 16;
            private const double MinimumControlSize = 40;

            private bool isDragging;
            private bool isResizing;
            private Point dragOffset;
            private Point resizeStartPoint;
            private double resizeStartWidth;
            private double resizeStartHeight;

            private readonly Border border;
            private readonly ShapePath resizeHandle;

            public Control InnerControl { get; }

            public bool IsSelected { get; private set; }

            public ConstructorControlContainer(Control innerControl)
            {
                InnerControl = innerControl;

                border = new Border
                {
                    BorderThickness = new Thickness(3),
                    BorderBrush = Brushes.Transparent,
                    Background = Brushes.Transparent,
                    Cursor = new Cursor(StandardCursorType.SizeAll)
                };

                resizeHandle = new ShapePath
                {
                    Width = ResizeHandleSize,
                    Height = ResizeHandleSize,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                    Data = Geometry.Parse("M 16,0 L 16,16 L 0,16 Z"),
                    Fill = Brushes.White,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1,
                    Cursor = new Cursor(StandardCursorType.BottomRightCorner)
                };

                Grid overlay = new Grid();

                overlay.Children.Add(innerControl);
                overlay.Children.Add(border);
                overlay.Children.Add(resizeHandle);

                Content = overlay;

                resizeHandle.PointerPressed += OnResizePointerPressed;
                resizeHandle.PointerMoved += OnResizePointerMoved;
                resizeHandle.PointerReleased += OnResizePointerReleased;
                resizeHandle.PointerCaptureLost += OnResizePointerCaptureLost;


                border.PointerPressed += OnPointerPressed;
                border.PointerMoved += OnPointerMoved;
                border.PointerReleased += OnPointerReleased;
                border.PointerCaptureLost += OnPointerCaptureLost;

                Select();
            }
            private void OnMoveHandlePointerPressed(
                object? sender,
                PointerPressedEventArgs e)
            {
                PointerPoint pointerPoint =
                    e.GetCurrentPoint(this);

                if (!pointerPoint.Properties.IsLeftButtonPressed)
                    return;

                Select();

                dragOffset = e.GetPosition(this);

                isDragging = true;

                e.Handled = true;
            }
            public void Select()
            {
                IsSelected = true;
                border.BorderBrush = Brushes.Blue;
            }

            public void Deselect()
            {
                IsSelected = false;
                border.BorderBrush = Brushes.Transparent;
            }
            private void OnPointerPressed(
                object? sender,
                PointerPressedEventArgs e)
            {
                if (!ReferenceEquals(e.Source, border))
                    return;

                PointerPoint pointerPoint =
                    e.GetCurrentPoint(this);

                if (!pointerPoint.Properties.IsLeftButtonPressed)
                    return;

                Select();

                dragOffset = e.GetPosition(this);

                isDragging = true;

                e.Pointer.Capture(border);

                e.Handled = true;
            }

            private void OnPointerMoved(
                object? sender,
                PointerEventArgs e)
            {
                if (!isDragging)
                    return;

                if (Parent is not Canvas canvas)
                    return;

                Point pointerPosition =
                    e.GetPosition(canvas);

                double newLeft =
                    pointerPosition.X - dragOffset.X;

                double newTop =
                    pointerPosition.Y - dragOffset.Y;

                double maxLeft =
                    Math.Max(
                        0,
                        canvas.Bounds.Width - Bounds.Width);

                double maxTop =
                    Math.Max(
                        0,
                        canvas.Bounds.Height - Bounds.Height);

                newLeft =
                    Math.Clamp(newLeft, 0, maxLeft);

                newTop =
                    Math.Clamp(newTop, 0, maxTop);

                Canvas.SetLeft(this, newLeft);
                Canvas.SetTop(this, newTop);

                e.Handled = true;
            }

            private void OnPointerReleased(
                object? sender,
                PointerReleasedEventArgs e)
            {
                if (!isDragging)
                    return;

                isDragging = false;

                e.Pointer.Capture(null);

                e.Handled = true;
            }

            private void OnPointerCaptureLost(
                object? sender,
                PointerCaptureLostEventArgs e)
            {
                isDragging = false;
            }

            private void OnResizePointerPressed(
                object? sender,
                PointerPressedEventArgs e)
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(resizeHandle);

                if (!pointerPoint.Properties.IsLeftButtonPressed ||
                    Parent is not Canvas canvas)
                {
                    return;
                }

                Select();
                isDragging = false;
                isResizing = true;
                resizeStartPoint = e.GetPosition(canvas);
                resizeStartWidth = Bounds.Width;
                resizeStartHeight = Bounds.Height;

                e.Pointer.Capture(resizeHandle);
                e.Handled = true;
            }

            private void OnResizePointerMoved(
                object? sender,
                PointerEventArgs e)
            {
                if (!isResizing || Parent is not Canvas canvas)
                    return;

                Point currentPoint = e.GetPosition(canvas);
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);

                if (double.IsNaN(left))
                    left = 0;

                if (double.IsNaN(top))
                    top = 0;

                double requestedWidth =
                    resizeStartWidth + currentPoint.X - resizeStartPoint.X;
                double requestedHeight =
                    resizeStartHeight + currentPoint.Y - resizeStartPoint.Y;

                double maximumWidth = Math.Max(
                    MinimumControlSize,
                    canvas.Bounds.Width - left);
                double maximumHeight = Math.Max(
                    MinimumControlSize,
                    canvas.Bounds.Height - top);

                Width = Math.Clamp(
                    requestedWidth,
                    MinimumControlSize,
                    maximumWidth);
                Height = Math.Clamp(
                    requestedHeight,
                    MinimumControlSize,
                    maximumHeight);

                e.Handled = true;
            }

            private void OnResizePointerReleased(
                object? sender,
                PointerReleasedEventArgs e)
            {
                if (!isResizing)
                    return;

                isResizing = false;
                e.Pointer.Capture(null);
                e.Handled = true;
            }

            private void OnResizePointerCaptureLost(
                object? sender,
                PointerCaptureLostEventArgs e)
            {
                isResizing = false;
            }
        }
    }
}
