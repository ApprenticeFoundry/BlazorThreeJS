// Decompiled with JetBrains decompiler
// Type: Blazor3D.Textures.Texture
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Enums;
using BlazorThreeJS.Maths;
using FoundryRulesAndUnits.Models;
using System;


namespace BlazorThreeJS.Core
{
    public class Texture
    {
        public string? Uuid { get; set; }

        public Texture() {}

        protected Texture(string type) => this.Type = type;

        public string Type { get; } = nameof(Texture);

        public string Name { get; set; } = string.Empty;


        public string TextureUrl { get; set; } = string.Empty;

        public WrappingType WrapS { get; set; } = WrappingType.ClampToEdgeWrapping;

        public WrappingType WrapT { get; set; } = WrappingType.ClampToEdgeWrapping;

        public Vector2 Repeat { get; set; } = new Vector2(1f, 1f);

        public Vector2 Offset { get; set; } = new Vector2(0.0f, 0.0f);

        public Vector2 Center { get; set; } = new Vector2(0.0f, 0.0f);

        public double Rotation { get; set; }
    }
}
