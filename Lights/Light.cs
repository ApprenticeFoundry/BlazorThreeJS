
using BlazorThreeJS.Core;


namespace BlazorThreeJS.Lights;

    public abstract class Light : Object3D
    {
        protected Light(string type)
          : base(type)
        {
        }

        public string Color { get; set; } = "white";

        public double Intensity { get; set; } = 1;
    }

