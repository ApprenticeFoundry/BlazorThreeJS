// Decompiled with JetBrains decompiler
// Type: Blazor3D.Cameras.PerspectiveCamera
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

namespace BlazorThreeJS.Cameras
{
    public sealed class PerspectiveCamera : Camera
    {
        public PerspectiveCamera()
          : base(nameof(PerspectiveCamera))
        {
        }

        public PerspectiveCamera(double fov, double near, double far)
          : this()
        {
            this.Fov = fov;
            this.Near = near;
            this.Far = far;
        }

        public double Fov { get; set; } = 75.0;

        public double Near { get; set; } = 0.1;

        public double Far { get; set; } = 1000.0;
    }
}
