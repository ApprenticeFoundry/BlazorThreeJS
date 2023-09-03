// Decompiled with JetBrains decompiler
// Type: Blazor3D.Materials.Material
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using System;



namespace BlazorThreeJS.Materials
{
    public abstract class Material
    {
        protected Material(string type) => this.Type = type;

        public string Type { get; } = nameof(Material);

        public string Name { get; set; } = string.Empty;

        public Guid Uuid { get; set; } = Guid.NewGuid();
    }
}
