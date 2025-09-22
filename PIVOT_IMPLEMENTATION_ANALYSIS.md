# Pivot Implementation Analysis for BlazorThreeJS

## Context
This document captures the analysis and implementation plan for adding center of rotation (pivot) support to the BlazorThreeJS library. The goal is to enable objects to rotate around custom pivot points, with a common use case being placing the bottom face of an object on the floor.

## Current Architecture Analysis

### ✅ C# Side - Complete Implementation
The C# side already has robust pivot support:

- **`Transform3.cs`** contains a fully implemented `Pivot` property with automatic dirty flag management
- **Matrix calculations** in `AppendTransform()` properly handle pivot transformations using translate-rotate-translate back pattern
- **Caching system** efficiently manages matrix recalculation when pivot changes
- **Event system** notifies subscribers when transformations change

```csharp
public Vector3 Pivot
{
    get => pivot;
    set => pivot = AssignVector(value, pivot);  // Triggers dirty flag automatically
}

// Matrix calculation includes pivot
matrix.AppendTransform(
    Position.X, Position.Y, Position.Z,
    Scale.X, Scale.Y, Scale.Z,
    Rotation.X, Rotation.Y, Rotation.Z,
    Pivot.X, Pivot.Y, Pivot.Z  // <-- Pivot integrated
);
```

### ❌ TypeScript Side - Incomplete Implementation
The TypeScript side currently ignores pivot data:

- **`Transforms.ts`** has commented-out `setPivot` method marked as incorrect
- **Transform application** only uses position, rotation, and scale
- **Pivot data is lost** when applying transformations to Three.js objects

```typescript
// Current broken implementation (commented out)
// static setPivot(object3d: Object3D, pivot: Vector3) {
//     let { x, y, z } = pivot;
//     if (Boolean(object3d)) //this will not work the math is wrong
//         object3d.position.set(x, y, z);
// }
```

## Implementation Strategy: Group-Based Pivot with Zero-Pivot Bypass

### Core Principle: Backward Compatibility First
```typescript
// If pivot is (0,0,0): Use existing direct approach (no changes)
// If pivot is non-zero: Use new Group-based approach
```

### The Group-Based Approach
Instead of complex matrix math in TypeScript, leverage Three.js's built-in hierarchical transformations:

1. **Parent Group** handles position and rotation at the pivot point
2. **Child Mesh** is offset by the negative pivot amount
3. **Rotation occurs around the Group's origin** (the pivot point)

### Example: "Bottom Face on Floor"
```typescript
// Box dimensions: (2, 4, 6), want bottom face at Y=0
// Pivot should be (0, -2, 0) to move rotation center to bottom

parentGroup.position.set(worldX, 0, worldZ);     // Floor position
parentGroup.rotation.set(rx, ry, rz);            // Rotation around bottom

childMesh.position.set(0, 2, 0);                 // -pivot = -(0, -2, 0) = (0, 2, 0)
// Result: Box bottom sits at Y=0, rotates around bottom edge
```

## Required Changes Documentation

### 1. MeshBuilder.ts Changes

#### Current Flow:
```typescript
CreateMesh(options) → returns { mesh, geometry, material }
ApplyMeshTransform(options, entity) → applies transforms directly to mesh
```

#### New Flow:
```typescript
CreateMesh(options) → {
    if (hasPivot) return { pivotGroup, mesh, geometry, material }
    else return { mesh, geometry, material }  // UNCHANGED
}

ApplyMeshTransform(options, transformTarget) → {
    if (hasPivot) apply transforms to pivotGroup, offset mesh
    else apply transforms directly to mesh  // UNCHANGED
}
```

### 2. Constructors.ts Changes

#### Current Code:
```typescript
establish3DGeometry(options, parent) {
    const result = MeshBuilder.CreateMesh(options);
    entity = result.mesh;
    parent.add(entity);
    MeshBuilder.ApplyMeshTransform(options, entity);
}
```

#### New Logic:
```typescript
establish3DGeometry(options, parent) {
    const result = MeshBuilder.CreateMesh(options);
    
    // Pivot detection and selection
    if (result.pivotGroup) {
        entity = result.pivotGroup;  // Group becomes the entity
        // mesh is child of pivotGroup
    } else {
        entity = result.mesh;        // Direct mesh (UNCHANGED)
    }
    
    parent.add(entity);
    MeshBuilder.ApplyMeshTransform(options, entity);
}
```

### 3. ObjectLookup.ts Changes

#### Current System:
```typescript
addPrimitive(guid, mesh)     // Stores direct mesh
findPrimitive(guid)          // Returns direct mesh
```

#### New System:
```typescript
addPrimitive(guid, entity)   // Stores Group OR mesh
findPrimitive(guid)          // Returns Group OR mesh
// Internal: Need helper to find actual mesh for material operations
```

### 4. Transforms.ts Changes

