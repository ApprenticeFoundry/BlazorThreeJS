

using BlazorThreeJS.Core;



namespace BlazorThreeJS.Helpers
{
    public sealed class BoxHelper : Object3D
    {
        public Object3D? Object3D { get; set; }

        public string Color { get; set; } = "0xffff00";

        public BoxHelper()
          : base(nameof(BoxHelper))
        {
        }

        public BoxHelper(Object3D object3d, string color = "0xffff00")
          : this()
        {
            this.Object3D = object3d;
            this.Color = color;
        }


    }
}
