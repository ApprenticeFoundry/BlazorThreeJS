

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Helpers
{
    public sealed class AxesHelper : Object3D
    {
        public AxesHelper()
          : base(nameof(AxesHelper))
        {
        }

        public AxesHelper(double size = 1.0)
          : this()
        {
            this.Size = size;
        }

        public double Size { get; set; } = 1.0;
    }
}
