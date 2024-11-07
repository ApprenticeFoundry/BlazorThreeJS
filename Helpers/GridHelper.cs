

using BlazorThreeJS.Core;



namespace BlazorThreeJS.Helpers
{
    public sealed class GridHelper : Object3D
    {
        public GridHelper()
          : base(nameof(GridHelper))
        {
        }

        public GridHelper(double size = 10.0, int devisions = 10, string colorCenterLine = "0x444444", string colorGrid = "0x888888")
          : this()
        {
            this.Size = size;
            this.Divisions = devisions;
            this.ColorCenterLine = colorCenterLine;
            this.ColorGrid = colorGrid;
        }

        public double Size { get; set; } = 10.0;

        public int Divisions { get; set; } = 10;

        public string ColorCenterLine { get; set; } = "0x444444";

        public string ColorGrid { get; set; } = "0x888888";
    }
}
