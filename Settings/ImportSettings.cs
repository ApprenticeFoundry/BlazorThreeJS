

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
        public ImportSettings() : base(nameof(ImportSettings))
        {
            Transform = null!;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Model3DFormats Format { get; set; }

        public string? FileURL { get; set; }

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
