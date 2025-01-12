

using BlazorThreeJS.Core;

namespace BlazorThreeJS.Geometires
{
    public sealed class BoxGeometry : BufferGeometry
    {
        public BoxGeometry()
          : base(nameof(BoxGeometry))
        {
        }

        public BoxGeometry(
          double width = 1f,
          double height = 1f,
          double depth = 1f,
          int widthSegments = 1,
          int heightSegments = 1,
          int depthSegments = 1)
          : this()
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.WidthSegments = widthSegments;
            this.HeightSegments = heightSegments;
            this.DepthSegments = depthSegments;
        }

        public double Width { get; set; } = 1;

        public double Height { get; set; } = 1;

        public double Depth { get; set; } = 1;

        public int WidthSegments { get; set; } = 1;

        public int HeightSegments { get; set; } = 1;

        public int DepthSegments { get; set; } = 1;
    }
}
