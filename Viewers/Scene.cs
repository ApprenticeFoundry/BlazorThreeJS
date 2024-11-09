// Decompiled with JetBrains decompiler
// Type: Blazor3D.Scenes.Scene
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorThreeJS.Cameras;
using BlazorThreeJS.ComponentHelpers;
using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Settings;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using Microsoft.JSInterop;



namespace BlazorThreeJS.Viewers;

public class Scene : Object3D
{
    public string Title { get; init; }
    public string BackGroundColor { get; set; } = "#505050";

    
    private static Dictionary<Guid, ImportSettings> ImportPromises { get; set; } = new();
    private IJSRuntime JsRuntime { get; set; }
    private Dictionary<Guid, ImportSettings> LoadedModels { get; set; } = new();

    private Action<Scene,string>? AfterUpdate { get; set; } = (scene,json) => { };


    public Camera Camera { get; set; } = new PerspectiveCamera()
    {
        Position = new Vector3(5f, 5f, 5f)
    };

    private static List<Scene> _AllScenes { get; set; } = new();

    public Scene(string title, IJSRuntime js) : base(nameof(Scene))
    {
        // $"Scene {title} creating".WriteInfo();
        JsRuntime = js;
        Title = title;
        Name = title.ToLower();
        _AllScenes.Add(this);
        // $"Scene {Title} created".WriteInfo();
    }

    public void SetAfterUpdateAction(Action<Scene,string> afterUpdate)
    {
        if ( afterUpdate != null)
            AfterUpdate = afterUpdate;
        else 
            AfterUpdate = (scene,json) => { };
    }

    public static (bool success, Scene scene) EstablishScene(string title, IJSRuntime jS)
    {
        //$"EstablishScene {title}".WriteInfo();
        var found = _AllScenes.FirstOrDefault(scene => scene.Title.Matches(title));   
        if (found == null)
        {
            found = new Scene(title, jS);
            return (true, found);
        }
        return (false, found);
    }




    public static List<Scene> GetAllScenes()
    {
        var dummy = new List<Scene>();
        dummy.AddRange(_AllScenes);
        return dummy;
    }

    public static List<Scene> RemoveScene(Scene scene)
    {
        if (scene != null)
        {
            _AllScenes.Remove(scene);
        }
        return GetAllScenes();
    }



    public override string GetTreeNodeTitle()
    {
        var baseTitle = base.GetTreeNodeTitle();
        return $"t:{Title} -- {baseTitle}";
    }

    public override IEnumerable<TreeNodeAction>? GetTreeNodeActions()
    {
        var result = base.GetTreeNodeActions()!.ToList();

        result.AddAction("Clear", "btn-warning", () => 
        {
            Task.Run(async () => await ClearScene());
        });
        
        result.AddAction("Clear All", "btn-warning", () => 
        {
            Task.Run(async () => await ClearAll());
        });


        return result;
    }

    public string Resolve(string functionName)
    {
        //return $"{Title}.{functionName}";
        return $"BlazorThreeJS.{functionName}";
    }

    public async Task ClearScene()
    {
        var functionName = Resolve("clearScene");
        await JsRuntime!.InvokeVoidAsync(functionName);
        this.GetAllChildren().RemoveAll(item => item.Type.Contains("LabelText") || item.Type.Contains("Mesh"));
        AfterUpdate?.Invoke(this,"");
    }

    public async Task ClearAll()
    {
        var functionName = Resolve("clearScene");
        await JsRuntime!.InvokeVoidAsync(functionName);
        this.GetAllChildren().Clear();
        AfterUpdate?.Invoke(this,"");
    }

    public async Task SetCameraPosition(Vector3 position, Vector3? lookAt = null) 
    {
        var functionName = Resolve("setCameraPosition");
        await JsRuntime!.InvokeVoidAsync(functionName, (object)position, lookAt);
    }

    public async Task UpdateCamera(Camera camera)
    {
        var functionName = Resolve("updateCamera");
        this.Camera = camera;
        var json = JsonSerializer.Serialize((object)this.Camera, JSONOptions);
        await JsRuntime!.InvokeVoidAsync(functionName, (object)json);
    }

    public async Task ShowCurrentCameraInfo() 
    {
        var functionName = Resolve("showCurrentCameraInfo");
        await JsRuntime!.InvokeVoidAsync(functionName);
    }

    public void ForceSceneRefresh()
    {
        Task.Run( async () => await UpdateScene());
    }
    public async Task UpdateScene(bool notify = false)
    {
        try
        {
            var functionName = Resolve("updateScene");
            var json = JsonSerializer.Serialize((object)this, JSONOptions);
            $"UpdateScene: {functionName} => ".WriteInfo();

            await JsRuntime!.InvokeVoidAsync(functionName, (object)json);

            if ( notify == true)
                AfterUpdate?.Invoke(this,json);
        }
        catch (System.Exception ex)
        {
            ex.Message.WriteError();
        }
    }
    private JsonSerializerOptions JSONOptions { get; set; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        IncludeFields = true,
        IgnoreReadOnlyFields = true
    };
    public async Task<Guid> Request3DModel(ImportSettings settings)
    {
        Guid uuid = settings.Uuid;
        settings.Scene = this;
        ImportPromises.Add(uuid, settings);

        //Console.WriteLine($"Adding settings={uuid}");
        LoadedModels.Add(uuid, settings);

        // settings.Material = settings.Material ?? new MeshStandardMaterial();
        var functionName = Resolve("import3DModel");

        var json = JsonSerializer.Serialize((object)settings, JSONOptions);
        //$"Request3DModel  JSONOptions: {json}".WriteInfo();
        await JsRuntime!.InvokeVoidAsync(functionName, (object)json);
        await UpdateScene();

        //$"Request3DModel: {settings} {uuid}".WriteInfo();

        return uuid;
    }

    public async Task RemoveByUuidAsync(Guid uuid)
    {
        var functionName = Resolve("deleteByUuid");
        if (!await JsRuntime!.InvokeAsync<bool>(functionName, (object)uuid))
            return;

        ChildrenHelper.RemoveObjectByUuid(uuid, GetAllChildren());
    }


    public async Task MoveObject(Object3D object3D)
    {
        var functionName = Resolve("moveObject");
        await JsRuntime!.InvokeAsync<bool>(functionName, object3D);
    }

    public async Task<Guid> Clone3DModel(Guid sourceGuid, List<ImportSettings> settings)
    {
        settings.ForEach((setting) =>
        {
            setting.Scene = this;
            ImportPromises.Add(setting.Uuid, setting);
        });

        var functionName = Resolve("clone3DModel");
        var json = (object)JsonSerializer.Serialize((object)settings, JSONOptions);
        var args = new List<object>() { $"{sourceGuid}", json };
        await JsRuntime!.InvokeVoidAsync(functionName, args.ToArray());
        return sourceGuid;
    }

    [JSInvokable]
    public static Task ReceiveLoadedObjectUUID(string containerId, string uuid)
    {
        var guid = Guid.Parse(uuid);

        if (ImportPromises.ContainsKey(guid))
        {
            var settings = ImportPromises[guid];
            Group3D group = new()
            {
                Name = settings.Uuid.ToString(),
                Uuid = settings.Uuid,
            };
            settings.Scene!.Add(group);

            settings.OnComplete.Invoke(settings.Scene!, group);
            settings.OnComplete = (Scene s, Object3D o) => { };
            ImportPromises.Remove(guid);
        }
        return Task.CompletedTask;
    }
}

