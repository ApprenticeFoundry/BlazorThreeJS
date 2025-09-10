using FoundryRulesAndUnits.Extensions;

namespace BlazorThreeJS.Maths;

public class Matrix3
{
    public static readonly double DEG_TO_RAD = Math.PI / 180;
    private static readonly Queue<Matrix3> cache = new();

    // 4x4 matrix stored in row-major order
    // [m11 m12 m13 m14]
    // [m21 m22 m23 m24]
    // [m31 m32 m33 m34]
    // [m41 m42 m43 m44]
    private double[] matrix = new double[16];

    public Matrix3()
    {
        Identity();
    }

    public double[] GetMatrix()
    {
        return matrix;
    }

    public static Matrix3 NewMatrix()
    {
        if (cache.Count == 0)
            return new Matrix3();
        return cache.Dequeue();
    }

    public static Matrix3? SmashMatrix(Matrix3? source)
    {
        if (source == null) return null;
        source.Identity();
        cache.Enqueue(source);
        $"SmashMatrix called".WriteWarning();
        return null;
    }

    public Matrix3 Identity()
    {
        Array.Clear(matrix, 0, 16);
        matrix[0] = matrix[5] = matrix[10] = matrix[15] = 1.0;
        return this;
    }

    // Apply translation
    public Matrix3 Translate(double x, double y, double z)
    {
        $"Applying Translate by ({x}, {y}, {z})".WriteInfo(1);
        matrix[12] += x;
        matrix[13] += y;
        matrix[14] += z;
        return this;
    }

    // Apply scale
    public Matrix3 Scale(double x, double y, double z)
    {
        $"Applying Scale by ({x}, {y}, {z})".WriteInfo(1);
        matrix[0] *= x;
        matrix[5] *= y;
        matrix[10] *= z;
        return this;
    }

    // Apply rotation around X axis
    public Matrix3 RotateX_radians(double angle)
    {
        $"Applying RotateX by {angle} radians".WriteInfo(1);

        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        double m21 = matrix[4], m22 = matrix[5], m23 = matrix[6];
        double m31 = matrix[8], m32 = matrix[9], m33 = matrix[10];

        matrix[4] = m21 * cos + m31 * sin;
        matrix[5] = m22 * cos + m32 * sin;
        matrix[6] = m23 * cos + m33 * sin;
        matrix[8] = -m21 * sin + m31 * cos;
        matrix[9] = -m22 * sin + m32 * cos;
        matrix[10] = -m23 * sin + m33 * cos;

        return this;
    }

    // Apply rotation around Y axis
    public Matrix3 RotateY_radians(double angle)
    {
        $"Rotating around Y by {angle} radians".WriteInfo(1);

        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        double m11 = matrix[0], m12 = matrix[1], m13 = matrix[2];
        double m31 = matrix[8], m32 = matrix[9], m33 = matrix[10];

        matrix[0] = m11 * cos - m31 * sin;
        matrix[1] = m12 * cos - m32 * sin;
        matrix[2] = m13 * cos - m33 * sin;
        matrix[8] = m11 * sin + m31 * cos;
        matrix[9] = m12 * sin + m32 * cos;
        matrix[10] = m13 * sin + m33 * cos;

        return this;
    }

    // Apply rotation around Z axis
    public Matrix3 RotateZ_radians(double angle)
    {
        $"Applying RotateZ by {angle} radians".WriteInfo(1);

        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        double m11 = matrix[0], m12 = matrix[1], m13 = matrix[2];
        double m21 = matrix[4], m22 = matrix[5], m23 = matrix[6];

        matrix[0] = m11 * cos + m21 * sin;
        matrix[1] = m12 * cos + m22 * sin;
        matrix[2] = m13 * cos + m23 * sin;
        matrix[4] = -m11 * sin + m21 * cos;
        matrix[5] = -m12 * sin + m22 * cos;
        matrix[6] = -m13 * sin + m23 * cos;

        return this;
    }

    public void Translate(Vector3 translation)
    {
        Identity();
        matrix[12] = translation.X;
        matrix[13] = translation.Y;
        matrix[14] = translation.Z;
    }

