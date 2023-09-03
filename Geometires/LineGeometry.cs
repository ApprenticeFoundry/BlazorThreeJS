// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.CylinderGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;

namespace BlazorThreeJS.Geometires
{
    public sealed class LineGeometry : BufferGeometry
    {
        public LineGeometry()
          : base(nameof(LineGeometry))
        {
        }

        public LineGeometry(
          List<Vector3> path)
          : this()
        {
            this.Path = path ?? new List<Vector3>();
        }

        public List<Vector3> Path { get; set; } = new();
    }
}
