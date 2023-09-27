// Decompiled with JetBrains decompiler
// Type: Blazor3D.ComponentHelpers.ChildrenHelper
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using BlazorThreeJS.Core;
using System;
using System.Collections.Generic;


namespace BlazorThreeJS.ComponentHelpers
{
    internal static class ChildrenHelper
    {

        internal static void RemoveObjectByUuid(Guid uuid, List<Object3D> children)
        {
            var object3D = GetObjectByUuid(uuid, children);
            if (object3D != null)
                children.Remove(object3D);
        }

        internal static Object3D? GetObjectByUuid(Guid uuid, List<Object3D> children)
        {
    
            foreach (Object3D child in children)
            {
                if (child.Uuid == uuid)
                    return child;

                if (child.HasChildren())
                {
                    var found = ChildrenHelper.GetObjectByUuid(uuid, child.GetAllChildren());
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
    }
}
