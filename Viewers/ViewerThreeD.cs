// Decompiled with JetBrains decompiler
// Type: Blazor3D.Viewers.Viewer
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BlazorThreeJS.Cameras;
using BlazorThreeJS.ComponentHelpers;
using BlazorThreeJS.Controls;
using BlazorThreeJS.Core;
using BlazorThreeJS.Events;

using BlazorThreeJS.Lights;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Menus;
using BlazorThreeJS.Objects;

using BlazorThreeJS.Settings;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Units;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace BlazorThreeJS.Viewers
{

    public class ViewerThreeD : ComponentBase, IDisposable
    {
        [Inject] private IJSRuntime? JsRuntime { get; set; }


        [Parameter,EditorRequired] public string SceneName { get; set; } = "Viewer3D";

        private JsonSerializerOptions JSONOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IncludeFields = true,
            IgnoreReadOnlyFields = true
        };

        private static Dictionary<string, Button> Buttons { get; set; } = new();
        private static Dictionary<string, ImportSettings> ImportPromises { get; set; } = new();
        private static Dictionary<string, ImportSettings> LoadedModels { get; set; } = new();

        // private event LoadedObjectEventHandler? ObjectLoadedPrivate;
        // public event LoadedObjectEventHandler? ObjectLoaded;
        // public event LoadedModuleEventHandler? JsModuleLoaded;

        //public event SelectedObjectEventHandler? ObjectSelected;
        //private delegate void SelectedObjectStaticEventHandler(Object3DStaticArgs e);
        //private delegate void LoadedObjectStaticEventHandler(Object3DStaticArgs e);
        //private static event ViewerThreeDSelectedObjectStaticEventHandler ObjectSelectedStatic;
        //private static event ViewerThreeDLoadedObjectStaticEventHandler ObjectLoadedStatic;

        private ViewerSettings _viewingSettings = null!;
        private Scene _activeScene = null!;

        [Parameter]
        public ViewerSettings ViewerSettings
        { 
            get => ComputeViewingSettings();
            set => _viewingSettings = value;
        }

        [Parameter]
        public Scene ActiveScene 
        { 
            get => ComputeActiveScene();
            set => _activeScene = value;
        }


        [Parameter] public int CanvasWidth { get; set; } = 1000;
        [Parameter] public int CanvasHeight { get; set; } = 800;

        public OrbitControls OrbitControls { get; set; } = new OrbitControls();



        private ViewerSettings ComputeViewingSettings()
        {
            if (_viewingSettings != null)
                return _viewingSettings;
            
            _viewingSettings = new ViewerSettings()
            {
                containerId = ComputeActiveScene().Title,
                CanSelect = true,  // default is false
                SelectedColor = "black",
                Width = CanvasWidth,
                Height = CanvasHeight,
                WebGLRendererSettings = new WebGLRendererSettings()
                {
                    Antialias = false
                }
            };

 
            return _viewingSettings;
        }

        private Scene ComputeActiveScene()
        {
            if (_activeScene != null)
                return _activeScene;

            var (success, scene) = Scene.EstablishScene(SceneName, JsRuntime!);

            _activeScene = scene;
            if ( success )
            {
                var ambient = new AmbientLight()
                { 
                    Name = "Ambient Light",
                    Uuid = Guid.NewGuid().ToString(),
                };
                _activeScene.AddChild(ambient);

                var point = new PointLight() 
                { 
                    Name = "Point Light" ,
                    Uuid = Guid.NewGuid().ToString(),
                    Position = new Vector3()
                    {
                        X = 1f,
                        Y = 3f,
                        Z = 0.0f
                    }
                };
                _activeScene.AddChild(point);
            }

            return _activeScene;
        }

        public string Resolve(string jsNamespace, string functionName)
        {
            //return $"{jsNamespace}.{functionName}";
            return $"BlazorThreeJS.{functionName}";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (!firstRender)
                return;

            //ViewerThreeD.ObjectSelectedStatic += new ViewerThreeD.SelectedObjectStaticEventHandler(ViewerThreeD.OnObjectSelectedStatic);
            //ViewerThreeD.ObjectLoadedStatic += new ViewerThreeD.LoadedObjectStaticEventHandler(ViewerThreeD.OnObjectLoadedStatic);
            //ObjectLoadedPrivate += new LoadedObjectEventHandler(OnObjectLoadedPrivate);

            LoadedModels.Clear();

            //await JsRuntime!.InvokeVoidAsync("import", DotNetObjectReference.Create(this));


            ViewerSettings.containerId = ActiveScene.Title;
            var dto = new SceneDTO()
            {
                Scene = ActiveScene,
                ViewerSettings = ViewerSettings,
                Camera = ActiveScene.Camera,
                OrbitControls = OrbitControls
            };

            var jsNameSpace = ActiveScene.Title;
            //await JsRuntime!.InvokeVoidAsync("ViewManager.establishViewer3D", (object)jsNameSpace);

            var functionName = Resolve(jsNameSpace, "loadViewer");
            $"calling {functionName}".WriteInfo();

            string str = JsonSerializer.Serialize<SceneDTO>(dto, JSONOptions);
            await JsRuntime!.InvokeVoidAsync(functionName, (object)str);
                        
            await ActiveScene.UpdateScene();
            //SRS  I bet we never load a module  so do not do this!!
            //await ViewerThreeD.OnModuleLoaded();
        }

        private void PopulateButtonsDict()
        {
            ViewerThreeD.Buttons.Clear();
            var menus = ActiveScene.GetAllChildren().FindAll((item) => item.Type == "Menu");

            foreach (var menu in menus)
            {
                foreach (var button in ((PanelMenu)menu).Buttons)
                {
                    var uuid = button.Uuid!;
                    $"From FoundryBlazor Button UUID={uuid}".WriteInfo();
                    if (!ViewerThreeD.Buttons.ContainsKey(uuid)) 
                        ViewerThreeD.Buttons.Add(uuid, button);
                }
            }
            //Console.WriteLine($"PopulateButtonsDict menus Count ={menus.Count}");
            //Console.WriteLine($"ViewerThreeD.Buttons Count ={ViewerThreeD.Buttons.Count}");
        }





        [JSInvokable]
        public static void ReceiveSelectedObjectUUID(string uuid, Vector3 size)
        {

            Console.WriteLine($"ReceiveSelectedObjectUUID size={size.X}, {size.Y}, {size.Z}");

            try
            {
                var item = LoadedModels[uuid];
                Console.WriteLine($"item={item}");
                if (item != null)
                {
                    item.ComputedSize = size;
                    item.OnClick.Invoke(item);
                }
                else
                {
                    Console.WriteLine($"uuid={uuid} not found in LoadedModels");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"uuid={uuid} problem (could be a problem in onClick callback). Message={ex.Message}");
            }

        }








        [JSInvokable]
        public static Task OnClickButton(string containerId, string uuid)
        {

            Console.WriteLine($"OnClickButton containerId, uuid={containerId}, {uuid}");
            Console.WriteLine($"After OnClickButton, ViewerThreeD.Buttons ContainsKey ={ViewerThreeD.Buttons.ContainsKey(uuid)}");

            if (ViewerThreeD.Buttons.ContainsKey(uuid))
            {
                var button = ViewerThreeD.Buttons[uuid];
                var parms = new List<String>();
                button.OnClick?.Invoke(button);
            }
            return Task.CompletedTask;
        }

        public static Object3D? GetObjectByUuid(string uuid, List<Object3D> children) 
        {
            return ChildrenHelper.GetObjectByUuid(uuid, children);
        }




        // private void OnObjectSelectedStatic(Object3DStaticArgs e)
        // {
        //     if (!(this.ViewerSettings.containerId == e.ContainerId))
        //         return;


        //     ObjectLoaded?.Invoke(new Object3DArgs() { UUID = e.UUID });
        // }

        // private void OnObjectLoadedStatic(Object3DStaticArgs e)
        // {
        //     if (!(this.ViewerSettings.containerId == e.ContainerId))
        //         return;

        //     this.ObjectLoadedPrivate?.Invoke(new Object3DArgs()
        //     {
        //         UUID = e.UUID
        //     });


        //     this.ObjectLoaded?.Invoke(new Object3DArgs()
        //     {
        //         UUID = e.UUID
        //     });

        // }

        // private List<Object3D> ParseChildren(JsonArray? source)
        // {
        //     var children = new List<Object3D>();
        //     if (source == null)
        //         return children;

        //     foreach (var child in source)
        //     {
        //         if (child is JsonNode jobject)
        //         {
        //             var name = jobject["name"]?.GetValue<string>() ?? string.Empty;
        //             var uuid = jobject["uuid"]?.GetValue<string>() ?? string.Empty;
        //             var type = jobject["type"]?.GetValue<string>() ?? string.Empty;
        //             var children1 = this.ParseChildren(jobject["children"]?.AsArray());

        //             if (type == "Mesh")
        //             {
        //                 var mesh = new Mesh()
        //                 {
        //                     Name = name,
        //                     Uuid = Guid.Parse(type)
        //                 };
        //                 children1.Add((Object3D)mesh);
        //             }
        //             if (type == "Group")
        //             {
        //                 var group = new Group3D()
        //                 {
        //                     Name = name,
        //                     Uuid = Guid.Parse(type)
        //                 };

        //                 group.AddRange(children1);
        //             }
        //         }
        //     }
        //     return children;
        // }

        // private async Task OnObjectLoadedPrivate(Object3DArgs e)
        // {
        //     var functionName = Resolve("getSceneItemByGuid");
        //     var options = UnitSpec.JsonHydrateOptions(false);
        //     string json = await JsRuntime!.InvokeAsync<string>(functionName, (object)e.UUID);


        //     var jobject = JsonNode.Parse(json);
        //     if (jobject == null)
        //         return;

        //     var name = jobject["name"]?.GetValue<string>() ?? string.Empty;
        //     var uuid = jobject["uuid"]?.GetValue<string>() ?? string.Empty;
        //     var type = jobject["type"]?.GetValue<string>() ?? string.Empty;
        //     var children = this.ParseChildren(jobject["children"]?.AsArray());

        //     if (type.Matches("Group"))
        //     {
        //         var group = new Group3D()
        //         {
        //             Name = name,
        //             Uuid = Guid.Parse(uuid),
        //         };

        //         group.AddRange(children);
        //         ActiveScene.Add(group);
        //     }

        //     if (type.Matches("Mesh"))
        //     {

        //         var mesh = new Mesh()
        //         {
        //             Name = name,
        //             Uuid = Guid.Parse(uuid),
        //         };

        //         mesh.AddRange(children);
        //         ActiveScene.Add(mesh);
        //     }

        //     this.ObjectLoaded?.Invoke(new Object3DArgs()
        //     {
        //         UUID = e.UUID
        //     });
        // }

        public void Dispose()
        {
            //ViewerThreeDObjectSelectedStatic -= new ViewerThreeDSelectedObjectStaticEventHandler(this.OnObjectSelectedStatic);
            //ViewerThreeDObjectLoadedStatic -= new ViewerThreeDLoadedObjectStaticEventHandler(this.OnObjectLoadedStatic);
            //this.ObjectLoadedPrivate -= new LoadedObjectEventHandler(this.OnObjectLoadedPrivate);
        }

        protected override void BuildRenderTree(RenderTreeBuilder __builder)
        {
            __builder.OpenElement(0, "div");
            __builder.AddAttribute(1, "class", "viewer3dContainer");
            __builder.AddAttribute(2, "id", this.SceneName);
           // __builder.AddAttribute(3, "b-h6holr0slw");
            __builder.CloseElement();
        }



    }
}
