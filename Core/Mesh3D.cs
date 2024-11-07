using BlazorThreeJS.Core;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;



namespace BlazorThreeJS.Objects;

  public class Mesh3D : Object3D
  {
      public Mesh3D()
        : base(nameof(Mesh3D))
      {
      }

      public Material Material { get; set; } = (Material)new MeshStandardMaterial();

      public BufferGeometry Geometry { get; set; } = (BufferGeometry)new BoxGeometry();
  }

