using BlazorThreeJS.Enums;
using BlazorThreeJS.Materials;

using BlazorThreeJS.Maths;
using BlazorThreeJS.Core;
using BlazorThreeJS.Objects;

using System.Text.Json.Serialization;
using FoundryRulesAndUnits.Extensions;



namespace BlazorThreeJS.Settings
{
    public class ImportSettings : Object3D
    {


        public ImportSettings() : base(nameof(ImportSettings))
        {
            Transform = null!;
        }

        public Model3D AddRequestedModel(Model3D model)
        {
            model.IsDirty = false;
            Uuid = model.Uuid;  //use the same uuid as the model
            AddChild(model);
            return model;
        }

        public override (bool success, Object3D result) AddChild(Object3D child)
        {
            if (child == null)
            {
                //$"AddChild missing  Uuid, {child.Name}".WriteError();  
                return (false, child!);
            }

            var uuid = child.Uuid;
            if (string.IsNullOrEmpty(uuid))
            {
                //$"AddChild missing  Uuid, {child.Name}".WriteError();  
                return (false, child);
            }

            //what if you have a child with the same uuid? but it is a different object?
            var (found, item) = FindChild(uuid);
            if (found)
            {
                //$"Object3D AddChild Exist: returning existing {child.Name} -> {item.Name} {item.Uuid}".WriteError();  
                return (false, item!);
            }

            //$"Object3D AddChild {child.Name} -> {this.Name} {this.Uuid}".WriteInfo();

            children.Add(child);

            return (true, child);
        }


        public void CopyAndReset(List<Object3D> items)
        {
            foreach (var child in items)
            {
                child.IsDirty = false;
                AddChild(child);
            }
            //$"CopyAndReset {Children.Count()} are dirty".WriteInfo();
        }


    }
}
