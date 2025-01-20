import { AnimationMixer, Group, Object3D } from 'three';
import { Text } from 'troika-three-text';
import { GLTF } from 'three/examples/jsm/loaders/GLTFLoader';
import * as ThreeMeshUI from 'three-mesh-ui';


export class ObjectLookupClass 
{
    private models = new Map<string, Group>();
    private groups = new Map<string, Group>();
    private gltfURLs = new Map<string, GLTF>();
    private labels = new Map<string, Text>();
    private primitives = new Map<string, Object3D>();
    private panels = new Map<string, ThreeMeshUI.Block>();
    private buttons = new Map<string, ThreeMeshUI.Block>();
    private mixers = new Array<AnimationMixer>();
        
    public addMixer(value: AnimationMixer) {
        this.mixers.push(value);
    }

    public allMixers(): AnimationMixer[] {
        return this.mixers;
    }

    public findGLTF(url: string): GLTF | null {
        return this.gltfURLs.get(url) || null;
    }
    public casheGLTF(url: string, obj: GLTF) {
        this.gltfURLs.set(url, obj);
        console.log('casheGLTF url=', url, ' obj=', obj);
    }


    public findGroup(guid: string): Group | null {
        return this.groups.get(guid) || null;
    }
    public addGroup(guid: string, obj: Group) {
        this.groups.set(guid, obj);
        console.log('addGroup url=', guid, ' obj=', obj);
    }
    public deleteGroup(key: string) {
        this.groups.delete(key);
    }

    public findModel(guid: string): Group | null {
        return this.models.get(guid) || null;
    }
    public addModel(guid: string, obj: Group) {
        this.models.set(guid, obj);
        console.log('addModel url=', guid, ' obj=', obj);
    }
    public deleteModel(key: string) {
        this.models.delete(key);
    }

    public allLabels(): any {
        return this.labels.values();
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


    public addPrimitive(key: string, value: Object3D) {
        this.primitives.set(key, value);
    }
    public deletePrimitive(key: string) {
        this.primitives.delete(key);
    }
    public findPrimitive(key: string): Object3D {
        return this.primitives.get(key) || null;
    }


    public addButton(key: string, value: ThreeMeshUI.Block) {
        this.buttons.set(key, value);
    }
    public deleteButton(key: string) {
        this.buttons.delete(key);
    }
    public findButton(key: string): ThreeMeshUI.Block {
        return this.buttons.get(key) || null;
    }


    public getAllButtons(): any {
        return this.buttons.values();
    }


    public addPanel(key: string, value: ThreeMeshUI.Block) {
        this.panels.set(key, value);
    }
    public deletePanel(key: string) {
        this.panels.delete(key);
    }
    public findPanel(key: string): ThreeMeshUI.Block {
        return this.panels.get(key) || null;
    }   
}

export const ObjectLookup = new ObjectLookupClass();
