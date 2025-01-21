using BlazorThreeJS.Maths;



namespace BlazorThreeJS.Maths
{
    public class Transform3 
    {
        public Vector3 Position { get; set; } = new Vector3();

        public Vector3 Pivot { get; set; } = new Vector3();

        public Euler Rotation { get; set; } = new Euler();  //maybe change to Quaternion

        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
    }
}
