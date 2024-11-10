// Decompiled with JetBrains decompiler
// Type: Blazor3D.Settings.ImportSettings
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Enums;
using BlazorThreeJS.Materials;

using BlazorThreeJS.Maths;
using BlazorThreeJS.Core;
using BlazorThreeJS.Objects;

using System.Text.Json.Serialization;
using BlazorThreeJS.Viewers;


namespace BlazorThreeJS.Settings
{
    public class ImportSettings
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Import3DFormats Format { get; set; }
        public string? FileURL { get; set; }
        //public string? TextureURL { get; set; }
        public string? Uuid { get; set; }
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Pivot { get; set; } = new Vector3();
        public Vector3 Scale { get; set; } = new Vector3(1.0, 1.0, 1.0);
        public Euler Rotation { get; set; } = new Euler();

        public Vector3? ComputedSize { get; set; }

        [JsonIgnore]
        public MeshStandardMaterial? Material { get; set; }
        [JsonIgnore]
        public Action? OnComplete { get; set; }
        [JsonIgnore]
        public Action<ImportSettings> OnClick { get; set; } = (ImportSettings model3D) => { };
        [JsonIgnore]
        public int ClickCount { get; set; } = 0;
        public bool IsShow()
        {
            return ClickCount % 2 == 1;
        }
        public int Increment()
        {
            return ++ClickCount;
        }

    }
}
