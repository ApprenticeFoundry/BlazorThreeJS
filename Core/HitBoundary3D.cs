
using BlazorThreeJS.Lights;
using BlazorThreeJS.Maths;

using BlazorThreeJS.Objects;
using BlazorThreeJS.Viewers;
using FoundryRulesAndUnits.Models;
using FoundryRulesAndUnits.Extensions;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using BlazorThreeJS.Settings;

namespace BlazorThreeJS.Core
{
    public class HitBoundary3D :ITreeNode
    {
        protected StatusBitArray StatusBits = new();
        public string? Uuid { get; set; }

        public string Type { get; } = nameof(HitBoundary3D);

        public string Name { get; set; } = string.Empty;

        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;
        public double Depth { get; set; } = 0;

        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;

        public HitBoundary3D() 
        {
        }

        public IEnumerable<ITreeNode> GetTreeChildren()
        {
            var result = new List<ITreeNode>();
            return result;
        }

        public IEnumerable<TreeNodeAction>? GetTreeNodeActions()
        {
            var result = new List<TreeNodeAction>();
            return result;
        }

        public string GetTreeNodeTitle()
        {
            return $"{Name} [{Uuid}] => {Type}({GetType().Name})";
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