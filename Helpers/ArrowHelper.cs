// Decompiled with JetBrains decompiler
// Type: Blazor3D.Helpers.ArrowHelper
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;



namespace BlazorThreeJS.Helpers
{
    public sealed class ArrowHelper : Object3D
    {
        public ArrowHelper()
          : base(nameof(ArrowHelper))
        {
        }

        public ArrowHelper(
          Vector3 dir = null,
          Vector3 origin = null,
          double length = 1.0,
          string color = "0xffff00",
          double headLength = 0.2,
          double headWidth = 0.040000000000000008)
          : this()
        {
            this.Dir = dir ?? new Vector3(0.0f, 0.0f, 1f);
            this.Origin = origin ?? new Vector3(0.0f, 0.0f, 0.0f);
            this.Length = length;
            this.Color = color;
            this.HeadLength = headLength;
            this.HeadWidth = headWidth;
        }

        public Vector3 Dir { get; set; } = new Vector3(0.0f, 0.0f, 1f);

        public Vector3 Origin { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);

        public double Length { get; set; } = 1.0;

        public string Color { get; set; } = "0xffff00";

        public double HeadLength { get; set; } = 0.2;

        public double HeadWidth { get; set; } = 0.040000000000000008;
    }
}
