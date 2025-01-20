

using System.Text.Json.Serialization;
using BlazorThreeJS.Core;
using FoundryRulesAndUnits.Models;

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
    public bool RunAnimation { get; set; } = true;
    private Action<Model3D, int, double>? OnAnimationUpdate { get; set; } = null;

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

    public void SetAnimationUpdate(Action<Model3D, int, double> update)
    {
        OnAnimationUpdate = update;
    }

    public override IEnumerable<TreeNodeAction> GetTreeNodeActions()
    {
        var result = base.GetTreeNodeActions().ToList();
        if ( OnAnimationUpdate == null)
            return result;

        if (RunAnimation == false)
            result.AddAction("Run", "btn-success", () =>
            {
                RunAnimation = true;
            });

        if (RunAnimation == true)
            result.AddAction("Stop", "btn-success", () =>
            {
                RunAnimation = false;
            });
        return result;
    }


    public override void UpdateForAnimation(int tick, double fps, List<Object3D>? dirtyObjects)
    {
        $"Model3D.UpdateForAnimation {Name} {tick} {fps}".WriteSuccess(2);
        if ( !RunAnimation  || OnAnimationUpdate == null) return;

        OnAnimationUpdate.Invoke(this, tick, fps);

        if ( IsDirty() )
            dirtyObjects?.Add(this);
            
        base.UpdateForAnimation(tick, fps, dirtyObjects);
    }
}

