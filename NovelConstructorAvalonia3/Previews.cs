using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovelConstructorAvalonia3
{
    internal class Previews
    {
        public class LayerPreview : Border
        {
            private readonly ModelsIn.ControlLayer layer;

            private readonly Image previewImage;
            private readonly TextBlock nameText;
            private readonly TextBox nameTextBox;
            private readonly CheckBox visibilityCheckBox;

            private readonly Button moveUpButton;
            private readonly Button moveDownButton;

            private readonly Border dragArea;

            public ModelsIn.ControlLayer Layer
            {
                get
                {
                    return layer;
                }
            }
            public void SetPreview(IImage image)
            {
                previewImage.Source = image;
            }

            public LayerPreview(
                ModelsIn.ControlLayer layer)
            {
                this.layer = layer;

                BorderThickness = new Thickness(1);
                BorderBrush = Brushes.Black;
                Background = Brushes.Gray;
                Padding = new Thickness(3);

                Grid mainGrid = new Grid
                {
                    ColumnDefinitions =
                        new ColumnDefinitions("22,*,24"),
                    RowDefinitions =
                        new RowDefinitions("*,24")
                };

                Grid leftPanel = new Grid
                {
                    RowDefinitions =
                        new RowDefinitions("22,*,22")
                };

                moveUpButton = new Button
                {
                    Content = "▲",
                    Padding = new Thickness(0),
                    HorizontalContentAlignment =
                        Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalContentAlignment =
                        Avalonia.Layout.VerticalAlignment.Center
                };

                dragArea = new Border
                {
                    Background = Brushes.DarkGray,
                    Cursor = new Cursor(
                        StandardCursorType.SizeNorthSouth)
                };

                TextBlock dragText = new TextBlock
                {
                    Text = "≡",
                    FontSize = 20,
                    HorizontalAlignment =
                        Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment =
                        Avalonia.Layout.VerticalAlignment.Center
                };

                dragArea.Child = dragText;

                moveDownButton = new Button
                {
                    Content = "▼",
                    Padding = new Thickness(0),
                    HorizontalContentAlignment =
                        Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalContentAlignment =
                        Avalonia.Layout.VerticalAlignment.Center
                };

                Grid.SetRow(moveUpButton, 0);
                Grid.SetRow(dragArea, 1);
                Grid.SetRow(moveDownButton, 2);

                leftPanel.Children.Add(moveUpButton);
                leftPanel.Children.Add(dragArea);
                leftPanel.Children.Add(moveDownButton);

                previewImage = new Image
                {
                    Stretch = Stretch.Uniform,
                    Height = 70,
                    HorizontalAlignment =
                        Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment =
                        Avalonia.Layout.VerticalAlignment.Stretch
                };

                visibilityCheckBox = new CheckBox
                {
                    IsChecked = layer.IsVisible,
                    HorizontalAlignment =
                        Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment =
                        Avalonia.Layout.VerticalAlignment.Center
                };

                Grid nameGrid = new Grid();

                nameText = new TextBlock
                {
                    Text = layer.Name,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment =
                        Avalonia.Layout.VerticalAlignment.Center
                };

                nameTextBox = new TextBox
                {
                    Text = layer.Name,
                    IsVisible = false,
                    Padding = new Thickness(2),
                    HorizontalContentAlignment =
                        Avalonia.Layout.HorizontalAlignment.Center
                };

                nameGrid.Children.Add(nameText);
                nameGrid.Children.Add(nameTextBox);

                Grid.SetColumn(leftPanel, 0);
                Grid.SetRowSpan(leftPanel, 2);

                Grid.SetColumn(previewImage, 1);
                Grid.SetRow(previewImage, 0);

                Grid.SetColumn(visibilityCheckBox, 2);
                Grid.SetRowSpan(visibilityCheckBox, 2);

                Grid.SetColumn(nameGrid, 1);
                Grid.SetRow(nameGrid, 1);

                mainGrid.Children.Add(leftPanel);
                mainGrid.Children.Add(previewImage);
                mainGrid.Children.Add(visibilityCheckBox);
                mainGrid.Children.Add(nameGrid);

                Child = mainGrid;

                nameText.DoubleTapped += OnNameDoubleTapped;

                nameTextBox.KeyDown += OnNameTextBoxKeyDown;
                nameTextBox.LostFocus += OnNameTextBoxLostFocus;

                visibilityCheckBox.IsCheckedChanged +=
                    OnVisibilityCheckBoxChanged;
            }

            private void OnNameDoubleTapped(
                object? sender,
                TappedEventArgs e)
            {
                nameTextBox.Text = layer.Name;

                nameText.IsVisible = false;
                nameTextBox.IsVisible = true;

                nameTextBox.Focus();
                nameTextBox.SelectAll();

                e.Handled = true;
            }

            private void OnNameTextBoxKeyDown(
                object? sender,
                KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    FinishRename();
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.Escape)
                {
                    CancelRename();
                    e.Handled = true;
                }
            }

            private void OnNameTextBoxLostFocus(
                object? sender,
                Avalonia.Interactivity.RoutedEventArgs e)
            {
                if (!nameTextBox.IsVisible)
                    return;

                FinishRename();
            }

            private void FinishRename()
            {
                string newName =
                    nameTextBox.Text?.Trim() ?? "";

                if (newName.Length == 0)
                {
                    newName = layer.Name;
                }

                layer.Name = newName;
                nameText.Text = newName;

                nameTextBox.IsVisible = false;
                nameText.IsVisible = true;
            }

            private void CancelRename()
            {
                nameTextBox.Text = layer.Name;

                nameTextBox.IsVisible = false;
                nameText.IsVisible = true;
            }

            private void OnVisibilityCheckBoxChanged(
                object? sender,
                Avalonia.Interactivity.RoutedEventArgs e)
            {
                layer.IsVisible =
                    visibilityCheckBox.IsChecked == true;
            }
        }
    }
}