    public void Scale(Vector3 scale)
    {
        Identity();
        matrix[0] = scale.X;
        matrix[5] = scale.Y;
        matrix[10] = scale.Z;
    }

    // // Apply Euler rotation (ZYX order)
    // public Matrix3 RotateEulerZYX_radians(double x, double y, double z)
    // {
    //     return RotateZ_radians(z).RotateY_radians(y).RotateX_radians(x);
    // }

    public Matrix3 Rotate(Euler rotation)
    {
        $"Applying Euler rotation: {rotation.X}, {rotation.Y}, {rotation.Z} (Order: {rotation.Order})".WriteInfo(1);
        Identity();
        // Directly set combined XYZ rotation matrix

        double cx = Math.Cos(rotation.X), sx = Math.Sin(rotation.X);
        double cy = Math.Cos(rotation.Y), sy = Math.Sin(rotation.Y);
        double cz = Math.Cos(rotation.Z), sz = Math.Sin(rotation.Z);

        //make sure you check the rotation order and implement other orders as needed
        if (rotation.Order != "XYZ")
            $"Warning: Rotation order {rotation.Order} not implemented, defaulting to XYZ".WriteWarning();

        // Combined rotation matrix for XYZ order
        matrix[0] = cy * cz;
        matrix[1] = -cy * sz;
        matrix[2] = sy;
        matrix[3] = 0;

        matrix[4] = sx * sy * cz + cx * sz;
        matrix[5] = -sx * sy * sz + cx * cz;
        matrix[6] = -sx * cy;
        matrix[7] = 0;

        matrix[8] = -cx * sy * cz + sx * sz;
        matrix[9] = cx * sy * sz + sx * cz;
        matrix[10] = cx * cy;
        matrix[11] = 0;

        matrix[12] = matrix[13] = matrix[14] = 0;
        matrix[15] = 1;
        return this;
    }

    /// <summary>
    /// Apply quaternion rotation to matrix
    /// Converts quaternion to rotation matrix for precise rotations
    /// </summary>
    public Matrix3 RotateQuaternion(Quaternion q)
    {
        $"Applying Quaternion rotation: ({q.X}, {q.Y}, {q.Z}, {q.W})".WriteInfo(1);
        // Normalize quaternion
        var length = Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W);
        if (length < 0.000001)
            return this; // No rotation

        var x = q.X / length;
        var y = q.Y / length;
        var z = q.Z / length;
        var w = q.W / length;

        // Convert quaternion to 4x4 rotation matrix
        var xx = x * x;
        var yy = y * y;
        var zz = z * z;
        var xy = x * y;
        var xz = x * z;
        var yz = y * z;
        var wx = w * x;
        var wy = w * y;
        var wz = w * z;

        double[] quatMatrix = {
            1 - 2 * (yy + zz), 2 * (xy - wz), 2 * (xz + wy), 0,
            2 * (xy + wz), 1 - 2 * (xx + zz), 2 * (yz - wx), 0,
            2 * (xz - wy), 2 * (yz + wx), 1 - 2 * (xx + yy), 0,
            0, 0, 0, 1
        };

