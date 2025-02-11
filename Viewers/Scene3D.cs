// Decompiled with JetBrains decompiler
// Type: Blazor3D.Scenes.Scene
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorThreeJS.Cameras;

using BlazorThreeJS.Core;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Settings;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using Microsoft.JSInterop;



namespace BlazorThreeJS.Viewers;

public class Scene3D : Object3D
{
    public string Title { get; init; }
    public string BackGroundColor { get; set; } = "#505050";
    //public bool SuspendAnimation { get; set; } = true;



    private static Dictionary<string, Func<string,Task>> ImportPromises { get; set; } = new();
    private IJSRuntime JsRuntime { get; set; }
    private Action<Scene3D,string>? AfterUpdate { get; set; } = (scene,json) => { };


    public Camera Camera { get; set; } = new PerspectiveCamera();

    private static List<Scene3D> _AllScenes { get; set; } = new();

    public Scene3D(string title, IJSRuntime js) : base(nameof(Scene3D))
    {
        // $"Scene {title} creating".WriteInfo();
        JsRuntime = js;
        Title = title;
        Name = title.ToLower();
            
        Camera.GetTransform().Position = new Vector3(10f, 10f, 10f);
        _AllScenes.Add(this);
        // $"Scene {Title} created".WriteInfo();
    }



    public void SetAfterUpdateAction(Action<Scene3D,string> afterUpdate)
    {
        if ( afterUpdate != null)
            AfterUpdate = afterUpdate;
        else 
            AfterUpdate = (scene,json) => { };
    }

    public static (bool success, Scene3D scene) FindScene(string title)
    {
        //$"FindScene {title}".WriteInfo();
        var found = _AllScenes.FirstOrDefault(scene => scene.Title.Matches(title));
        return (found != null, found!);
    }

    public static Scene3D EstablishScene(string title, IJSRuntime jS, Action<Scene3D>? OnCreate)
    {
        //$"EstablishScene {title}".WriteInfo();
        var (found, scene) = FindScene(title);
        if (found) return scene;

        var result = new Scene3D(title, jS);
        OnCreate?.Invoke(result);
        return result;

    }




    public static List<Scene3D> GetAllScenes()
    {
        var dummy = new List<Scene3D>();
        dummy.AddRange(_AllScenes);
        return dummy;
    }

    public static List<Scene3D> RemoveScene(Scene3D scene)
    {
        if (scene != null)
        {
            _AllScenes.Remove(scene);
        }
        return GetAllScenes();
    }

    public void UpdateHitBoundaries(Action? OnComplete = null)
    {
        var list = new List<Task>();
        foreach (var item in Children)
        {
            list.Add(item.ComputeHitBoundary(this, true));
        }
        Task.WhenAll(list).ContinueWith((task) => 
        {
            OnComplete?.Invoke();
        });
        
    }

    public override string GetTreeNodeTitle()
    {
        var baseTitle = base.GetTreeNodeTitle();
        return $"t:{Title} -- {baseTitle}";
    }

    public override IEnumerable<TreeNodeAction> GetTreeNodeActions()
    {
        var result = base.GetTreeNodeActions().ToList();

        result.AddAction("Clear", "btn-warning", () => 
        {
            ClearScene();
        });
        
        result.AddAction("Clear All", "btn-warning", () => 
        {
            Task.Run(async () => await ClearAll());
        });

        result.AddAction("Boundary", "btn-primary", () => 
        {
            UpdateHitBoundaries(()=>
            {
                $"UpdateHitBoundaries for {Title} complete".WriteInfo();
            }); 
        });


        return result;
    }

    public (bool success, Task refresh, Task delete) ComputeRefreshObjects()
    {
        Task refreshTask = Task.CompletedTask;
        Task deleteTask = Task.CompletedTask;

        var dirtyObjects = new List<Object3D>();
        var deletedObjects = new List<Object3D>();
        if (!this.CollectDirtyObjects(dirtyObjects, deletedObjects))
        {
            return (false, refreshTask, deleteTask);
        }

        if (dirtyObjects.Count > 0)
        {
            $"Need to refresh {dirtyObjects.Count} objects".WriteSuccess();
            var refresh = new ImportSettings();
            refresh.CopyAndReset(dirtyObjects);
            refreshTask = this.Request3DSceneRefresh(refresh, (_) =>
            {
               $"ComputeRefreshObjects  {dirtyObjects.Count} dirty objects updated".WriteSuccess(1);
               foreach(var item in dirtyObjects)
               {
                   $"Refreshed {item.Name} {item.Type} IsDirty {item.IsDirty}".WriteInfo(1);
               }
            });
        }


        if (deletedObjects.Count > 0)
        {
           // $"Need to delete {deletedObjects.Count} objects".WriteSuccess();
            var delete = new ImportSettings();
            delete.CopyAndReset(deletedObjects);
            deleteTask = this.Request3DSceneDelete(delete, (_) =>
            {
               // $"ComputeRefreshObjects  {deletedObjects.Count} deleted objects".WriteSuccess();
            });
        }
        return (true, refreshTask, deleteTask);
    }

