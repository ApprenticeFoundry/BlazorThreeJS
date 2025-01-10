import { Group } from 'three';
import { GLTF } from 'three/examples/jsm/loaders/GLTFLoader';


export class ObjectLookupClass 
{
    private gltfGroups = new Map<string, Group>();
    private gltfURLs = new Map<string, GLTF>();
    private labels = new Map<string, Text>();

    public findGLTF(url: string): GLTF | null {
        return this.gltfURLs.get(url) || null;
    }
    public casheGLTF(url: string, obj: GLTF) {
        this.gltfURLs.set(url, obj);
        console.log('casheGLTF url=', url, ' obj=', obj);
    }

    public findGroup(guid: string): Group | null {
        return this.gltfGroups.get(guid) || null;
    }

    public addGroup(guid: string, obj: Group) {
        this.gltfGroups.set(guid, obj);
        console.log('casheGroup url=', guid, ' obj=', obj);
    }

    public addLabel(key: string, value: Text) {
        this.labels.set(key, value);
    }

    public deleteLabel(key: string) {
        this.labels.delete(key);
    }

    public findLabel(key: string): Text | null {
        return this.labels.get(key) || null;
    }
}

export const ObjectLookup = new ObjectLookupClass();
