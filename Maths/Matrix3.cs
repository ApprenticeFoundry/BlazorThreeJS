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
        matrix[12] += x;
        matrix[13] += y;
        matrix[14] += z;
        return this;
    }

    // Apply scale
    public Matrix3 Scale(double x, double y, double z)
    {
        matrix[0] *= x;
        matrix[5] *= y;
        matrix[10] *= z;
        return this;
    }

    // Apply rotation around X axis
    public Matrix3 RotateX(double angle)
    {
        double rad = angle * DEG_TO_RAD;
        double cos = Math.Cos(rad);
        double sin = Math.Sin(rad);

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
    public Matrix3 RotateY(double angle)
    {
        double rad = angle * DEG_TO_RAD;
        double cos = Math.Cos(rad);
        double sin = Math.Sin(rad);

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
    public Matrix3 RotateZ(double angle)
    {
        double rad = angle * DEG_TO_RAD;
        double cos = Math.Cos(rad);
        double sin = Math.Sin(rad);

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

    // Apply Euler rotation (ZYX order)
    public Matrix3 RotateEuler(double x, double y, double z)
    {
        return RotateZ(z).RotateY(y).RotateX(x);
    }

    public void Rotate(Euler rotation)
    {
        Identity();
        // Assuming rotation order is XYZ
        double cosX = Math.Cos(rotation.X * DEG_TO_RAD);
        double sinX = Math.Sin(rotation.X * DEG_TO_RAD);
        double cosY = Math.Cos(rotation.Y * DEG_TO_RAD);
        double sinY = Math.Sin(rotation.Y * DEG_TO_RAD);
        double cosZ = Math.Cos(rotation.Z * DEG_TO_RAD);
        double sinZ = Math.Sin(rotation.Z * DEG_TO_RAD);

        // Rotation matrices
        double[] rotX = {
            1, 0, 0, 0,
            0, cosX, -sinX, 0,
            0, sinX, cosX, 0,
            0, 0, 0, 1
        };

        double[] rotY = {
            cosY, 0, sinY, 0,
            0, 1, 0, 0,
            -sinY, 0, cosY, 0,
            0, 0, 0, 1
        };

        double[] rotZ = {
            cosZ, -sinZ, 0, 0,
            sinZ, cosZ, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        };

        // Combine rotations
        Multiply(rotX);
        Multiply(rotY);
        Multiply(rotZ);
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
            .RotateEuler(rotX, rotY, rotZ)   // Apply rotation
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
            .RotateEuler(rotX, rotY, rotZ)   // Apply rotation
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
}
