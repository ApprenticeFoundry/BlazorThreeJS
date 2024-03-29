﻿// Decompiled with JetBrains decompiler
// Type: Blazor3D.Settings.ViewerSettings
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll



namespace BlazorThreeJS.Settings
{
    public sealed class ViewerSettings
    {
        public string containerId { get; set; } = "blazorview3d";
        public bool CanSelect { get; set; }
        public bool CanSelectHelpers { get; set; }
        public string SelectedColor { get; set; } = "lime";
        public int Width = 0;
        public int Height = 0;
        public WebGLRendererSettings WebGLRendererSettings { get; set; } = new WebGLRendererSettings();
    }
}
