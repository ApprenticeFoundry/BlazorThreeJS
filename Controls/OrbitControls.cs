

using BlazorThreeJS.Maths;



namespace BlazorThreeJS.Controls
{
    public sealed class OrbitControls
    {
        public bool Enabled { get; set; } = true;

        public double MinDistance { get; set; }

        public double MaxDistance { get; set; } = 10000;

        public Vector3 TargetPosition { get; set; } = new Vector3();
    }
}
