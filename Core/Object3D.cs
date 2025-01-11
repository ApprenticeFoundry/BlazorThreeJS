
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



namespace BlazorThreeJS.Core
{
    [JsonDerivedType(typeof(Mesh3D))]
    [JsonDerivedType(typeof(Group3D))]
    [JsonDerivedType(typeof(TextPanel3D))]
    [JsonDerivedType(typeof(PanelMenu3D))]
    [JsonDerivedType(typeof(Button3D))]
    [JsonDerivedType(typeof(PanelGroup3D))]
    [JsonDerivedType(typeof(AmbientLight))]
    [JsonDerivedType(typeof(PointLight))]
    [JsonDerivedType(typeof(Text3D))]
    [JsonDerivedType(typeof(Scene3D))]
    public abstract class Object3D : ITreeNode
    {
        protected StatusBitArray StatusBits = new();
        public string? Uuid { get; set; }
        public static int Object3DCount { get; set; } = 0;


        [JsonIgnore]
        public Action<Object3D>? OnDelete { get; set; }

        public Vector3 Position { get; set; } = new Vector3();

        public Vector3 Pivot { get; set; } = new Vector3();

        public Euler Rotation { get; set; } = new Euler();

        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);

        public string Type { get; } = nameof(Object3D);

        public string Name { get; set; } = string.Empty;


        private List<Object3D> children = new();

        public IEnumerable<Object3D> Children => children;

        protected Object3D(string type) 
        {
            Object3DCount++;
            this.Type = type;
            Uuid = Object3DCount.ToString();
        }
        public List<Object3D> GetAllChildren()
        {
            return children;
        }

        public virtual string GetTreeNodeTitle()
        {
            return $"{Name} [{Uuid}] => {Type} C#: ({GetType().Name})";
        }


        public virtual IEnumerable<TreeNodeAction> GetTreeNodeActions()
        {
            var result = new List<TreeNodeAction>();

            if (OnDelete != null)
                result.AddAction("Del", "btn-danger", () =>
                {
                    Delete();
                });
            return result;
        }

        public virtual IEnumerable<ITreeNode> GetTreeChildren()
        {
            var result = new List<ITreeNode>();
            result.AddRange(GetAllChildren());
            return result;
        }

        public virtual void Delete()
        {
            $"Deleting {GetTreeNodeTitle()}".WriteWarning();
            OnDelete?.Invoke(this);
        }

        public virtual (bool success, Object3D result) AddChild(Object3D child)
        {
            var uuid = child.Uuid;
            if ( string.IsNullOrEmpty(uuid))
            {
                $"AddChild missing  Uuid, {child.Name}".WriteError();  
                return (false, child);
            }

            var (found, item) = FindChild(uuid);
            if ( found )
            {
                $"Object3D AddChild Exist: returning existing {child.Name} -> {item.Name} {item.Uuid}".WriteError();  
                return (false, item!);
            }

            $"Object3D AddChild {child.Name} -> {this.Name} {this.Uuid}".WriteInfo();
            this.children.Add(child);
            return (true, child);
        }


        public virtual (bool success, Object3D result) RemoveChild(Object3D child)
        {
            var found = this.children.Find((item) => item == child);
            if (found == null)
            {
                $"Object3D RemoveChild missing {child.Name} -> {this.Name} {this.Uuid}".WriteError();  
                return (false, child);
            }
            this.children.Remove(child);
            return (true, child);
        }

        public bool HasChildren()
        {
            return children.Count > 0;
        }


        public (bool success, Object3D child) FindChild(string uuid)
        {
            var found = this.children.Find((item) => item.Uuid == uuid);
            return (found != null, found!);
        }

        // public Object3D Update(Object3D child)
        // {
        //     var uuid = child.Uuid;
        //     if ( string.IsNullOrEmpty(uuid))
        //     {
        //         $"AddChild missing  Uuid, {child.Name}".WriteError();  
        //         return child;
        //     }

        //     var found = this.children.Find((item) => item.Uuid == uuid);
        //     if (found != null)
        //     {
        //         this.RemoveChild(found);
        //     }

        //     this.AddChild(child);
        //     return child;
        // }

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
