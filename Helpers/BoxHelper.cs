// Decompiled with JetBrains decompiler
// Type: Blazor3D.Helpers.BoxHelper
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;



namespace BlazorThreeJS.Helpers
{
    public sealed class BoxHelper : Object3D
    {
          public Object3D? Object3D { get; set; }

        public string Color { get; set; } = "0xffff00";

        public BoxHelper()
          : base(nameof(BoxHelper))
        {
        }

        public BoxHelper(Object3D object3d, string color = "0xffff00")
          : this()
        {
            this.Object3D = object3d;
            this.Color = color;
        }


    }
}
