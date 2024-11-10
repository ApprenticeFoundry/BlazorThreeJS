
import { Viewer3D } from './Viewer/Viewer3D';


namespace JSInterop 
{
    export class ViewManager 
    {
        public ViewerLookup:any = {};
    
        public establishViewer3D(namespace: string): Viewer3D
        {
            console.log('start window[namespace] ', window[namespace]);
            console.log('calling establishViewer3D ' + namespace);
            if (Boolean(this.ViewerLookup[namespace]) == false)
            {
                console.log('creating namespace ' + namespace);
                var viewer = new Viewer3D();
                window[namespace] = viewer;
                this.ViewerLookup[namespace] = viewer;
                console.log('manager', this);
            }
            console.log('finish window[namespace] ', this.ViewerLookup[namespace]);
            return this.ViewerLookup[namespace];
        }
    
        public removeViewer3D(namespace: string): Viewer3D
        {
            var result = window[namespace];
            if (result != null)
            {
                this.ViewerLookup[namespace] = null;
                console.log('removing namespace ' + namespace);
                delete window[namespace];
                result.dispose();
                console.log('manager', this);
            }
    
            return result;
        }
    }

    export function Load(): void {
        console.log('In Load JSInterop');
        var manager = new ViewManager();
        window['ViewManager'] = manager;

        //window['Canvas3DComponent'] = new Viewer3D();
        //window['ViewerThreeD'] = new Viewer3D();
        window['BlazorThreeJS'] = new Viewer3D();
        
        //manager.establishViewer3D('ViewerThreeD');
        //manager.establishViewer3D('Canvas3DX');
        //manager.establishViewer3D('BlazorThreeJS');

        

        console.log('Complete Load JSInterop');
        //window['BlazorThreeJS'] = new Viewer3D();
    }

    JSInterop.Load();
}
