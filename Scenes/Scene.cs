// Decompiled with JetBrains decompiler
// Type: Blazor3D.Scenes.Scene
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using System.Security.Cryptography.X509Certificates;
using BlazorThreeJS.Core;
using FoundryRulesAndUnits.Models;



namespace BlazorThreeJS.Scenes
{
  public sealed class Scene : Object3D
  {
    public Scene()
      : base(nameof(Scene))
    {
    }

    public string BackGroundColor { get; set; } = "#505050";


  }
}
