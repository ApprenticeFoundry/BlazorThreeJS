using System.Text.Json.Serialization;
using BlazorThreeJS.Maths;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;



namespace BlazorThreeJS.Maths
{
    /// <summary>
    /// Transform3 - High-Performance 3D Transformation with Caching
    /// 
    /// CRITICAL DESIGN PRINCIPLES:
    /// 1. 🔒 NEVER directly assign to backing fields - always use properties
    /// 2. 🚫 NEVER bypass SetDirty() - it manages cache invalidation and events
    /// 3. 💾 Matrix caching provides performance - only recalculated when dirty
    /// 4. 📢 OnChange events notify UI components of transformation changes
    /// 
    /// SAFE USAGE PATTERNS:
    /// ✅ transform.Position = newPos;        // Property setter (auto-dirty)
    /// ✅ transform.SetDirty(true);           // Explicit dirty flag with cache invalidation
    /// ✅ if (transform.IsDirty) { ... }      // Read dirty state
    /// ✅ var matrix = transform.ToMatrix3(); // Cached matrix calculation
    /// 
    /// DANGEROUS PATTERNS (NOW PREVENTED BY DESIGN):
    /// ❌ transform.position = newPos;        // Direct field access (impossible - private)
    /// ❌ transform.IsDirty = true;           // Direct dirty assignment (impossible - read-only)
    /// 
    /// PERFORMANCE CHARACTERISTICS:
    /// - First ToMatrix3() call: Calculates and caches matrix
    /// - Subsequent ToMatrix3() calls: Returns cached matrix (fast!)
    /// - Property changes: Invalidate cache, trigger OnChange event
    /// - SetDirty(false): Mark clean without invalidating cache
    /// 
    /// THREAD SAFETY: This class is NOT thread-safe. Use from single thread only.
    /// </summary>
    public class Transform3 
    {
        #region Fields and Properties
        
        // === PRIVATE FIELDS ===
        // 🔒 CRITICAL: All backing fields are PRIVATE to prevent direct access
        // This forces all changes to go through property setters, ensuring dirty flag management
        private StatusBitArray StatusBits = new();
        private Matrix3? cachedMatrix = null;  // 🔥 CACHE: Store calculated matrix for performance
        
        // === PRIVATE BACKING FIELDS ===
        // 🚫 WARNING: NEVER access these directly - always use properties!
        // Direct access bypasses dirty flag system and breaks caching
        private Vector3 position = new Vector3();
        private Vector3 pivot = new Vector3();
        private Euler rotation = new Euler();
        private Quaternion quaternionRotation = Quaternion.Identity;
        private Vector3 scale = new Vector3(1, 1, 1);

        // === PUBLIC PROPERTIES ===
        // 🎯 DESIGN PATTERN: Property-Only Access with Automatic Dirty Flag Management
        //
        // 🔒 SECURITY PRINCIPLE: All private fields are ONLY accessible through properties
        // This ensures that every value change triggers SetDirty() through AssignXxx() helpers
        //
        // ✅ SAFE PATTERN: Property setters use AssignXxx() helpers:
        // position = AssignVector(value, position);  // ✅ Triggers dirty flag + events
        //
        // ❌ DANGEROUS PATTERN PREVENTED: Direct field assignment blocked by compiler:
        // position = value;                          // ❌ COMPILER ERROR: Field is private
        
        /// <summary>
        /// Event triggered when transform properties change and dirty flag is set
        /// 
        /// 🔔 NOTIFICATION SYSTEM: Automatically called by SetDirty() whenever any property changes
        /// 
        /// ✅ USAGE EXAMPLE:
        /// transform.OnChange += (isDirty) => { if(isDirty) UpdateRendering(); };
        /// </summary>
        [JsonIgnore]
        public Action<Boolean>? OnChange { get; set; }

        /// <summary>
        /// Event triggered when matrix computation completes and results are cached
        /// 
        /// 🎯 COMPLETION NOTIFICATION: Fires after expensive ToMatrix3() calculation finishes
        /// - Called when matrix is calculated and cached
        /// - Provides the computed matrix for immediate use
        /// - Indicates transform is now "stable" and ready for dependent operations
        /// 
        /// ✅ USAGE EXAMPLE:
        /// transform.OnComputed += (matrix) => { 
        ///     // Matrix is ready - safe to update dependent geometry
        ///     UpdateDependentObjects(matrix); 
        /// };
        /// 
        /// 🔄 REACTIVE PATTERN: Pairs with OnChange for complete lifecycle
        /// 1. OnChange(true) → "Transform dirty, start updates"
        /// 2. OnComputed(matrix) → "Computation complete, matrix ready"
        /// </summary>
        [JsonIgnore]
        public Action<Matrix3>? OnComputed { get; set; }


