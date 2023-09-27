// Decompiled with JetBrains decompiler
// Type: Blazor3D.Helpers.PointLightHelper
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;
using BlazorThreeJS.Lights;



namespace BlazorThreeJS.Helpers
{
    public sealed class PointLightHelper : Object3D
    {
        public PointLight Light { get; set; } = new PointLight();

        public double SphereSize { get; set; } = 1.0;

        public string? Color { get; set; }

        public PointLightHelper()
          : base(nameof(PointLightHelper))
        {
        }

        public PointLightHelper(PointLight light, string color, double sphereSize = 1.0 )
          : this()
        {
            this.Light = light ?? new PointLight();
            this.SphereSize = sphereSize;
            this.Color = color;
        }


    }
}
