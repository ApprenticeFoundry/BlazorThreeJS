

namespace BlazorThreeJS.Maths;

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

    // Add two vectors to create a third
    public Vector3 Add(Vector3 b)
    {
        return new Vector3
        {
            X = this.X + b.X,
            Y = this.Y + b.Y,
            Z = this.Z + b.Z
        };
    }

    // Lerp a point along a line specified by a vector using a scaler
    public Vector3 Lerp(Vector3 b, double t)
    {
        return new Vector3
        {
            X = this.X + (b.X - this.X) * t,
            Y = this.Y + (b.Y - this.Y) * t,
            Z = this.Z + (b.Z - this.Z) * t
        };
    }

    // Convert to euler angles
    public Vector3 ToEuler()
    {
        var x = Math.Atan2(this.Y, this.Z);
        var y = Math.Atan2(this.Z, this.X);
        var z = Math.Atan2(this.X, this.Y);
        return new Vector3
        {
            X = x,
            Y = y,
            Z = z
        };
    }

    // Subtract two vectors to create a third
    public Vector3 Subtract(Vector3 b)
    {
        return new Vector3
        {
            X = this.X - b.X,
            Y = this.Y - b.Y,
            Z = this.Z - b.Z
        };
    }

    // Center point of two vectors
    public Vector3 Center(Vector3 b)
    {
        return new Vector3
        {
            X = (this.X + b.X) / 2,
            Y = (this.Y + b.Y) / 2,
            Z = (this.Z + b.Z) / 2
        };
    }

    // Multiply vector by scalar
    public Vector3 Multiply(double scalar)
    {
        return new Vector3
        {
            X = this.X * scalar,
            Y = this.Y * scalar,
            Z = this.Z * scalar
        };
    }

    // Bounding box (absolute difference)
    public Vector3 BoundingBox(Vector3 b)
    {
        return new Vector3
        {
            X = Math.Abs(this.X - b.X),
            Y = Math.Abs(this.Y - b.Y),
            Z = Math.Abs(this.Z - b.Z)
        };
    }

    // Distance between two vectors
    public double Distance(Vector3 b)
    {
        return Math.Sqrt(
            Math.Pow(this.X - b.X, 2) +
            Math.Pow(this.Y - b.Y, 2) +
            Math.Pow(this.Z - b.Z, 2)
        );
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

    // Operator overloads for compatibility with FoVector3D
    public static Vector3 operator +(Vector3 a, Vector3 b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3 operator -(Vector3 a, Vector3 b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector3 operator *(Vector3 v, double scalar)
        => new(v.X * scalar, v.Y * scalar, v.Z * scalar);

    public static Vector3 operator *(double scalar, Vector3 v)
        => new(v.X * scalar, v.Y * scalar, v.Z * scalar);

    // Static methods for compatibility with FoVector3D extensions
    public static Vector3 Cross(Vector3 a, Vector3 b)
        => new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

    public static double Dot(Vector3 a, Vector3 b)
        => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static double Distance(Vector3 a, Vector3 b)
        => (a - b).Length();

    public Vector3 Clamp(double min, double max)
        => new(Math.Clamp(X, min, max), Math.Clamp(Y, min, max), Math.Clamp(Z, min, max));

    // Common vector constants
    public static Vector3 Zero => new(0, 0, 0);
    public static Vector3 One => new(1, 1, 1);
    public static Vector3 Up => new(0, 1, 0);
    public static Vector3 Down => new(0, -1, 0);
    public static Vector3 Left => new(-1, 0, 0);
    public static Vector3 Right => new(1, 0, 0);
    public static Vector3 Forward => new(0, 0, 1);
    public static Vector3 Back => new(0, 0, -1);
}


