

using System.Text.Json.Serialization;
using BlazorThreeJS.Geometires;
using FoundryRulesAndUnits.Models;



namespace BlazorThreeJS.Core
{
    [JsonDerivedType(typeof(BoxGeometry))]
    [JsonDerivedType(typeof(BoundaryGeometry))]
    [JsonDerivedType(typeof(CapsuleGeometry))]
    [JsonDerivedType(typeof(CircleGeometry))]
    [JsonDerivedType(typeof(ConeGeometry))]
    [JsonDerivedType(typeof(CylinderGeometry))]
    [JsonDerivedType(typeof(DodecahedronGeometry))]
    [JsonDerivedType(typeof(IcosahedronGeometry))]
    [JsonDerivedType(typeof(LineGeometry))]
    [JsonDerivedType(typeof(OctahedronGeometry))]
    [JsonDerivedType(typeof(PlaneGeometry))]
    [JsonDerivedType(typeof(RingGeometry))]
    [JsonDerivedType(typeof(SphereGeometry))]
    [JsonDerivedType(typeof(TetrahedronGeometry))]
    [JsonDerivedType(typeof(TorusGeometry))]
    [JsonDerivedType(typeof(TorusKnotGeometry))]
    [JsonDerivedType(typeof(TubeGeometry))]
    public abstract class BufferGeometry : ITreeNode
    {
        protected StatusBitArray StatusBits = new();
        public string? Uuid { get; set; }
        protected BufferGeometry(string type) => this.Type = type;

        public string Name { get; set; } = string.Empty;


        public string Type { get; } = nameof(BufferGeometry);

        public virtual string GetTreeNodeTitle()
        {
            return $"{Name} [{Uuid}] => {Type} ({GetType().Name})";
        }

        public virtual IEnumerable<TreeNodeAction>? GetTreeNodeActions()
        {
            var result = new List<TreeNodeAction>();
            return result;
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

        public IEnumerable<ITreeNode> GetTreeChildren()
        {
            return Enumerable.Empty<ITreeNode>();
        }
    }
}
