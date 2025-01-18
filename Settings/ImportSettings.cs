

using BlazorThreeJS.Enums;
using BlazorThreeJS.Materials;

using BlazorThreeJS.Maths;
using BlazorThreeJS.Core;
using BlazorThreeJS.Objects;

using System.Text.Json.Serialization;



namespace BlazorThreeJS.Settings
{
    public class ImportSettings : Object3D
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Model3DFormats Format { get; set; } = Model3DFormats.Gltf;
        public string FileURL { get; set; } = "";
        public ImportSettings() : base(nameof(ImportSettings))
        {
            Transform = null!;
        }

        public Model3D AddRequestedModel(Model3D model)
        {
            AddChild(model);
            FileURL = model.Url;
            Uuid = model.Uuid;
            Format = model.Format;
            return model;
        }

        public (bool success, Model3D result) FindRequestedModel()
        {
            var (found, item) = FindChild(Uuid!);
            if (!found)
                return (false, null!);

            if (item is Model3D model)
                return (true, model);
            
            return (false, null!);
        }


        public void ResetChildren(List<Object3D> children)
        {
            ClearChildren();
            foreach (var child in children)
            {
                AddChild(child);
                child.SetDirty(false);
            }
        }


    }
}
