
import { ViewManager } from './Viewer/ViewManager';

namespace JSInterop {
    export function Load(): void {
        console.log('In Load JSInterop');
        var manager = new ViewManager();
        window['ViewManager'] = manager;
        var original = manager.establishViewer3D('BlazorThreeJS');


        console.log('original', original);

        //window['BlazorThreeJS'] = new Viewer3D();
    }

    JSInterop.Load();
}
