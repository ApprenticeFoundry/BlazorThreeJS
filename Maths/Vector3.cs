

namespace BlazorThreeJS.Maths;
public class Vector3
{

    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public double Z { get; set; } = 0;

    public Vector3()
    {
    }
    public Vector3(double x, double y, double z)
    {
        Set(x, y, z);
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
}

