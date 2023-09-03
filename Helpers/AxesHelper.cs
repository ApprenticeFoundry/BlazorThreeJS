// Decompiled with JetBrains decompiler
// Type: Blazor3D.Helpers.AxesHelper
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Helpers
{
    public sealed class AxesHelper : Object3D
    {
        public AxesHelper()
          : base(nameof(AxesHelper))
        {
        }

        public AxesHelper(double size = 1.0)
          : this()
        {
            this.Size = size;
        }

        public double Size { get; set; } = 1.0;
    }
}
