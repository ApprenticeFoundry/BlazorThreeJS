# BlazorThreeJS Component Library - NuGet Packaging Guide

## Overview
This guide documents the proper configuration for BlazorThreeJS to deploy as a reliable NuGet package that can be easily consumed by other Blazor applications. The library provides Three.js integration with C# components and JavaScript interop.

## Project Structure

### 1. Create the Class Library Project
```bash
dotnet new razorclasslib -n MyBlazorComponentLibrary
cd MyBlazorComponentLibrary
```

### 2. Required Project File Configuration
Edit `MyBlazorComponentLibrary.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- Essential for static web assets -->
    <GenerateEmbeddedFilesManifest>true</GenerateEmbeddedFilesManifest>
    <StaticWebAssetBasePath>_content/MyBlazorComponentLibrary</StaticWebAssetBasePath>
    
    <!-- Package information -->
    <PackageId>MyBlazorComponentLibrary</PackageId>
    <Version>1.0.0</Version>
    <Authors>Your Name</Authors>
    <Description>Blazor component library with static assets</Description>
  </PropertyGroup>

  <ItemGroup>
    <SupportedPlatform Include="browser" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="8.0.0" />
  </ItemGroup>

  <!-- Ensure all wwwroot content is included as static web assets -->
  <ItemGroup>
    <Content Include="wwwroot\**" BuildAction="StaticWebAsset" />
  </ItemGroup>

</Project>
```

### 3. Directory Structure
Create the following folder structure:
```
MyBlazorComponentLibrary/
├── Components/
│   └── MyComponent.razor
├── wwwroot/
│   ├── css/
│   │   └── mycomponent.css
│   ├── js/
│   │   └── mycomponent.js
│   └── images/
│       └── icon.png
├── MyBlazorComponentLibrary.csproj
└── _Imports.razor
```

## Component Implementation

### 4. Sample Component (Components/MyComponent.razor)
```razor
@using Microsoft.AspNetCore.Components.Web

<div class="my-component">
    <h3>@Title</h3>
    <p>@Description</p>
    <button class="my-button" @onclick="OnClick">Click Me</button>
</div>

@code {
    [Parameter] public string Title { get; set; } = "Default Title";
    [Parameter] public string Description { get; set; } = "Default Description";
    [Parameter] public EventCallback OnClick { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Initialize JavaScript if needed
            await JSRuntime.InvokeVoidAsync("MyComponentLibrary.initialize");
        }
    }

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
}
```

### 4b. HTML5 Canvas Component (Components/CanvasChart.razor)
```razor
@using Microsoft.AspNetCore.Components.Web
@implements IAsyncDisposable

<div class="canvas-container" style="width: @Width; height: @Height;">
    <canvas @ref="canvasElement" 
            id="@CanvasId" 
            width="@CanvasWidth" 
            height="@CanvasHeight"
            class="canvas-chart">
        Canvas not supported in this browser.
    </canvas>
</div>

@code {
    private ElementReference canvasElement;
    private IJSObjectReference? jsModule;
    private IJSObjectReference? canvasInstance;

    [Parameter] public string Width { get; set; } = "400px";
    [Parameter] public string Height { get; set; } = "300px";
    [Parameter] public int CanvasWidth { get; set; } = 400;
    [Parameter] public int CanvasHeight { get; set; } = 300;
    [Parameter] public object[]? Data { get; set; }
    [Parameter] public EventCallback<string> OnCanvasClick { get; set; }

    private string CanvasId { get; set; } = Guid.NewGuid().ToString("N")[..8];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load the JavaScript module
            jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/MyBlazorComponentLibrary/js/canvas-chart.js");

            // Initialize the canvas with a reference back to this component
            var dotNetRef = DotNetObjectReference.Create(this);
            canvasInstance = await jsModule.InvokeAsync<IJSObjectReference>(
                "createCanvasChart", canvasElement, dotNetRef, CanvasId);

            // Load initial data if provided
            if (Data != null)
            {
                await UpdateData(Data);
            }
        }
    }

    public async Task UpdateData(object[] newData)
    {
        if (canvasInstance != null)
        {
            await canvasInstance.InvokeVoidAsync("updateData", newData);
        }
    }

    public async Task ClearCanvas()
    {
        if (canvasInstance != null)
        {
            await canvasInstance.InvokeVoidAsync("clear");
        }
    }

    [JSInvokable]
    public async Task OnCanvasClickedFromJS(string data)
    {
        await OnCanvasClick.InvokeAsync(data);
    }

    public async ValueTask DisposeAsync()
    {
        if (canvasInstance != null)
        {
            await canvasInstance.InvokeVoidAsync("dispose");
            await canvasInstance.DisposeAsync();
        }

        if (jsModule != null)
        {
            await jsModule.DisposeAsync();
        }
    }

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
}
```

