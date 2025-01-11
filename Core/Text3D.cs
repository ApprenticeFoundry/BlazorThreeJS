
namespace BlazorThreeJS.Core
{
  public sealed class Text3D : Object3D
  {
    public Text3D(): base(nameof(Text3D))
    {
    }

    public Text3D(string text = "Default Label Text"): this()
    {
      this.Text = text;
    }
    
    public string Color { get; set; } = "#";
    public double Intensity { get; set; } = 1;
    public string Text { get; set; } = "Default Label Text";
    public double FontSize { get; set; } = 0.5;
  }
}
