using System.Text.Json.Serialization;

namespace BlazorThreeJS.Core;

    // Enums for text properties
    public enum Text3DAlign
    {
        Left,
        Center,
        Right,
        Justify
    }
    
    public enum Text3DAnchor
    {
        Left,
        Center,
        Right,
        Top,
        Middle,
        Bottom
    }
    
    public enum FontWeight
    {
        Normal,
        Bold
    }

public sealed class Text3D : Object3D
{
    // Basic text properties
    public string Text { get; set; } = "Default Label Text";
    public string Color { get; set; } = "#FFFFFF";
    public double Intensity { get; set; } = 1;
    public double FontSize { get; set; } = 0.5;
    
    // Text alignment and anchoring
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Text3DAlign TextAlign { get; set; } = Text3DAlign.Left;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Text3DAnchor AnchorX { get; set; } = Text3DAnchor.Center;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Text3DAnchor AnchorY { get; set; } = Text3DAnchor.Middle;
    
    // Font styling
    public string FontFamily { get; set; } = "Arial";
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FontWeight FontWeight { get; set; } = FontWeight.Normal;
    public double LetterSpacing { get; set; } = 0;
    public double LineHeight { get; set; } = 1.15;

    public double MaxWidth { get; set; } = 10000000000.0;
    
    // Material properties
    public double Opacity { get; set; } = 1;
    public string StrokeColor { get; set; } = "#000000";
    public double StrokeWidth { get; set; } = 0;
    public double OutlineWidth { get; set; } = 0;
    

    
    // Constructors
    public Text3D() : base(nameof(Text3D))
    {
    }

    public Text3D(string text) : this()
    {
        Text = text;
    }
    
    public Text3D(string text, string color) : this(text)
    {
        Color = color;
    }
    
    // Optional: Method to clone the text object
    public Text3D Clone()
    {
        return new Text3D
        {
            Text = this.Text,
            Color = this.Color,
            Intensity = this.Intensity,
            FontSize = this.FontSize,
            TextAlign = this.TextAlign,
            AnchorX = this.AnchorX,
            AnchorY = this.AnchorY,
            FontFamily = this.FontFamily,
            FontWeight = this.FontWeight,
            LetterSpacing = this.LetterSpacing,
            LineHeight = this.LineHeight,
            MaxWidth = this.MaxWidth,
            Opacity = this.Opacity,
            StrokeColor = this.StrokeColor,
            StrokeWidth = this.StrokeWidth,
            OutlineWidth = this.OutlineWidth
        };
    }
    
    // Optional: Method to reset to default values
    public void Reset()
    {
        Text = "Default Label Text";
        Color = "#FFFFFF";
        Intensity = 1;
        FontSize = 0.5;
        TextAlign = Text3DAlign.Left;
        AnchorX = Text3DAnchor.Center;
        AnchorY = Text3DAnchor.Middle;
        FontFamily = "Arial";
        FontWeight = FontWeight.Normal;
        LetterSpacing = 0;
        LineHeight = 1.15;
        MaxWidth = double.PositiveInfinity;
        Opacity = 1;
        StrokeColor = "#000000";
        StrokeWidth = 0;
        OutlineWidth = 0;
    }
}

