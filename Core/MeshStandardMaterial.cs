

using BlazorThreeJS.Core;



namespace BlazorThreeJS.Materials;

    public sealed class MeshStandardMaterial : Material
    {
        public MeshStandardMaterial()
          : base(nameof(MeshStandardMaterial))
        {
        }
        public MeshStandardMaterial(string color)
          : base(nameof(MeshStandardMaterial))
        {
            this.Color = color;
        }
        
        public string Color { get; set; } = "orange";

        public bool FlatShading { get; set; }

        public double Metalness { get; set; }

        public double Roughness { get; set; } = 1;

        public bool Wireframe { get; set; }

        public Texture Map { get; set; } = new Texture();
    }

