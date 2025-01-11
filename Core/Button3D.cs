

using System.Text.Json.Serialization;

namespace BlazorThreeJS.Core
{
    public sealed class Button3D : Object3D
    {
        [JsonIgnore]
        public Action<Button3D> OnClick = (Button3D b) => { };
        public string Text { get; set; } = "Default Button Text";
        public Button3D()
          : base(nameof(Button3D))
        {
        }

        public Button3D(string name = "BTN0",
          string text = "Default Button Text")
          : this()
        {
            Name = name;
            Text = text;
        }
    }
}
