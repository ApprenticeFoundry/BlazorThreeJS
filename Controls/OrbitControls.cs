// Decompiled with JetBrains decompiler
// Type: Blazor3D.Controls.OrbitControls
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Maths;



namespace BlazorThreeJS.Controls
{
    public sealed class OrbitControls
    {
        public bool Enabled { get; set; } = true;

        public double MinDistance { get; set; }

        public double MaxDistance { get; set; } = 10000;

        public Vector3 TargetPosition { get; set; } = new Vector3();
    }
}
