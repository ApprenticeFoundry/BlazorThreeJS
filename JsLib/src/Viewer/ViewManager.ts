import { Viewer3D } from "./Viewer3D";


export class ViewManager 
{

    public establishViewer3D(namespace: string): Viewer3D
    {
        if (window[namespace] == null)
        {
            console.log('creating namespace ' + namespace);
            window[namespace] = new Viewer3D();
        }

        return window[namespace];
    }

    public removeViewer3D(namespace: string): Viewer3D
    {
        var result = window[namespace];
        if (result != null)
        {
            console.log('removing namespace ' + namespace);
            delete window[namespace];
            result.dispose();
        }

        return result;
    }
}