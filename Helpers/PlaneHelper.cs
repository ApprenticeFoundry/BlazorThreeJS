

using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;



namespace BlazorThreeJS.Helpers
{
    public sealed class PlaneHelper : Object3D
    {
        public PlaneHelper()
          : base(nameof(PlaneHelper))
        {
        }

        public PlaneHelper(Plane plane, double size = 1.0, string color = "0xffff00")
          : this()
        {
            this.Plane = plane ?? new Plane();
            this.Size = size;
            this.Color = color;
        }

        public Plane Plane { get; set; } = new Plane();

        public double Size { get; set; } = 1.0;

        public string Color { get; set; } = "0xffff00";
    }
}
