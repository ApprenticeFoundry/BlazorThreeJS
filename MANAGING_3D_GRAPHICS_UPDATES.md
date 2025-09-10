# Managing Updates and Refreshes in 3D Graphics Animation

This document summarizes best practices and mechanisms for efficiently updating and refreshing 3D graphics during animation in the BlazorThreeJS framework.

## 1. Dirty Flag System
- Every 3D object (e.g., `Object3D`, `FoGlyph3D`, `FoGroup3D`) maintains an `IsDirty` flag.
- When an object's state changes (geometry, transform, etc.), call `SetDirty(true)` to mark it as dirty. The `IsDirty` property is read-only and cannot be set directly.
- The animation loop collects all objects with `IsDirty = true` before rendering or updating the scene.

## 2. Transform3 Change Notification
- The `Transform3` class manages position, rotation, scale, and pivot for 3D objects.
- All property setters in `Transform3` (e.g., `Position`, `Rotation`) call `SetDirty(true)`, which:
  - Invalidates cached matrices.
  - Triggers the `OnChange` event.
- Parent objects should subscribe to `Transform3.OnChange` and set their own `IsDirty` flag when notified:
  ```csharp
  transform.OnChange += (isDirty) => { if (isDirty) parent.SetDirty(true); };
  ```

## 3. Animation Loop
- The animation loop periodically calls a method (e.g., `CollectDirtyObjects`) to gather all dirty objects.
- Only dirty objects are refreshed or re-rendered, improving performance.
- After refresh, objects are marked clean (`SetDirty(false)`).

## 4. Scene Refresh
- Scene refresh is triggered when dirty objects are detected.
- Methods like `RefreshScene` or `ComputeRefreshObjects` traverse the object tree and update only those marked as dirty.

## 5. Best Practices
- Never bypass the dirty flag system (do not set fields directly).
- Always use property setters and helper methods to ensure proper event notification and cache invalidation.
- Document and maintain event subscriptions between `Transform3` and parent objects.
- Use the provided mechanisms for adding/removing children to ensure dirty flags are set correctly.

## 6. Troubleshooting
- If changes to transforms do not trigger a refresh, check that the parent object subscribes to `Transform3.OnChange`.
- Ensure all property changes go through the correct setter methods.
- Validate that the animation loop is running and collecting dirty objects as expected.

---

This document should be kept up to date as the framework evolves. For further details, refer to the source code for `Object3D`, `Transform3`, and related classes.
