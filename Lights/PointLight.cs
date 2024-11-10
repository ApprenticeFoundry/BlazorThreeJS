

namespace BlazorThreeJS.Lights;

    public sealed class PointLight : Light
    {
        public PointLight()
          : base(nameof(PointLight))
        {
        }

        public double Distance { get; set; }

        public double Decay { get; set; } = 1;
    }

