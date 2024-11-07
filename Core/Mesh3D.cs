using BlazorThreeJS.Core;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;



namespace BlazorThreeJS.Objects;

  public class Mesh : Object3D
  {
      public Mesh()
        : base(nameof(Mesh))
      {
      }

      public Material Material { get; set; } = (Material)new MeshStandardMaterial();

      public BufferGeometry Geometry { get; set; } = (BufferGeometry)new BoxGeometry();
  }

