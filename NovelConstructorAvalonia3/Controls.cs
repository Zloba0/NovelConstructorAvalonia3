using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvRichTextBox;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                FlowDocument = new FlowDocument();
            }

            public void SetPlainText(string text)
            {
                FlowDocument.Selection.Text = text;
            }

            private async Task LoadTextFileAsync(string filePath)
            {
                string text = await File.ReadAllTextAsync(
                    filePath,
                    Encoding.UTF8);

                FlowDocument.Selection.Text = text;
            }

            private void LoadDocxFile(string filePath)
            {
                LoadWordDoc(filePath);

                using WordprocessingDocument document =
                    WordprocessingDocument.Open(filePath, false);

                IEnumerable<Word.Paragraph> sourceParagraphs =
                    document.MainDocumentPart?.Document.Body?
                        .Elements<Word.Paragraph>()
                    ?? Enumerable.Empty<Word.Paragraph>();

                IEnumerable<AvRichTextBox.Paragraph> loadedParagraphs =
                    FlowDocument.Blocks.OfType<AvRichTextBox.Paragraph>();

                foreach ((Word.Paragraph source, AvRichTextBox.Paragraph loaded)
                    in sourceParagraphs.Zip(loadedParagraphs))
                {
                    string alignment = source.ParagraphProperties?
                        .Justification?.Val?.Value.ToString()
                        .ToLowerInvariant() ?? "left";

                    loaded.TextAlignment = alignment switch
                    {
                        "center" => TextAlignment.Center,
                        "right" or "end" => TextAlignment.Right,
                        "both" or "distribute" => TextAlignment.Justify,
                        _ => TextAlignment.Left
                    };
                }
            }

            public async Task LoadFromFileAsync(string filePath)
            {
                string extension = Path.GetExtension(filePath).ToLowerInvariant();

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
            private void LoadRtfFile(string filePath)
            {
                using FileStream stream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                FlowDocument.Selection.Load(
                    stream,
                    ContentDataFormat.Rtf);
            }
        }
        public class ConstructorControlContainer : ContentControl
        {
            private bool isDragging;
            private Point dragStartPoint;
            private double startLeft;
            private double startTop;
            private Point dragOffset;

            private readonly Border border;

            public Control InnerControl { get; }

            public bool IsSelected { get; private set; }

            public ConstructorControlContainer(Control innerControl)
            {
                InnerControl = innerControl;

                border = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Transparent,
                    Background = Brushes.Transparent,
                    Child = innerControl
                };

                Content = border;

                PointerPressed += OnPointerPressed;
                PointerMoved += OnPointerMoved;
                PointerReleased += OnPointerReleased;
                PointerCaptureLost += OnPointerCaptureLost;

                Select();
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
                PointerPoint pointerPoint = e.GetCurrentPoint(this);

                if (!pointerPoint.Properties.IsLeftButtonPressed)
                    return;

                Select();

                dragOffset = e.GetPosition(this);

                isDragging = true;

                e.Pointer.Capture(this);

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

                Point pointerPosition = e.GetPosition(canvas);

                double newLeft = pointerPosition.X - dragOffset.X;
                double newTop = pointerPosition.Y - dragOffset.Y;

                double maxLeft = Math.Max(0, canvas.Bounds.Width - Bounds.Width);
                double maxTop = Math.Max(0, canvas.Bounds.Height - Bounds.Height);

                newLeft = Math.Clamp(newLeft, 0, maxLeft);
                newTop = Math.Clamp(newTop, 0, maxTop);

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
        }
    }
}
