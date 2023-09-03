import { OBJLoader } from 'three/examples/jsm/loaders/OBJLoader';
import { ColladaLoader } from 'three/examples/jsm/loaders/ColladaLoader';
import { FBXLoader } from 'three/examples/jsm/loaders/FBXLoader';
import { GLTF, GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader';
import { STLLoader } from 'three/examples/jsm/loaders/STLLoader.js';
import { MaterialBuilder } from '../Builders/MaterialBuilder';
import { Box3, Group, LoadingManager, Mesh, Object3D, Scene, TextureLoader, Vector3 } from 'three';
import { SceneState } from '../Utils/SceneState';
// import { GUI } from 'dat.gui';

// const gui = new GUI();
export class Loaders {
    private assignPosition(object: Group | Object3D, options: any) {
        const { x, y, z } = options.position;
        object.position.x = x;
        object.position.y = y;
        object.position.z = z;

        return object;
    }

    private assignRotation(object: Group | Object3D, options: any) {
        const { x, y, z } = options.rotation;
        object.rotation.x = x;
        object.rotation.y = y;
        object.rotation.z = z;

        return object;
    }

    private setGLTFSceneProps(gltfScene: Group, guid: string, options: any): Group {
        gltfScene.name = `name-${guid}`;
        if (Boolean(options.pivot)) {
            this.assignPosition(gltfScene, { position: options.pivot });
            const group = new Group();
            group.uuid = options.uuid;
            group.add(gltfScene);
            this.assignPosition(group, options);
            this.assignRotation(group, options);
            return group;
        } else {
            this.assignPosition(gltfScene, options);
            this.assignRotation(gltfScene, options);
        }
        return gltfScene;
    }

    // private addDebuggerWindow(url: string, group: Group) {
    //     const guiName = url.split('/').reverse()[0];
    //     const gltfFolder = gui.addFolder(guiName);
    //     gltfFolder.add(group.rotation, 'x', 0, Math.PI * 2);
    //     gltfFolder.add(group.rotation, 'y', 0, Math.PI * 2);
    //     gltfFolder.add(group.rotation, 'z', 0, Math.PI * 2);
    //     gltfFolder.open();
    // }

    private loadGltf(
        scene: Scene,
        url: string,
        guid: string,
        containerId: string,
        options: any,
        animationCallBack: Function
    ) {
        if (SceneState.gltfNotLoaded(url, guid)) {
            const loader = new GLTFLoader();
            loader.load(url, (gltf: GLTF) => {
                const group = this.setGLTFSceneProps(gltf.scene, guid, options);

                const box = new Box3().setFromObject(group);
                const size = box.getSize(new Vector3());

                group.userData = { isGLTFGroup: true, url, uuid: guid, size };
                // this.addDebuggerWindow(url, group);
                SceneState.establishGLTF(scene, url, guid, group);
                animationCallBack(gltf);
                this.callDotNet(containerId, guid);
            });
        } else {
            // Found GLTF by URL or GUID so we don't want to load it again.
            const gltf = SceneState.findGLTFByURL(url);
            if (Boolean(gltf)) {
                const clone = gltf.clone();
                // this.addDebuggerWindow(url, clone);
                this.setGLTFSceneProps(clone, guid, options);
                SceneState.establishClone(scene, guid, clone);
                this.callDotNet(containerId, guid);
            }
        }
    }

    private cloneGltf(scene: Scene, sourceGuid: string, containerId: string, cloneOptions: any[]) {
        const gltf = SceneState.findGLTFToCloneByGuid(sourceGuid);
        if (Boolean(gltf)) {
            cloneOptions.forEach((options) => {
                const clone = gltf.clone();
                const cloneGuid = options.uuid;
                const group = this.setGLTFSceneProps(clone, cloneGuid, options);
                SceneState.establishClone(scene, cloneGuid, group);
                this.callDotNet(containerId, cloneGuid);
            });
        } else {
            console.log('CloneGLTF: GLTF not found in SceneState sourceGuid, cloneOptions=', sourceGuid, cloneOptions);
        }
    }

    private loadFbx(scene: Scene, url: string, guid: string, containerId: string, options: any) {
        const loader = new FBXLoader();
        loader.load(url, (object) => {
            object.uuid = guid;
            scene.add(object);
            this.assignPosition(object, options);
            this.assignRotation(object, options);

            this.callDotNet(containerId, guid);
        });
    }

    private loadCollada(scene: Scene, url: string, guid: string, containerId: string, options: any) {
        let object: Object3D;
        const manager = new LoadingManager(() => {
            scene.add(object);
            object.uuid = guid;
            this.callDotNet(containerId, guid);
        });
        const loader = new ColladaLoader(manager);
        loader.load(url, (obj) => {
            object = obj.scene;
        });
    }

    private loadOBJ(scene: Scene, objUrl: string, textureUrl: string, guid: string, containerId: string, options: any) {
        let object: Object3D;
        const manager = new LoadingManager(() => {
            if (textureUrl) {
                object.traverse((child: any) => {
                    if (child.isMesh) child.material.map = texture;
                });
            }
            scene.add(object);
            object.uuid = guid;
            this.callDotNet(containerId, guid);
        });

        const textureLoader = new TextureLoader(manager);
        const texture = textureLoader.load(textureUrl);

        const loader = new OBJLoader(manager);
        loader.load(objUrl, (obj) => {
            object = obj;
        });
        return object;
    }

    private loadStl(scene: Scene, url: string, guid: string, containerId: string, materialSettings: any, options: any) {
        let mesh: Mesh;
        const loader = new STLLoader();
        loader.load(url, function (geometry) {
            // const material = new MeshPhongMaterial( { color: 0xff5533, specular: 0x111111, shininess: 200 } );
            // const material = new MeshStandardMaterial({
            //   color: 0xff5533,
            // });
            const material = MaterialBuilder.buildMaterial(materialSettings);
            mesh = new Mesh(geometry, material);
            mesh.uuid = guid;

            // mesh.position.set( 0, - 0.25, 0.6 );
            // mesh.rotation.set( 0, - Math.PI / 2, 0 );
            // mesh.scale.set( 0.5, 0.5, 0.5 );

            // mesh.castShadow = true;
            // mesh.receiveShadow = true;

            scene.add(mesh);
            this.callDotNet(containerId, guid);
        });
    }

    public import3DModel(scene: Scene, settings: any, containerId: string, animationCallBack: Function) {
        const format = settings.format;
        let objUrl = settings.fileURL;
        let textureUrl = settings.textureURL;
        let guid = settings.uuid;
        let material = settings.material;

        if (format == 'Obj') {
            return this.loadOBJ(scene, objUrl, textureUrl, guid, containerId, settings);
        }
        if (format == 'Collada') {
            return this.loadCollada(scene, objUrl, guid, containerId, settings);
        }
        if (format == 'Fbx') {
            return this.loadFbx(scene, objUrl, guid, containerId, settings);
        }
        if (format == 'Gltf') {
            return this.loadGltf(scene, objUrl, guid, containerId, settings, animationCallBack);
        }

        if (format == 'Stl') {
            return this.loadStl(scene, objUrl, guid, containerId, material, settings);
        }

        return null;
    }

    public clone3DModel(scene: Scene, sourceGuid: string, settings: any[], containerId: string) {
        // const format = settings.format;
        // let objUrl = settings.fileURL;
        // let textureUrl = settings.textureURL;
        // let guid = settings.uuid;
        // let material = settings.material;

        // if (format == 'Obj') {
        //     return this.loadOBJ(scene, objUrl, textureUrl, guid, containerId, settings);
        // }
        // if (format == 'Collada') {
        //     return this.loadCollada(scene, objUrl, guid, containerId, settings);
        // }
        // if (format == 'Fbx') {
        //     return this.loadFbx(scene, objUrl, guid, containerId, settings);
        // }
        // if (format == 'Gltf') {
        return this.cloneGltf(scene, sourceGuid, containerId, settings);
        // }

        // if (format == 'Stl') {
        //     return this.loadStl(scene, objUrl, guid, containerId, material, settings);
        // }

        // return null;
    }

    private callDotNet(containerId: string, uuid: string) {
        DotNet.invokeMethodAsync('BlazorThreeJS', 'ReceiveLoadedObjectUUID', containerId, uuid);
    }
}
