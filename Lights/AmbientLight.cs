

namespace BlazorThreeJS.Lights;

    public sealed class AmbientLight : Light
    {
        public AmbientLight()
          : base(nameof(AmbientLight))
        {
            this.Intensity = 0.6;
        }
    }