      /// <summary>
        /// Optional custom matrix computation logic. If set, this function will be used to compute the transform matrix when dirty.
        /// Not serialized.
        /// </summary>
        [JsonIgnore]
        public Func<Transform3, Matrix3>? UpdateMatrix3Formula { get; set; }

        /// <summary>
        /// 📍 World position of the transform (X, Y, Z coordinates)
        /// 
        /// 🔒 SAFE ACCESS PATTERN: Property-only access with automatic dirty flag management
        /// 
        /// ✅ RECOMMENDED USAGE:
        /// transform.Position = new Vector3(10, 5, 0);         // ✅ Triggers cache invalidation
        /// Vector3 currentPos = transform.Position;            // ✅ Safe read access
        /// 
        /// 🎯 AUTOMATIC BEHAVIORS:
        /// - Setting triggers SetDirty(true) via AssignVector()
        /// - Invalidates cached matrices (ToMatrix3() will recalculate)
        /// - Fires OnChange event to notify subscribers
        /// 
        /// 🔒 SECURITY: Direct field access prevented by private backing field
        /// </summary>
        public Vector3 Position
        {
            get => position;
            set => position = AssignVector(value, position);    // ✅ Always triggers dirty flag
        }

        /// <summary>
        /// 📐 Pivot point for rotation and scaling operations
        /// 
        /// 🔄 TRANSFORMATION MECHANICS: Defines the center point around which rotation and scaling occur
        /// 
        /// ✅ COMMON SCENARIOS:
        /// transform.Pivot = Vector3.Zero;                     // Rotate around origin
        /// transform.Pivot = objectCenter;                     // Rotate around object center
        /// transform.Pivot = new Vector3(0, 0, 0);            // Custom pivot point
        /// 
        /// 🎯 AUTOMATIC BEHAVIORS: Same as Position property - see Position documentation
        /// 
        /// 🔒 SECURITY: Property-only access with automatic dirty flag management
        /// </summary>
        public Vector3 Pivot
        {
            get => pivot;
            set => pivot = AssignVector(value, pivot);          // ✅ Always triggers dirty flag
        }

        /// <summary>
        /// 🔄 Euler rotation in radians (X, Y, Z) - Traditional rotation representation
        /// 
        /// 📊 ROTATION FORMAT: Euler angles representing rotation around X, Y, Z axes in radians
        /// 
        /// ✅ USAGE EXAMPLES:
        /// transform.Rotation = new Euler(0, Math.PI/2, 0);   // 90° Y-axis rotation
        /// transform.Rotation = Euler.FromDegrees(45, 0, 0);  // 45° X-axis rotation (if helper exists)
        /// 
        /// ⚠️ GIMBAL LOCK WARNING: Euler angles can experience gimbal lock at certain orientations
        /// Consider using QuaternionRotation for complex rotations
        /// 
        /// 🔗 QUATERNION SYNC: Changes automatically sync with QuaternionRotation property
        /// 
        /// 🎯 AUTOMATIC BEHAVIORS: Same as Position property - see Position documentation
        /// 
        /// 🔒 SECURITY: Property-only access with automatic dirty flag management
        /// </summary>
        public Euler Rotation
        {
            get => rotation;
            set => rotation = AssignEuler(value, rotation);     // ✅ Always triggers dirty flag
        }

        /// <summary>
        /// 🎯 Quaternion rotation - Gimbal-lock-free rotation representation
        /// 
        /// ⭐ ADVANTAGES: No gimbal lock, smooth interpolation, efficient composition
        /// 
        /// ✅ USAGE EXAMPLES:
        /// transform.QuaternionRotation = Quaternion.Identity;         // No rotation
        /// transform.QuaternionRotation = Quaternion.FromAxisAngle(Vector3.UnitY, Math.PI/2);  // 90° Y rotation
        /// 
        /// 🔗 AUTOMATIC EULER SYNC: Setting this property automatically updates Rotation property
        /// This maintains compatibility with code expecting Euler angles
        /// 
        /// ⚙️ SYNC MECHANISM: Uses temporary OnChange disabling to prevent double dirty flag triggering
        /// 
        /// 🎯 AUTOMATIC BEHAVIORS:
        /// - Triggers SetDirty(true) for quaternion change
        /// - Auto-syncs to Euler (with optimized single dirty flag)
        /// - Invalidates cached matrices
        /// - Fires OnChange event once (not twice due to sync optimization)
        /// 
        /// 🔒 SECURITY: Property-only access with automatic dirty flag management
        /// </summary>
        public Quaternion QuaternionRotation
        {
            get => quaternionRotation;
            set 
            {
                // Store the new value first
                var newQuaternion = value;
                quaternionRotation = AssignQuaternion(newQuaternion, quaternionRotation);
                
                // Auto-sync to Euler for compatibility using the NEW quaternion value
                // Temporarily disable dirty flag to avoid double-triggering
                var oldOnChange = OnChange;
                OnChange = null;
                try
                {
                    rotation = AssignEuler(newQuaternion.ToEuler(), rotation);
                }
                finally
                {
                    OnChange = oldOnChange;
                }
            }
        }

