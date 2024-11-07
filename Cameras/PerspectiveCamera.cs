

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
