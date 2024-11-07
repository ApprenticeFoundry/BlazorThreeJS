// Decompiled with JetBrains decompiler
// Type: Blazor3D.Objects.Mesh
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;



namespace BlazorThreeJS.Objects
{
    public class Mesh3D : Object3D
    {
        public Mesh3D()
          : base(nameof(Mesh3D))
        {
        }

        public Material Material { get; set; } = (Material)new MeshStandardMaterial();

        public BufferGeometry Geometry { get; set; } = (BufferGeometry)new BoxGeometry();
    }
}
