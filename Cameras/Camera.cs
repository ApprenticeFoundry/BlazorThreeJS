

using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Settings;


namespace BlazorThreeJS.Cameras
{
    public abstract class Camera : Object3D
    {
        protected Camera(string type = "Camera")
          : base(type)
        {
        }

        public AnimateRotationSettings AnimateRotationSettings { get; set; } = new AnimateRotationSettings();

        public Vector3 LookAt { get; set; } = new Vector3();
    }
}
