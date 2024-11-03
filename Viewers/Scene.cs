// Decompiled with JetBrains decompiler
// Type: Blazor3D.Scenes.Scene
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorThreeJS.ComponentHelpers;
using BlazorThreeJS.Core;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Settings;
using FoundryRulesAndUnits.Extensions;
using Microsoft.JSInterop;



namespace BlazorThreeJS.Viewers;

public class Scene : Object3D
{
    public string Title { get; init; }
    public bool IsActive { get; set; } = false;
    
    private IJSRuntime JsRuntime { get; set; }
    private static Dictionary<Guid, ImportSettings> ImportPromises { get; set; } = new();
    private Dictionary<Guid, ImportSettings> LoadedModels { get; set; } = new();
    public string BackGroundColor { get; set; } = "#505050";

    private static List<Scene> _AllScenes { get; set; } = new();

    public Scene(string title, IJSRuntime js) : base(nameof(Scene))
    {
        JsRuntime = js;
        Title = title;
        _AllScenes.Add(this);
    }

    public static bool FindBestScene(string title, out Scene? found)
    {
        found = Scene.GetSceneByTitle(title);
        if ( found == null)
            found = _AllScenes.First();
        return found != null;
    }

    public static Scene? GetSceneByTitle(string title)
    {
        return _AllScenes.FirstOrDefault(scene => scene.Title.Matches(title));
    }

    public static List<Scene> GetAllScenes()
    {
        return _AllScenes;
    }

    public static List<Scene> RemoveScene(Scene scene)
    {
        if (scene != null)
        {
            _AllScenes.Remove(scene);
        }
        return GetAllScenes();
    }

    public static Scene SetActiveScene(Scene scene)
    {
        GetAllScenes().ForEach(s => s.IsActive = false);
        scene.IsActive = true;
        return scene;
    }

    public override string GetTreeNodeTitle()
    {
        var baseTitle = base.GetTreeNodeTitle();
        return $"t:{Title} a:{IsActive} -- {baseTitle}";
    }

    public async Task ClearSceneAsync()
    {
        await JsRuntime!.InvokeVoidAsync("BlazorThreeJS.clearScene");
        this.GetAllChildren().RemoveAll(item => item.Type.Contains("LabelText") || item.Type.Contains("Mesh"));
    }

    public async Task ClearLightsAsync()
    {
        await JsRuntime!.InvokeVoidAsync("BlazorThreeJS.clearScene");
        this.GetAllChildren().Clear();
    }
    public async Task UpdateScene()
    {
        var json = JsonSerializer.Serialize((object)this, JSONOptions);
        await JsRuntime!.InvokeVoidAsync("BlazorThreeJS.updateScene", (object)json);
    }
    private JsonSerializerOptions JSONOptions { get; set; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
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

        var json = JsonSerializer.Serialize((object)settings, JSONOptions);
        //$"Request3DModel  JSONOptions: {json}".WriteInfo();
        await JsRuntime!.InvokeVoidAsync("BlazorThreeJS.import3DModel", (object)json);
        await UpdateScene();

        //$"Request3DModel: {settings} {uuid}".WriteInfo();

        return uuid;
    }

    public async Task RemoveByUuidAsync(Guid uuid)
    {
        if (!await JsRuntime!.InvokeAsync<bool>("BlazorThreeJS.deleteByUuid", (object)uuid))
            return;

        ChildrenHelper.RemoveObjectByUuid(uuid, GetAllChildren());
    }


    public async Task MoveObject(Object3D object3D)
    {
        if (!await JsRuntime!.InvokeAsync<bool>("BlazorThreeJS.moveObject", object3D))
            return;
    }

    public async Task<Guid> Clone3DModel(Guid sourceGuid, List<ImportSettings> settings)
    {
        settings.ForEach((setting) =>
        {
            setting.Scene = this;
            ImportPromises.Add(setting.Uuid, setting);
        });


        var json = (object)JsonSerializer.Serialize((object)settings, JSONOptions);
        var args = new List<object>() { $"{sourceGuid}", json };
        await JsRuntime!.InvokeVoidAsync("BlazorThreeJS.clone3DModel", args.ToArray());
        return sourceGuid;
    }

    [JSInvokable]
    public static Task ReceiveLoadedObjectUUID(string containerId, string uuid)
    {
        var guid = Guid.Parse(uuid);

        if (ImportPromises.ContainsKey(guid))
        {
            var settings = ImportPromises[guid];
            Group group = new()
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

