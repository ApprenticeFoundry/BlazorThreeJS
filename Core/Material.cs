

using System.Text.Json.Serialization;
using FoundryRulesAndUnits.Models;



namespace BlazorThreeJS.Materials;

[JsonDerivedType(typeof(MeshStandardMaterial))]
[JsonDerivedType(typeof(MeshPhongMaterial))]
public abstract class Material : ITreeNode
{
    protected StatusBitArray StatusBits = new();
    public string? Uuid { get; set; }

    protected Material(string type) 
    {
        this.Type = type;
    }

    public string Type { get; } = nameof(Material);

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = "orange";
    public double Opacity { get; set; } = 1;

    public bool Wireframe { get; set; } = false;

    public virtual string GetTreeNodeTitle()
    {
        return $"{Name} [{Uuid}] => {Type} ({GetType().Name})";
    }

    public virtual IEnumerable<TreeNodeAction>? GetTreeNodeActions()
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

