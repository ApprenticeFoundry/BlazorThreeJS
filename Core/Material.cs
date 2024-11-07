

using System.Text.Json.Serialization;
using FoundryRulesAndUnits.Models;



namespace BlazorThreeJS.Materials;

[JsonDerivedType(typeof(MeshStandardMaterial))]
public abstract class Material : ITreeNode
{
    protected StatusBitArray StatusBits = new();

    protected Material(string type) => this.Type = type;

    public string Type { get; } = nameof(Material);

    public string Name { get; set; } = string.Empty;

    public Guid Uuid { get; set; } = Guid.NewGuid();

    public virtual string GetTreeNodeTitle()
    {
        return $"{Name} [{Uuid}] => {Type} ({GetType().Name})";
    }

    public IEnumerable<TreeNodeAction>? GetTreeNodeActions()
    {
        return null;
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

