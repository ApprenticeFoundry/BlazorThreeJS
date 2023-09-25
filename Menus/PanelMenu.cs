// Decompiled with JetBrains decompiler
// Type: Blazor3D.Lights.Light
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using System.Text.Json.Serialization;
using BlazorThreeJS.Core;

namespace BlazorThreeJS.Menus
{
    public class PanelMenu : Object3D
    {
        public PanelMenu(string type = "Menu") : base(type)
        {
        }

        public double Width { get; set; } = 2.0;
        public double Height { get; set; } = 1.0;
        public List<Button> Buttons { get; set; } = new();
        public PanelMenu AddButton(Button button)
        {
            Buttons.Add(button);
            return this;
        }

    }
}
