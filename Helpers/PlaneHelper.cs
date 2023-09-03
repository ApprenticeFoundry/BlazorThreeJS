// Decompiled with JetBrains decompiler
// Type: Blazor3D.Helpers.PlaneHelper
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;



namespace BlazorThreeJS.Helpers
{
    public sealed class PlaneHelper : Object3D
    {
        public PlaneHelper()
          : base(nameof(PlaneHelper))
        {
        }

        public PlaneHelper(Plane plane = null, double size = 1.0, string color = "0xffff00")
          : this()
        {
            this.Plane = plane ?? new Plane();
            this.Size = size;
            this.Color = color;
        }

        public Plane Plane { get; set; } = new Plane();

        public double Size { get; set; } = 1.0;

        public string Color { get; set; } = "0xffff00";
    }
}
