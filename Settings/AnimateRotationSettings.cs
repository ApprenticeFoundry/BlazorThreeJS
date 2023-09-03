// Decompiled with JetBrains decompiler
// Type: Blazor3D.Settings.AnimateRotationSettings
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

namespace BlazorThreeJS.Settings
{
    public sealed class AnimateRotationSettings
    {
        public AnimateRotationSettings()
        {
        }

        public AnimateRotationSettings(
          bool animateRotation = false,
          double thetaX = 0.1,
          double thetaY = 0.1,
          double thetaZ = 0.1,
          double radius = 5.0)
        {
            this.AnimateRotation = animateRotation;
            this.ThetaX = thetaX;
            this.ThetaY = thetaY;
            this.ThetaZ = thetaZ;
            this.Radius = radius;
        }

        public bool AnimateRotation { get; set; }

        public double ThetaX { get; set; } = 0.1;

        public double ThetaY { get; set; } = 0.1;

        public double ThetaZ { get; set; } = 0.1;

        public double Radius { get; set; } = 5.0;

        public bool StopAnimationOnOrbitControlMove { get; set; }
    }
}
