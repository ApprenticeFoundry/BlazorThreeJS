namespace BlazorThreeJS.Maths;

/// <summary>
/// Advanced Matrix3 extensions ported from FoundryBlazor Matrix3DExtensions
/// Provides fluent API for complex 3D transformations
/// </summary>
public static class Matrix3Extensions
{
    // Movement operations
    public static Matrix3 MoveBy(this Matrix3 matrix, Vector3 delta)
    {
        matrix.Translate(delta.X, delta.Y, delta.Z);
        return matrix;
    }

    public static Matrix3 MoveTo(this Matrix3 matrix, Vector3 position)
    {
        var current = matrix.GetTranslation();
        var delta = position - current;
        return matrix.MoveBy(delta);
    }

    // Scaling operations
    public static Matrix3 ScaleUniform(this Matrix3 matrix, double factor)
    {
        matrix.Scale(factor, factor, factor);
        return matrix;
    }

    public static Matrix3 ScaleBy(this Matrix3 matrix, Vector3 scale)
    {
        matrix.Scale(scale.X, scale.Y, scale.Z);
        return matrix;
    }

    // Rotation operations with degrees
    public static Matrix3 RotateXDegrees(this Matrix3 matrix, double degrees)
    {
        return matrix.RotateX(degrees);
    }

    public static Matrix3 RotateYDegrees(this Matrix3 matrix, double degrees)
    {
        return matrix.RotateY(degrees);
    }

    public static Matrix3 RotateZDegrees(this Matrix3 matrix, double degrees)
    {
        return matrix.RotateZ(degrees);
    }

    public static Matrix3 RotateEuler(this Matrix3 matrix, Vector3 eulerAngles, bool inRadians = false)
    {
        if (inRadians)
        {
            var radToDeg = 180.0 / Math.PI;
            return matrix.RotateEuler(eulerAngles.X * radToDeg, eulerAngles.Y * radToDeg, eulerAngles.Z * radToDeg);
        }
        return matrix.RotateEuler(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);
    }

    // Orientation operations
    public static Matrix3 LookAt(this Matrix3 matrix, Vector3 target, Vector3? up = null)
    {
        var position = matrix.GetTranslation();
        var direction = (target - position).Normalize();
        var upVector = up ?? Vector3.Up;
        
        // Create look-at rotation matrix
        var right = Vector3.Cross(direction, upVector).Normalize();
        var actualUp = Vector3.Cross(right, direction).Normalize();
        
        // Apply rotation
        matrix.SetRotationFromBasis(right, actualUp, direction.Negate());
        return matrix;
    }

    public static Matrix3 AlignWith(this Matrix3 matrix, Vector3 direction, Vector3? up = null)
    {
        var position = matrix.GetTranslation();
        var target = position + direction;
        return matrix.LookAt(target, up);
    }

    // Hierarchical operations
    public static Matrix3 CreateChild(this Matrix3 parent)
    {
        var child = Matrix3.NewMatrix();
        // Note: Parent-child relationship would need additional infrastructure
        // This is a placeholder for the concept
        return child;
    }

    public static Matrix3 TransformRelativeToParent(this Matrix3 matrix, Matrix3 transform)
    {
        // This would require parent reference infrastructure
        // For now, just copy the transform
        matrix.Copy(transform);
        return matrix;
    }