#### New Methods Needed:
```typescript
static hasPivot(transform) → boolean
static createPivotGroup(mesh, pivot) → Group
static applyPivotTransforms(group, mesh, transform) → void
```

## Zero-Pivot Bypass Logic

### Decision Points:
```typescript
// 1. In MeshBuilder.CreateMesh()
const hasPivot = transform?.pivot && 
    (transform.pivot.x !== 0 || transform.pivot.y !== 0 || transform.pivot.z !== 0);

if (!hasPivot) {
    // EXISTING CODE PATH - no changes
    return { mesh, geometry, material };
}

// 2. In all transform applications
if (entity.isPivotGroup) {
    // New pivot logic
} else {
    // EXISTING CODE PATH - no changes
}
```

## Bounding Box and Vertex Calculations - No Changes Needed!

### Critical Discovery: Three.js Handles This Automatically

The existing bounding box calculation code will work seamlessly with pivot Groups:

```typescript
// Current boundary calculation in establish3DHitBoundary()
entity.updateMatrixWorld(true);
const gPosition = entity.getWorldPosition(new Vector3());
const box = new Box3().setFromObject(entity, true);  // <-- Handles Groups automatically!
const size = box.getSize(new Vector3());
```

#### Why This Works:
- **`Box3.setFromObject()`** automatically traverses Groups and their children
- **`getWorldPosition()`** returns the Group's world position (the pivot point)
- **`updateMatrixWorld(true)`** ensures all child transforms are current
- **Final bounding box** is identical to direct mesh calculation

#### Expected Behavior:
- **Without Pivot**: `entity = mesh`, `box = boundingBoxOfMesh`
- **With Pivot**: `entity = pivotGroup`, `box = boundingBoxOfGroupAndChildren` (same result!)

## Impact Assessment

### ✅ What Stays Unchanged
- All objects with pivot (0,0,0) use existing code paths
- Selection system works (targets the entity, whether Group or Mesh)  
- Animation system works (animates the entity)
- Object hierarchy and parent-child relationships
- Existing C# Transform3 class and data structures
- **Bounding box calculations work automatically**
- **Vertex/position calculations work automatically**

### ⚠️ What Needs Careful Handling
- Material updates (need to find mesh within Group for material changes)
- Raycasting (might hit Group vs Mesh - needs testing)
- Dispose/cleanup (need to dispose both Group and Mesh)

### 🔍 Testing Strategy
1. **Phase 1**: Test with all pivot (0,0,0) - should be identical behavior
2. **Phase 2**: Test simple non-zero pivots on basic shapes  
3. **Phase 3**: Test complex hierarchies with mixed pivot/non-pivot objects

## File Modification Priority

### High Impact (Must Change):
- `MeshBuilder.ts` - Core creation logic
- `Constructors.ts` - Object instantiation  
- `Transforms.ts` - Add pivot methods

### Medium Impact (May Need Updates):
- `ObjectLookup.ts` - Handle Group vs Mesh storage
- Any material update logic that assumes direct mesh access

### Low Impact (Should Work As-Is):
- `Viewer3D.ts` - Works with Object3D hierarchy
- Animation systems - Work with any Object3D
- Camera and controls - Unaffected
- **Boundary/bounding box calculations - Work automatically**

## Alternative Approaches Considered

### 1. Full Matrix Passing from C#
**Pros**: Leverages existing C# matrix math
**Cons**: Requires significant changes to data flow, TypeScript would need to apply matrices directly

### 2. TypeScript Matrix Math
**Pros**: Self-contained in TypeScript
**Cons**: Duplicates complex math logic, harder to maintain consistency

### 3. Group-Based Approach (Chosen)
**Pros**: 
- Leverages Three.js built-in capabilities
- Minimal changes to existing code
- Zero-pivot bypass maintains 100% compatibility
- Bounding box calculations work automatically
**Cons**: Slight complexity in object management

## Conclusion

The Group-based approach with zero-pivot bypass provides the cleanest implementation path:

1. **Preserves all existing functionality** for objects with pivot (0,0,0)
2. **Leverages Three.js strengths** for hierarchical transformations
3. **Requires minimal changes** to the existing codebase
4. **Automatically handles** complex world-space calculations including bounding boxes
5. **Enables incremental implementation** with easy rollback capability

The key insight is that Three.js Groups are designed exactly for this use case - they separate the "where to position/rotate" (Group) from the "what to render" (Mesh) concerns, and the Three.js engine handles all the complex world-space math automatically.

## Next Steps

1. Implement `hasPivot()` utility function in `Transforms.ts`
2. Modify `MeshBuilder.CreateMesh()` to return Group when pivot is non-zero
3. Update `Constructors.ts` to handle Group vs Mesh entities
4. Test with simple cases to verify zero-pivot bypass works
5. Gradually test more complex pivot scenarios

---

*Document created: September 22, 2025*  
*Branch: pivot*