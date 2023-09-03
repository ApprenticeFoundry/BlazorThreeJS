// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.CircleGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Geometires
{
    public sealed class CircleGeometry : BufferGeometry
    {
        public CircleGeometry()
          : base(nameof(CircleGeometry))
        {
        }

        public CircleGeometry(double radius = 1, int segments = 8, double thetaStart = 0.0, double thetaLength = 6.28318548)
          : this()
        {
            this.Radius = radius;
            this.Segments = segments;
            this.ThetaStart = thetaStart;
            this.ThetaLength = thetaLength;
        }

        public double Radius { get; set; } = 1;

        public int Segments { get; set; } = 8;

        public double ThetaStart { get; set; }

        public double ThetaLength { get; set; } = 6.28318548;
    }
}
