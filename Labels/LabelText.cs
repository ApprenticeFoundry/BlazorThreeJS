
namespace BlazorThreeJS.Labels
{
  public sealed class LabelText : LabelBase
  {
    public LabelText()
      : base(nameof(LabelText))
    {
    }

    public LabelText(
      string text = "Default Label Text")
      : this()
    {
      this.Text = text;
    }
    public string Text { get; set; } = "Default Label Text";
    public double FontSize { get; set; } = 0.5;
  }
}
