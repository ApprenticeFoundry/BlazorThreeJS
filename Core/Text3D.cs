
namespace BlazorThreeJS.Core
{
    public abstract class LabelBase : Object3D
  {
      protected LabelBase(string type = "LabelBase")
        : base(type)
      {
      }

      public string Color { get; set; } = "#";

      public double Intensity { get; set; } = 1;
  }

  public sealed class Text3D : LabelBase
  {
    public Text3D()
      : base(nameof(Text3D))
    {
    }

    public Text3D(
      string text = "Default Label Text")
      : this()
    {
      this.Text = text;
    }
    public string Text { get; set; } = "Default Label Text";
    public double FontSize { get; set; } = 0.5;
  }
}
