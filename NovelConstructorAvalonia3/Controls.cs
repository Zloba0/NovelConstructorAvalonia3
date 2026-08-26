using Avalonia.Controls;
using Avalonia.Media;
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
        //public class ConstructorTextBox : RichTextBox
        //{
        //    public ConstructorTextBox()
        //    {
        //        // настройки конструктора
        //    }
        //}
        public class ConstructorControlContainer : ContentControl
        {
            public Control InnerControl { get; }
        }
    }
}
