

using BlazorThreeJS.Menus;

namespace BlazorThreeJS.Core
{
    public class PanelMenu3D : Object3D
    {
        public PanelMenu3D(string type = "PanelMenu3D") : base(type)
        {
        }

        public double Width { get; set; } = 2.0;
        public double Height { get; set; } = 1.0;
        public List<Button3D> Buttons { get; set; } = new();
        public PanelMenu3D AddButton(Button3D button)
        {
            Buttons.Add(button);
            return this;
        }

    }
}
