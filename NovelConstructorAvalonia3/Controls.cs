using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvRichTextBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }

            public void SetPlainText(string text)
            {
                FlowDocument.Selection.Text = text;
            }
        }
        public class ConstructorControlContainer : ContentControl
        {
            private bool isDragging;
            private Point dragStartPoint;
            private double startLeft;
            private double startTop;

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

                dragStartPoint = e.GetPosition(this);

                startLeft = Canvas.GetLeft(this);
                startTop = Canvas.GetTop(this);

                if (double.IsNaN(startLeft))
                    startLeft = 0;

                if (double.IsNaN(startTop))
                    startTop = 0;

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

                if (Parent is not Visual parent)
                    return;

                Point currentPosition = e.GetPosition(parent);

                Point pressPosition = e.GetPosition(this);

                double deltaX = currentPosition.X - pressPosition.X;
                double deltaY = currentPosition.Y - pressPosition.Y;

                double newLeft = startLeft + deltaX;
                double newTop = startTop + deltaY;

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