    public string ResolveFunction(string functionName)
    {
        //return $"{Title}.{functionName}";
        //$"Calling BlazorThreeJS.{functionName}".WriteInfo();
        return $"BlazorThreeJS.{functionName}";
    }

    public void ClearScene()
    {
        Task.Run(async () => {
            var functionName = ResolveFunction("clearScene");
            await JsRuntime!.InvokeVoidAsync(functionName);
        });

        this.GetAllChildren().RemoveAll(item => item.Type.Contains("Text") || item.Type.Contains("Mesh"));
        AfterUpdate?.Invoke(this,"");
    }

    public async Task ClearAll()
    {
        var functionName = ResolveFunction("clearScene");
        await JsRuntime!.InvokeVoidAsync(functionName);
        this.GetAllChildren().Clear();
        AfterUpdate?.Invoke(this,"");
    }

    public async Task SetCameraPosition(Vector3 position, Vector3? lookAt = null) 
    {
        var functionName = ResolveFunction("setCameraPosition");
        await JsRuntime!.InvokeVoidAsync(functionName, (object)position, lookAt);
    }

    public async Task UpdateCamera(Camera camera)
    {
        var functionName = ResolveFunction("updateCamera");
        this.Camera = camera;
        var json = JsonSerializer.Serialize((object)this.Camera, JSONOptions);
        await JsRuntime!.InvokeVoidAsync(functionName, (object)json);
    }

    public async Task ShowCurrentCameraInfo() 
    {
        var functionName = ResolveFunction("showCurrentCameraInfo");
        await JsRuntime!.InvokeVoidAsync(functionName);
    }

    public async Task<HitBoundaryDTO> Request3DHitBoundary(Object3D source)
    {
        //$"Request3DHitBoundary {source}".WriteInfo();
        try
        {
            var functionName = ResolveFunction("request3DHitBoundary");
            var json = JsonSerializer.Serialize((object)source, JSONOptions);
            //WriteToFolder("Data", "Request3DHitBoundary.json", json); 
            var boundary = await JsRuntime!.InvokeAsync<HitBoundaryDTO>(functionName, (object)json);

            //var result = JsonSerializer.Serialize((object)boundary, JSONOptions);
            //$"Return HitBoundary: {result}".WriteNote();
            
            if ( boundary is HitBoundaryDTO dto)
            {
                source.HitBoundary = new HitBoundary3D()
                {
                    Uuid = dto.Uuid,
                    Name = dto.Name,
                    X = dto.X,
                    Y = dto.Y,
                    Z = dto.Z,
                    Width = dto.Width,
                    Height = dto.Height,
                    Depth = dto.Depth
                };
            } else {
                $"Request3DHitBoundary: boundary for {source.Name} {source.Type} is null".WriteError();
            }

             
            return boundary;

        }
        catch (System.Exception ex)
        {  
           $"Request3DHitBoundary: {ex.Message}".WriteError();
        }
        return null!;
    }

    public override void UpdateForAnimation(int tick, double fps)
    {
        //if (SuspendAnimation)
        //    return;

        base.UpdateForAnimation(tick, fps);
    }

    public async Task<List<string>> Request3DSceneRefresh(ImportSettings settings, Action<List<string>>? onComplete = null)
    {
        var uuids = settings.Children.Select(child => child.Uuid).ToList();
        if (uuids.Count == 0)
            return uuids!;

        try
        {
            $"Request3DSceneRefresh {uuids.Count}".WriteInfo();
            var functionName = ResolveFunction("request3DSceneRefresh");
            var json = JsonSerializer.Serialize((object)settings, JSONOptions);
            //WriteToFolder("Data", "Scene3D_Request3SceneRefresh.json", json); 

            await JsRuntime!.InvokeVoidAsync(functionName, (object)json);
            if (onComplete != null)
            {
                //$"Request3DSceneRefresh onComplete {uuids.Count}".WriteInfo();
                onComplete(uuids!);  // Now we can await the async callback
            }
        }
        catch (System.Exception ex)
        {  
           $"Request3DSceneRefresh: {ex.Message}".WriteError();
        }

        return uuids!;
    }

    public async Task<List<string>> Request3DSceneDelete(ImportSettings settings, Action<List<string>>? onComplete = null)
    {
        var uuids = settings.Children.Select(child => child.Uuid).ToList();
        if (uuids.Count == 0)
            return uuids!;

        try
        {
            //$"Request3DSceneRefresh {uuids.Count}".WriteInfo();
            var functionName = ResolveFunction("request3DSceneDelete");
            var json = JsonSerializer.Serialize((object)settings, JSONOptions);
            //WriteToFolder("Data", "Scene3D_Request3SceneDelete.json", json); 

            await JsRuntime!.InvokeVoidAsync(functionName, (object)json);
            if (onComplete != null)
            {
                //$"Request3DSceneDelete onComplete {uuids.Count}".WriteInfo();
                onComplete(uuids!);  // Now we can await the async callback
            }
        }
        catch (System.Exception ex)
        {  
           $"Request3DSceneDelete: {ex.Message}".WriteError();
        }

        return uuids!;
    }


