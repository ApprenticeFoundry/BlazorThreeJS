

using FoundryRulesAndUnits.Extensions;

namespace BlazorThreeJS.Maths
{
    public enum RotationOrder
    {
        XYZ,
        YZX,
        ZXY,
        XZY,
        YXZ,
        ZYX
    }
    public enum AngleUnit
    {
        Radians,
        Degrees
    }
    public class Euler
        /// <summary>
        /// Create an Euler from degrees (converts to radians internally)
        /// </summary>

    {
        //these values are stored in Radians internally 

        //lest add a small warning to the setters to remind users
        //that they are in Radians not Degrees so test if they are > 2PI
        //and if so, assume they meant degrees and convert to Radians

        private double x = 0;

        public double X
        {
            get => x;
            set
            {
                if (Math.Abs(value) > 2 * Math.PI)
                    $"SETTING EULER ANGLES STORED IN RADIANS! ARE YOU USING DEGREES? {value}".WriteWarning(2);
                x = value;
            }
        }

        private double y = 0;
        public double Y
        {
            get => y;
            set
            {
                if (Math.Abs(value) > 2 * Math.PI)
                    $"SETTING EULER ANGLES STORED IN RADIANS! ARE YOU USING DEGREES? {value}".WriteWarning(2);
                y = value;
            }
        }

        private double z = 0;
        public double Z
        {
            get => z;
            set
            {
                if (Math.Abs(value) > 2 * Math.PI)
                    $"SETTING EULER ANGLES STORED IN RADIANS! ARE YOU USING DEGREES? {value}".WriteWarning(2);
                z = value;
            }
        }


        public string Order { get; set; } = "XYZ";

        public Euler()
        {
        }

        public Euler(double x, double y, double z)
        {
            SetAsRadians(x, y, z);
        }

        public Euler Copy()
        {
            return new Euler(X, Y, Z, Order);
        }
        public Euler(double x, double y, double z, AngleUnit units)
        {
            if (units == AngleUnit.Degrees)
                SetAsDegrees(x, y, z);
            else
                SetAsRadians(x, y, z);
        }
        public Euler(double x, double y, double z, string order) : this(x, y, z)
        {
            Order = order;
        }

        public static Euler FromDegrees(double x, double y, double z, string order = "XYZ")
        {
            var e = new Euler(x,y,z, AngleUnit.Degrees);
            e.SetAsDegrees(x, y, z);
            e.Order = order;
            return e;
        }

        public static Euler FromRadians(double x, double y, double z, string order = "XYZ")
        {
            var e = new Euler(x,y,z, AngleUnit.Radians);
            e.SetAsRadians(x, y, z);
            e.Order = order;
            return e;
        }

        public Euler SetAsRadians(double x, double y, double z)
        {

            X = x;
            Y = y;
            Z = z;
            return this;
        }

        public Euler SetAsDegrees(double x, double y, double z)
        {

            X = x * Matrix3.DEG_TO_RAD;
            Y = y * Matrix3.DEG_TO_RAD;
            Z = z * Matrix3.DEG_TO_RAD;
            return this;
        }

        //public override string ToString() => $"Euler({X:0.00}, {Y:0.00}, {Z:0.00}, '{Order}')";
    }
}