        Multiply(quatMatrix);
        return this;
    }

    /// <summary>
    /// Set matrix to quaternion rotation (replaces existing rotation)
    /// </summary>
    public void SetQuaternionRotation(Quaternion q)
    {
        Identity();
        RotateQuaternion(q);
    }

    // Transform a point using the matrix
    public Vector3 TransformPoint(Vector3 point)
    {
        double x = point.X * matrix[0] + point.Y * matrix[4] + point.Z * matrix[8] + matrix[12];
        double y = point.X * matrix[1] + point.Y * matrix[5] + point.Z * matrix[9] + matrix[13];
        double z = point.X * matrix[2] + point.Y * matrix[6] + point.Z * matrix[10] + matrix[14];
        double w = point.X * matrix[3] + point.Y * matrix[7] + point.Z * matrix[11] + matrix[15];

        if (w != 1 && w != 0)
        {
            x /= w;
            y /= w;
            z /= w;
        }

        return new Vector3(x, y, z);
    }

    public void Multiply(double[] other)
    {
        double[] result = new double[16];
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                result[row * 4 + col] = 0;
                for (int k = 0; k < 4; k++)
                {
                    result[row * 4 + col] += matrix[row * 4 + k] * other[k * 4 + col];
                }
            }
        }
        Array.Copy(result, matrix, 16);
    }

    // Multiply two matrices
    public Matrix3 Multiply(Matrix3 other)
    {
        var result = new double[16];

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                double sum = 0;
                for (int i = 0; i < 4; i++)
                {
                    sum += matrix[row * 4 + i] * other.matrix[i * 4 + col];
                }
                result[row * 4 + col] = sum;
            }
        }

        matrix = result;
        return this;
    }

    // Create a transformation matrix with pin point (registration point)
    public Matrix3 AppendTransform(double x, double y, double z,
                                  double scaleX, double scaleY, double scaleZ,
                                  double rotX, double rotY, double rotZ,
                                  double regX, double regY, double regZ)
    {
        var transform = NewMatrix()
            .Translate(-regX, -regY, -regZ)  // Move to origin
            .Scale(scaleX, scaleY, scaleZ)   // Apply scale
            .Rotate(new Euler(rotX, rotY, rotZ))   // Apply rotation
            .Translate(x, y, z)              // Move to final position
            .Translate(regX, regY, regZ);    // Move back by registration point

        AppendMatrix(transform);
        return this;
    }

    public Matrix3 PrependTransform(double x, double y, double z,
                                     double scaleX, double scaleY, double scaleZ,
                                     double rotX, double rotY, double rotZ,
                                     double regX, double regY, double regZ)
    {
        var transform = NewMatrix()
            .Translate(-regX, -regY, -regZ)  // Move to origin
            .Scale(scaleX, scaleY, scaleZ)   // Apply scale
            .Rotate(new Euler(rotX, rotY, rotZ))   // Apply rotation
            .Translate(x, y, z)              // Move to final position
            .Translate(regX, regY, regZ);    // Move back by registration point

        PrependMatrix(transform);
        return this;
    }

    public Matrix3 Append(double m11, double m12, double m13, double m14,
                           double m21, double m22, double m23, double m24,
                           double m31, double m32, double m33, double m34,
                           double m41, double m42, double m43, double m44)
    {
        var result = new double[16];
        var m = matrix;

        result[0] = m[0] * m11 + m[4] * m12 + m[8] * m13 + m[12] * m14;
        result[1] = m[1] * m11 + m[5] * m12 + m[9] * m13 + m[13] * m14;
        result[2] = m[2] * m11 + m[6] * m12 + m[10] * m13 + m[14] * m14;
        result[3] = m[3] * m11 + m[7] * m12 + m[11] * m13 + m[15] * m14;

        result[4] = m[0] * m21 + m[4] * m22 + m[8] * m23 + m[12] * m24;
        result[5] = m[1] * m21 + m[5] * m22 + m[9] * m23 + m[13] * m24;
        result[6] = m[2] * m21 + m[6] * m22 + m[10] * m23 + m[14] * m24;
        result[7] = m[3] * m21 + m[7] * m22 + m[11] * m23 + m[15] * m24;

        result[8] = m[0] * m31 + m[4] * m32 + m[8] * m33 + m[12] * m34;
        result[9] = m[1] * m31 + m[5] * m32 + m[9] * m33 + m[13] * m34;
        result[10] = m[2] * m31 + m[6] * m32 + m[10] * m33 + m[14] * m34;
        result[11] = m[3] * m31 + m[7] * m32 + m[11] * m33 + m[15] * m34;

        result[12] = m[0] * m41 + m[4] * m42 + m[8] * m43 + m[12] * m44;
        result[13] = m[1] * m41 + m[5] * m42 + m[9] * m43 + m[13] * m44;
        result[14] = m[2] * m41 + m[6] * m42 + m[10] * m43 + m[14] * m44;
        result[15] = m[3] * m41 + m[7] * m42 + m[11] * m43 + m[15] * m44;

        matrix = result;
        return this;
    }

    public Matrix3 Prepend(double m11, double m12, double m13, double m14,
                            double m21, double m22, double m23, double m24,
                            double m31, double m32, double m33, double m34,
                            double m41, double m42, double m43, double m44)
    {
        var result = new double[16];
        var m = matrix;

        result[0] = m11 * m[0] + m12 * m[1] + m13 * m[2] + m14 * m[3];
        result[1] = m11 * m[4] + m12 * m[5] + m13 * m[6] + m14 * m[7];
        result[2] = m11 * m[8] + m12 * m[9] + m13 * m[10] + m14 * m[11];
        result[3] = m11 * m[12] + m12 * m[13] + m13 * m[14] + m14 * m[15];

        result[4] = m21 * m[0] + m22 * m[1] + m23 * m[2] + m24 * m[3];
        result[5] = m21 * m[4] + m22 * m[5] + m23 * m[6] + m24 * m[7];
        result[6] = m21 * m[8] + m22 * m[9] + m23 * m[10] + m24 * m[11];
        result[7] = m21 * m[12] + m22 * m[13] + m23 * m[14] + m24 * m[15];

        result[8] = m31 * m[0] + m32 * m[1] + m33 * m[2] + m34 * m[3];
        result[9] = m31 * m[4] + m32 * m[5] + m33 * m[6] + m34 * m[7];
        result[10] = m31 * m[8] + m32 * m[9] + m33 * m[10] + m34 * m[11];
        result[11] = m31 * m[12] + m32 * m[13] + m33 * m[14] + m34 * m[15];

        result[12] = m41 * m[0] + m42 * m[1] + m43 * m[2] + m44 * m[3];
        result[13] = m41 * m[4] + m42 * m[5] + m43 * m[6] + m44 * m[7];
        result[14] = m41 * m[8] + m42 * m[9] + m43 * m[10] + m44 * m[11];
        result[15] = m41 * m[12] + m42 * m[13] + m43 * m[14] + m44 * m[15];

        matrix = result;
        return this;
    }

    public Matrix3 Set(double m11, double m12, double m13, double m14,
                        double m21, double m22, double m23, double m24,
                        double m31, double m32, double m33, double m34,
                        double m41, double m42, double m43, double m44)
    {
        matrix[0] = m11;
        matrix[1] = m12;
        matrix[2] = m13;
        matrix[3] = m14;
        matrix[4] = m21;
        matrix[5] = m22;
        matrix[6] = m23;
        matrix[7] = m24;
        matrix[8] = m31;
        matrix[9] = m32;
        matrix[10] = m33;
        matrix[11] = m34;
        matrix[12] = m41;
        matrix[13] = m42;
        matrix[14] = m43;
        matrix[15] = m44;
        return this;
    }

    public Matrix3 Zero()
    {
        Array.Clear(matrix, 0, 16);
        return this;
    }

    public Matrix3 AppendMatrix(Matrix3 matrix)
    {
        return Append(matrix.matrix[0], matrix.matrix[1], matrix.matrix[2], matrix.matrix[3],
                      matrix.matrix[4], matrix.matrix[5], matrix.matrix[6], matrix.matrix[7],
                      matrix.matrix[8], matrix.matrix[9], matrix.matrix[10], matrix.matrix[11],
                      matrix.matrix[12], matrix.matrix[13], matrix.matrix[14], matrix.matrix[15]);
    }

    public Matrix3 PrependMatrix(Matrix3 matrix)
    {
        return Prepend(matrix.matrix[0], matrix.matrix[1], matrix.matrix[2], matrix.matrix[3],
                       matrix.matrix[4], matrix.matrix[5], matrix.matrix[6], matrix.matrix[7],
                       matrix.matrix[8], matrix.matrix[9], matrix.matrix[10], matrix.matrix[11],
                       matrix.matrix[12], matrix.matrix[13], matrix.matrix[14], matrix.matrix[15]);
    }

    public Matrix3 Invert()
    {
        var m = matrix;
        var result = new double[16];

        var det = m[0] * (m[5] * m[10] * m[15] + m[9] * m[14] * m[7] + m[13] * m[6] * m[11]
                        - m[13] * m[10] * m[7] - m[9] * m[6] * m[15] - m[5] * m[14] * m[11])
                - m[4] * (m[1] * m[10] * m[15] + m[9] * m[14] * m[3] + m[13] * m[2] * m[11]
                        - m[13] * m[10] * m[3] - m[9] * m[2] * m[15] - m[1] * m[14] * m[11])
                + m[8] * (m[1] * m[6] * m[15] + m[5] * m[14] * m[3] + m[13] * m[2] * m[7]
                        - m[13] * m[6] * m[3] - m[5] * m[2] * m[15] - m[1] * m[14] * m[7])
                - m[12] * (m[1] * m[6] * m[11] + m[5] * m[10] * m[3] + m[9] * m[2] * m[7]
                         - m[9] * m[6] * m[3] - m[5] * m[2] * m[11] - m[1] * m[10] * m[7]);

        if (Math.Abs(det) < double.Epsilon)
            throw new InvalidOperationException("Matrix is not invertible.");

        var invDet = 1.0 / det;

        result[0] = invDet * (m[5] * m[10] * m[15] + m[9] * m[14] * m[7] + m[13] * m[6] * m[11]
                            - m[13] * m[10] * m[7] - m[9] * m[6] * m[15] - m[5] * m[14] * m[11]);
        result[1] = invDet * (m[1] * m[14] * m[11] + m[9] * m[2] * m[15] + m[13] * m[10] * m[3]
                            - m[13] * m[2] * m[11] - m[9] * m[10] * m[3] - m[1] * m[14] * m[15]);
        result[2] = invDet * (m[1] * m[6] * m[15] + m[5] * m[10] * m[3] + m[13] * m[2] * m[7]
                            - m[13] * m[6] * m[3] - m[5] * m[2] * m[15] - m[1] * m[10] * m[7]);
        result[3] = invDet * (m[1] * m[6] * m[11] + m[5] * m[2] * m[15] + m[9] * m[10] * m[3]
                            - m[9] * m[6] * m[3] - m[5] * m[2] * m[11] - m[1] * m[10] * m[7]);

        result[4] = invDet * (m[4] * m[14] * m[11] + m[8] * m[2] * m[15] + m[12] * m[10] * m[3]
                            - m[12] * m[2] * m[11] - m[8] * m[10] * m[3] - m[4] * m[14] * m[15]);
        result[5] = invDet * (m[0] * m[10] * m[15] + m[8] * m[14] * m[3] + m[12] * m[2] * m[7]
                            - m[12] * m[10] * m[3] - m[8] * m[2] * m[15] - m[0] * m[14] * m[7]);
        result[6] = invDet * (m[0] * m[6] * m[15] + m[4] * m[10] * m[3] + m[12] * m[2] * m[7]
                            - m[12] * m[6] * m[3] - m[4] * m[2] * m[15] - m[0] * m[10] * m[7]);
        result[7] = invDet * (m[0] * m[6] * m[11] + m[4] * m[2] * m[15] + m[8] * m[10] * m[3]
                            - m[8] * m[6] * m[3] - m[4] * m[2] * m[11] - m[0] * m[10] * m[7]);

        result[8] = invDet * (m[4] * m[9] * m[15] + m[8] * m[13] * m[3] + m[12] * m[1] * m[7]
                            - m[12] * m[9] * m[3] - m[8] * m[1] * m[15] - m[4] * m[13] * m[7]);
        result[9] = invDet * (m[0] * m[13] * m[7] + m[8] * m[1] * m[15] + m[12] * m[9] * m[3]
                            - m[12] * m[1] * m[7] - m[8] * m[9] * m[3] - m[0] * m[13] * m[15]);
        result[10] = invDet * (m[0] * m[5] * m[15] + m[4] * m[9] * m[3] + m[12] * m[1] * m[7]
                             - m[12] * m[5] * m[3] - m[4] * m[1] * m[15] - m[0] * m[9] * m[7]);
        result[11] = invDet * (m[0] * m[5] * m[11] + m[4] * m[1] * m[15] + m[8] * m[9] * m[3]
                             - m[8] * m[5] * m[3] - m[4] * m[1] * m[11] - m[0] * m[9] * m[7]);

        result[12] = invDet * (m[4] * m[9] * m[14] + m[8] * m[13] * m[2] + m[12] * m[1] * m[6]
                             - m[12] * m[9] * m[2] - m[8] * m[1] * m[14] - m[4] * m[13] * m[6]);
        result[13] = invDet * (m[0] * m[13] * m[6] + m[8] * m[1] * m[14] + m[12] * m[9] * m[2]
                             - m[12] * m[1] * m[6] - m[8] * m[9] * m[2] - m[0] * m[13] * m[14]);
        result[14] = invDet * (m[0] * m[5] * m[14] + m[4] * m[9] * m[2] + m[12] * m[1] * m[6]
                             - m[12] * m[5] * m[2] - m[4] * m[1] * m[14] - m[0] * m[9] * m[6]);
        result[15] = invDet * (m[0] * m[5] * m[10] + m[4] * m[1] * m[14] + m[8] * m[9] * m[2]
                             - m[8] * m[5] * m[2] - m[4] * m[1] * m[10] - m[0] * m[9] * m[6]);

        matrix = result;
        return this;
    }

    public Matrix3 InvertCopy()
    {
        var result = new Matrix3();
        result.matrix = (double[])matrix.Clone();
        return result.Invert();
    }

    public bool IsIdentity()
    {
        return matrix[0] == 1 && matrix[5] == 1 && matrix[10] == 1 && matrix[15] == 1
            && matrix[1] == 0 && matrix[2] == 0 && matrix[3] == 0
            && matrix[4] == 0 && matrix[6] == 0 && matrix[7] == 0
            && matrix[8] == 0 && matrix[9] == 0 && matrix[11] == 0
            && matrix[12] == 0 && matrix[13] == 0 && matrix[14] == 0;
    }

    public Matrix3 Clone()
    {
        var clone = new Matrix3();
        clone.matrix = (double[])this.matrix.Clone();
        return clone;
    }

    // Additional methods for Matrix3D compatibility
    public double[] Elements => matrix;

    public Vector3 GetTranslation()
        => new(matrix[12], matrix[13], matrix[14]);

    public Vector3 GetScale()
    {
        var sx = new Vector3(matrix[0], matrix[1], matrix[2]).Length();
        var sy = new Vector3(matrix[4], matrix[5], matrix[6]).Length();
        var sz = new Vector3(matrix[8], matrix[9], matrix[10]).Length();
        return new Vector3(sx, sy, sz);
    }

    public Vector3 GetRotation()
    {
        var scale = GetScale();
        var m11 = matrix[0] / scale.X;
        var m12 = matrix[1] / scale.X;
        var m13 = matrix[2] / scale.X;
        var m21 = matrix[4] / scale.Y;
        var m22 = matrix[5] / scale.Y;
        var m23 = matrix[6] / scale.Y;
        var m31 = matrix[8] / scale.Z;
        var m32 = matrix[9] / scale.Z;
        var m33 = matrix[10] / scale.Z;

        var sy = Math.Sqrt(m11 * m11 + m21 * m21);
        var singular = sy < 1e-6;

        double x, y, z;
        if (!singular)
        {
            x = Math.Atan2(m32, m33);
            y = Math.Atan2(-m31, sy);
            z = Math.Atan2(m21, m11);
        }
        else
        {
            x = Math.Atan2(-m23, m22);
            y = Math.Atan2(-m31, sy);
            z = 0;
        }

        return new Vector3(x, y, z);
    }

    public void SetPosition(Vector3 position)
    {
        matrix[12] = position.X;
        matrix[13] = position.Y;
        matrix[14] = position.Z;
    }

    public Matrix3 Copy(Matrix3 source)
    {
        Array.Copy(source.matrix, matrix, 16);
        return this;
    }

    public void SetRotationFromBasis(Vector3 right, Vector3 up, Vector3 forward)
    {
        matrix[0] = right.X; matrix[1] = right.Y; matrix[2] = right.Z;
        matrix[4] = up.X; matrix[5] = up.Y; matrix[6] = up.Z;
        matrix[8] = forward.X; matrix[9] = forward.Y; matrix[10] = forward.Z;
    }

    public Matrix3 GetInverse()
    {
        return Clone().Invert();
    }

    public Matrix3 MultiplyMatrices(Matrix3 a, Matrix3 b)
    {
        Copy(a);
        return Multiply(b);
    }

    // Additional compatibility methods for FoundryBlazor Matrix3D replacement
    public Matrix3 SetTranslation(double x, double y, double z)
    {
        matrix[12] = x;
        matrix[13] = y;
        matrix[14] = z;
        return this;
    }

    public Matrix3 SetTranslation(Vector3 translation)
    {
        return SetTranslation(translation.X, translation.Y, translation.Z);
    }

    public Vector3 TransformDirection(Vector3 direction)
    {
        // Transform direction without translation (w=0)
        double x = direction.X * matrix[0] + direction.Y * matrix[4] + direction.Z * matrix[8];
        double y = direction.X * matrix[1] + direction.Y * matrix[5] + direction.Z * matrix[9];
        double z = direction.X * matrix[2] + direction.Y * matrix[6] + direction.Z * matrix[10];
        return new Vector3(x, y, z);
    }

    public bool ApproximatelyEquals(Matrix3 other, double tolerance = 0.001)
    {
        for (int i = 0; i < 16; i++)
        {
            if (Math.Abs(matrix[i] - other.matrix[i]) > tolerance)
                return false;
        }
        return true;
    }

    public Matrix3 Reset()
    {
        return Identity();
    }

    public string ToStringFormatted()
    {
        return $"Matrix3[{matrix[12]:F2},{matrix[13]:F2},{matrix[14]:F2}]";
    }

    // Static factory methods for common matrices
    public static Matrix3 CreateTranslation(Vector3 translation)
    {
        var matrix = NewMatrix();
        matrix.SetTranslation(translation);
        return matrix;
    }



    public static Matrix3 CreateScale(Vector3 scale)
    {
        var matrix = NewMatrix();
        return matrix.Scale(scale.X, scale.Y, scale.Z);
    }



    // === ADVANCED MATRIX OPERATIONS ===
    // (Moved from Matrix3Extensions for better design)

    // Movement operations
    public Matrix3 MoveBy(Vector3 delta)
    {
        Translate(delta.X, delta.Y, delta.Z);
        return this;
    }

    public Matrix3 MoveTo(Vector3 position)
    {
        var current = GetTranslation();
        var delta = position - current;
        return MoveBy(delta);
    }

    // Scaling operations
    public Matrix3 ScaleUniform(double factor)
    {
        Scale(factor, factor, factor);
        return this;
    }

    public Matrix3 ScaleBy(Vector3 scale)
    {
        Scale(scale.X, scale.Y, scale.Z);
        return this;
    }



    // Orientation operations
    public Matrix3 LookAt(Vector3 target, Vector3? up = null)
    {
        var position = GetTranslation();
        var direction = (target - position).Normalize();
        var upVector = up ?? Vector3.Up;

        // Create look-at rotation matrix
        var right = Vector3.Cross(direction, upVector).Normalize();
        var actualUp = Vector3.Cross(right, direction).Normalize();

        // Apply rotation
        SetRotationFromBasis(right, actualUp, direction.Negate());
        return this;
    }

    public Matrix3 AlignWith(Vector3 direction, Vector3? up = null)
    {
        var position = GetTranslation();
        var target = position + direction;
        return LookAt(target, up);
    }

    // Grid and assembly operations
    public List<Matrix3> CreateGridAssembly(int countX, int countY, int countZ, Vector3 spacing)
    {
        var assembly = new List<Matrix3>();

        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int z = 0; z < countZ; z++)
                {
                    var instance = Clone();
                    var offset = new Vector3(x * spacing.X, y * spacing.Y, z * spacing.Z);
                    instance.MoveBy(offset);
                    assembly.Add(instance);
                }
            }
        }

        return assembly;
    }

    public List<Matrix3> CreateLinkage(List<Vector3> positions, Vector3? direction = null)
    {
        var linkage = new List<Matrix3>();
        var dir = direction ?? Vector3.Forward;

        foreach (var position in positions)
        {
            var instance = Clone();
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
    public bool HitTest(Vector3 point, double tolerance = 0.001)
    {
        var localPoint = GetInverse().TransformPoint(point);
        return Math.Abs(localPoint.X) <= tolerance &&
               Math.Abs(localPoint.Y) <= tolerance &&
               Math.Abs(localPoint.Z) <= tolerance;
    }

    public Matrix3 Lerp(Matrix3 target, double t)
    {
        var result = NewMatrix();

        // Interpolate position
        var pos1 = GetTranslation();
        var pos2 = target.GetTranslation();
        var lerpedPos = pos1.Lerp(pos2, t);

        // Interpolate scale
        var scale1 = GetScale();
        var scale2 = target.GetScale();
        var lerpedScale = scale1.Lerp(scale2, t);

        // For rotation, this is a simplified version
        // Proper quaternion interpolation would be better
        var rot1 = GetRotation();
        var rot2 = target.GetRotation();
        var lerpedRot = rot1.Lerp(rot2, t);

        result.SetPosition(lerpedPos);
        result.ScaleBy(lerpedScale);
        result.Rotate(new Euler(lerpedRot.X, lerpedRot.Y, lerpedRot.Z));

        return result;
    }

    public (Vector3 position, Vector3 rotation, Vector3 scale) Decompose()
    {
        return (
            GetTranslation(),
            GetRotation(),
            GetScale()
        );
    }

    // Constraint-based operations
    public Matrix3 ConstrainToPlane(Vector3 planeNormal, Vector3 pointOnPlane)
    {
        var position = GetTranslation();
        var toPoint = position - pointOnPlane;
        var distance = Vector3.Dot(toPoint, planeNormal.Normalize());
        var constrainedPosition = position - planeNormal.Normalize() * distance;
        SetPosition(constrainedPosition);
        return this;
    }

    public Matrix3 ConstrainToLine(Vector3 lineStart, Vector3 lineDirection)
    {
        var position = GetTranslation();
        var toPoint = position - lineStart;
        var projectionLength = Vector3.Dot(toPoint, lineDirection.Normalize());
        var constrainedPosition = lineStart + lineDirection.Normalize() * projectionLength;
        SetPosition(constrainedPosition);
        return this;
    }

    public Matrix3 ConstrainDistance(Vector3 anchor, double distance)
    {
        var position = GetTranslation();
        var direction = (position - anchor).Normalize();
        var constrainedPosition = anchor + direction * distance;
        SetPosition(constrainedPosition);
        return this;
    }

  

    public Matrix3 CreateSlider(Vector3 slideDirection, double distance)
    {
        var result = Clone();
        result.MoveBy(slideDirection.Normalize() * distance);
        return result;
    }

    // Utility operations
    public Matrix3 ApplyPivot(Vector3 pivot)
    {
        // Move to pivot, apply transformation, move back
        var result = NewMatrix();
        result.Translate(-pivot.X, -pivot.Y, -pivot.Z);
        result.Multiply(this);
        result.Translate(pivot.X, pivot.Y, pivot.Z);
        return result;
    }

    // Static factory methods
    // public static Matrix3 FromPositionRotationScale(Vector3 position, Vector3 rotation, Vector3 scale)
    // {
    //     var matrix = NewMatrix();
    //     matrix.ScaleBy(scale);
    //     matrix.RotateEuler(rotation.X, rotation.Y, rotation.Z);
    //     matrix.SetPosition(position);
    //     return matrix;
    // }

    public static Matrix3 FromLookAt(Vector3 position, Vector3 target, Vector3? up = null)
    {
        var matrix = NewMatrix();
        matrix.SetPosition(position);
        return matrix.LookAt(target, up);
    }
}
