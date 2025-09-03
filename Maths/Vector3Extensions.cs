namespace BlazorThreeJS.Maths;

/// <summary>
/// Advanced Vector3 extensions ported from FoundryBlazor Vector3DMathExtensions
/// </summary>
public static class Vector3Extensions
{
    /// <summary>Project vector onto another vector</summary>
    public static Vector3 Project(this Vector3 vector, Vector3 onto)
    {
        var ontoNormalized = onto.Normalize();
        var projectionLength = Vector3.Dot(vector, ontoNormalized);
        return ontoNormalized * projectionLength;
    }
    
    /// <summary>Reflect vector across a normal</summary>
    public static Vector3 Reflect(this Vector3 vector, Vector3 normal)
    {
        var normalizedNormal = normal.Normalize();
        return vector - normalizedNormal * (2 * Vector3.Dot(vector, normalizedNormal));
    }
    
    /// <summary>Linear interpolation between two vectors</summary>
    public static Vector3 Lerp(this Vector3 from, Vector3 to, double t)
    {
        return new Vector3(
            from.X + (to.X - from.X) * t,
            from.Y + (to.Y - from.Y) * t,
            from.Z + (to.Z - from.Z) * t
        );
    }
    
    /// <summary>Check if vectors are approximately equal</summary>
    public static bool ApproximatelyEqual(this Vector3 a, Vector3 b, double tolerance = 0.001)
    {
        return Math.Abs(a.X - b.X) < tolerance && 
               Math.Abs(a.Y - b.Y) < tolerance && 
               Math.Abs(a.Z - b.Z) < tolerance;
    }
    
    /// <summary>Clamp vector components to range</summary>
    public static Vector3 Clamp(this Vector3 vector, double min, double max)
    {
        return new Vector3(
            Math.Clamp(vector.X, min, max),
            Math.Clamp(vector.Y, min, max),
            Math.Clamp(vector.Z, min, max)
        );
    }
    
    /// <summary>Get the angle between two vectors in radians</summary>
    public static double AngleTo(this Vector3 from, Vector3 to)
    {
        var denominator = Math.Sqrt(from.Length() * to.Length());
        if (denominator < 1e-8) return Math.PI / 2;
        
        var dot = Vector3.Dot(from, to) / denominator;
        return Math.Acos(Math.Clamp(dot, -1.0, 1.0));
    }
    
    /// <summary>Get the signed angle between two vectors around an axis</summary>
    public static double SignedAngleTo(this Vector3 from, Vector3 to, Vector3 axis)
    {
        var angle = from.AngleTo(to);
        var cross = Vector3.Cross(from, to);
        var sign = Vector3.Dot(cross, axis);
        return sign < 0 ? -angle : angle;
    }
    
    /// <summary>Convert to string with formatted precision</summary>
    public static string ToStringFormatted(this Vector3 vector, int precision = 2)
    {
        var format = $"F{precision}";
        return $"({vector.X.ToString(format)}, {vector.Y.ToString(format)}, {vector.Z.ToString(format)})";
    }
    
    /// <summary>Create a vector with absolute values</summary>
    public static Vector3 Abs(this Vector3 vector)
    {
        return new Vector3(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
    }
    
    /// <summary>Get the component-wise minimum of two vectors</summary>
    public static Vector3 Min(this Vector3 a, Vector3 b)
    {
        return new Vector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
    }
    
    /// <summary>Get the component-wise maximum of two vectors</summary>
    public static Vector3 Max(this Vector3 a, Vector3 b)
    {
        return new Vector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
    }
    
    /// <summary>Floor all components</summary>
    public static Vector3 Floor(this Vector3 vector)
    {
        return new Vector3(Math.Floor(vector.X), Math.Floor(vector.Y), Math.Floor(vector.Z));
    }
    
    /// <summary>Ceiling all components</summary>
    public static Vector3 Ceiling(this Vector3 vector)
    {
        return new Vector3(Math.Ceiling(vector.X), Math.Ceiling(vector.Y), Math.Ceiling(vector.Z));
    }
    
    /// <summary>Round all components</summary>
    public static Vector3 Round(this Vector3 vector)
    {
        return new Vector3(Math.Round(vector.X), Math.Round(vector.Y), Math.Round(vector.Z));
    }
    
    /// <summary>Negate the vector</summary>
    public static Vector3 Negate(this Vector3 vector)
    {
        return new Vector3(-vector.X, -vector.Y, -vector.Z);
    }
    
    /// <summary>Get squared distance to another vector (faster than Distance)</summary>
    public static double DistanceSquared(this Vector3 from, Vector3 to)
    {
        var diff = from - to;
        return diff.X * diff.X + diff.Y * diff.Y + diff.Z * diff.Z;
    }
    
    /// <summary>Get squared length (faster than Length)</summary>
    public static double LengthSquared(this Vector3 vector)
    {
        return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
    }
    
    /// <summary>Set the length of the vector while maintaining direction</summary>
    public static Vector3 SetLength(this Vector3 vector, double length)
    {
        return vector.Normalize() * length;
    }
    
    /// <summary>Limit the length of the vector to a maximum value</summary>
    public static Vector3 ClampLength(this Vector3 vector, double minLength, double maxLength)
    {
        var length = vector.Length();
        if (length < minLength) return vector.SetLength(minLength);
        if (length > maxLength) return vector.SetLength(maxLength);
        return vector;
    }
}
