// Decompiled with JetBrains decompiler
// Type: Blazor3D.Lights.Light
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;



namespace BlazorThreeJS.Lights
{
    public abstract class Light : Object3D
    {
        protected Light(string type = "Light")
          : base(type)
        {
        }

        public string Color { get; set; } = "white";

        public double Intensity { get; set; } = 1;
    }
}
