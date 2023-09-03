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
            Object3D object3D = (Object3D)null;
            foreach (Object3D child in children)
            {
                if (child.Uuid == uuid)
                {
                    object3D = child;
                    break;
                }
                if (child.Children.Count > 0)
                    ChildrenHelper.RemoveObjectByUuid(uuid, child.Children);
            }
            if (object3D == null)
                return;
            children.Remove(object3D);
        }

        internal static Object3D? GetObjectByUuid(Guid uuid, List<Object3D> children)
        {
            Object3D objectByUuid = (Object3D)null;
            foreach (Object3D child in children)
            {
                if (child.Uuid == uuid)
                    return child;
                if (child.Children.Count > 0)
                {
                    objectByUuid = ChildrenHelper.GetObjectByUuid(uuid, child.Children);
                    if (objectByUuid != null)
                        return objectByUuid;
                }
            }
            return objectByUuid;
        }
    }
}