    private JsonSerializerOptions JSONOptions { get; set; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        IncludeFields = true,
        IgnoreReadOnlyFields = true
    };

    public async Task<string> Request3DModel(Model3D model, Func<string,Task>? onComplete = null)
    {

        //$"Request3DModel {model.Name} {model.Url}".WriteInfo();
        var uuid = model.Uuid!;
        if (ImportPromises.ContainsKey(uuid))
        {
            $"Request3DModel waiting on {uuid} to load {model.Url}".WriteInfo();
            return uuid;
        }

        var fileURL = model.Url ?? "";   
        if (string.IsNullOrEmpty(fileURL))
        {
            $"Request3DModel: FileURL is empty".WriteError();
            return uuid;
        }

        //because we expect an async process in loading a GBL file, we need to wait for the completion
        if ( onComplete != null)
            ImportPromises.Add(uuid, onComplete);

        try
        {
            var spec = new ImportSettings();
            spec.AddRequestedModel(model);

            var functionName = ResolveFunction("request3DModel");
            var json = JsonSerializer.Serialize((object)spec, JSONOptions);
            //WriteToFolder("Data", "Scene3D_Request3DModel.json", json); 
            //$"Request3DModel calling {functionName} with {json}".WriteInfo();

            await JsRuntime!.InvokeVoidAsync(functionName, (object)json);  
            //$"Request3DModel:Scene3D {functionName} {settings.FileURL} {uuid}".WriteInfo();
        }
        catch (System.Exception ex)
        {  
           $"Request3DModel: {ex.Message}".WriteError();
        }

        return uuid;
    }

    


    public async Task<string> Request3DGeometry(ImportSettings settings, Func<string,Task>? onComplete = null)
    {
        var uuid = settings.Uuid!;

        try
        {
            var functionName = ResolveFunction("request3DGeometry");
            var json = JsonSerializer.Serialize((object)settings, JSONOptions);
            //WriteToFolder("Data", "Scene3D_Request3Geometry.json", json); 
            //$"request3DGeometry calling {functionName} with {json}".WriteInfo();

            await JsRuntime!.InvokeVoidAsync(functionName, (object)json);
            if (onComplete != null)
            {
                await onComplete(uuid);  // Now we can await the async callback
            }
        }
        catch (System.Exception ex)
        {  
           $"Request3DGeometry: {ex.Message}".WriteError();
        }

        return uuid;
    }
    public async Task<string> Request3DLabel(ImportSettings settings, Func<string,Task>? onComplete = null)
    {
        var uuid = settings.Uuid!;

        try
        {
            var functionName = ResolveFunction("request3DLabel");
            var json = JsonSerializer.Serialize((object)settings, JSONOptions);
            //WriteToFolder("Data", "Scene3D_Request3DLabel.json", json); 
            //$"Request3DLabel calling {functionName} with {json}".WriteInfo();

            await JsRuntime!.InvokeVoidAsync(functionName, (object)json);
            if (onComplete != null)
            {
                await onComplete(uuid);  // Now we can await the async callback
            }

        }
        catch (System.Exception ex)
        {  
           $"Request3DLabel: {ex.Message}".WriteError();
        }

        return uuid;
    }

    public string WriteToFolder(string folder, string filename, string result)
    {
        try
        {
            FileHelpers.WriteData(folder, filename, result);
            return result;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }


    public override (bool success, Object3D result) AddChild(Object3D child)
    {
        if (child == null)
            return (false, null!);

        child.OnDelete = (Object3D item) =>
        {
            item.SetShouldDelete(true);
        };
        return base.AddChild(child);
    }

    public override (bool success, Object3D result) RemoveChild(Object3D child)
    {
        if (child == null) 
            return (false, child!);

        child.Delete();  //mark this as dirty so that value ill also smash
        return base.RemoveChild(child);
    }




    //public async Task MoveObject(Object3D object3D)
    //{
    //    var functionName = ResolveFunction("moveObject");
    //    await JsRuntime!.InvokeAsync<bool>(functionName, object3D);
    //}


    [JSInvokable]
    public static void LoadedObjectComplete(string uuid)
    {
        //$"CALLBACK LoadedObjectComplete  {uuid}".WriteWarning();

        if ( ImportPromises.TryGetValue(uuid, out Func<string,Task>? promise))
        {
            ImportPromises.Remove(uuid);
            if (promise != null)
            {
                promise.Invoke(uuid);
            }
        }
    }
}

