# BlazorThreeJS NuGet Package

## Publishing Steps

1. Build JavaScript assets:
   ```bash
   cd BlazorThreeJS\JsLib
   npm run build
   cd ..
   ```

2. Build and pack:
   ```bash
   dotnet build --configuration Release
   dotnet pack --configuration Release  
   ```

3. Upload to NuGet:
   - Login to nuget.org
   - Click Upload
   - In File Explorer locate `.\bin\Release\ApprenticeFoundryBlazorThreeJS.<version>.nupkg`
   - Upload to nuget.org

## Version 22.0.0+ Changes

⚠️ **JavaScript Asset Renamed**: The JavaScript bundle is now `app-lib-threejs.js` instead of `app-lib.js` to prevent conflicts with other Blazor libraries.

**Impact for users**: Minimal to none - Blazor automatically handles static web asset discovery.