// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.RingGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Geometires
{
    public sealed class RingGeometry : BufferGeometry
    {
        public RingGeometry()
          : base(nameof(RingGeometry))
        {
        }

        public RingGeometry(
          double innerRadius = 0.5,
          double outerRadius = 1,
          int thetaSegments = 8,
          int phiSegments = 1,
          double thetaStart = 0.0,
          double thetaLength = 6.28318548)
          : this()
        {
            this.InnerRadius = innerRadius;
            this.OuterRadius = outerRadius;
            this.ThetaSegments = thetaSegments;
            this.PhiSegments = phiSegments;
            this.ThetaStart = thetaStart;
            this.ThetaLength = thetaLength;
        }

        public double InnerRadius { get; set; } = 0.5;

        public double OuterRadius { get; set; } = 1;

        public int ThetaSegments { get; set; } = 8;

        public int PhiSegments { get; set; } = 1;

        public double ThetaStart { get; set; }

        public double ThetaLength { get; set; } = 6.28318548;
    }
}
