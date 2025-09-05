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

            // CORRECT PIVOT TRANSFORMATION ORDER:
            // 1. Translate to origin (move pivot to origin)
            // 2. Apply scale (around origin/pivot)
            // 3. Apply rotation (around origin/pivot) 
            // 4. Translate back from origin (restore pivot offset)
            // 5. Apply final position (move to world space)
            
            // Step 1: Move pivot point to origin
            if (pivot.X != 0 || pivot.Y != 0 || pivot.Z != 0)
            {
                var toPivotMatrix = Matrix3.NewMatrix();
                toPivotMatrix.Translate(-pivot.X, -pivot.Y, -pivot.Z);
                matrix.Multiply(toPivotMatrix.GetMatrix());
            }

            // Step 2: Apply scaling around origin/pivot
            if (scale.X != 1 || scale.Y != 1 || scale.Z != 1)
            {
                var scaleMatrix = Matrix3.NewMatrix();
                scaleMatrix.Scale(scale);
                matrix.Multiply(scaleMatrix.GetMatrix());
            }

            // Step 3: Apply rotation around origin/pivot
            if (quaternionRotation != Quaternion.Identity)
            {
                var rotationMatrix = Matrix3.NewMatrix();
                rotationMatrix.RotateQuaternion(quaternionRotation);
                matrix.Multiply(rotationMatrix.GetMatrix());
            }
            else if (rotation.X != 0 || rotation.Y != 0 || rotation.Z != 0)
            {
                var rotationMatrix = Matrix3.NewMatrix();
                rotationMatrix.Rotate(rotation);
                matrix.Multiply(rotationMatrix.GetMatrix());
            }

            // Step 4: Move back from origin (restore pivot offset)
            if (pivot.X != 0 || pivot.Y != 0 || pivot.Z != 0)
            {
                var fromPivotMatrix = Matrix3.NewMatrix();
                fromPivotMatrix.Translate(pivot.X, pivot.Y, pivot.Z);
                matrix.Multiply(fromPivotMatrix.GetMatrix());
            }

            // Step 5: Apply final position translation
            if (position.X != 0 || position.Y != 0 || position.Z != 0)
            {
                var positionMatrix = Matrix3.NewMatrix();
                positionMatrix.Translate(position);
                matrix.Multiply(positionMatrix.GetMatrix());
            }

            return matrix;
        }

        // Compatibility methods for FoundryBlazor migration
        public Vector3 TransformPoint(Vector3 point)
        {
            return ToMatrix3().TransformPoint(point);
        }

        public Vector3 TransformDirection(Vector3 direction)
        {
            return ToMatrix3().TransformDirection(direction);
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
            position = new Vector3(position.X + x, position.Y + y, position.Z + z);
            SetDirty(true);
            return this;
        }

        public Transform3 SetPivot(Vector3 vec)
        {
            pivot = vec;
            SetDirty(true);
            return this;
        }
        public Transform3 SetScale(double x, double y, double z)
        {
            scale = new Vector3(x, y, z);
            SetDirty(true);
            return this;
        }

        public Transform3 RotateEuler(double x, double y, double z)
        {
            rotation = new Euler(x, y, z);
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
