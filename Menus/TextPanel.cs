// Decompiled with JetBrains decompiler
// Type: Blazor3D.Lights.Light
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Menus
{
    public class TextPanel : Object3D
    {
        public TextPanel(string type = "TextPanel") : base(type)
        {
        }
        public double Width { get; set; } = 2.0;
        public double Height { get; set; } = 1.0;
        public List<string> TextLines { get; set; } = new();
        public string Color { get; set; } = "#333333";
    }
}
