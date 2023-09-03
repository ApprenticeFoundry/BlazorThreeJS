// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.CylinderGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;

namespace BlazorThreeJS.Geometires
{
    public sealed class TubeGeometry : BufferGeometry
    {
        public TubeGeometry()
          : base(nameof(TubeGeometry))
        {
        }

        public TubeGeometry(
          double radius,
          List<Vector3> path,
          int radialSegments = 8,
          int tubularSegments = 10)
          : this()
        {
            this.Radius = radius;
            this.RadialSegments = radialSegments;
            this.TubularSegments = tubularSegments;
            this.Path = path ?? new List<Vector3>();
        }

        public double Radius { get; set; } = 1;
        public List<Vector3> Path { get; set; } = new();
        public int TubularSegments { get; set; } = 8;
        public int RadialSegments { get; set; } = 8;

    }
}
