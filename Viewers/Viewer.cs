// Decompiled with JetBrains decompiler
// Type: Blazor3D.Viewers.Viewer
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Cameras;
using BlazorThreeJS.ComponentHelpers;
using BlazorThreeJS.Controls;
using BlazorThreeJS.Core;
using BlazorThreeJS.Events;

using BlazorThreeJS.Lights;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Menus;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Scenes;
using BlazorThreeJS.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace BlazorThreeJS.Viewers
{
    public sealed class Viewer : ComponentBase, IDisposable
    {
        [Inject] private IJSRuntime JSBridge { get; set; }
        // private IJSObjectReference bundleModule;

        private static event Viewer.SelectedObjectStaticEventHandler ObjectSelectedStatic;

        private static event Viewer.LoadedObjectStaticEventHandler ObjectLoadedStatic;

        private static Dictionary<Guid, ImportSettings> ImportPromises { get; set; } = new();
        private static Dictionary<Guid, Button> Buttons { get; set; } = new();
        private static Dictionary<Guid, ImportSettings> LoadedModels { get; set; } = new();

        private string JSRootPath = "./_content/BlazorThreeJS/dist";
        private string JSRootPathDevelopment = "/dist";

        private event LoadedObjectEventHandler ObjectLoadedPrivate;

        public event SelectedObjectEventHandler ObjectSelected;

        public event LoadedObjectEventHandler ObjectLoaded;

        public event LoadedModuleEventHandler JsModuleLoaded;

        [Parameter]
        public ViewerSettings ViewerSettings { get; set; }

        [Parameter]
        public Scene Scene { get; set; }

        [Parameter]
        public bool UseDefaultScene { get; set; }

        [Parameter]
        public Camera Camera { get; set; }

        public OrbitControls OrbitControls { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Viewer viewer = this;
            if (!firstRender)
                return;
            Viewer.ObjectSelectedStatic += new Viewer.SelectedObjectStaticEventHandler(viewer.OnObjectSelectedStatic);
            Viewer.ObjectLoadedStatic += new Viewer.LoadedObjectStaticEventHandler(viewer.OnObjectLoadedStatic);
            viewer.ObjectLoadedPrivate += new LoadedObjectEventHandler(viewer.OnObjectLoadedPrivate);

            LoadedModels.Clear();

            // NOTE: change JSRootPath to use the _content when building for use in other apps
            await viewer.JSBridge.InvokeAsync<IJSObjectReference>("import", (object)$"{JSRootPath}/app-lib.js").AsTask();
            // await viewer.JSBridge.InvokeAsync<IJSObjectReference>("import", (object)$"{JSRootPathDevelopment}/app-lib.js").AsTask();

            if (viewer.UseDefaultScene && !viewer.Scene.Children.Any<Object3D>())
                viewer.AddDefaultScene();
            string str = JsonConvert.SerializeObject((object)new
            {
                Scene = viewer.Scene,
                ViewerSettings = viewer.ViewerSettings,
                Camera = viewer.Camera,
                OrbitControls = viewer.OrbitControls
            }, SerializationHelper.GetSerializerSettings());

            await JSBridge.InvokeVoidAsync("BlazorThreeJS.loadViewer", (object)str);
            await UpdateScene();
            await viewer.OnModuleLoaded();
        }

        private void PopulateButtonsDict()
        {
            Viewer.Buttons.Clear();
            var menus = this.Scene.Children.FindAll((item) => item.Type == "Menu");
            foreach (var menu in menus)
            {
                foreach (var button in ((PanelMenu)menu).Buttons)
                {
                    Console.WriteLine($"From FoundryBlazor Button UUID={button.Uuid}");
                    if (!Viewer.Buttons.ContainsKey(button.Uuid)) Viewer.Buttons.Add(button.Uuid, button);
                }
            }
            Console.WriteLine($"menus Count ={menus.Count}");
            Console.WriteLine($"Viewer.Buttons Count ={Viewer.Buttons.Count}");
        }

        public async Task UpdateScene()
        {
            PopulateButtonsDict();

            await JSBridge.InvokeVoidAsync("BlazorThreeJS.updateScene", (object)JsonConvert.SerializeObject((object)this.Scene, SerializationHelper.GetSerializerSettings()));
        }

        public async Task SetCameraPositionAsync(Vector3 position, Vector3? lookAt = null) => await JSBridge.InvokeVoidAsync("BlazorThreeJS.setCameraPosition", (object)position, lookAt);

        public async Task UpdateCamera(Camera camera)
        {
            this.Camera = camera;
            await JSBridge.InvokeVoidAsync("BlazorThreeJS.updateCamera", (object)JsonConvert.SerializeObject((object)this.Camera, SerializationHelper.GetSerializerSettings()));
        }

        public async Task ShowCurrentCameraInfo() => await JSBridge.InvokeVoidAsync("BlazorThreeJS.showCurrentCameraInfo");

        [JSInvokable]
        public static void ReceiveSelectedObjectUUID(string uuid, Vector3 size)
        {
            Guid guid = string.IsNullOrWhiteSpace(uuid) ? Guid.Empty : Guid.Parse(uuid);
            Console.WriteLine($"ReceiveSelectedObjectUUID size={size.X}, {size.Y}, {size.Z}");

            try
            {
                var item = LoadedModels[guid];
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

        public async Task RemoveByUuidAsync(Guid uuid)
        {
            if (!await JSBridge.InvokeAsync<bool>("BlazorThreeJS.deleteByUuid", (object)uuid))
                return;
            ChildrenHelper.RemoveObjectByUuid(uuid, this.Scene.Children);
        }

        public async Task MoveObject(Object3D object3D)
        {
            if (!await JSBridge.InvokeAsync<bool>("BlazorThreeJS.moveObject", object3D))
                return;
        }

        public async Task ClearSceneAsync()
        {
            await JSBridge.InvokeVoidAsync("BlazorThreeJS.clearScene");
            this.Scene.Children.RemoveAll(item => item.Type.Contains("LabelText") || item.Type.Contains("Mesh"));
        }

        public async Task ClearLightsAsync()
        {
            await JSBridge.InvokeVoidAsync("BlazorThreeJS.clearScene");
            this.Scene.Children.Clear();
        }

        public async Task<Guid> Request3DModel(ImportSettings settings)
        {
            Guid uuid = settings.Uuid;
            settings.Scene = Scene;
            ImportPromises.Add(uuid, settings);

            Console.WriteLine($"Adding settings={uuid}");
            LoadedModels.Add(uuid, settings);

            // settings.Material = settings.Material ?? new MeshStandardMaterial();
            await JSBridge.InvokeVoidAsync("BlazorThreeJS.import3DModel", (object)JsonConvert.SerializeObject((object)settings, SerializationHelper.GetSerializerSettings()));

            return uuid;
        }

        public async Task<Guid> Clone3DModel(Guid sourceGuid, List<ImportSettings> settings)
        {
            settings.ForEach((setting) =>
            {
                setting.Scene = Scene;
                ImportPromises.Add(setting.Uuid, setting);
            });


            var serializedSettings = (object)JsonConvert.SerializeObject((object)settings, SerializationHelper.GetSerializerSettings());
            var args = new List<object>() { $"{sourceGuid}", serializedSettings };
            await JSBridge.InvokeVoidAsync("BlazorThreeJS.clone3DModel", args.ToArray());
            return sourceGuid;
        }

        [JSInvokable]
        public static Task ReceiveLoadedObjectUUID(string containerId, string uuid)
        {
            var guid = Guid.Parse(uuid);

            if (Viewer.ImportPromises.ContainsKey(guid))
            {
                var settings = Viewer.ImportPromises[guid];
                Group group = new()
                {
                    Name = settings.Uuid.ToString(),
                    Uuid = settings.Uuid,
                };
                settings.Scene!.Children.Add(group);

                settings.OnComplete.Invoke(settings.Scene!, group);
                settings.OnComplete = (Scene s, Object3D o) => { };
                Viewer.ImportPromises.Remove(guid);
            }
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task OnClickButton(string containerId, string uuid)
        {
            var guid = Guid.Parse(uuid);

            Console.WriteLine($"OnClickButton containerId, uuid={containerId}, {uuid}");
            Console.WriteLine($"After OnClickButton, Viewer.Buttons ContainsKey ={Viewer.Buttons.ContainsKey(guid)}");

            if (Viewer.Buttons.ContainsKey(guid))
            {
                var button = Viewer.Buttons[guid];
                var parms = new List<String>();
                button.OnClick?.Invoke(button);
            }
            return Task.CompletedTask;
        }

        public static Object3D? GetObjectByUuid(Guid uuid, List<Object3D> children) => ChildrenHelper.GetObjectByUuid(uuid, children);

        private async Task OnModuleLoaded()
        {
            LoadedModuleEventHandler jsModuleLoaded = this.JsModuleLoaded;
            if (jsModuleLoaded == null)
                return;

            Delegate[] invocationList = jsModuleLoaded.GetInvocationList();
            Task[] taskArray = new Task[invocationList.Length];
            for (int index = 0; index < invocationList.Length; ++index)
                taskArray[index] = ((LoadedModuleEventHandler)invocationList[index])();
            await Task.WhenAll(taskArray);
        }

        private void AddDefaultScene()
        {
            this.Scene.Add((Object3D)new AmbientLight());
            Scene scene = this.Scene;
            PointLight child = new PointLight();
            child.Position = new Vector3()
            {
                X = 1f,
                Y = 3f,
                Z = 0.0f
            };
            scene.Add((Object3D)child);
            this.Scene.Add((Object3D)new Mesh());
        }

        private void OnObjectSelectedStatic(Object3DStaticArgs e)
        {
            if (!(this.ViewerSettings.ContainerId == e.ContainerId))
                return;
            SelectedObjectEventHandler objectSelected = this.ObjectSelected;
            if (objectSelected == null)
                return;
            objectSelected(new Object3DArgs() { UUID = e.UUID });
        }

        private void OnObjectLoadedStatic(Object3DStaticArgs e)
        {
            if (!(this.ViewerSettings.ContainerId == e.ContainerId))
                return;
            LoadedObjectEventHandler objectLoadedPrivate = this.ObjectLoadedPrivate;
            if (objectLoadedPrivate != null)
            {
                Task task = objectLoadedPrivate(new Object3DArgs()
                {
                    UUID = e.UUID
                });
            }
            LoadedObjectEventHandler objectLoaded = this.ObjectLoaded;
            if (objectLoaded != null)
            {
                Task task = objectLoaded(new Object3DArgs()
                {
                    UUID = e.UUID
                });
            }
        }

        private List<Object3D> ParseChildren(JToken? children)
        {
            var children1 = new List<Object3D>();
            if ((children != null ? (children.Type != JTokenType.Array ? 1 : 0) : 1) != 0)
                return children1;

            foreach (var child in (IEnumerable<JToken>)children)
            {
                if (child is JObject jobject)
                {
                    string str1 = jobject.Property("type")?.Value.ToString();
                    string str2 = jobject.Property("name")?.Value.ToString() ?? string.Empty;
                    string input = jobject.Property("uuid")?.Value.ToString() ?? string.Empty;

                    if (str1 == "Mesh")
                    {
                        Mesh mesh1 = new()
                        {
                            Name = str2,
                            Uuid = Guid.Parse(input)
                        };
                        Mesh mesh2 = mesh1;
                        children1.Add((Object3D)mesh2);
                    }
                    if (str1 == "Group")
                    {
                        List<Object3D> children2 = this.ParseChildren(jobject.Property(nameof(children))?.Value);
                        Group group = new()
                        {
                            Name = str2,
                            Uuid = Guid.Parse(input)
                        };

                        group.Children.AddRange((IEnumerable<Object3D>)children2);
                    }
                }
            }
            return children1;
        }

        private async Task OnObjectLoadedPrivate(Object3DArgs e)
        {
            string json = await JSBridge.InvokeAsync<string>("BlazorThreeJS.getSceneItemByGuid", (object)e.UUID);
            if (json.Contains("\"type\":\"Group\""))
            {
                JObject jobject = JObject.Parse(json);
                string str = jobject.Property("name")?.Value.ToString() ?? string.Empty;
                string input = jobject.Property("uuid")?.Value.ToString() ?? string.Empty;
                List<Object3D> children = this.ParseChildren(jobject.Property("children")?.Value);
                Group group1 = new()
                {
                    Name = str,
                    Uuid = Guid.Parse(input)
                };
                Group group2 = group1;
                group2.Children.AddRange((IEnumerable<Object3D>)children);
                this.Scene.Children.Add((Object3D)group2);
                LoadedObjectEventHandler objectLoaded = this.ObjectLoaded;
                if (objectLoaded != null)
                {
                    Task task = objectLoaded(new Object3DArgs()
                    {
                        UUID = e.UUID
                    });
                }
            }
            if (!json.Contains("\"type\":\"Mesh\""))
                return;
            Mesh mesh = JsonConvert.DeserializeObject<Mesh>(json);
            if (mesh == null)
                return;
            this.Scene.Children.Add((Object3D)mesh);
            LoadedObjectEventHandler objectLoaded1 = this.ObjectLoaded;
            if (objectLoaded1 == null)
                return;
            Task task1 = objectLoaded1(new Object3DArgs()
            {
                UUID = e.UUID
            });
        }

        public void Dispose()
        {
            Viewer.ObjectSelectedStatic -= new Viewer.SelectedObjectStaticEventHandler(this.OnObjectSelectedStatic);
            Viewer.ObjectLoadedStatic -= new Viewer.LoadedObjectStaticEventHandler(this.OnObjectLoadedStatic);
            this.ObjectLoadedPrivate -= new LoadedObjectEventHandler(this.OnObjectLoadedPrivate);
        }

        protected override void BuildRenderTree(

        RenderTreeBuilder __builder)
        {
            __builder.OpenElement(0, "div");
            __builder.AddAttribute(1, "class", "viewer3dContainer");
            __builder.AddAttribute(2, "id", this.ViewerSettings.ContainerId);
            __builder.AddAttribute(3, "b-h6holr0slw");
            __builder.CloseElement();
        }

        public Viewer()
        {
            // OrthographicCamera camera = new();
            PerspectiveCamera camera = new()
            {
                Position = new Vector3(3f, 3f, 3f)
            };
            // PerspectiveCamera perspectiveCamera = new PerspectiveCamera();
            // perspectiveCamera.Position = new Vector3(3f, 3f, 3f);
            // ISSUE: reference to a compiler-generated field
            this.Camera = (Camera)camera;
            // ISSUE: reference to a compiler-generated field
            this.OrbitControls = new OrbitControls();
            // ISSUE: explicit constructor call
            // base.\u002Ector();
        }

        private delegate void SelectedObjectStaticEventHandler(Object3DStaticArgs e);
        private delegate void LoadedObjectStaticEventHandler(Object3DStaticArgs e);
    }
}
