// Decompiled with JetBrains decompiler
// Type: Blazor3D.Cameras.OrthographicCamera
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

namespace BlazorThreeJS.Cameras
{
    public sealed class OrthographicCamera : Camera
    {
        public OrthographicCamera()
          : base(nameof(OrthographicCamera))
        {
        }

        public OrthographicCamera(
          double left = -1.0,
          double right = 1.0,
          double top = 1.0,
          double bottom = -1.0,
          double near = 0.1,
          double far = 2000.0,
          double zoom = 1.0)
          : this()
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
            this.Near = near;
            this.Far = far;
            this.Zoom = zoom;
        }

        public double Left { get; set; } = -1.0;

        public double Right { get; set; } = 1.0;

        public double Top { get; set; } = 1.0;

        public double Bottom { get; set; } = -1.0;

        public double Near { get; set; } = 0.1;

        public double Far { get; set; } = 2000.0;

        public double Zoom { get; set; } = 1.0;
    }
}
