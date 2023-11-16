// Decompiled with JetBrains decompiler
// Type: Blazor3D.Geometires.BoxGeometry
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

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
