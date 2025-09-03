using System;

namespace BlazorThreeJS.Maths
{
    /// <summary>
    /// Enhanced Quaternion class with mathematical operations for 3D rotations
    /// Quaternions provide gimbal-lock-free rotation and smooth interpolation
    /// </summary>
    public class Quaternion
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;
        public double W { get; set; } = 1; // Identity quaternion has W=1

        public Quaternion()
        {
            W = 1; // Identity quaternion
        }

        public Quaternion(double x, double y, double z, double w)
        {
            Set(x, y, z, w);
        }

        public Quaternion Set(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            return this;
        }

        /// <summary>
        /// Identity quaternion (no rotation)
        /// </summary>
        public static Quaternion Identity => new Quaternion(0, 0, 0, 1);

        /// <summary>
        /// Create quaternion from axis-angle rotation
        /// </summary>
        public static Quaternion AngleAxis(double angle, Vector3 axis)
        {
            var halfAngle = angle * 0.5;
            var sin = Math.Sin(halfAngle);
            var cos = Math.Cos(halfAngle);
            
            var normalized = axis.Normalize();
            return new Quaternion(
                normalized.X * sin,
                normalized.Y * sin,
                normalized.Z * sin,
                cos
            );
        }

        /// <summary>
        /// Create quaternion that rotates from one vector to another
        /// Perfect for face-to-face alignment in constraints!
        /// </summary>
        public static Quaternion FromToRotation(Vector3 from, Vector3 to)
        {
            var fromNorm = from.Normalize();
            var toNorm = to.Normalize();
            
            var dot = Vector3.Dot(fromNorm, toNorm);
            
            // Vectors are already aligned
            if (dot >= 0.999999)
                return Identity;
            
            // Vectors are opposite
            if (dot <= -0.999999)
            {
                // Find perpendicular axis
                var axis = Math.Abs(fromNorm.X) < 0.9 ? 
                    Vector3.Cross(Vector3.Right, fromNorm) : 
                    Vector3.Cross(Vector3.Up, fromNorm);
                return AngleAxis(Math.PI, axis.Normalize());
            }
            
            // Standard case
            var cross = Vector3.Cross(fromNorm, toNorm);
            var w = Math.Sqrt((1 + dot) * 2);
            var invW = 1.0 / w;
            
            return new Quaternion(
                cross.X * invW,
                cross.Y * invW,
                cross.Z * invW,
                w * 0.5
            );
        }

        /// <summary>
        /// Create quaternion from Euler angles
        /// </summary>
        public static Quaternion FromEuler(Euler euler)
        {
            return FromEuler(euler.X, euler.Y, euler.Z);
        }

        public static Quaternion FromEuler(double x, double y, double z)
        {
            var xRad = x * Math.PI / 180.0 * 0.5;
            var yRad = y * Math.PI / 180.0 * 0.5;
            var zRad = z * Math.PI / 180.0 * 0.5;

            var cx = Math.Cos(xRad);
            var sx = Math.Sin(xRad);
            var cy = Math.Cos(yRad);
            var sy = Math.Sin(yRad);
            var cz = Math.Cos(zRad);
            var sz = Math.Sin(zRad);

            return new Quaternion(
                sx * cy * cz - cx * sy * sz,
                cx * sy * cz + sx * cy * sz,
                cx * cy * sz - sx * sy * cz,
                cx * cy * cz + sx * sy * sz
            );
        }

        /// <summary>
        /// Convert quaternion to Euler angles
        /// </summary>
        public Euler ToEuler()
        {
            // Roll (x-axis rotation)
            var sinr_cosp = 2 * (W * X + Y * Z);
            var cosr_cosp = 1 - 2 * (X * X + Y * Y);
            var roll = Math.Atan2(sinr_cosp, cosr_cosp);

            // Pitch (y-axis rotation)
            var sinp = 2 * (W * Y - Z * X);
            var pitch = Math.Abs(sinp) >= 1 ? 
                Math.CopySign(Math.PI / 2, sinp) : 
                Math.Asin(sinp);

            // Yaw (z-axis rotation)
            var siny_cosp = 2 * (W * Z + X * Y);
            var cosy_cosp = 1 - 2 * (Y * Y + Z * Z);
            var yaw = Math.Atan2(siny_cosp, cosy_cosp);

            return new Euler(
                (float)(roll * 180.0 / Math.PI),
                (float)(pitch * 180.0 / Math.PI),
                (float)(yaw * 180.0 / Math.PI)
            );
        }

        /// <summary>
        /// Multiply two quaternions (combine rotations)
        /// </summary>
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
                a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
            );
        }

        /// <summary>
        /// Normalize quaternion (ensure unit length)
        /// </summary>
        public Quaternion Normalize()
        {
            var length = Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
            if (length < 0.000001)
                return Identity;
            
            var invLength = 1.0 / length;
            return new Quaternion(X * invLength, Y * invLength, Z * invLength, W * invLength);
        }

        /// <summary>
        /// Spherical linear interpolation between quaternions
        /// Perfect for smooth rotation animations
        /// </summary>
        public static Quaternion Slerp(Quaternion a, Quaternion b, double t)
        {
            var dot = a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
            
            // If dot is negative, slerp won't take the shorter path
            if (dot < 0.0)
            {
                b = new Quaternion(-b.X, -b.Y, -b.Z, -b.W);
                dot = -dot;
            }
            
            if (dot > 0.9995)
            {
                // Linear interpolation for very close quaternions
                return new Quaternion(
                    a.X + t * (b.X - a.X),
                    a.Y + t * (b.Y - a.Y),
                    a.Z + t * (b.Z - a.Z),
                    a.W + t * (b.W - a.W)
                ).Normalize();
            }
            
            var theta0 = Math.Acos(Math.Abs(dot));
            var theta = theta0 * t;
            var sinTheta = Math.Sin(theta);
            var sinTheta0 = Math.Sin(theta0);
            
            var s0 = Math.Cos(theta) - dot * sinTheta / sinTheta0;
            var s1 = sinTheta / sinTheta0;
            
            return new Quaternion(
                s0 * a.X + s1 * b.X,
                s0 * a.Y + s1 * b.Y,
                s0 * a.Z + s1 * b.Z,
                s0 * a.W + s1 * b.W
            );
        }

        public override string ToString()
        {
            return $"Quaternion({X:F3}, {Y:F3}, {Z:F3}, {W:F3})";
        }

        /// <summary>
        /// Check if two quaternions are approximately equal
        /// </summary>
        public static bool operator ==(Quaternion a, Quaternion b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            
            const double epsilon = 0.000001;
            return Math.Abs(a.X - b.X) < epsilon &&
                   Math.Abs(a.Y - b.Y) < epsilon &&
                   Math.Abs(a.Z - b.Z) < epsilon &&
                   Math.Abs(a.W - b.W) < epsilon;
        }

        public static bool operator !=(Quaternion a, Quaternion b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            return obj is Quaternion q && this == q;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }
    }
}
