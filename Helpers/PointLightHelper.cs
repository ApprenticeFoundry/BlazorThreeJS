

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
