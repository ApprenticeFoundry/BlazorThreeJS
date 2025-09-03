

namespace BlazorThreeJS.Maths
{
    public class Vector3
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;

        public Vector3() { }
        public Vector3(double x, double y, double z)
        {
            Set(x, y, z);
        }

        public Vector3 Cross(Vector3 other)
        {
            return new Vector3(
                Y * other.Z - Z * other.Y,
                Z * other.X - X * other.Z,
                X * other.Y - Y * other.X
            );
        }

        public double Dot(Vector3 other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector3 Normalize()
        {
            var len = Length();
            if (len < 1e-8) return new Vector3(0, 0, 0);
            return new Vector3(X / len, Y / len, Z / len);
        }

        public double distanceXZ()
        {
            return Math.Sqrt(this.X * this.X + this.Z * this.Z);
        }

        public double bearingXZ()
        {
            return Math.Atan2(this.X, this.Z);
        }

        public Vector3 copyFrom(Vector3 pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            return this;
        }

        public Vector3 Set(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            return this;
        }

        public Vector3 Add(double x, double y, double z)
        {
            X += x;
            Y += y;
            Z += z;
            return this;
        }

        public Vector3 CreatePlus(double x, double y, double z)
        {
            return new Vector3(X + x, Y + y, Z + z);
        }

        public static double LengthOf(Vector3 v)
        {
            if (v == null) return 0.0;
            return Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }
    }
}

