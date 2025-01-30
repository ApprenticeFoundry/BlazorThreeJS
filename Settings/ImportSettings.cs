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
            Uuid = model.Uuid;  //use the same uuid as the model
            AddChild(model);
            return model;
        }

        public (bool success, Model3D result) FindRequestedModel()
        {
            var (found, item) = FindChild(Uuid!);
            if (!found)
                return (false, null!);

            if (item is Model3D model)
                return (true, model);
            
            return (false, null!);
        }


        public void ResetChildren(List<Object3D> items)
        {
            ClearChildren();
            foreach (var child in items)
            {
                AddChild(child);
                child.SetDirty(false);
                //$"ResetChildren {child.Name} is dirty".WriteInfo();
            }
            //$"ResetChildren {Children.Count()} are dirty".WriteInfo();
        }


    }
}
