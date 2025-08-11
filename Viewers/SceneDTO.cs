// Decompiled with JetBrains decompiler
// Type: Blazor3D.Viewers.Viewer
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Cameras;
using BlazorThreeJS.Controls;

using BlazorThreeJS.Settings;

namespace BlazorThreeJS.Viewers
{
    public class SceneDTO
    {
        public Scene3D? Scene { get; set; }
        public ViewerSettings? ViewerSettings { get; set; }
        public Camera? Camera { get; set; }
        public OrbitControls? OrbitControls { get; set; }
    }
}
