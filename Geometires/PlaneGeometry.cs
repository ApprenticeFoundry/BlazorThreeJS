// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.PlaneGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Geometires
{
    public sealed class PlaneGeometry : BufferGeometry
    {
        public PlaneGeometry()
          : base(nameof(PlaneGeometry))
        {
        }

        public PlaneGeometry(double width = 1, double height = 1, int widthSegments = 1, int heightSegments = 1)
          : this()
        {
            this.Width = width;
            this.Height = height;
            this.WidthSegments = widthSegments;
            this.HeightSegments = heightSegments;
        }

        public double Width { get; set; } = 1;

        public double Height { get; set; } = 1;

        public int WidthSegments { get; set; } = 1;

        public int HeightSegments { get; set; } = 1;
    }
}
