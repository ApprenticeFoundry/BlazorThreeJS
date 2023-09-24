
using BlazorThreeJS.Core;
using System.Text.Json.Serialization;
namespace BlazorThreeJS.Menus
{
    public sealed class Button : Object3D
    {
        [JsonIgnore]
        public Action<Button> OnClick = (Button b) => { };
        public string Text { get; set; } = "Default Button Text";
        public Button()
          : base(nameof(Button))
        {
        }

        public Button(string name = "BTN0",
          string text = "Default Button Text")
          : this()
        {
            Name = name;
            Text = text;
        }
    }
}