### 5. Component Stylesheet (wwwroot/css/mycomponent.css)
```css
.my-component {
    border: 1px solid #ddd;
    border-radius: 4px;
    padding: 16px;
    margin: 8px 0;
    background-color: #f9f9f9;
}

.my-button {
    background-color: #007bff;
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.2s;
}

.my-button:hover {
    background-color: #0056b3;
}

/* Canvas component styles */
.canvas-container {
    position: relative;
    display: inline-block;
    border: 1px solid #ccc;
    border-radius: 4px;
    overflow: hidden;
    background-color: white;
}

.canvas-chart {
    display: block;
    cursor: crosshair;
    user-select: none;
}

.canvas-chart:hover {
    opacity: 0.9;
}

/* Responsive canvas container */
.canvas-container.responsive {
    width: 100%;
    height: auto;
}

.canvas-container.responsive .canvas-chart {
    width: 100%;
    height: auto;
}
```

### 6. JavaScript Module (wwwroot/js/mycomponent.js)
```javascript
window.MyComponentLibrary = {
    initialize: function() {
        console.log('MyComponentLibrary initialized');
    },
    
    doSomething: function(element, value) {
        // Custom JavaScript functionality
        element.style.transform = `scale(${value})`;
    }
};
```

### 6b. Canvas JavaScript Module (wwwroot/js/canvas-chart.js)
```javascript
// ES6 Module for Canvas Chart Component
export function createCanvasChart(canvasElement, dotNetRef, canvasId) {
    const canvas = canvasElement;
    const ctx = canvas.getContext('2d');
    let animationId = null;
    let currentData = [];

    // Canvas chart instance
    const instance = {
        updateData: function(data) {
            currentData = data || [];
            render();
        },

        clear: function() {
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            currentData = [];
        },

        dispose: function() {
            if (animationId) {
                cancelAnimationFrame(animationId);
            }
            canvas.removeEventListener('click', handleCanvasClick);
            canvas.removeEventListener('mousemove', handleMouseMove);
            canvas.removeEventListener('mouseout', handleMouseOut);
        }
    };

    // Event handlers
    function handleCanvasClick(event) {
        const rect = canvas.getBoundingClientRect();
        const x = event.clientX - rect.left;
        const y = event.clientY - rect.top;
        
        // Find clicked data point
        const clickedData = findDataPointAt(x, y);
        if (clickedData) {
            // Call back to Blazor component
            dotNetRef.invokeMethodAsync('OnCanvasClickedFromJS', JSON.stringify(clickedData));
        }
    }

    function handleMouseMove(event) {
        const rect = canvas.getBoundingClientRect();
        const x = event.clientX - rect.left;
        const y = event.clientY - rect.top;
        
        // Update cursor based on hover state
        const hoveredData = findDataPointAt(x, y);
        canvas.style.cursor = hoveredData ? 'pointer' : 'crosshair';
        
        // Optionally highlight hovered element
        render(hoveredData);
    }

    function handleMouseOut() {
        canvas.style.cursor = 'crosshair';
        render(); // Clear hover effects
    }

    function findDataPointAt(x, y) {
        // Simple implementation - you'd customize this based on your chart type
        for (let i = 0; i < currentData.length; i++) {
            const point = getPointCoordinates(i);
            const distance = Math.sqrt(Math.pow(x - point.x, 2) + Math.pow(y - point.y, 2));
            if (distance <= 10) { // 10px radius for click detection
                return {
                    index: i,
                    value: currentData[i],
                    coordinates: point
                };
            }
        }
        return null;
    }

    function getPointCoordinates(index) {
        // Convert data index to canvas coordinates
        // This is a simple example - customize based on your chart type
        const padding = 40;
        const chartWidth = canvas.width - (padding * 2);
        const chartHeight = canvas.height - (padding * 2);
        
        const x = padding + (index * chartWidth / Math.max(1, currentData.length - 1));
        const maxValue = Math.max(...currentData.map(d => typeof d === 'number' ? d : d.value || 0));
        const value = typeof currentData[index] === 'number' ? currentData[index] : currentData[index]?.value || 0;
        const y = canvas.height - padding - ((value / maxValue) * chartHeight);
        
        return { x, y };
    }

    function render(hoveredData = null) {
        // Clear canvas
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        
        if (currentData.length === 0) return;

        const padding = 40;
        const chartWidth = canvas.width - (padding * 2);
        const chartHeight = canvas.height - (padding * 2);

        // Draw axes
        ctx.strokeStyle = '#ddd';
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(padding, padding);
        ctx.lineTo(padding, canvas.height - padding);
        ctx.lineTo(canvas.width - padding, canvas.height - padding);
        ctx.stroke();

        // Draw data points and lines
        const maxValue = Math.max(...currentData.map(d => typeof d === 'number' ? d : d.value || 0));
        
        ctx.strokeStyle = '#007bff';
        ctx.fillStyle = '#007bff';
        ctx.lineWidth = 2;
        ctx.beginPath();

        for (let i = 0; i < currentData.length; i++) {
            const point = getPointCoordinates(i);
            
            if (i === 0) {
                ctx.moveTo(point.x, point.y);
            } else {
                ctx.lineTo(point.x, point.y);
            }
        }
        ctx.stroke();

        // Draw data points
        for (let i = 0; i < currentData.length; i++) {
            const point = getPointCoordinates(i);
            const isHovered = hoveredData && hoveredData.index === i;
            
            ctx.beginPath();
            ctx.arc(point.x, point.y, isHovered ? 6 : 4, 0, 2 * Math.PI);
            ctx.fillStyle = isHovered ? '#ff6b35' : '#007bff';
            ctx.fill();
            
            // Draw value labels for hovered points
            if (isHovered) {
                const value = typeof currentData[i] === 'number' ? currentData[i] : currentData[i]?.value || 0;
                ctx.fillStyle = '#333';
                ctx.font = '12px Arial';
                ctx.textAlign = 'center';
                ctx.fillText(value.toString(), point.x, point.y - 10);
            }
        }
    }

    // Initialize event listeners
    canvas.addEventListener('click', handleCanvasClick);
    canvas.addEventListener('mousemove', handleMouseMove);
    canvas.addEventListener('mouseout', handleMouseOut);

    // Initial render
    render();

    return instance;
}

// Additional utility functions for canvas operations
export function downloadCanvasAsImage(canvasElement, filename = 'chart.png') {
    const link = document.createElement('a');
    link.download = filename;
    link.href = canvasElement.toDataURL();
    link.click();
}

export function resizeCanvas(canvasElement, width, height) {
    canvasElement.width = width;
    canvasElement.height = height;
    // Note: This clears the canvas, so you'll need to re-render after resize
}
```

