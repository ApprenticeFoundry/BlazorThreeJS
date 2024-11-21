// Decompiled with JetBrains decompiler
// Type: Blazor3D.Lights.Light
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;
using BlazorThreeJS.Objects;

namespace BlazorThreeJS.Menus
{
    public class PanelGroup : Object3D
    {
        public PanelGroup(string type = "PanelGroup") : base(type)
        {
        }
        public double Width { get; set; } = 2.0;
        public double Height { get; set; } = 1.0;
        public string Color { get; set; } = "#333333";
        public List<string> TextLines { get; set; } = new();
        public List<TextPanel> TextPanels { get; set; } = new();
        public List<Object3D> Meshes { get; set; } = new();
    }
}
