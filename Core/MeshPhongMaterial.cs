

using BlazorThreeJS.Core;



namespace BlazorThreeJS.Materials;

  public sealed class MeshPhongMaterial : Material
  {
      public MeshPhongMaterial()
        : base(nameof(MeshPhongMaterial))
      {
      }
      public MeshPhongMaterial(string color, double opacity)
        : base(nameof(MeshPhongMaterial))
      {
          this.Color = color;
          this.Opacity = opacity;
      }
      
      public bool Transparent { get; set; } = false;

      public bool FlatShading { get; set; }


      public Texture Map { get; set; } = new Texture();
  }