## Library Setup Files

### 7. _Imports.razor
```razor
@using Microsoft.AspNetCore.Components
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.JSInterop
```

### 8. LibraryImports.razor (for CSS bundling)
Create this file in the root of your library:
```razor
@* This file ensures CSS is properly bundled *@
<link href="_content/MyBlazorComponentLibrary/css/mycomponent.css" rel="stylesheet" />
```

## Advanced Configuration

### 9. MSBuild Targets (Optional but Recommended)
Create `build/MyBlazorComponentLibrary.targets`:
```xml
<Project>
  <Target Name="IncludeStaticWebAssets" BeforeTargets="Build">
    <ItemGroup>
      <StaticWebAsset Include="$(MSBuildThisFileDirectory)../wwwroot/**/*.*">
        <SourceType>Package</SourceType>
        <SourceId>$(PackageId)</SourceId>
        <ContentRoot>$(MSBuildThisFileDirectory)../wwwroot/</ContentRoot>
        <BasePath>_content/$(PackageId)</BasePath>
      </StaticWebAsset>
    </ItemGroup>
  </Target>
</Project>
```

Update your `.csproj` to include this:
```xml
<ItemGroup>
  <None Include="build\**" Pack="true" PackagePath="build\" />
</ItemGroup>
```

## Consumer Application Setup

### 10. Consuming the Library
In the consuming Blazor application:

**Program.cs (or Startup.cs):**
```csharp
// No special configuration needed for static assets - they're automatic

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Standard configuration
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

**_Host.cshtml or App.razor:**
```html
<!-- Reference your library's CSS -->
<link href="_content/MyBlazorComponentLibrary/css/mycomponent.css" rel="stylesheet" />

<!-- Reference your library's JS -->
<script src="_content/MyBlazorComponentLibrary/js/mycomponent.js"></script>
```

**Using the components:**
```razor
@page "/"
@using MyBlazorComponentLibrary.Components

<h1>Basic Component</h1>
<MyComponent Title="Hello World" 
             Description="This is a test component"
             OnClick="HandleClick" />

<h1>Canvas Chart Component</h1>
<CanvasChart Width="600px" 
             Height="400px" 
             CanvasWidth="600" 
             CanvasHeight="400"
             Data="chartData" 
             OnCanvasClick="HandleCanvasClick" />

