using BlazorThreeJS.Maths;
using FoundryRulesAndUnits.Models;



namespace BlazorThreeJS.Maths
{
    public class Transform3 
    {
        private StatusBitArray StatusBits = new();
        protected Vector3 position = new Vector3();
        public Vector3 Position
        {
            get => position;
            set => position = AssignVector(value, position);
        }

        protected Vector3 pivot = new Vector3();
        public Vector3 Pivot
        {
            get => pivot;
            set => pivot = AssignVector(value, pivot);
        }

        protected Euler rotation = new Euler();
        public Euler Rotation
        {
            get => rotation;
            set => rotation = AssignEuler(value, rotation); //maybe change to Quaternion
        }

        protected Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 Scale
        {
            get => scale;
            set => scale = AssignVector(value, scale);
        }



        public virtual bool IsDirty()
        {
            return StatusBits.IsDirty;
        }
        
        public virtual void SetDirty(bool value)
        {
            StatusBits.IsDirty = value;
        }

        protected Vector3 AssignVector(Vector3 newValue, Vector3 oldValue)
        {
            SetDirty(true);
            return newValue;
        }

        protected Euler AssignEuler(Euler newValue, Euler oldValue)
        {
            SetDirty(true);
            return newValue;
        }
    }
}
