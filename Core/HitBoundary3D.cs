
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
    public class HitBoundaryDTO
    {
        public string Uuid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;
        public double Depth { get; set; } = 0;

        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;

        public double QX { get; set; } = 0;
        public double QY { get; set; } = 0;
        public double QZ { get; set; } = 0;
        public double QW { get; set; } = 0;
    }

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

        public Vector3 GetPosition()
        {
            return new Vector3(X, Y, Z);
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
            return $"{Name} BB {Width:F2} x {Height:F2} x {Depth:F2} at {X:F2}, {Y:F2}, {Z:F2}";
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