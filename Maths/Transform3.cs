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

        public Matrix3 ToMatrix3()
        {
            var matrix = Matrix3.NewMatrix();

            // Apply translation to pivot
            var pivotTranslation = Matrix3.NewMatrix();
            pivotTranslation.Translate(pivot);
            matrix.Multiply(pivotTranslation.GetMatrix());

            // Apply rotation - use quaternion if it's not identity, otherwise fallback to Euler
            var rotationMatrix = Matrix3.NewMatrix();
            if (quaternionRotation != Quaternion.Identity)
            {
                rotationMatrix.RotateQuaternion(quaternionRotation);
            }
            else
            {
                rotationMatrix.Rotate(rotation);
            }
            matrix.Multiply(rotationMatrix.GetMatrix());

            // Apply scaling
            var scaleMatrix = Matrix3.NewMatrix();
            scaleMatrix.Scale(scale);
            matrix.Multiply(scaleMatrix.GetMatrix());

            // Apply translation to position
            var positionTranslation = Matrix3.NewMatrix();
            positionTranslation.Translate(position);
            matrix.Multiply(positionTranslation.GetMatrix());

            return matrix;
        }

        // Compatibility methods for FoundryBlazor migration
        public Vector3 TransformPoint(Vector3 point)
        {
            return ToMatrix3().TransformPoint(point);
        }

        public Transform3 Identity()
        {
            position = new Vector3();
            pivot = new Vector3();
            rotation = new Euler();
            scale = new Vector3(1, 1, 1);
            SetDirty(true);
            return this;
        }

        public Transform3 Translate(double x, double y, double z)
        {
            position = new Vector3(position.X + (float)x, position.Y + (float)y, position.Z + (float)z);
            SetDirty(true);
            return this;
        }

        public Transform3 SetScale(double x, double y, double z)
        {
            scale = new Vector3((float)x, (float)y, (float)z);
            SetDirty(true);
            return this;
        }

        public Transform3 RotateEuler(double x, double y, double z)
        {
            rotation = new Euler((float)x, (float)y, (float)z);
            // Sync quaternion with new Euler rotation
            quaternionRotation = Quaternion.FromEuler(rotation);
            SetDirty(true);
            return this;
        }

        /// <summary>
        /// Set rotation using quaternion - ideal for constraints and smooth rotations
        /// </summary>
        public Transform3 RotateQuaternion(Quaternion quat)
        {
            QuaternionRotation = quat;
            return this;
        }

        /// <summary>
        /// Rotate from one direction to another using quaternions
        /// Perfect for face-to-face alignment in snapping constraints
        /// </summary>
        public Transform3 RotateFromTo(Vector3 fromDirection, Vector3 toDirection)
        {
            QuaternionRotation = Quaternion.FromToRotation(fromDirection, toDirection);
            return this;
        }

        /// <summary>
        /// Apply additional rotation using quaternion multiplication
        /// </summary>
        public Transform3 ApplyRotation(Quaternion additionalRotation)
        {
            QuaternionRotation = quaternionRotation * additionalRotation;
            return this;
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
        protected Quaternion quaternionRotation = Quaternion.Identity;
        
        public Euler Rotation
        {
            get => rotation;
            set => rotation = AssignEuler(value, rotation);
        }

        /// <summary>
        /// Quaternion rotation - provides gimbal-lock-free rotation
        /// When set, automatically syncs with Euler rotation for compatibility
        /// </summary>
        public Quaternion QuaternionRotation
        {
            get => quaternionRotation;
            set 
            {
                quaternionRotation = AssignQuaternion(value, quaternionRotation);
                // Auto-sync to Euler for compatibility
                rotation = quaternionRotation.ToEuler();
                SetDirty(true);
            }
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
            // if ( IsDirty == value )
            //     return;

            IsDirty = value;
            if ( value )
            {
               // $"Transform is dirty  notify parent".WriteNote();
                OnChange?.Invoke(value);
            }

        }

        protected Vector3 AssignVector(Vector3 newValue, Vector3 oldValue)
        {
            SetDirty(true);
            //$"AssignVector: {newValue} to {oldValue}".WriteNote();
            return newValue;
        }

        protected Euler AssignEuler(Euler newValue, Euler oldValue)
        {
            SetDirty(true);
            return newValue;
        }

        protected Quaternion AssignQuaternion(Quaternion newValue, Quaternion oldValue)
        {
            SetDirty(true);
            return newValue;
        }
    }
}
