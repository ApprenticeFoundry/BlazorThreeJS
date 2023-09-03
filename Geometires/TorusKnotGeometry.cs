// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.TorusKnotGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Geometires
{
    public sealed class TorusKnotGeometry : BufferGeometry
    {
        public TorusKnotGeometry()
          : base(nameof(TorusKnotGeometry))
        {
        }

        public TorusKnotGeometry(
          double radius = 1,
          double tube = 0.4,
          int radialSegments = 8,
          int tubularSegments = 64,
          int p = 2,
          int q = 3)
          : this()
        {
            this.Radius = radius;
            this.Tube = tube;
            this.RadialSegments = radialSegments;
            this.TubularSegments = tubularSegments;
            this.P = p;
            this.Q = q;
        }

        public double Radius { get; set; } = 1;

        public double Tube { get; set; } = 0.4;

        public int RadialSegments { get; set; } = 8;

        public int TubularSegments { get; set; } = 64;

        public int P { get; set; } = 2;

        public int Q { get; set; } = 3;
    }
}
