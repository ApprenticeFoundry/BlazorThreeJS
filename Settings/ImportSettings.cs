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
    public class ImportSettings : Object3D
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Import3DFormats Format { get; set; }

        public string? FileURL { get; set; }
        public string? FontSize { get; set; }
        public string? Text { get; set; }    
        public string? Color { get; set; }   
        public Mesh3D? Mesh { get; set; }
        
        public Vector3? ComputedSize { get; set; }

        // [JsonIgnore]
        // public MeshStandardMaterial? Material { get; set; }
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
        public ImportSettings() : base(nameof(ImportSettings))
        {
        }
    }
}
