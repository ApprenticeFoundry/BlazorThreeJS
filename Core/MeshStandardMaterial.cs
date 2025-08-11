

using BlazorThreeJS.Core;



namespace BlazorThreeJS.Materials;

    public sealed class MeshStandardMaterial : Material
    {
        public MeshStandardMaterial()
          : base(nameof(MeshStandardMaterial))
        {
        }
        public MeshStandardMaterial(string color, double opacity)
          : base(nameof(MeshStandardMaterial))
        {
            this.Color = color;
            this.Opacity = opacity;
        }
        


        public bool FlatShading { get; set; }

        public double Metalness { get; set; }

        public double Roughness { get; set; } = 1;



        public Texture Map { get; set; } = new Texture();
    }

