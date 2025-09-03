namespace BlazorThreeJS.Maths;

/// <summary>
/// Conversion utilities between different vector/point types
/// Provides smooth transition from FoundryBlazor math types to BlazorThreeJS
/// </summary>
public static class VectorConversions
{
    // Conversion methods for Point3D will be added when needed during migration
    // For now, these are placeholder methods for the conversion concept
    
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
}
