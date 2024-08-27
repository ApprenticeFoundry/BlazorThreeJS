// Decompiled with JetBrains decompiler
// Type: Blazor3D.Core.BufferGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using System;
using System.Text.Json.Serialization;
using BlazorThreeJS.Geometires;
using FoundryRulesAndUnits.Models;



namespace BlazorThreeJS.Core
{
    [JsonDerivedType(typeof(BoxGeometry))]
    [JsonDerivedType(typeof(CapsuleGeometry))]
    [JsonDerivedType(typeof(DodecahedronGeometry))]
    [JsonDerivedType(typeof(TubeGeometry))]
    public abstract class BufferGeometry: ITreeNode
    {
        private StatusBitArray StatusBits = new();
        protected BufferGeometry(string type) => this.Type = type;

        public string Name { get; set; } = string.Empty;

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public string Type { get; } = "Geometry";

        public IEnumerable<ITreeNode> GetChildren()
        {
            return new List<ITreeNode>();
        }


        public IEnumerable<TreeNodeAction>? GetTreeNodeActions()
        {
            return null;
        }

        public virtual string GetTreeNodeTitle()
        {
            return $"{Name}: {Type} ({GetType().Name})";
        }

        public bool GetIsExpanded()
        {
            return this.StatusBits.IsExpanded;
        }

        public bool SetExpanded(bool value)
        {
            this.StatusBits.IsExpanded = value;
            return value;
        }

        public bool GetIsSelected()
        {
            return this.StatusBits.IsSelected;
        }

        public bool SetSelected(bool value)
        {
            this.StatusBits.IsSelected = value;
            return value;
        }
    }
}
