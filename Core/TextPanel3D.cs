

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Core
{
    public class TextPanel3D : Object3D
    {
        public TextPanel3D() : base(nameof(TextPanel3D))
        {
        }
        public double Width { get; set; } = 2.0;
        public double Height { get; set; } = 1.0;
        public List<string> TextLines { get; set; } = new();
        public string Color { get; set; } = "#333333";
    }
}
