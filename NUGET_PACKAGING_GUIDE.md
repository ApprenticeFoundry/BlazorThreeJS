# BlazorThreeJS NuGet Package Configuration Guide

## ✅ PROJECT IS NOW READY FOR NUGET DEPLOYMENT!

## Critical Configuration Changes Made

### 1. Project SDK Change ✅
**CRITICAL**: Changed from `Microsoft.NET.Sdk.Web` to `Microsoft.NET.Sdk.Razor`
- ✅ Essential for Blazor component libraries
- ✅ Enables proper static web asset packaging
- ✅ Ensures compatibility with consuming Blazor applications

### 2. Essential Properties Added ✅
```xml
<!-- Essential for Blazor component library static web assets -->
<GenerateEmbeddedFilesManifest>true</GenerateEmbeddedFilesManifest>
<StaticWebAssetBasePath>_content/ApprenticeFoundryBlazorThreeJS</StaticWebAssetBasePath>
```

### 3. Static Assets Configuration ✅
- ✅ wwwroot content automatically included by Microsoft.NET.Sdk.Razor
- ✅ Static web assets properly configured
- ✅ All assets (JS, CSS, models, fonts) included in package

### 4. Build Targets for NuGet ✅
- ✅ Created `build/ApprenticeFoundryBlazorThreeJS.targets`
- ✅ Ensures proper asset inclusion in consuming applications
- ✅ Verified inclusion in generated NuGet package

### 5. Code Cleanup ✅
- ✅ Removed inappropriate Program.cs (not needed for component library)
- ✅ Cleaned up using statements (removed MVC references)
- ✅ Updated _Imports.razor for component library usage
- ✅ Fixed TargetFrameworks vs TargetFramework

## Package Verification ✅

**NuGet Package Generated**: `ApprenticeFoundryBlazorThreeJS.22.0.0.nupkg`

**Package Contents Verified**:
- ✅ `lib/net9.0/BlazorThreeJS.dll` - Main assembly
- ✅ `staticwebassets/dist/app-lib.js` - Compiled Three.js bundle (751KB)
- ✅ `staticwebassets/css/blazor-threejs.css` - Component styles
- ✅ `staticwebassets/assets/` - All 3D models and fonts
- ✅ `build/ApprenticeFoundryBlazorThreeJS.targets` - MSBuild integration
- ✅ Static web assets properly categorized

## Packaging Process

### 1. Build JavaScript Assets
```bash
cd JsLib
npm install
npm run build
```
This creates the compiled Three.js bundle in `wwwroot/dist/app-lib.js`

### 2. Build and Package
```bash
# Clean previous builds
dotnet clean

# Build in Release mode
dotnet build --configuration Release

# Create NuGet package
dotnet pack --configuration Release
```

### 3. Verify Package Contents
Check that the generated `.nupkg` file contains:
- `lib/net9.0/ApprenticeFoundryBlazorThreeJS.dll`
- `build/ApprenticeFoundryBlazorThreeJS.targets`
- `staticwebassets/` folder with wwwroot contents
- `ApprenticeFoundryBlazorThreeJS.StaticWebAssets.xml`

## Consumer Application Setup

### 1. Install Package
```bash
dotnet add package ApprenticeFoundryBlazorThreeJS
```

### 2. Reference Static Assets in Host Page
Add to `_Host.cshtml`, `_Layout.cshtml`, or `App.razor`:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>My Blazor App</title>
    <base href="~/" />
    
    <!-- BlazorThreeJS CSS (if any) -->
    <link href="_content/ApprenticeFoundryBlazorThreeJS/css/blazor-threejs.css" rel="stylesheet" />
</head>
<body>
    <!-- Your app content -->
    
    <!-- BlazorThreeJS JavaScript -->
    <script src="_content/ApprenticeFoundryBlazorThreeJS/dist/app-lib.js"></script>
    
    <!-- Blazor script -->
    <script src="_framework/blazor.server.js"></script>
</body>
</html>
```

### 3. Use Components
```razor
@page "/3d-scene"
@using BlazorThreeJS.Viewers
@using BlazorThreeJS.Core

<ViewerThreeD @ref="viewer" 
              Width="800" 
              Height="600" 
              SceneName="MyScene" />

@code {
    private ViewerThreeD? viewer;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && viewer != null)
        {
            // Initialize your 3D scene
            await SetupScene();
        }
    }
    
    private async Task SetupScene()
    {
        // Your Three.js scene setup code here
    }
}
```

## File Structure for Package
```
ApprenticeFoundryBlazorThreeJS/
├── build/
│   └── ApprenticeFoundryBlazorThreeJS.targets  # MSBuild targets
├── wwwroot/
│   ├── dist/
│   │   └── app-lib.js                          # Compiled Three.js bundle
│   ├── css/
│   │   └── blazor-threejs.css                  # Component styles
│   └── assets/                                 # Other static assets
├── Viewers/
│   ├── ViewerThreeD.cs                         # Main 3D viewer component
│   └── Scene3D.cs                              # Scene management
├── Core/
│   ├── Object3D.cs                             # 3D object base class
│   ├── Mesh3D.cs                               # 3D mesh components
│   └── Transform3.cs                           # Transform system
├── BlazorThreeJS.csproj                        # Updated project file
└── _Imports.razor                              # Component imports
```

## Troubleshooting

### Assets Not Loading (404 Errors)
1. Verify the package was built with `Microsoft.NET.Sdk.Razor`
2. Check that `GenerateEmbeddedFilesManifest` is `true`
3. Ensure static assets are referenced correctly in consuming app
4. Clear NuGet cache: `dotnet nuget locals all --clear`

### JavaScript Not Working
1. Verify `app-lib.js` is included in the package
2. Check browser dev tools for script loading errors
3. Ensure the script tag uses the correct path: `_content/ApprenticeFoundryBlazorThreeJS/dist/app-lib.js`

### Components Not Found
1. Check that `_Imports.razor` includes `@using BlazorThreeJS`
2. Verify package reference is correct in consuming project
3. Rebuild both library and consuming application

## Testing the Package

### Local Testing
```bash
# Pack the library
dotnet pack --configuration Release

# In test application
dotnet add package ApprenticeFoundryBlazorThreeJS --source "./bin/Release"
```

### Verification Steps
1. Build succeeds without errors
2. Static assets appear in consuming app's `wwwroot/_content/ApprenticeFoundryBlazorThreeJS/`
3. JavaScript loads without 404 errors
4. Components render correctly
5. Three.js functionality works as expected

## Best Practices
1. Always increment version number before packing
2. Test with both local references and NuGet packages
3. Include comprehensive XML documentation
4. Maintain backward compatibility when possible
5. Document breaking changes in release notes

## Ready for NuGet.org
With these changes, BlazorThreeJS is now properly configured as a Blazor component library and can be reliably consumed by other Blazor applications through NuGet.