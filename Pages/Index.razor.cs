using Microsoft.AspNetCore.Components;
using BlazorThreeJS.Core;
using BlazorThreeJS.Viewers;
using BlazorThreeJS.Settings;
using BlazorThreeJS.Scenes;
using BlazorThreeJS.Lights;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Events;
using BlazorThreeJS.Labels;
using BlazorThreeJS.Enums;
using BlazorThreeJS.Menus;

namespace BlazorThreeJS.Pages;

public class IndexPage : ComponentBase, IDisposable
{
    public Viewer View3D1 = null!;
    public Guid objGuid;
    public string Msg = string.Empty;

    public Object3D? SelectedObject { get; set; }
    public LabelText? TestText { get; private set; }
    public PanelMenu MenuBtn1 { get; set; } = new();
    public TextPanel TextPanel1 { get; set; } = new();
    public TextPanel TextPanel2 { get; set; } = new();
    public PanelGroup PanelGroup1 { get; set; } = new();

    public ViewerSettings settings = new ViewerSettings()
    {
        ContainerId = "example1",
        CanSelect = true,// default is false
        SelectedColor = "#808080",
        WebGLRendererSettings = new WebGLRendererSettings
        {
            Antialias = false // if you need poor quality for some reasons
        }
    };

    public Scene scene = new Scene();

    public void Dispose()
    {
        // View3D1.ObjectSelected -= OnObjectSelected;
        // View3D1.ObjectLoaded -= OnObjectLoaded;
        View3D1.JsModuleLoaded -= OnJsModuleLoaded;
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // subscribe events only once
            // View3D1.ObjectSelected += OnObjectSelected;
            // View3D1.ObjectLoaded += OnObjectLoaded;
            View3D1.JsModuleLoaded += OnJsModuleLoaded;

        }

