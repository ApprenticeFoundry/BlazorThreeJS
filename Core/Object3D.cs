// Decompiled with JetBrains decompiler
// Type: Blazor3D.Core.Object3D
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Cameras;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Menus;
using BlazorThreeJS.Objects;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;



namespace BlazorThreeJS.Core
{
    // [JsonDerivedType(typeof(Mesh))]
    // [JsonDerivedType(typeof(PanelMenu))]
    // [JsonDerivedType(typeof(PanelGroup))]
    // [JsonDerivedType(typeof(Camera))]
    // [JsonDerivedType(typeof(OrthographicCamera))]
    // [JsonDerivedType(typeof(PerspectiveCamera))]
    public abstract class Object3D
    {
        protected Object3D(string type) => this.Type = type;

        public Vector3 Position { get; set; } = new Vector3();

        public Vector3 Pivot { get; set; } = new Vector3();

        public Euler Rotation { get; set; } = new Euler();

        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);

        public string Type { get; } = nameof(Object3D);

        public string Name { get; set; } = string.Empty;

        public Guid Uuid { get; set; } = Guid.NewGuid();

        private List<Object3D> children = new();

        public List<Object3D> GetAllChildren()
        {
            return children;
        }

        public object[] Children
        {
            get
            {
                return (object[])children.ToArray();
            }
        }

        public Object3D Add(Object3D child)
        {
            this.children.Add(child);
            return child;
        }
        public List<Object3D> AddRange(List<Object3D> newChildren)
        {
            this.children.AddRange(newChildren);
            return newChildren;
        }
        public Object3D RemoveChild(Object3D child)
        {
            this.children.Remove(child);
            return child;
        }

        public bool HasChildren()
        {
            return children.Count > 0;
        }
        // public T AddChild<T>(T child) where T : Object3D
        // {
        //     this.children.Add(child);
        //     return child;
        // }
        public bool Remove(Object3D child) => this.children.Remove(child);
        public Object3D Update(Object3D child)
        {
            var obj = this.children.Find((item) =>
            {
                return item.Uuid == child.Uuid;
            });

            if (obj != null)
            {
                this.children.Remove(obj);
            }

            this.Add(child);
            return child;
        }
    }
}
