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
        public ImportSettings() : base(nameof(ImportSettings))
        {
            Transform = null!;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Import3DFormats Format { get; set; }

        public string? FileURL { get; set; }

    }
}
