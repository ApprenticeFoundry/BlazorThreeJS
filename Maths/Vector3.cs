

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
    public Vector3 Copy()
    {
        return new Vector3(X, Y, Z);
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

    // Static version for convenience (matching extension method API)
    public static Vector3 Lerp(Vector3 from, Vector3 to, double t)
    {
        return from.Lerp(to, t);
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

    // public double distanceXZ()
    // {
    //     return Math.Sqrt(this.X * this.X + this.Z * this.Z);
    // }

    // public double bearingXZ()
    // {
    //     return Math.Atan2(this.X, this.Z);
    // }

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

    // === ADVANCED VECTOR OPERATIONS ===
    // (Moved from Vector3Extensions for better design)
    
    /// <summary>Project this vector onto another vector</summary>
    public Vector3 Project(Vector3 onto)
    {
        var ontoNormalized = onto.Normalize();
        var projectionLength = Dot(ontoNormalized);
        return ontoNormalized * projectionLength;
    }
    
    /// <summary>Reflect this vector across a normal</summary>
    public Vector3 Reflect(Vector3 normal)
    {
        var normalizedNormal = normal.Normalize();
        return this - normalizedNormal * (2 * Dot(normalizedNormal));
    }
    
    /// <summary>Check if vectors are approximately equal</summary>
    public bool ApproximatelyEqual(Vector3 other, double tolerance = 0.001)
    {
        return Math.Abs(X - other.X) < tolerance && 
               Math.Abs(Y - other.Y) < tolerance && 
               Math.Abs(Z - other.Z) < tolerance;
    }
    
    /// <summary>Get the angle between this vector and another in radians</summary>
    public double AngleTo(Vector3 other)
    {
        var denominator = Math.Sqrt(Length() * other.Length());
        if (denominator < 1e-8) return Math.PI / 2;
        
        var dot = Dot(other) / denominator;
        return Math.Acos(Math.Clamp(dot, -1.0, 1.0));
    }
    
    /// <summary>Get the signed angle between this vector and another around an axis</summary>
    public double SignedAngleTo(Vector3 other, Vector3 axis)
    {
        var angle = AngleTo(other);
        var cross = Cross(other);
        var sign = Dot(cross, axis);
        return sign < 0 ? -angle : angle;
    }
    
    /// <summary>Convert to string with formatted precision</summary>
    public string ToStringFormatted(int precision = 2)
    {
        var format = $"F{precision}";
        return $"({X.ToString(format)}, {Y.ToString(format)}, {Z.ToString(format)})";
    }
    
    /// <summary>Create a vector with absolute values</summary>
    public Vector3 Abs()
    {
        return new Vector3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
    }
    
    /// <summary>Get the component-wise minimum of this vector and another</summary>
    public Vector3 Min(Vector3 other)
    {
        return new Vector3(Math.Min(X, other.X), Math.Min(Y, other.Y), Math.Min(Z, other.Z));
    }
    
    /// <summary>Get the component-wise maximum of this vector and another</summary>
    public Vector3 Max(Vector3 other)
    {
        return new Vector3(Math.Max(X, other.X), Math.Max(Y, other.Y), Math.Max(Z, other.Z));
    }
    
    /// <summary>Floor all components</summary>
    public Vector3 Floor()
    {
        return new Vector3(Math.Floor(X), Math.Floor(Y), Math.Floor(Z));
    }
    
    /// <summary>Ceiling all components</summary>
    public Vector3 Ceiling()
    {
        return new Vector3(Math.Ceiling(X), Math.Ceiling(Y), Math.Ceiling(Z));
    }
    
    /// <summary>Round all components</summary>
    public Vector3 Round()
    {
        return new Vector3(Math.Round(X), Math.Round(Y), Math.Round(Z));
    }
    
    /// <summary>Negate the vector</summary>
    public Vector3 Negate()
    {
        return new Vector3(-X, -Y, -Z);
    }
    
    /// <summary>Get squared distance to another vector (faster than Distance)</summary>
    public double DistanceSquared(Vector3 other)
    {
        var diff = this - other;
        return diff.X * diff.X + diff.Y * diff.Y + diff.Z * diff.Z;
    }
    
    /// <summary>Get squared length (faster than Length)</summary>
    public double LengthSquared()
    {
        return X * X + Y * Y + Z * Z;
    }
    
    /// <summary>Set the length of the vector while maintaining direction</summary>
    public Vector3 SetLength(double length)
    {
        return Normalize() * length;
    }
    
    /// <summary>Limit the length of the vector to a maximum value</summary>
    public Vector3 ClampLength(double minLength, double maxLength)
    {
        var length = Length();
        if (length < minLength) return SetLength(minLength);
        if (length > maxLength) return SetLength(maxLength);
        return this;
    }

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


