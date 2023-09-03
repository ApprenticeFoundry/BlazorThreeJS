import { Viewer3D } from './Viewer/Viewer3D';

namespace JSInterop {
    export function Load(): void {
        console.log('In Load');
        window['BlazorThreeJS'] = new Viewer3D();
    }

    JSInterop.Load();
}
