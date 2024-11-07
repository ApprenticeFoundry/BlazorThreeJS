

using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;



namespace BlazorThreeJS.Helpers
{
    public sealed class ArrowHelper : Object3D
    {
        public ArrowHelper()
          : base(nameof(ArrowHelper))
        {
        }

        public ArrowHelper(
          Vector3 dir,
          Vector3 origin,
          double length = 1.0,
          string color = "0xffff00",
          double headLength = 0.2,
          double headWidth = 0.04)
          : this()
        {
            this.Dir = dir ?? new Vector3(0.0f, 0.0f, 1f);
            this.Origin = origin ?? new Vector3(0.0f, 0.0f, 0.0f);
            this.Length = length;
            this.Color = color;
            this.HeadLength = headLength;
            this.HeadWidth = headWidth;
        }

        public Vector3 Dir { get; set; } = new Vector3(0.0f, 0.0f, 1f);

        public Vector3 Origin { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);

        public double Length { get; set; } = 1.0;

        public string Color { get; set; } = "0xffff00";

        public double HeadLength { get; set; } = 0.2;

        public double HeadWidth { get; set; } = 0.04;
    }
}
