// Decompiled with JetBrains decompiler
// Type: Blazor3D.Cameras.Camera
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

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
