namespace BlazorThreeJS.Maths;

/// <summary>
/// Conversion utilities for migrating from FoundryBlazor FoVector3D to BlazorThreeJS Vector3
/// Provides seamless bridging between double-precision and float-precision vector systems
/// </summary>
public static class VectorConversions
{
    // =====================================================
    // FOUNDRYBLAZOR FOVECTOR3D COMPATIBILITY CONVERSIONS
    // =====================================================
    
    /// <summary>Convert from FoVector3D format (X, Y, Z) to Vector3</summary>
    public static Vector3 FromFoVector3D(double x, double y, double z)
        => new(x, y, z);
    
    /// <summary>Convert Vector3 to FoVector3D format (returns tuple for now)</summary>
    public static (double X, double Y, double Z) ToFoVector3DFormat(this Vector3 vector)
        => (vector.X, vector.Y, vector.Z);
    
    /// <summary>Helper for creating Vector3 from transformation parameters</summary>
    public static Vector3 FromTransformParams(double x, double y, double z)
        => new(x, y, z);
    
    /// <summary>Helper for creating Vector3 from scale parameters</summary>
    public static Vector3 FromScaleParams(double sx, double sy, double sz)
        => new(sx, sy, sz);
    
    /// <summary>Helper for creating Vector3 from rotation parameters (degrees)</summary>
    public static Vector3 FromRotationParams(double rx, double ry, double rz)
        => new(rx, ry, rz);
    
    /// <summary>Create Vector3 with double precision inputs (auto-convert to float)</summary>
    public static Vector3 CreateFromDoubles(double x, double y, double z)
        => new((float)x, (float)y, (float)z);
    
    /// <summary>Convert Vector3 to formatted string like FoVector3D.ToStringFormatted()</summary>
    public static string ToFoVector3DString(this Vector3 vector, int precision = 2)
    {
        var format = $"F{precision}";
        return $"({vector.X.ToString(format)}, {vector.Y.ToString(format)}, {vector.Z.ToString(format)})";
    }
    
    // =====================================================
    // FOVECTOR3D MATHEMATICAL OPERATIONS AS STATIC METHODS
    // =====================================================
    
    /// <summary>Add two vectors (FoVector3D.Add equivalent)</summary>
    public static Vector3 Add(Vector3 a, Vector3 b) => a + b;
    
    /// <summary>Subtract two vectors (FoVector3D.Subtract equivalent)</summary>
    public static Vector3 Subtract(Vector3 a, Vector3 b) => a - b;
    
    /// <summary>Multiply vector by scalar (FoVector3D.Multiply equivalent)</summary>
    public static Vector3 Multiply(Vector3 vector, double scalar) => vector * scalar;
    
    /// <summary>Normalize vector (FoVector3D.Normalize equivalent)</summary>
    public static Vector3 Normalize(Vector3 vector) => vector.Normalize();
    
    /// <summary>Cross product (FoVector3D.Cross equivalent)</summary>
    public static Vector3 Cross(Vector3 a, Vector3 b) => Vector3.Cross(a, b);
    
    /// <summary>Dot product (FoVector3D.Dot equivalent)</summary>
    public static double Dot(Vector3 a, Vector3 b) => Vector3.Dot(a, b);
    
    /// <summary>Vector length (FoVector3D.Length equivalent)</summary>
    public static double Length(Vector3 vector) => vector.Length();
    
    /// <summary>Linear interpolation (FoVector3D.Lerp equivalent)</summary>
    public static Vector3 Lerp(Vector3 from, Vector3 to, double t) => from.Lerp(to, t);
    
    // =====================================================
    // POINT3D COMPATIBILITY (FOUNDRYBLAZOR PATTERN)
    // =====================================================
    
    /// <summary>Convert Point3D-style data to Vector3</summary>
    public static Vector3 FromPoint3D(double x, double y, double z, string name = "")
    {
        // Name is ignored as Vector3 doesn't support naming
        return new Vector3((float)x, (float)y, (float)z);
    }
    
    /// <summary>Convert Vector3 to Point3D-style tuple with name</summary>
    public static (double X, double Y, double Z, string Name) ToPoint3DFormat(this Vector3 vector, string name = "")
        => ((double)vector.X, (double)vector.Y, (double)vector.Z, name);
    
    // =====================================================
    // MATRIX3D COMPATIBILITY HELPERS
    // =====================================================
    
