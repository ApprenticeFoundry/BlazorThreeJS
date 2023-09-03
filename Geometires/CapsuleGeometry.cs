// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.CapsuleGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Geometires
{
    public sealed class CapsuleGeometry : BufferGeometry
    {
        public CapsuleGeometry()
          : base(nameof(CapsuleGeometry))
        {
        }

        public CapsuleGeometry(double radius = 1f, double length = 1f, int capSegments = 4, int radialSegments = 8)
          : this()
        {
            this.Radius = radius;
            this.Length = length;
            this.CapSegments = capSegments;
            this.RadialSegments = radialSegments;
        }

        public double Radius { get; set; } = 1;

        public double Length { get; set; } = 1;

        public int CapSegments { get; set; } = 4;

        public int RadialSegments { get; set; } = 8;
    }
}
