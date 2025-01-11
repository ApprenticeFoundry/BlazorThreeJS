

using BlazorThreeJS.Core;
using System;
using System.Collections.Generic;


namespace BlazorThreeJS.ComponentHelpers
{
    public static class ChildrenHelper
    {

        public static void RemoveObjectByUuid(string uuid, List<Object3D> children)
        {
            var object3D = GetObjectByUuid(uuid, children);
            if (object3D != null)
                children.Remove(object3D);
        }

        public static Object3D? GetObjectByUuid(string uuid, List<Object3D> children)
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
