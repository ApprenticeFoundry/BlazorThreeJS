import { Viewer3D } from './Viewer/Viewer3D';
import { ViewManager } from './Viewer/ViewManager';

namespace JSInterop {
    export function Load(): void {
        console.log('In Load');
        var manager = new ViewManager();
        window['ViewManager'] = manager;
        manager.establishViewer3D('BlazorThreeJS');

        //window['BlazorThreeJS'] = new Viewer3D();
    }

    JSInterop.Load();
}
