import { Group, Object3D, Scene } from 'three';
import { Text } from 'troika-three-text';
import { SceneBuilder } from '../Builders/SceneBuilder';
import { Transforms } from './Transforms';
import { MenuBuilder } from '../Builders/MenuBuilder';
import { TextPanelBuilder } from '../Builders/TextPanelBuilder';
import { PanelGroupBuilder } from '../Builders/PanelGroupBuilder';

export class SceneStateClass {
    private primitives = new Map<string, Object3D>();
    private gltfs = new Map<string, Group>();
    private gltfURLs = new Map<string, Group>();
    // private gltfs = new Map<string, GLTF>();
    // private gltfURLs = new Map<string, GLTF>();
    private labels = new Map<string, Text>();

    public addPrimitive(key: string, value: Object3D) {
        this.primitives.set(key, value);

        value.children.forEach((child) => {
            this.addPrimitive(child.uuid, child);
        });
    }

    public deletePrimitive(key: string) {
        this.primitives.delete(key);
    }

    public findPrimitive(key: string): Object3D {
        return this.primitives.get(key) || null;
    }

    public addLabel(key: string, value: Text) {
        this.labels.set(key, value);
    }

    public deleteLabel(key: string) {
        this.labels.delete(key);
    }

    public findLabel(key: string): Text {
        return this.labels.get(key) || null;
    }

    public isExist(key: string): boolean {
        const primitive = this.findPrimitive(key);
        const label = this.findLabel(key);
        const gltf = this.findGLTFByGuid(key);

        return Boolean(primitive) || Boolean(label) || Boolean(gltf);
    }

    public addToScene(scene: Scene, item: Object3D | Text) {
        scene.add(item);
    }

    public establishGLTF(scene: Scene, url: string, guid: string, group: Group) {
        group.uuid = guid;
        this.gltfs.set(guid, group);
        this.gltfURLs.set(url, group);
        // const gltfScene = this.findGLTFByGuid(guid).scene;
        const gltfScene = this.findGLTFByGuid(guid);
        scene.add(gltfScene);
    }

    // public establishGLTF(scene: Scene, url: string, guid: string, object: GLTF) {
    //     object.scene.uuid = guid;
    //     this.gltfs.set(guid, object);
    //     this.gltfURLs.set(url, object);
    //     const gltfScene = this.findGLTFByGuid(guid).scene;
    //     scene.add(gltfScene);
    // }

    public establishClone(scene: Scene, guid: string, clone: Group) {
        clone.uuid = guid;
        scene.add(clone);
    }

    public findGLTFByGuid(guid: string): Group | null {
        return this.gltfs.get(guid) || null;
    }

    public findGLTFToCloneByGuid(guid: string): Group | null {
        const groupedGltf = this.gltfs.get(guid);
        if (Boolean(groupedGltf)) {
            const gltf = groupedGltf.children[0].children[0] as Group;
            return gltf;
        }
        return null;
    }

    // public findGLTFByGuid(guid: string): GLTF | null {
    //     return this.gltfs.get(guid) || null;
    // }

    public findGLTFByURL(url: string): Group | null {
        return this.gltfURLs.get(url) || null;
    }

    // public findGLTFByURL(url: string): GLTF | null {
    //     return this.gltfURLs.get(url) || null;
    // }

    public gltfNotLoaded(url: string, guid: string): boolean {
        const foundByGuid = this.findGLTFByGuid(guid);
        const foundByURL = this.findGLTFByURL(url);
        return !Boolean(foundByGuid) && !Boolean(foundByURL);
    }

    public deleteGLTF(key: string) {
        this.gltfs.delete(key);
    }

    public renderToScene(scene: Scene, options: any) {
        this.primitives.forEach((value) => {
            scene.add(value);
        });
        this.gltfs.forEach((value) => {
            // scene.add(value.scene);
            scene.add(value);
        });
        this.labels.forEach((value) => {
            scene.add(value);
        });
        SceneBuilder.BuildMenus(scene, options);
    }

    public removeItem(scene: Scene, uuid: string) {
        let obj = scene.getObjectByProperty('uuid', uuid);
        console.log('removeItem obj=', obj);
        if (obj) {
            scene.remove(obj);
        }
        this.deleteLabel(uuid);
        this.deletePrimitive(uuid);
        this.deleteGLTF(uuid);
        return true;
    }

    public clearScene(scene: Scene, sceneOptions: any, onClearScene: Function) {
        // const removalList: Array<Text | Object3D> = [];
        // const refreshList = this.itemsToUpdate(sceneOptions);
        // refreshList.forEach((option: any) => {
        //     this.queueItemRemoval(scene, option, removalList);
        // });

        // this.gltfs.forEach((value, key) => {
        //     removalList.push(value.scene);
        // });

        scene.clear();

        this.labels.clear();
        this.primitives.clear();
        this.gltfs.clear();
        onClearScene();
    }

    // We don't clear the entire scene so we can retain 3D models like GLTF and FBX.  The 3D models are very expensive to reload.
    public refreshScene(scene: Scene, sceneOptions: any) {
        const refreshList = this.itemsToUpdate(sceneOptions);
        this.refresh(scene, refreshList);
        MenuBuilder.BuildMenus(scene, sceneOptions);
        TextPanelBuilder.BuildTextPanels(scene, sceneOptions);
        PanelGroupBuilder.BuildPanelGroup(scene, sceneOptions);
    }

    public moveObject(scene: Scene, object3D: Object3D): Object3D {
        const uuid = object3D.uuid;
        const item = scene.getObjectByProperty('uuid', uuid);

        if (Boolean(item)) {
            Transforms.setPosition(item, object3D.position);
            Transforms.setRotation(item, object3D.rotation);
        }

        // this.addPrimitive(uuid, object3D);
        return object3D;
    }

    private itemsToUpdate(sceneOptions: any): any[] {

        let options = [];
        if (Boolean(sceneOptions.children)) {
            options = sceneOptions.children.filter((childOptions: any) => {
                return 'LabelText|Mesh|Group'.indexOf(childOptions.type) >= 0;
            });
        }
        return options;
    }

    private queueItemRemoval(scene: Scene, option: any, removalList: Array<Text | Object3D>) {
        const { type } = option;
        if (type === 'LabelText') {
            this.queueLabelTextRemoval(scene, option.uuid, removalList);
        } else if (type === 'Mesh') {
            this.queueMeshRemoval(scene, option.uuid, removalList);
        }
    }

    private queueLabelTextRemoval(scene: Scene, uuid: string, removalList: Array<Text | Object3D>) {
        const item = this.findLabel(uuid);
        removalList.push(item);
        this.deleteLabel(uuid);
    }

    private queueMeshRemoval(scene: Scene, uuid: string, removalList: Array<Text | Object3D>) {
        const item = this.findPrimitive(uuid);
        removalList.push(item);
        this.deletePrimitive(uuid);
    }

    private refresh(scene: Scene, refreshList: Array<Text | Object3D | Group>) {
        refreshList.forEach((updatedOption: any) => {
            if (this.isExist(updatedOption.uuid)) {
                SceneBuilder.UpdateChild(updatedOption, scene);
            } else {
                const item = SceneBuilder.BuildChild(scene, updatedOption);
                if (Boolean(item)) this.addToScene(scene, item);
                // else this.addToScene(scene, item.scene);
            }
        });
    }
}

export const SceneState = new SceneStateClass();
