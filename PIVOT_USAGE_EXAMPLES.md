# Pivot Functionality Usage Examples

## Overview
The pivot feature allows objects to rotate around custom center points instead of their geometric center. This is particularly useful for scenarios like placing the bottom face of an object on the floor.

## Implementation Status ✅
The pivot functionality has been successfully implemented with:
- **Zero-pivot bypass**: Objects with pivot (0,0,0) use existing code paths (100% backward compatibility)
- **Group-based approach**: Objects with non-zero pivots use Three.js Groups for proper rotation centers
- **Automatic boundary calculations**: Three.js handles all world-space calculations automatically

## Basic Usage

### C# Side (No Changes Needed)
Your existing C# code already supports pivots through the `Transform3` class:

```csharp
var mesh = new Mesh3D("TestBox")
{
    Transform = new Transform3("TestBox")
    {
        Position = new Vector3(0, 0, 0),
        Pivot = new Vector3(0, -1, 0),  // Rotate around bottom face
        Rotation = new Euler(0, 45 * Math.PI/180, 0),  // 45° Y rotation
        Scale = Vector3.One
    }
    // ... geometry and material setup
};
```

### TypeScript Side (Automatically Handled)
The TypeScript implementation automatically:
1. Detects if `pivot` is non-zero
2. Creates a `Group` container for the mesh
3. Positions the mesh at `-pivot` offset within the group
4. Applies transformations to the group (not the mesh)

## Common Use Cases

### 1. Bottom Face on Floor
```csharp
// Box with dimensions (2, 4, 6) - want bottom face at Y=0
var box = new Mesh3D("FloorBox")
{
    Transform = new Transform3("FloorBox")
    {
        Position = new Vector3(10, 0, 5),    // Floor position
        Pivot = new Vector3(0, -2, 0),       // Half height offset
        // Now rotations happen around the bottom face
    }
};
```

### 2. Rotate Around Corner
```csharp
// Rotate around bottom-left-front corner
var box = new Mesh3D("CornerBox")
{
    Transform = new Transform3("CornerBox")
    {
        Position = new Vector3(0, 0, 0),
        Pivot = new Vector3(-1, -2, -3),    // To corner offset
        Rotation = new Euler(0, 90 * Math.PI/180, 0)
    }
};
```

### 3. Custom Rotation Center
```csharp
// Rotate around a point 5 units above the object
var mesh = new Mesh3D("HighPivot")
{
    Transform = new Transform3("HighPivot")
    {
        Position = new Vector3(0, 5, 0),     // Pivot point location
        Pivot = new Vector3(0, -5, 0),       // Object offset from pivot
        // Object appears at (0,0,0) but rotates around (0,5,0)
    }
};
```

## Zero-Pivot Behavior (Unchanged)
```csharp
// This behaves exactly as before - no pivot group created
var normalMesh = new Mesh3D("Normal")
{
    Transform = new Transform3("Normal")
    {
        Position = new Vector3(1, 2, 3),
        Pivot = new Vector3(0, 0, 0),        // Zero pivot
        Rotation = new Euler(0.5, 0, 0)
        // Rotates around geometric center (existing behavior)
    }
};
```

## Technical Details

### How It Works
1. **Zero Pivot**: Direct mesh added to scene, transforms applied directly
2. **Non-Zero Pivot**: 
   - Creates a `Group` as parent
   - Positions mesh at `-pivot` offset within group
   - Applies all transforms to the group
   - Group becomes the scene entity

### Scene Hierarchy
```
// Zero pivot (existing)
Scene
└── Mesh (transforms applied here)

// Non-zero pivot (new)
Scene
└── PivotGroup (transforms applied here)
    └── Mesh (positioned at -pivot offset)
```

### Boundary Calculations
Three.js automatically handles:
- World position calculations (`getWorldPosition()`)
- Bounding box calculations (`Box3.setFromObject()`)
- Ray casting and selection
- Animation and rendering

## Backward Compatibility

### ✅ What Stays the Same
- All existing objects with pivot (0,0,0) use original code paths
- Selection system works identically
- Animation system works identically  
- Boundary calculations work identically
- C# API remains unchanged

### ⚠️ Minor Considerations
- Objects with non-zero pivots now have a Group wrapper in the scene hierarchy
- Material updates work automatically (uses helper methods to find the mesh)
- Performance impact is minimal (only for objects that actually need pivot functionality)

## Testing

The implementation has been tested with:
- ✅ TypeScript compilation (no errors)
- ✅ .NET build (successful)
- ✅ Zero-pivot bypass (existing sample scene works unchanged)
- ✅ Group-based pivot calculations (boundary math verified)

## Future Enhancements

Potential improvements:
- Runtime pivot changes (currently requires recreation)
- Pivot visualization helpers for debugging
- Pivot point gizmos in editor interfaces

---

*Implementation completed: September 22, 2025*  
*Branch: pivot*