        return base.OnAfterRenderAsync(firstRender);
    }

    public async Task OnJsModuleLoaded()
    {

        await View3D1.SetCameraPositionAsync(new Vector3(0, 3, 6), new Vector3(0, 0, 0));
        // await View3D1.UpdateScene();

        // await View3D1.Request3DModel(model1);
        // await RenderLegoMan();

        var model2Pos = new Vector3(2.5, 3, 0);
        var model2Guid = Guid.NewGuid();
        // var model2Guid = new Guid("66ca8b5b-970e-448c-b695-ee4c3fdfe65f");
        Console.WriteLine($"model2Guid={model2Guid}");
        var model2 = new ImportSettings
        {
            Uuid = model2Guid,
            Format = Import3DFormats.Gltf,
            // FileURL = "/assets/legoMan/pr_Body.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/MK48_88480001_Pixyz_RnD.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/Tail_ANIM_v017.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/BoxAnimated.glb",
            FileURL = "https://localhost:5001/storage/StaticFiles/pr_Leg_Left.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/8847996_STEERING_ASSY.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/pr_2505431__PISTON_RING_1_1185_278260.glb",
            Position = model2Pos,
            OnClick = (ImportSettings self) =>
            {
                self.Increment();
                Console.WriteLine($"OnClick model2 fileURL, ClickCount={self.FileURL}, {self.ClickCount}, size={self.ComputedSize?.X},{self.ComputedSize?.Y},{self.ComputedSize?.Z}");

                if (self.IsShow())
                {
                    Console.WriteLine("Add TextPanel");
                    TextPanel2 = BuildTextPanel();
                    scene.Add(TextPanel2);
                }
                else
                {
                    Console.WriteLine("Remove TextPanel");
                    scene.Remove(TextPanel2);
                }

                Task.Run(async () =>
                {
                    await View3D1.UpdateScene();
                });
            }
            // OnComplete = (Scene scene, Object3D object3D) =>
            // {
            //     if (object3D != null)
            //     {
            //         Console.WriteLine($"OnComplete callback model1 object3d.Uuid={object3D.Uuid}");
            //         Msg = $"Loaded 3D Model with id = {object3D.Uuid}";
            //         StateHasChanged();

            //         object3D.Position.Loc(model2Pos.X, model2Pos.Y, model2Pos.Z);

            //         // await View3D1.UpdateScene();
            //     }
            //     else
            //     {
            //         Console.WriteLine($"object3D is null");
            //     }
            // }
        };

        await View3D1.Request3DModel(model2);

        // var settings = new List<ImportSettings>() { model1, model2 };

        var piv = new Vector3(-2, 0, 0);
        var pos = new Vector3(0, 0, 0);
        var rot = new Euler(0, Math.PI * 45 / 180, 0);
        // var rot = new Euler(0, 0, 0);

        var sourceModel = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            // FileURL = "https://localhost:5001/storage/StaticFiles/8847996_STEERING_ASSY.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/pr_2505431__PISTON_RING_1_1185_278260.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/barrel.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/pr_H64_wheel_H187_0048_inst_0.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/block_large.glb",
            FileURL = "https://localhost:5001/storage/StaticFiles/block_small.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/MK48_273694.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/MK48_88480001_Pixyz_RnD.glb",
            Position = pos,
            Rotation = rot,
            Pivot = piv,
            OnComplete = (Scene scene, Object3D object3D) =>
            {
                if (object3D != null)
                {
                    Console.WriteLine($"OnComplete callback pivoted sourceModel object3d.Uuid={object3D.Uuid}");
                    Msg = $"Loaded 3D Model with id = {object3D.Uuid}";

                    // var primaryGuid = $"{object3D.Uuid}";

                    Task.Run(async () =>
                    {
                        await RenderClones(object3D.Uuid);
                    });

                    // View3D1.Clone3DModel(object3D.Uuid);
                    // View3D1.Clone3DModel();
                    StateHasChanged();

                }
                else
                {
                    Console.WriteLine($"object3D is null");
                }
            }
        };

        await View3D1.Request3DModel(sourceModel);



        var model3Pos = new Vector3(0, 0, -2);
        var model3 = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            // FileURL = "https://localhost:5001/storage/StaticFiles/jet.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/block_large.glb",
            FileURL = "https://localhost:5001/storage/StaticFiles/block_small.glb",

            // FileURL = "https://localhost:5001/storage/StaticFiles/pr_H64_wheel_H187_0048_inst_0.glb",
            Position = model3Pos,
            OnComplete = (Scene scene, Object3D object3D) =>
            {
                if (object3D != null)
                {
                    Console.WriteLine($"OnComplete callback model3 object3d.Uuid={object3D.Uuid}");
                    Msg = $"Loaded 3D Model with id = {object3D.Uuid}";
                    StateHasChanged();

                    object3D.Position.Set(model3Pos.X, model3Pos.Y, model3Pos.Z);

                    // await View3D1.UpdateScene();
                }
                else
                {
                    Console.WriteLine($"object3D is null");
                }
            }
        };

        await View3D1.Request3DModel(model3);

    }

    private async Task RenderClones(Guid sourceGuid)
    {
        var m1 = RenderModel1(sourceGuid);
        var m2 = RenderModel2(sourceGuid);

        var settings = new List<ImportSettings>() { m1, m2 };
        // var settings = new List<ImportSettings>() { m1 };
        await View3D1.Clone3DModel(sourceGuid, settings);
    }

    private async Task<ImportSettings> RenderLegoMan()
    {
        Console.WriteLine("in RenderLegoMan");
        var bodyPos = new Vector3(2.5, 3, 0);
        var body = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            FileURL = "/assets/legoMan/pr_Body.glb",
            Position = bodyPos,
        };

        var headPos = new Vector3(bodyPos.X, bodyPos.Y + 0.015, bodyPos.Z);
        var head = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            FileURL = "/assets/legoMan/pr_Head.glb",
            Position = headPos,
        };

        var armLeftPos = new Vector3(bodyPos.X + 0.01, bodyPos.Y + 0.0125, bodyPos.Z);
        var armLeft = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            FileURL = "/assets/legoMan/pr_Arm_Left.glb",
            Position = armLeftPos,
        };

        var armRightPos = new Vector3(bodyPos.X - 0.01, bodyPos.Y + 0.0125, bodyPos.Z);
        var armRight = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            FileURL = "/assets/legoMan/pr_Arm_Right.glb",
            Position = armRightPos,
        };

        var pelvisPos = new Vector3(bodyPos.X, bodyPos.Y - 0.001, bodyPos.Z);
        var pelvis = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            FileURL = "/assets/legoMan/pr_Pelvis.glb",
            Position = pelvisPos,
        };

        var legLeftPos = new Vector3(bodyPos.X - 0.01, bodyPos.Y - 0.015, bodyPos.Z);
        var legLeft = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            FileURL = "/assets/legoMan/pr_Leg_Left.glb",
            Position = legLeftPos,
        };

        var legRightPos = new Vector3(bodyPos.X + 0.01, bodyPos.Y - 0.015, bodyPos.Z);
        var legRight = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            FileURL = "/assets/legoMan/pr_Leg_Right.glb",
            Position = legRightPos,
        };



        await View3D1.Request3DModel(body);
        await View3D1.Request3DModel(head);
        await View3D1.Request3DModel(armLeft);
        await View3D1.Request3DModel(armRight);
        await View3D1.Request3DModel(pelvis);
        await View3D1.Request3DModel(legLeft);
        await View3D1.Request3DModel(legRight);
        return body;
    }

    private ImportSettings RenderModel1(Guid sourceGuid)
    {
        Console.WriteLine("in RenderModel1");
        var pos = new Vector3(5, 0, 0);
        var model = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            // FileURL = "https://localhost:5001/storage/StaticFiles/8847996_STEERING_ASSY.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/pr_2505431__PISTON_RING_1_1185_278260.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/pr_H64_wheel_H187_0048_inst_0.glb",
            FileURL = "https://localhost:5001/storage/StaticFiles/barrel.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/MK48_273694.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/MK48_88480001_Pixyz_RnD.glb",
            Position = pos,
            OnComplete = (Scene scene, Object3D object3D) =>
            {
                if (object3D != null)
                {
                    // object3D.Position = new Vector3(-2.5, 3, -3);
                    Console.WriteLine($"OnComplete callback model1 object3d.Uuid={object3D.Uuid}");
                    Msg = $"Loaded 3D Model with id = {object3D.Uuid}";
                    StateHasChanged();
                }
                else
                {
                    Console.WriteLine($"object3D is null");
                }
            }
        };
        return model;
    }

    private ImportSettings RenderModel2(Guid sourceGuid)
    {
        Console.WriteLine("in RenderModel2");
        var pos = new Vector3(0, -2, 0);
        var model = new ImportSettings
        {
            Uuid = Guid.NewGuid(),
            Format = Import3DFormats.Gltf,
            // FileURL = "https://localhost:5001/storage/StaticFiles/8847996_STEERING_ASSY.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/pr_2505431__PISTON_RING_1_1185_278260.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/pr_H64_wheel_H187_0048_inst_0.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/barrel.glb",
            FileURL = "https://localhost:5001/storage/StaticFiles/block_small.glb",

            // FileURL = "https://localhost:5001/storage/StaticFiles/MK48_273694.glb",
            // FileURL = "https://localhost:5001/storage/StaticFiles/MK48_88480001_Pixyz_RnD.glb",
            Position = pos,
            OnComplete = (Scene scene, Object3D object3D) =>
            {
                if (object3D != null)
                {
                    // object3D.Position = new Vector3(0, 0, 0);
                    Console.WriteLine($"OnComplete callback model2 object3d.Uuid={object3D.Uuid}");
                    Msg = $"Loaded 3D Model with id = {object3D.Uuid}";
                    StateHasChanged();
                }
                else
                {
                    Console.WriteLine($"object3D is null");
                }
            }
        };
        // await View3D1.Request3DModel(model1);
        // await View3D1.Clone3DModel(model1);
        return model;
    }


    protected override Task OnInitializedAsync()
    {
        scene.Add(new AmbientLight());
        scene.Add(new PointLight()
        {
            Position = new Vector3(1, 3, 0)
        });

        // var height = 4;

        // var piv = new Vector3(-1, -height / 2, -3);
        // var pos = new Vector3(0, height, 0);
        // var rot = new Euler(0, Math.PI * 45 / 180, 0);

        // var realPos = new Vector3(piv.X + pos.X, piv.Y + pos.Y, piv.Z + pos.Z);

        // scene.Add(new Mesh
        // {
        //     Geometry = new BoxGeometry(width: 2, height: height, depth: 6),
        //     Position = pos,
        //     Rotation = rot,
        //     Pivot = piv,
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "magenta"
        //     }
        // });

        // TestText = scene.Add(new LabelText("My First Text") { Position = new Vector3(0, 3, 0), Color = "#33333a" }) as LabelText;

        // scene.Add(new Mesh
        // {
        //     Geometry = new CircleGeometry(radius: 0.75f, segments: 12),
        //     Position = new Vector3(2, 0, 0),
        //     Scale = new Vector3(1, 0.75f, 1),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "#98AFC7"
        //     }
        // });

        var buttons = new List<Button>() {
            new Button("BTN1", "OFF") {
                OnClick = ClickButton1
            },
            new Button("BTN2","Button 2"),
            new Button("BTN3","Button 3")
        };
        var menuPos = new Vector3(-4, 3, -2);
        var menuRot = new Euler(-1 * Math.PI * 30 / 180, 0, 0);

        scene.Add(new PanelMenu
        {
            Name = "MENU1",
            Width = 1.0,
            Height = 3.0,
            Buttons = buttons,
            Position = menuPos,
            Rotation = menuRot
        });

        // scene.Add(BuildTextPanel());
        scene.Add(BuildPanelGroup());

        var capsuleLength = 0.35;
        var capsuleRadius = 0.15f;
        var capsulePositions = new List<Vector3>() {
        new Vector3(0, 0, 0),
        new Vector3(4, 0, 0),
        new Vector3(4, 4, 0),
        new Vector3(4, 4, -4)
        };

        scene.Add(new Mesh
        {
            Geometry = new CapsuleGeometry(radius: capsuleRadius, length: capsuleLength),
            Position = capsulePositions.ElementAt(0),
            Material = new MeshStandardMaterial()
            {
                Color = "darkgreen"
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new CapsuleGeometry(radius: capsuleRadius, length: capsuleLength),
            Position = capsulePositions.ElementAt(1),
            Material = new MeshStandardMaterial()
            {
                Color = "blue"
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new CapsuleGeometry(radius: capsuleRadius, length: capsuleLength),
            Position = capsulePositions.ElementAt(2),
            Material = new MeshStandardMaterial()
            {
                Color = "yellow"
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new CapsuleGeometry(radius: capsuleRadius, length: capsuleLength),
            Position = capsulePositions.ElementAt(3),
            Material = new MeshStandardMaterial()
            {
                Color = "red"
            }
        });

        // var tube = scene.Add(new Mesh
        // {
        //     Geometry = new LineGeometry(path: capsulePositions),
        //     Position = new Vector3(0, 0, 0),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "white"
        //     }
        // });


        var tube = scene.Add(new Mesh
        {
            Geometry = new TubeGeometry(tubularSegments: 10, radialSegments: 8, radius: capsuleRadius, path: capsulePositions),
            Position = new Vector3(0, 0, 0),
            Material = new MeshStandardMaterial()
            {
                Color = "yellow"
            }
        });


        // var cone = new Mesh
        // {
        //     Geometry = new ConeGeometry(radius: 0.5f, height: 2, radialSegments: 16),
        //     Position = new Vector3(4, 2, 0),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "green",
        //         FlatShading = true,
        //         Metalness = 0.5f,
        //         Roughness = 0.5f
        //     }
        // };
        // scene.Add(cone);


        // scene.Add(new Mesh
        // {
        //     Geometry = new CylinderGeometry(radiusTop: 0.5f, height: 1.2f, radialSegments: 16),
        //     Position = new Vector3(0, 4, -2),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "red",
        //         Wireframe = true
        //     }
        // });
        // scene.Add(new Mesh
        // {
        //     Geometry = new DodecahedronGeometry(radius: 0.8f),
        //     Position = new Vector3(-2, 6, -2),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "darkviolet",
        //         Metalness = 0.5f,
        //         Roughness = 0.5f
        //     }
        // });

        // scene.Add(new Mesh
        // {
        //     Geometry = new IcosahedronGeometry(radius: 0.8f),
        //     Position = new Vector3(-4, 1, -2),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "violet"
        //     }
        // });

        // scene.Add(new Mesh
        // {

        //     Geometry = new OctahedronGeometry(radius: 0.75f),
        //     Position = new Vector3(2, 8, -2),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "aqua"
        //     }
        // });

        // scene.Add(new Mesh
        // {
        //     Geometry = new PlaneGeometry(width: 0.5f, height: 2),
        //     Position = new Vector3(4, 3, -2),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "purple"
        //     }
        // });
        // scene.Add(new Mesh
        // {
        //     Geometry = new RingGeometry(innerRadius: 0.6f, outerRadius: 0.7f),
        //     Position = new Vector3(0, 7, -4),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "DodgerBlue"
        //     }
        // });
        // scene.Add(new Mesh
        // {
        //     Geometry = new SphereGeometry(radius: 0.6f),
        //     Position = new Vector3(-2, 1, -4),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "darkgreen"
        //     },
        // });
        // scene.Add(new Mesh
        // {
        //     Geometry = new TetrahedronGeometry(radius: 0.75f),
        //     Position = new Vector3(2, 4, -4),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "lightblue"
        //     }
        // });
        // scene.Add(new Mesh
        // {
        //     Geometry = new TorusGeometry(radius: 0.6f, tube: 0.4f, radialSegments: 12, tubularSegments: 12),
        //     Position = new Vector3(4, 0, -4),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "lightgreen"
        //     }
        // });
        // scene.Add(new Mesh
        // {
        //     Geometry = new TorusKnotGeometry(radius: 0.6f, tube: 0.1f),
        //     Position = new Vector3(-4, 6, -4),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "RosyBrown"
        //     }
        // });
        return base.OnInitializedAsync();
    }

    private PanelMenu BuildMenu()
    {
        var buttons = new List<Button>()
        {
        };
        var menuPos = new Vector3(4, 3, -2);
        var menuRot = new Euler(-1 * Math.PI * 30 / 180, 0, 0);

        MenuBtn1 = new PanelMenu
        {
            Name = "MENU2",
            Width = 2.5,
            Height = 3.0,
            Buttons = buttons,
            Position = menuPos,
            Rotation = menuRot
        };
        return MenuBtn1;
    }

    private TextPanel BuildTextPanel()
    {
        var textLines = new List<string>()
        {
            "Lorem ipsum dolor sit amet.", "Lorem ipsum dolor sit amet.", "Lorem ipsum dolor sit amet.", "Lorem ipsum dolor sit amet.", "Lorem ipsum dolor sit amet.", "Lorem ipsum dolor sit amet.", "Lorem ipsum dolor sit amet.", "Lorem ipsum dolor sit amet."
        };
        var panelPos = new Vector3(-1, 2, -2);
        var panelRot = new Euler(-1 * Math.PI * 30 / 180, 0, 0);

        var textPanel = new TextPanel
        {
            Name = "TEXTPANEL1",
            // Width = 1,
            // Height = 1,
            TextLines = textLines,
            Position = panelPos,
            Rotation = panelRot
        };
        return textPanel;
    }
    private TextPanel BuildChildPanel(string text)
    {
        var textLines = new List<string>() { text };
        // var panelPos = new Vector3(-1, 2, -2);
        // var panelRot = new Euler(-1 * Math.PI * 30 / 180, 0, 0);

        TextPanel1 = new TextPanel
        {
            Name = "TEXTPANEL1",
            Color = "red",
            // Width = 1,
            // Height = 1,
            TextLines = textLines,
            // Position = panelPos,
            // Rotation = panelRot
        };
        return TextPanel1;
    }

    private Mesh BuildTube()
    {
        var path = new List<Vector3>() {
            new Vector3(-2, 0, 0),
            new Vector3(4, 0, 0),
            new Vector3(4, 4, 0),
            new Vector3(4, 4, -4)
        };
        var radius = 0.1;
        var mesh = new Mesh()
        {
            Geometry = new TubeGeometry(tubularSegments: 10, radialSegments: 8, radius: radius, path: path),
            Position = new Vector3(0, 0, 0),
            Material = new MeshStandardMaterial()
            {
                Color = "yellow"
            }
        };
        return mesh;
    }

    private Mesh BuildSomething()
    {
        var mesh = new Mesh
        {
            Geometry = new DodecahedronGeometry(radius: 0.8f),
            Position = new Vector3(-2, 6, -2),
            Material = new MeshStandardMaterial()
            {
                Color = "darkviolet",
                Metalness = 0.5f,
                Roughness = 0.5f
            }
        };
        return mesh;
    }

    private PanelGroup BuildPanelGroup()
    {
        var textLines = new List<string>()
        {
            "Lorem ipsum dolor sit amet."
        };
        var panelPos = new Vector3(2, 2, -6);
        var panelRot = new Euler(-1 * Math.PI * 45 / 180, 0, Math.PI * 30 / 180);
        var panelW = 5;
        var panelH = 5;

        var childPanelW = 1;
        var childPanelH = 0.5;
        var childPadding = 0;

        var colors = new List<string>() { "red", "orange", "green", "purple", "blue" };
        var childPanels = new List<TextPanel>();

        for (int i = 0; i < 5; i++)
        {
            var text = $"Child Panel {i}";
            var childPanel = BuildChildPanel(text);
            childPanel.Color = colors[i];
            childPanel.Position = new Vector3(-panelW / 2 + i * childPanelW + childPadding, -panelH / 2 + i, 0.1);
            childPanel.TextLines = new List<string>() { text };
            childPanel.Width = childPanelW;
            childPanel.Height = childPanelH;
            childPanels.Add(childPanel);
        }

        PanelGroup1 = new PanelGroup
        {
            Name = "PANELGROUP1",
            Width = panelW,
            Height = panelH,
            TextLines = textLines,
            Position = panelPos,
            Rotation = panelRot,
            TextPanels = childPanels,
            Meshes = new List<Mesh>() { BuildTube(), BuildSomething() }
        };
        return PanelGroup1;
    }

    private void ClickButton1(Button self)
    {
        Console.WriteLine($"self.Name={self.Name}");
        self.Text = self.Text == "OFF" ? "ON" : "OFF";

        if (self.Text == "ON")
        {
            TextPanel1 = BuildTextPanel();
            scene.Add(TextPanel1);
        }
        else
        {
            scene.Remove(TextPanel1);
        }


        // scene.Add(new Mesh
        // {
        //     Geometry = new TetrahedronGeometry(radius: 0.75f),
        //     Position = new Vector3(2, 4, -4),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "lightblue"
        //     }
        // });
        Task.Run(async () =>
        {
            await View3D1.UpdateScene();
        });

    }

    // public void OnObjectSelected(Object3DArgs e)
    // public void OnObjectSelected(Scene scene, Object3D object3D)
    // {
    //     Console.WriteLine($"OnObjectSelected object3D.UUID={object3D.Uuid}");
    //     // foreach (var item in scene.Children)
    //     // {
    //     //     if (item.Uuid == e.UUID)
    //     //     {
    //     //         this.Msg = $"Selected object with id = {e.UUID} and type {item.Type}";
    //     //         SelectedObject = item;
    //     //         StateHasChanged();
    //     //         break;
    //     //     }
    //     // }
    // }
    // public Task OnObjectLoaded(Object3DArgs e)
    // {
    //     var list = scene.Children;

    //     var shapeObject3D = Viewer.GetObjectByUuid((Guid)e.UUID!, list);
    //     if (shapeObject3D != null)
    //     {
    //         Console.WriteLine($"Found PreRenderImport {e.UUID} {list.Count} ShapeObject3D [{shapeObject3D}] ");
    //         Msg = $"Loaded 3D Model with id = {e.UUID}";
    //         StateHasChanged();
    //     }
    //     return Task.CompletedTask;
    // }

    public async Task OnDeleteSelected()
    {
        if (SelectedObject != null)
        {
            await View3D1.RemoveByUuidAsync(SelectedObject.Uuid);
            SelectedObject = null;
        }
    }

    public async Task OnClearScene()
    {
        SelectedObject = null;
        await View3D1.ClearSceneAsync();
        // await View3D1.UpdateScene();
    }

    // public async Task OnClearSceneAll()
    // {
    //     SelectedObject = null;
    //     await View3D1.ClearSceneAsync(true);
    //     // await View3D1.UpdateScene();
    // }

    public async Task OnMoveSelected(int axis, int moveBy)
    {
        if (SelectedObject != null)
        {
            var pos = SelectedObject.Position;
            if (axis == 0) SelectedObject.Position.X = pos.X + moveBy;
            if (axis == 1) SelectedObject.Position.Y = pos.Y + moveBy;
            if (axis == 2) SelectedObject.Position.Z = pos.Z + moveBy;

            // await View3D1.MoveObject(SelectedObject);
            await View3D1.UpdateScene();

        }
    }

    public async Task OnAddTorus()
    {
        scene.Add(new Mesh
        {
            Geometry = new TorusGeometry(radius: 0.6f, tube: 0.4f, radialSegments: 12, tubularSegments: 12),
            Position = new Vector3(-2, 3, -2),
            Material = new MeshStandardMaterial()
            {
                Color = "lightgreen"
            }
        });

        await View3D1.UpdateScene();
    }

    public async Task OnUpdateText()
    {
        // scene.Add(new Mesh
        // {
        //     Geometry = new TorusGeometry(radius: 0.6f, tube: 0.4f, radialSegments: 12, tubularSegments: 12),
        //     Position = new Vector3(-2, 3, -2),
        //     Material = new MeshStandardMaterial()
        //     {
        //         Color = "lightgreen"
        //     }
        // });

        var sec = DateTime.Now.Second;

        if (TestText != null) TestText.Text = $"Second {sec}";

        await View3D1.UpdateScene();
    }

}
