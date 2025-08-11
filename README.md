# BlazorThreeJS

BlazorThreeJS is a NuGet package that provides seamless integration of the Three.js library with Blazor applications. This package allows developers to create rich 3D graphics and animations using the power of Three.js within a Blazor project.

## Features

- Easy integration of Three.js with Blazor
- Create and manipulate 3D objects and scenes
- Support for animations 
- Comprehensive documentation and examples

## Installation

To install BlazorThreeJS, run the following command in the NuGet Package Manager Console:

```sh
Install-Package BlazorThreeJS
```

Alternatively, you can add the package reference directly to your `.csproj` file:

```xml
<PackageReference Include="BlazorThreeJS" Version="17.1.0" />
```

## Getting Started

1. **Setup**: Add the BlazorThreeJS package to your Blazor project.
2. **Import**: Import the necessary namespaces in your Blazor components.
3. **Create**: Use the provided components and services to create and manipulate 3D scenes.

### Example

```razor
@page "/clock"

@using FoundryBlazor.Shared
@using FoundryRulesAndUnits.Extensions
@using BlazorThreeJS.Viewers

@rendermode InteractiveServer

<PageTitle>Clock</PageTitle>
<h2>Canvas3DComponent and calls API's top create Clock</h2>

<div class="d-flex">
    <Canvas3DComponent SceneName="Clock3D" @ref="Canvas3DReference" CanvasWidth=@CanvasWidth CanvasHeight=@CanvasHeight />
    <ShapeTreeView/>
</div>
```

## Documentation

For detailed documentation and examples, please visit the [official documentation](https://github.com/ApprenticeFoundry/BlazorThreeJS).



## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/ApprenticeFoundry/BlazorThreeJS/blob/develop/LICENSE) file for more details.

## Contact

For any questions or feedback, please open an issue on the [GitHub repository](https://apprenticefoundry.github.io/).
