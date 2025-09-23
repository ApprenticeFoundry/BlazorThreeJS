
# NuGet Package Build and Deploy Process

## Prerequisites
- Node.js and npm installed
- .NET 9.0 SDK

## Step 1: Build JavaScript Assets
```bash
cd JsLib
npm install  # Only needed first time or after package.json changes
npm run build
```
This creates the compiled Three.js bundle in `wwwroot/dist/app-lib.js`

## Step 2: Build .NET Library
```bash
# Clean previous builds
dotnet clean

# Build in Release mode  
dotnet build --configuration Release
```

## Step 3: Create NuGet Package
```bash
dotnet pack --configuration Release
```

## Step 4: Verify Package
Check that `.\bin\Release\ApprenticeFoundryBlazorThreeJS.<version>.nupkg` contains:
- Static web assets in the package
- Build targets file
- All required DLLs

## Step 5: Upload to NuGet.org
1. Login to nuget.org
2. Click "Upload"  
3. In File Explorer, locate `.\bin\Release\ApprenticeFoundryBlazorThreeJS.<version>.nupkg`
4. Upload to nuget.org

## Alternative: Command Line Upload
```bash
dotnet nuget push .\bin\Release\ApprenticeFoundryBlazorThreeJS.<version>.nupkg --api-key <your-api-key> --source https://api.nuget.org/v3/index.json
```

## Important Notes
- Always build JavaScript first before packaging
- Version number is automatically taken from the .csproj file
- Package is now properly configured as a Blazor component library
- Static assets will be automatically available to consuming applications