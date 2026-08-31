using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovelConstructorAvalonia3
{
    internal class ModelsIn
    {
        public class ControlLayer
        {
            public List<Control> Controls { get; set; } = new List<Control>();

            public string Name { get; set; } = "Слой";

            public bool IsVisible { get; set; } = true;
        }
        public class Slide
        {
            public List<ControlLayer> Layers { get; set; } = new List<ControlLayer>();

            public ControlLayer? ActiveLayer { get; set; }

            public string? AudioFile { get; set; }

            public int Number { get; set; }
        }
        public class ChoiceSlide : Slide
        {
            public List<SlideGroup> SlideGroups { get; set; } = new List<SlideGroup>();
        }
        public class SlideGroup
        {
            public List<Slide> Slides { get; set; } = new List<Slide>();

            public Slide? CurrentSlide { get; set; }

            public int Number { get; set; }
        }
    }
}
