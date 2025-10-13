# JavaScript Asset Naming Changes

## Background

This document explains the JavaScript asset renaming implemented to resolve Blazor static web asset conflicts.

## The Problem

When multiple Blazor libraries include JavaScript files with generic names like `app-lib.js`, they can conflict when published to the same application. This causes build errors like:

```
Duplicate assets with the same target path 'app-lib.js' for applications
```

## The Solution

**Changed**: `wwwroot/dist/app-lib.js` → `wwwroot/dist/app-lib-threejs.js`

### Files Modified

1. **`JsLib/webpack.config.js`**
   ```javascript
   // Before
   filename: 'app-lib.js',
   
   // After  
   filename: 'app-lib-threejs.js',
   ```

### Files NOT Changed (No Impact)

- All C# code continues to work unchanged
- JavaScript interop calls via `IJSRuntime` are unaffected
- Global namespace `BlazorThreeJS` remains the same
- No HTML `<script>` tags needed updating (none existed)

## For Library Users

### ✅ No Action Required

Most users will experience **zero impact** because:

1. **Automatic Discovery**: Blazor automatically discovers and loads static web assets
2. **Namespace-Based Calls**: Your C# code uses `IJSRuntime.InvokeAsync("BlazorThreeJS.functionName")` which references the JavaScript namespace, not the file
3. **NuGet Packaging**: The file is served from `_content/ApprenticeFoundryBlazorThreeJS/dist/app-lib-threejs.js` automatically

### ⚠️ Action Required (Rare Cases)

If you were directly referencing the JavaScript file (not recommended):

```html
<!-- Update this: -->
<script src="_content/ApprenticeFoundryBlazorThreeJS/dist/app-lib.js"></script>

<!-- To this: -->
<script src="_content/ApprenticeFoundryBlazorThreeJS/dist/app-lib-threejs.js"></script>
```

## For Library Maintainers

### Build Process

The build process remains unchanged:
- `npm run build` in `JsLib/` folder
- `BuildJavascript.sh` script works as before
- Webpack automatically handles the new filename

### Future Considerations

1. **Version Hashing**: Consider adding `[contenthash]` to filename for cache busting
2. **Library Name**: Consider using full package name in filename for even better uniqueness
3. **Monitoring**: Watch for similar conflicts with other asset types (CSS, images)

## Verification

After the change, verify:

1. **File exists**: `wwwroot/dist/app-lib-threejs.js`
2. **JavaScript works**: Test `IJSRuntime.InvokeAsync("BlazorThreeJS.functionName")`
3. **No conflicts**: Build succeeds with other Blazor libraries

## Timeline

- **Implemented**: October 13, 2025
- **Version**: 22.0.0+
- **Breaking Change**: Minimal impact, documented in README

## Related Files

- `README.md` - User-facing breaking changes notice
- `JsLib/webpack.config.js` - Webpack configuration
- `wwwroot/dist/` - Generated JavaScript assets