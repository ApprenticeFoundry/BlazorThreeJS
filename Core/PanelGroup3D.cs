

namespace BlazorThreeJS.Core
{
    public class PanelGroup3D : Object3D
    {
        public PanelGroup3D() : base(nameof(PanelGroup3D))
        {
        }
        public double Width { get; set; } = 2.0;
        public double Height { get; set; } = 1.0;
        public string Color { get; set; } = "#333333";
        public List<string> TextLines { get; set; } = new();
        public List<TextPanel3D> TextPanels { get; set; } = new();
        public List<Object3D> Meshes { get; set; } = new();
    }
}