<button @onclick="UpdateChartData">Update Chart Data</button>
<button @onclick="ClearChart">Clear Chart</button>

@code {
    private CanvasChart? canvasChartRef;
    private object[] chartData = { 10, 25, 15, 40, 30, 35, 20 };

    private void HandleClick()
    {
        // Handle the basic component click event
    }

    private void HandleCanvasClick(string clickData)
    {
        Console.WriteLine($"Canvas clicked: {clickData}");
        // Handle canvas click with data about what was clicked
    }

    private async Task UpdateChartData()
    {
        var random = new Random();
        chartData = Enumerable.Range(0, 7)
            .Select(_ => (object)random.Next(10, 50))
            .ToArray();
        
        if (canvasChartRef != null)
        {
            await canvasChartRef.UpdateData(chartData);
        }
    }

    private async Task ClearChart()
    {
        if (canvasChartRef != null)
        {
            await canvasChartRef.ClearCanvas();
        }
    }
}
```

## HTML5 Canvas-Specific Considerations

### Canvas Component Best Practices

1. **Memory Management:**
   - Always implement `IAsyncDisposable` for canvas components
   - Dispose of JavaScript object references properly
   - Cancel animation frames on disposal

2. **Event Handling:**
   - Use ES6 modules for better isolation
   - Implement proper mouse/touch event handling
   - Use `DotNetObjectReference` for callbacks to Blazor

3. **Performance:**
   - Avoid frequent re-renders during animations
   - Use `requestAnimationFrame` for smooth animations
   - Cache calculations when possible

4. **Responsive Design:**
   - Handle canvas resizing properly
   - Consider device pixel ratio for high-DPI displays
   - Implement touch events for mobile devices

5. **Accessibility:**
   - Provide alternative text descriptions
   - Implement keyboard navigation where applicable
   - Consider screen reader compatibility

### Canvas Module Loading

For canvas components, prefer ES6 modules over global JavaScript objects:

```csharp
// Load module dynamically
jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
    "import", "./_content/MyLibraryName/js/canvas-module.js");
```

This approach provides better isolation and prevents conflicts with other libraries.

## Troubleshooting Checklist

### Common Issues and Solutions:

1. **Assets not found (404 errors):**
   - Verify `StaticWebAssetBasePath` in `.csproj`
   - Ensure `GenerateEmbeddedFilesManifest` is `true`
   - Check that files are in `wwwroot` folder
   - Rebuild both library and consumer application

2. **CSS not loading:**
   - Verify CSS file path in consuming app
   - Check browser dev tools for 404s
   - Ensure CSS file has correct Build Action

3. **JavaScript not working:**
   - Verify script tag in consuming app
   - Check browser console for errors
   - Ensure JS file is properly referenced

4. **Canvas-specific issues:**
   - Check for JavaScript module import errors
   - Verify canvas element references are valid
   - Ensure proper disposal of JS object references
   - Test canvas rendering in different browsers
   - Verify touch/mouse events work on mobile devices

5. **NuGet package issues:**
   - Clean and rebuild solution
   - Clear NuGet cache: `dotnet nuget locals all --clear`
   - Verify package references are correct

### Verification Steps:

1. **Build the library:**
   ```bash
   dotnet build
   ```

2. **Check generated files:**
   Look in `bin/Debug/net8.0/` for:
   - `MyBlazorComponentLibrary.dll`
   - `MyBlazorComponentLibrary.StaticWebAssets.xml`

3. **Verify in consuming app:**
   After referencing the library, check that assets appear in:
   - `wwwroot/_content/MyBlazorComponentLibrary/`

4. **Test asset URLs:**
   Navigate to: `https://localhost:port/_content/MyBlazorComponentLibrary/css/mycomponent.css`

## Best Practices

1. **Naming Convention:** Use consistent naming that matches your assembly name
2. **Version Management:** Update version in `.csproj` for each release
3. **Documentation:** Include XML documentation for public APIs
4. **Testing:** Test with both local references and NuGet packages
5. **CSS Isolation:** Consider using scoped CSS (`.razor.css` files) for component-specific styles

## Example Commands for Testing

```bash
# Build the library
dotnet build

# Pack for NuGet
dotnet pack

# Create test consumer app
dotnet new blazorserver -n TestConsumerApp
cd TestConsumerApp
dotnet add reference ../MyBlazorComponentLibrary/MyBlazorComponentLibrary.csproj

# Or install from NuGet
dotnet add package MyBlazorComponentLibrary
```

Following this guide ensures that your Blazor component library will properly package and serve static web assets, just like commercial libraries such as Radzen components.