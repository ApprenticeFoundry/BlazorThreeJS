using BlazorComponentBus;

using FoundryRulesAndUnits.Extensions;
using Microsoft.JSInterop;


namespace BlazorThreeJS.Solutions;

public class AnimationEvent
{
    public double fps = 0;
}

public interface IThreeDService
{
    IJSRuntime JS();
    ComponentBus AnimationBus();
}

public class ThreeDService : IThreeDService
{
    protected IJSRuntime js { get; set; }
    protected ComponentBus pubsub { get; set; }

    private static ComponentBus Publish { get; set; } = null!;
    private static DateTime _lastRender;

    public ThreeDService(
        IJSRuntime js,
        ComponentBus pubsub)
    {
        this.js = js;
        this.pubsub = pubsub;
        Publish = new ComponentBus();
        _lastRender = DateTime.Now;
    }

    [JSInvokable]
    public static async void TriggerAnimationFrame()
    {
            await Task.CompletedTask;
            var framerate = 1.0 / (DateTime.Now - _lastRender).TotalSeconds;
            _lastRender = DateTime.Now; // update for the next time 
            $"TriggerAnimationFrame  {framerate}".WriteSuccess();

            //this is where you refresh the dirty items
            //await Publish.Publish<AnimationEvent>(new AnimationEvent() { fps = framerate });
        
    }

    public ComponentBus AnimationBus()
    {
        return Publish;
    }

    public IJSRuntime JS()
    {
        return js;
    }

    public ComponentBus PubSub()
    {
        return pubsub;
    }



}
