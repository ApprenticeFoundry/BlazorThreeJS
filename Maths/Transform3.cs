using System.Text.Json.Serialization;
using BlazorThreeJS.Maths;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;



namespace BlazorThreeJS.Maths
{
    public class Transform3 
    {
        private StatusBitArray StatusBits = new();
        [JsonIgnore]
        public Action<Boolean>? OnChange { get; set; }

        public Transform3()
        {
        }

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



        public bool IsDirty
        {
            get { return this.StatusBits.IsDirty; }
            set { 
                this.StatusBits.IsDirty = value; 
                //if ( value )
                //    {
                //        $"Transform is dirty".WriteNote();
                //    }
                }
        }
        
        public virtual void SetDirty(bool value)
        {
            if ( IsDirty == value )
                return;

            IsDirty = value;
            if ( value )
                OnChange?.Invoke(value);

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
