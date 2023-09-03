// Decompiled with JetBrains decompiler
// Type: Blazor3D.Core.Object3D
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Maths;
using System;
using System.Collections.Generic;



namespace BlazorThreeJS.Core
{
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

        public List<Object3D> Children { get; } = new List<Object3D>();

        public Object3D Add(Object3D child)
        {
            this.Children.Add(child);
            return child;
        }
        public bool Remove(Object3D child) => this.Children.Remove(child);
        public Object3D Update(Object3D child)
        {
            var obj = this.Children.Find((item) =>
            {
                return item.Uuid == child.Uuid;
            });

            if (obj != null)
            {
                this.Children.Remove(obj);
            }

            this.Add(child);
            return child;
        }
    }
}
