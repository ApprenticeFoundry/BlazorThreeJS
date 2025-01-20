using System.Runtime.CompilerServices;
using System.Text.Json;
using BlazorComponentBus;
using BlazorThreeJS.Core;
using BlazorThreeJS.Settings;
using BlazorThreeJS.Viewers;
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
    void SetActiveScene(Scene3D scene);
    void WhenFrameRefreshComplete(Action action);

}

public class ThreeDService : IThreeDService
{
    protected IJSRuntime js { get; set; }
    protected ComponentBus pubsub { get; set; }

    private static DateTime _lastRender;

    private static bool IsCurrentlyRendering = false;

    private static Action FrameRefreshComplete = null!;

    public static Scene3D ActiveScene { get; set; } = null!;

    public static int tick { get; private set; }


    public ThreeDService(
        IJSRuntime js,
        ComponentBus pubsub)
    {
        this.js = js;
        this.pubsub = pubsub;
        _lastRender = DateTime.Now;
        tick = 0;
    }

    public void SetActiveScene(Scene3D scene)
    {
        ActiveScene = scene;
    }
    public void WhenFrameRefreshComplete(Action action)
    {
        FrameRefreshComplete = action;
    }

    public static bool SetCurrentlyRendering(bool isRendering, int tick)
    {
        var oldValue = IsCurrentlyRendering;


        if (!isRendering)
        {

            // while (DirtyObjectQueue.Count > 0)
            // {
            //     var args = DirtyObjectQueue.Dequeue();
            //     // $"SetCurrentlyRendering is Dequeueing {args.Topic} ".WriteSuccess(2);
            //     //ApplyMouseArgs(args);
            // }
        }

        IsCurrentlyRendering = isRendering;
        if (!isRendering)
            FrameRefreshComplete?.Invoke();
            
        return oldValue;
    }



    [JSInvokable]
    public static async void TriggerAnimationFrame()
    {
        if ( ActiveScene == null)
            return;

        SetCurrentlyRendering(true, 0);


        var fps = 1.0 / (DateTime.Now - _lastRender).TotalSeconds;
        _lastRender = DateTime.Now; // update for the next time 
        //$"TriggerAnimationFrame  {framerate}".WriteSuccess();

        var dirtyObjects = new List<Object3D>();
        ActiveScene.UpdateForAnimation(tick++, fps, dirtyObjects);


        var settings = new ImportSettings();
        settings.ResetChildren(dirtyObjects);

        if ( dirtyObjects.Count == 0)
            return;

        await ActiveScene.Request3DSceneRefresh(settings, (_) =>
        {
            SetCurrentlyRendering(false, 0);
        });
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