    // Grid and assembly operations
    public static List<Matrix3> CreateGridAssembly(this Matrix3 baseMatrix, 
        int countX, int countY, int countZ, 
        Vector3 spacing)
    {
        var assembly = new List<Matrix3>();
        
        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int z = 0; z < countZ; z++)
                {
                    var instance = baseMatrix.Clone();
                    var offset = new Vector3(x * spacing.X, y * spacing.Y, z * spacing.Z);
                    instance.MoveBy(offset);
                    assembly.Add(instance);
                }
            }
        }
        
        return assembly;
    }

    public static List<Matrix3> CreateLinkage(this Matrix3 baseMatrix, 
        List<Vector3> positions, 
        Vector3? direction = null)
    {
        var linkage = new List<Matrix3>();
        var dir = direction ?? Vector3.Forward;
        
        foreach (var position in positions)
        {
            var instance = baseMatrix.Clone();
            instance.MoveTo(position);
            if (direction != null)
            {
                instance.AlignWith(direction);
            }
            linkage.Add(instance);
        }
        
        return linkage;
    }

    // Advanced operations
    public static bool HitTest(this Matrix3 matrix, Vector3 point, double tolerance = 0.001)
    {
        var localPoint = matrix.GetInverse().TransformPoint(point);
        return Math.Abs(localPoint.X) <= tolerance && 
               Math.Abs(localPoint.Y) <= tolerance && 
               Math.Abs(localPoint.Z) <= tolerance;
    }

    public static Matrix3 Lerp(this Matrix3 matrix, Matrix3 target, double t)
    {
        var result = Matrix3.NewMatrix();
        
        // Interpolate position
        var pos1 = matrix.GetTranslation();
        var pos2 = target.GetTranslation();
        var lerpedPos = pos1.Lerp(pos2, t);
        
        // Interpolate scale
        var scale1 = matrix.GetScale();
        var scale2 = target.GetScale();
        var lerpedScale = scale1.Lerp(scale2, t);
        
        // For rotation, this is a simplified version
        // Proper quaternion interpolation would be better
        var rot1 = matrix.GetRotation();
        var rot2 = target.GetRotation();
        var lerpedRot = rot1.Lerp(rot2, t);
        
        result.SetPosition(lerpedPos);
        result.ScaleBy(lerpedScale);
        result.RotateEuler(lerpedRot.X, lerpedRot.Y, lerpedRot.Z);
        
        return result;
    }

    public static (Vector3 position, Vector3 rotation, Vector3 scale) Decompose(this Matrix3 matrix)
    {
        return (
            matrix.GetTranslation(),
            matrix.GetRotation(),
            matrix.GetScale()
        );
    }

    // Constraint-based operations (from FoundryBlazor)
    public static Matrix3 ConstrainToPlane(this Matrix3 matrix, Vector3 planeNormal, Vector3 pointOnPlane)
    {
        var position = matrix.GetTranslation();
        var toPoint = position - pointOnPlane;
        var distance = Vector3.Dot(toPoint, planeNormal.Normalize());
        var constrainedPosition = position - planeNormal.Normalize() * distance;
        matrix.SetPosition(constrainedPosition);
        return matrix;
    }

    public static Matrix3 ConstrainToLine(this Matrix3 matrix, Vector3 lineStart, Vector3 lineDirection)
    {
        var position = matrix.GetTranslation();
        var toPoint = position - lineStart;
        var projectionLength = Vector3.Dot(toPoint, lineDirection.Normalize());
        var constrainedPosition = lineStart + lineDirection.Normalize() * projectionLength;
        matrix.SetPosition(constrainedPosition);
        return matrix;
    }

    public static Matrix3 ConstrainDistance(this Matrix3 matrix, Vector3 anchor, double distance)
    {
        var position = matrix.GetTranslation();
        var direction = (position - anchor).Normalize();
        var constrainedPosition = anchor + direction * distance;
        matrix.SetPosition(constrainedPosition);
        return matrix;
    }

    // Mechanical operations (from FoundryBlazor)
    public static Matrix3 CreateHinge(this Matrix3 matrix, Vector3 hingeAxis, Vector3 hingePoint, double angle)
    {
        var result = matrix.Clone();
        result.MoveTo(hingePoint);
        
        // Rotate around hinge axis
        // This is a simplified version - proper axis-angle rotation would be better
        if (hingeAxis.ApproximatelyEqual(Vector3.Up))
            result.RotateY(angle);
        else if (hingeAxis.ApproximatelyEqual(Vector3.Right))
            result.RotateX(angle);
        else if (hingeAxis.ApproximatelyEqual(Vector3.Forward))
            result.RotateZ(angle);
        
        return result;
    }

    public static Matrix3 CreateSlider(this Matrix3 matrix, Vector3 slideDirection, double distance)
    {
        var result = matrix.Clone();
        result.MoveBy(slideDirection.Normalize() * distance);
        return result;
    }

    // Utility operations
    public static Matrix3 ApplyPivot(this Matrix3 matrix, Vector3 pivot)
    {
        // Move to pivot, apply transformation, move back
        var result = Matrix3.NewMatrix();
        result.Translate(-pivot.X, -pivot.Y, -pivot.Z);
        result.Multiply(matrix);
        result.Translate(pivot.X, pivot.Y, pivot.Z);
        return result;
    }

    public static Matrix3 FromPositionRotationScale(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        var matrix = Matrix3.NewMatrix();
        matrix.ScaleBy(scale);
        matrix.RotateEuler(rotation.X, rotation.Y, rotation.Z);
        matrix.SetPosition(position);
        return matrix;
    }

    public static Matrix3 FromLookAt(Vector3 position, Vector3 target, Vector3? up = null)
    {
        var matrix = Matrix3.NewMatrix();
        matrix.SetPosition(position);
        return matrix.LookAt(target, up);
    }
}
