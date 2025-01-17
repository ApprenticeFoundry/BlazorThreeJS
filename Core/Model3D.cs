

using System.Text.Json.Serialization;
using BlazorThreeJS.Core;

namespace BlazorThreeJS.Objects;


public enum Model3DFormats
{
    Obj,
    Collada,
    Fbx,
    Gltf,
    Stl,
}



public class Model3D : Object3D
{
    public string Url { get; set; } = "";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Model3DFormats Format { get; set; } = Model3DFormats.Gltf;
    public Model3D()
      : base(nameof(Model3D))
    {
    }

    public Model3D(string url, Model3DFormats format)
      : this()
    {
        this.Url = url;
        this.Format = format;
    }
}

