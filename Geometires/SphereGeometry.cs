// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.SphereGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Geometires
{
    public sealed class SphereGeometry : BufferGeometry
    {
        public SphereGeometry()
          : base(nameof(SphereGeometry))
        {
        }

        public SphereGeometry(
          double radius = 1,
          int widthSegments = 32,
          int heightSegments = 16,
          double phiStart = 0.0,
          double phiLength = 6.28318548,
          double thetaStart = 0.0,
          double thetaLength = 6.28318548)
          : this()
        {
            this.Radius = radius;
            this.WidthSegments = widthSegments;
            this.HeightSegments = heightSegments;
            this.PhiStart = phiStart;
            this.PhiLength = phiLength;
            this.ThetaStart = thetaStart;
            this.ThetaLength = thetaLength;
        }

        public double Radius { get; set; } = 1;

        public int WidthSegments { get; set; } = 32;

        public int HeightSegments { get; set; } = 16;

        public double PhiStart { get; set; }

        public double PhiLength { get; set; } = 6.28318548;

        public double ThetaStart { get; set; }

        public double ThetaLength { get; set; } = 6.28318548;
    }
}
