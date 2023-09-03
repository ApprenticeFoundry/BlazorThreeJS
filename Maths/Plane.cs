// Decompiled with JetBrains decompiler
// Type: Blazor3D.Maths.Plane
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll


namespace BlazorThreeJS.Maths
{
    public sealed class Plane
    {
        public Plane()
        {
        }

        public Plane(Vector3 normal = null, double constant = 0.0)
        {
            this.Normal = normal ?? new Vector3(1f, 0.0f, 0.0f);
            this.Constant = constant;
        }

        public Vector3 Normal { get; set; } = new Vector3(1f, 0.0f, 0.0f);

        public double Constant { get; set; }
    }
}