    /// <summary>Transform point using Matrix3 (Matrix3D.TransformPoint equivalent)</summary>
    public static Vector3 TransformPoint(Matrix3 matrix, double x, double y, double z)
    {
        var point = new Vector3((float)x, (float)y, (float)z);
        return matrix.TransformPoint(point);
    }
    
    /// <summary>Transform point and return as double tuple (Matrix3D/FoVector3D pattern)</summary>
    public static (double X, double Y, double Z) TransformPointToDoubles(Matrix3 matrix, double x, double y, double z)
    {
        var result = TransformPoint(matrix, x, y, z);
        return ((double)result.X, (double)result.Y, (double)result.Z);
    }
    
    // =====================================================
    // COMMON FOVECTOR3D CONSTANTS AS VECTOR3
    // =====================================================
    
    /// <summary>Zero vector (0, 0, 0)</summary>
    public static Vector3 Zero => Vector3.Zero;
    
    /// <summary>Up vector (0, 1, 0)</summary>
    public static Vector3 Up => Vector3.Up;
    
    /// <summary>Forward vector (0, 0, 1)</summary>
    public static Vector3 Forward => Vector3.Forward;
    
    /// <summary>Right vector (1, 0, 0)</summary>
    public static Vector3 Right => Vector3.Right;
    
    /// <summary>One vector (1, 1, 1)</summary>
    public static Vector3 One => new Vector3(1, 1, 1);
    
    // =====================================================
    // SPACIALFRAME3D COMPATIBILITY BRIDGE
    // =====================================================
    
    /// <summary>Convert Point3D to Vector3 for SpacialFrame3D compatibility</summary>
    public static Vector3 ToVector3FromSpacialPoint(double x, double y, double z)
        => new((float)x, (float)y, (float)z);
    
    /// <summary>Convert Vector3 back to Point3D coordinates for SpacialFrame3D</summary>
    public static (double X, double Y, double Z) ToSpacialPointFromVector3(Vector3 vector)
        => ((double)vector.X, (double)vector.Y, (double)vector.Z);
    
    // =====================================================
    // MIGRATION UTILITY METHODS
    // =====================================================
    
    /// <summary>Replace FoVector3D constructor calls in migrated code</summary>
    public static Vector3 NewFoVector3D(double x = 0, double y = 0, double z = 0)
        => new((float)x, (float)y, (float)z);
    
    /// <summary>Replace Matrix3D.TransformPoint calls in migrated code</summary>
    public static Vector3 Matrix3DTransformPoint(Matrix3 matrix, Vector3 point)
        => matrix.TransformPoint(point);
    
    /// <summary>Replace Matrix3D.SmashMatrix calls (object pooling)</summary>
    public static Matrix3? SmashMatrix3(Matrix3? matrix)
        => Matrix3.SmashMatrix(matrix);
    
    /// <summary>Replace Matrix3D.NewMatrix calls</summary>
    public static Matrix3 NewMatrix3()
        => Matrix3.NewMatrix();
    
    /// <summary>Check if double precision loss is significant</summary>
    public static bool HasSignificantPrecisionLoss(double original, float converted, double tolerance = 1e-6)
        => Math.Abs(original - (double)converted) > tolerance;
}

/// <summary>
/// Extension methods to make Vector3 behave more like FoVector3D
/// </summary>
public static class Vector3FoCompatibilityExtensions
{
    /// <summary>Add method for FoVector3D compatibility</summary>
    public static Vector3 Add(this Vector3 vector, Vector3 other) => vector + other;
    
    /// <summary>Subtract method for FoVector3D compatibility</summary>
    public static Vector3 Subtract(this Vector3 vector, Vector3 other) => vector - other;
    
    /// <summary>Multiply method for FoVector3D compatibility</summary>
    public static Vector3 Multiply(this Vector3 vector, double scalar) => vector * scalar;
    
    /// <summary>Cross product method for FoVector3D compatibility</summary>
    public static Vector3 FoCross(this Vector3 vector, Vector3 other) => Vector3.Cross(vector, other);
    
    /// <summary>Dot product method for FoVector3D compatibility</summary>
    public static double FoDot(this Vector3 vector, Vector3 other) => Vector3.Dot(vector, other);
    
    /// <summary>To formatted string for FoVector3D compatibility</summary>
    public static string ToStringFormatted(this Vector3 vector) => vector.ToFoVector3DString();
}
