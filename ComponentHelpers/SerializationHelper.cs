// Decompiled with JetBrains decompiler
// Type: Blazor3D.ComponentHelpers.SerializationHelper
// Assembly: Blazor3D, Version=0.1.24.0, Culture=neutral, PublicKeyToken=null
// MVID: 8589B0D0-D62F-4099-9D8A-332F65D16B15
// Assembly location: Blazor3D.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;



namespace BlazorThreeJS.ComponentHelpers
{
    internal static class SerializationHelper
    {
        internal static JsonSerializerSettings GetSerializerSettings() => new JsonSerializerSettings()
        {
            ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }
}
