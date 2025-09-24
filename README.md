# BlazorThreeJS

BlazorThreeJS is a NuGet package that provides seamless integration of the Three.js library with Blazor applications. This package allows developers to create rich 3D graphics and animations using the power of Three.js within a Blazor project.

## Features

- Easy integration of Three.js with Blazor
- Create and manipulate 3D objects and scenes
- **🆕 Custom Pivot Points**: Rotate objects around any point (e.g., bottom face on floor)
- Full C# API with strong typing and intellisense
- Advanced transform system with position, rotation, scale, and pivot support
- Support for animations with customizable update callbacks
- Hierarchical scene management with parent-child relationships
- Performance optimized with matrix caching and dirty-flag system
- Comprehensive documentation and examples

## Installation

To install BlazorThreeJS, run the following command in the NuGet Package Manager Console:

```sh
Install-Package ApprenticeFoundryBlazorThreeJS
```

Alternatively, you can add the package reference directly to your `.csproj` file:

```xml
<PackageReference Include="ApprenticeFoundryBlazorThreeJS" Version="23.0.0" />
```

## Getting Started

### 1. Install the Package
Add the BlazorThreeJS package to your Blazor project using the installation instructions above.

### 2. Add Static Assets to Your App
In your Blazor app's `_Host.cshtml`, `_Layout.cshtml`, or `App.razor`, add the required JavaScript reference:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>My Blazor App</title>
    <base href="~/" />
    
    <!-- Optional: BlazorThreeJS styles -->
    <link href="_content/ApprenticeFoundryBlazorThreeJS/css/blazor-threejs.css" rel="stylesheet" />
</head>
<body>
    <!-- Your app content -->
    
    <!-- Required: BlazorThreeJS JavaScript -->
    <script src="_content/ApprenticeFoundryBlazorThreeJS/dist/app-lib.js"></script>
    
    <!-- Blazor framework script -->
    <script src="_framework/blazor.server.js"></script>
</body>
</html>
```

### 3. Use in Your Components
```razor
@page "/3d-scene"
@using BlazorThreeJS.Viewers
@using BlazorThreeJS.Core
@using BlazorThreeJS.Maths

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
            await CreateScene();
        }
    }
    
    private async Task CreateScene()
    {
        // Create a box that rotates around its bottom face
        var box = new Mesh3D("FloorBox")
        {
            Transform = new Transform3("FloorBox")
            {
                Position = new Vector3(0, 0, 0),      // Position on floor
                Pivot = new Vector3(0, -1, 0),        // Rotate around bottom
                Rotation = new Euler(0, 45 * Math.PI/180, 0), // 45° rotation
                Scale = Vector3.One
            },
            // Add geometry and material...
        };
        
        await viewer.AddObjectToScene(box);
    }
}
```

## Custom Pivot Points (New Feature!)

One of the most powerful new features is the ability to set custom pivot points for object rotation:

### Bottom Face on Floor
```csharp
// Perfect for placing objects on the ground
var box = new Mesh3D("FloorBox")
{
    Transform = new Transform3("FloorBox")
    {
        Position = new Vector3(0, 0, 0),     // Floor position  
        Pivot = new Vector3(0, -1, 0),       // Rotate around bottom face
        // Now rotations happen around the bottom edge, not center
    }
};
```

### Rotate Around Corner
```csharp
// Rotate around bottom-left-front corner
var box = new Mesh3D("CornerBox")
{
    Transform = new Transform3("CornerBox")
    {
        Position = new Vector3(0, 0, 0),
        Pivot = new Vector3(-1, -2, -3),     // Corner offset
        Rotation = new Euler(0, 90 * Math.PI/180, 0)  // 90° Y rotation
    }
};
```

### Custom Rotation Center
```csharp
// Rotate around a point above the object
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

### Backward Compatibility
Objects with pivot `(0,0,0)` work exactly as before - no changes needed to existing code!

## Architecture & Performance

BlazorThreeJS features a sophisticated architecture:

- **C# Layer**: Strongly-typed object model with `Object3D`, `Transform3`, `Mesh3D`, etc.
- **TypeScript Layer**: Efficient Three.js integration with automatic object lifecycle management
- **Smart Caching**: Transformation matrices are cached and only recalculated when needed
- **Automatic Interop**: Changes in C# automatically sync to the Three.js scene

### Transform System
The `Transform3` class provides robust transformation capabilities:

```csharp
public class Transform3
{
    public Vector3 Position { get; set; }     // World position
    public Vector3 Pivot { get; set; }        // Custom rotation center (NEW!)
    public Euler Rotation { get; set; }       // Euler angle rotation  
    public Quaternion QuaternionRotation { get; set; } // Alternative rotation
    public Vector3 Scale { get; set; }        // Scale factors
    
    // Automatic matrix caching with dirty flag optimization
    public Matrix3 ToMatrix3() { /* Cached calculation */ }
}
```

## Package Information

**Current Version**: 23.0.0  
**Target Framework**: .NET 9.0  
**License**: MIT  
**Repository**: https://github.com/ApprenticeFoundry/BlazorThreeJS

## What's Included

This NuGet package includes:
- ✅ **Core Library**: Complete C# API for Three.js integration
- ✅ **JavaScript Bundle**: Compiled Three.js bundle (`app-lib.js` - 751KB)
- ✅ **Component Styles**: Ready-to-use CSS for 3D viewers
- ✅ **3D Assets**: Sample models and fonts for quick start
- ✅ **Build Integration**: Automatic static asset deployment

## Troubleshooting

### Assets Not Loading (404 Errors)
Make sure you've added the JavaScript reference to your app:
```html
<script src="_content/ApprenticeFoundryBlazorThreeJS/dist/app-lib.js"></script>
```

### Components Not Found
Add the using statement to your component or `_Imports.razor`:
```razor
@using BlazorThreeJS.Viewers
@using BlazorThreeJS.Core
```

## Documentation

- **[Pivot Implementation Analysis](PIVOT_IMPLEMENTATION_ANALYSIS.md)**: Technical details of the pivot system
- **[Pivot Usage Examples](PIVOT_USAGE_EXAMPLES.md)**: Comprehensive examples and use cases
- **[Sample Scene](Data/sample-scene.json)**: Complete scene example with various objects
- **[GitHub Repository](https://github.com/ApprenticeFoundry/BlazorThreeJS)**: Full source code and examples



## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/ApprenticeFoundry/BlazorThreeJS/blob/develop/LICENSE) file for more details.

## Support

- **Issues**: Report bugs or request features on [GitHub Issues](https://github.com/ApprenticeFoundry/BlazorThreeJS/issues)
- **Discussions**: Ask questions on [GitHub Discussions](https://github.com/ApprenticeFoundry/BlazorThreeJS/discussions)
- **Website**: Visit [ApprenticeFoundry.github.io](https://apprenticefoundry.github.io/) for more resources

## Release Notes

### Version 23.0.0
- ✅ **Blazor Component Library**: Properly configured for NuGet consumption
- ✅ **Static Web Assets**: Automatic deployment of JS/CSS/assets to consuming apps
- ✅ **Build Integration**: MSBuild targets for seamless project integration
- ✅ **Updated Dependencies**: Compatible with .NET 9.0 and latest Three.js
- ✅ **Improved Documentation**: Enhanced setup and usage instructions