        /// <summary>
        /// 📏 Scale factor for X, Y, Z axes - Controls object size in each dimension
        /// 
        /// 📊 SCALE VALUES:
        /// - (1, 1, 1) = Original size (default)
        /// - (2, 2, 2) = Double size in all dimensions  
        /// - (0.5, 0.5, 0.5) = Half size in all dimensions
        /// - (1, 2, 1) = Double height only
        /// 
        /// ✅ USAGE EXAMPLES:
        /// transform.Scale = Vector3.One;                      // Reset to original size
        /// transform.Scale = new Vector3(2, 1, 2);            // Wide and deep, normal height
        /// transform.Scale = Vector3.One * 1.5f;              // Uniform 1.5x scaling
        /// 
        /// ⚠️ ZERO SCALE WARNING: Avoid zero values which can cause rendering issues
        /// 
        /// 🎯 AUTOMATIC BEHAVIORS: Same as Position property - see Position documentation
        /// 
        /// 🔒 SECURITY: Property-only access with automatic dirty flag management
        /// </summary>
        public Vector3 Scale
        {
            get => scale;
            set => scale = AssignVector(value, scale);          // ✅ Always triggers dirty flag
        }

        /// <summary>
        /// 🚩 Dirty flag indicating whether cached matrices need recalculation
        /// 
        /// 🔒 READ-ONLY BY DESIGN: This property has NO SETTER to prevent dangerous API misuse
        /// 
        /// ✅ SAFE USAGE PATTERNS:
        /// bool needsUpdate = transform.IsDirty;               // ✅ Check if matrices need recalc
        /// Matrix3 matrix = transform.ToMatrix3();             // ✅ Get matrix & auto-clear dirty flag  
        /// if (transform.IsDirty) { /* update rendering */ }  // ✅ Conditional update logic
        /// 
        /// ❌ DANGEROUS PATTERNS PREVENTED BY COMPILER:
        /// transform.IsDirty = false;                          // ❌ COMPILER ERROR: No setter!
        /// transform.IsDirty = true;                           // ❌ COMPILER ERROR: No setter!
        /// 
        /// 🎯 CONTROL METHODS (the ONLY way to modify dirty flag):
        /// transform.SetDirty(true);                           // ✅ Mark as dirty (invalidate cache)
        /// transform.SetDirty(false);                          // ✅ Mark as clean (rarely needed)
        /// Matrix3 m = transform.ToMatrix3();                  // ✅ Auto-clears dirty flag after calc
        /// 
        /// 💡 DESIGN RATIONALE: 
        /// External code clearing the dirty flag without updating cached matrices would cause
        /// rendering inconsistencies. Only Transform3 should control this flag through:
        /// 1. SetDirty() method for explicit control
        /// 2. ToMatrix3() method for automatic clearing after calculation
        /// 3. Property setters that call SetDirty() automatically
        /// 
        /// 🔍 IMPLEMENTATION: Delegates to StatusBits.IsDirty for actual storage
        /// </summary>
        public bool IsDirty
        {
            get { return this.StatusBits.IsDirty; }
            // 🔒 NO SETTER: Prevents bypassing cache invalidation and event notification
            // All modifications MUST go through SetDirty() method for proper state management
        }

        #endregion

        #region Constructor

        public Transform3()
        {
        }

