// Decompiled with JetBrains decompiler
// Type: Blazor3D.Lights.PointLight
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

namespace BlazorThreeJS.Lights
{
    public sealed class PointLight : Light
    {
        public PointLight()
          : base(nameof(PointLight))
        {
        }

        public double Distance { get; set; }

        public double Decay { get; set; } = 1;
    }
}
