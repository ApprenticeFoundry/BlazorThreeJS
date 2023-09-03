// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.DodecahedronGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Geometires
{
    public class DodecahedronGeometry : BufferGeometry
    {
        public DodecahedronGeometry()
          : base(nameof(DodecahedronGeometry))
        {
        }

        public DodecahedronGeometry(double radius = 1, int detail = 0)
          : this()
        {
            this.Radius = radius;
            this.Detail = detail;
        }

        public double Radius { get; set; } = 1;

        public int Detail { get; set; }
    }
}