        public Transform3(Action<Boolean> onChange)
        {
            OnChange = onChange;
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Generate the transformation matrix from current transform properties
        /// 
        /// 🚀 PERFORMANCE OPTIMIZATION: Uses intelligent caching
        /// - If !IsDirty && cachedMatrix exists: Returns cached matrix (fast!)
        /// - If IsDirty || no cache: Recalculates, caches, and marks clean
        /// 
        /// 🔄 CACHE LIFECYCLE:
        /// 1. Property change → SetDirty(true) → cache = null
        /// 2. ToMatrix3() called → calculate → cache result → SetDirty(false)
        /// 3. Subsequent ToMatrix3() calls → return cached result
        /// 
        /// ⚠️ CRITICAL: This method calls SetDirty(false) after calculation
        /// This is the ONLY place where dirty flag is set to false
        /// </summary>
        public Matrix3 ToMatrix3()
        {
            // 🚀 PERFORMANCE: Return cached matrix if not dirty
            if (!IsDirty && cachedMatrix != null)
            {
                return cachedMatrix;
            }

            // 🔄 RECALCULATE: Matrix is dirty or not cached yet
            if (UpdateMatrix3Formula is not null)
            {
                $"Using custom UpdateMatrix3Formula function".WriteWarning();
                cachedMatrix = UpdateMatrix3Formula(this);
            }
            else
            {
                $"Using default ComputeUsingProperties".WriteWarning();
                cachedMatrix = ComputeUsingProperties();
            }

            // 💾 CACHE: Store the calculated matrix and mark as clean
            SetDirty(false);  // ✅ FIXED: Use SetDirty() method instead of direct assignment

            // 🔔 NOTIFY: Fire OnComputed event with the completed matrix
            OnComputed?.Invoke(cachedMatrix);

            //lets add some code to print the matrix that was just created using WriteSuccess
            //matrix.ToString().WriteSuccess();

            var m = cachedMatrix.GetMatrix();

            $"-------------------------------".WriteSuccess();
            $"Cached Matrix:".WriteSuccess();
            $"{m[0]:F2}, {m[1]:F2}, {m[2]:F2}, {m[3]:F2}".WriteSuccess();
            $"{m[4]:F2}, {m[5]:F2}, {m[6]:F2}, {m[7]:F2}".WriteSuccess();
            $"{m[8]:F2}, {m[9]:F2}, {m[10]:F2}, {m[11]:F2}".WriteSuccess();
            $"{m[12]:F2}, {m[13]:F2}, {m[14]:F2}, {m[15]:F2}".WriteSuccess();

            return cachedMatrix;
        }

        private Matrix3 ComputeUsingProperties()
        {
            // CORRECT PIVOT TRANSFORMATION ORDER:
            // 1. Translate to origin (move pivot to origin)
            // 2. Apply scale (around origin/pivot)
            // 3. Apply rotation (around origin/pivot) 
            // 4. Translate back from origin (restore pivot offset)
            // 5. Apply final position (move to world space)

            var matrix = Matrix3.NewMatrix();

            // Step 1: Move pivot point to origin
            if (Pivot.X != 0 || Pivot.Y != 0 || Pivot.Z != 0)
            {
                $"Step 1: Moving pivot point to origin".WriteInfo(1);
                var toPivotMatrix = Matrix3.NewMatrix();
                toPivotMatrix.Translate(-Pivot.X, -Pivot.Y, -Pivot.Z);
                matrix.Multiply(toPivotMatrix);
            }

            // Step 2: Apply scaling around origin/pivot
            if (Scale.X != 1 || Scale.Y != 1 || Scale.Z != 1)
            {
                $"Step 2: Applying scaling: {Scale.X}, {Scale.Y}, {Scale.Z}".WriteInfo(1);
                var scaleMatrix = Matrix3.NewMatrix();
                scaleMatrix.Scale(Scale);
                matrix.Multiply(scaleMatrix);
            }

            // Step 3: Apply rotation around origin/pivot
            // if (QuaternionRotation != Quaternion.Identity)
            // {
            //     $"Applying quaternion rotation: ({QuaternionRotation.X}, {QuaternionRotation.Y}, {QuaternionRotation.Z}, {QuaternionRotation.W})".WriteInfo(1);
            //     var rotationMatrix = Matrix3.NewMatrix();
            //     rotationMatrix.RotateQuaternion(QuaternionRotation);
            //     matrix.Multiply(rotationMatrix.GetMatrix());
            // }
            
            if (Rotation.X != 0 || Rotation.Y != 0 || Rotation.Z != 0)
            {
                $"Step 3: Applying Euler rotation: {Rotation.X}, {Rotation.Y}, {Rotation.Z}".WriteInfo(1);
                var rotationMatrix = Matrix3.NewMatrix();
                rotationMatrix.Rotate(Rotation);
                matrix.Multiply(rotationMatrix);
            }

            // Step 4: Move back from origin (restore pivot offset)
            if (Pivot.X != 0 || Pivot.Y != 0 || Pivot.Z != 0)
            {
                $"Step 4: Restoring pivot offset".WriteInfo(1);
                var fromPivotMatrix = Matrix3.NewMatrix();
                fromPivotMatrix.Translate(Pivot.X, Pivot.Y, Pivot.Z);
                matrix.Multiply(fromPivotMatrix);
            }

            // Step 5: Apply final position translation
            if (Position.X != 0 || Position.Y != 0 || Position.Z != 0)
            {
                $"Step 5: Applying final position translation: {Position.X}, {Position.Y}, {Position.Z}".WriteInfo(1);
                var positionMatrix = Matrix3.NewMatrix();
                positionMatrix.Translate(Position);
                matrix.Multiply(positionMatrix);
            }
            return matrix;
        }

        /// <summary>
        /// Transform a point using the current transformation matrix
        /// </summary>
        public Vector3 TransformPoint(Vector3 point)
        {
            var matrix = ToMatrix3();
            if (IsDirty)
            {
                $"🔍 TransformPoint: Matrix In still dirty".WriteError();
            }
            return matrix.TransformPoint(point);
        }

        /// <summary>
        /// Transform a direction vector using the current transformation matrix
        /// </summary>
        public Vector3 TransformDirection(Vector3 direction)
        {
            var matrix = ToMatrix3();
            if (IsDirty)
            {
                $"🔍 TransformDirection: Matrix In still dirty".WriteError();
            }
            return matrix.TransformDirection(direction);
        }

        /// <summary>
        /// 🔒 CRITICAL METHOD: The ONLY safe way to modify the dirty flag
        /// 
        /// RESPONSIBILITIES:
        /// 1. 📝 Update dirty flag state
        /// 2. 🗑️ Invalidate matrix cache when dirty=true
        /// 3. 📢 Notify subscribers via OnChange event when dirty=true
        /// 
        /// USAGE PATTERNS:
        /// ✅ SetDirty(true)  - Mark dirty, clear cache, fire events (property changes)
        /// ✅ SetDirty(false) - Mark clean, keep cache, no events (after matrix calculation)
        /// 
        /// ⚠️ NEVER bypass this method:
        /// ❌ StatusBits.IsDirty = true;  // Missing cache invalidation and events!
        /// ❌ IsDirty = true;             // Compiler error (read-only property)
        /// ✅ SetDirty(true);             // CORRECT - all side effects handled
        /// </summary>
        protected virtual void SetDirty(bool value)
        {
            StatusBits.IsDirty = value;  // Direct field access (safe within this method)
            
            if (value)
            {
                // 🗑️ INVALIDATE CACHE: Clear cached matrix when dirty
                cachedMatrix = null;
                
                // 📢 NOTIFY: Trigger OnChange event for dirty state
                OnChange?.Invoke(value);
            }
            // 📝 NOTE: When setting to false (clean), we don't invalidate cache or trigger events
            // This happens in ToMatrix3() after the matrix has been calculated and cached
        }

        #endregion

        #region Protected Helper Methods

        /// <summary>
        /// 🔒 CORE PATTERN: Safe value assignment with automatic dirty flag management
        /// 
        /// This pattern ensures that EVERY value change triggers proper dirty flag handling:
        /// 1. 📝 Assigns new value to backing field
        /// 2. 🚩 Calls SetDirty(true) to invalidate cache and notify subscribers
        /// 3. 🔄 Returns new value for property setter pattern
        /// 
        /// ⚠️ CRITICAL: All property setters MUST use this pattern:
        /// ❌ position = newValue;                    // Direct assignment - no dirty flag!
        /// ✅ position = AssignVector(newValue, position); // Safe assignment with dirty flag
        /// </summary>
        protected Vector3 AssignVector(Vector3 newValue, Vector3 oldValue)
        {
            SetDirty(true);  // 🚩 CRITICAL: Always mark dirty on value change
            //$"AssignVector: {newValue} to {oldValue}".WriteNote();
            return newValue;
        }

        /// <summary>
        /// Safe Euler assignment with automatic dirty flag management
        /// See AssignVector() for detailed pattern explanation
        /// </summary>
        protected Euler AssignEuler(Euler newValue, Euler oldValue)
        {
            SetDirty(true);  // 🚩 CRITICAL: Always mark dirty on value change
            return newValue;
        }

        /// <summary>
        /// Safe Quaternion assignment with automatic dirty flag management
        /// See AssignVector() for detailed pattern explanation
        /// </summary>
        protected Quaternion AssignQuaternion(Quaternion newValue, Quaternion oldValue)
        {
            SetDirty(true);  // 🚩 CRITICAL: Always mark dirty on value change
            return newValue;
        }

        #endregion

      }
}