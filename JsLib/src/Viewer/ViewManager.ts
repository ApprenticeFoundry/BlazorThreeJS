import { Viewer3D } from "./Viewer3D";


export class ViewManager 
{
    public ViewerLookup:any = {};

    public establishViewer3D(namespace: string): Viewer3D
    {
        if (window[namespace] == null)
        {
            console.log('creating namespace ' + namespace);
            var viewer = new Viewer3D();
            this.ViewerLookup[namespace] = viewer;
            window[namespace] = viewer;
            console.log('manager', this);
        }

        return window[namespace];
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