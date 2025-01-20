import { OBJLoader } from 'three/examples/jsm/loaders/OBJLoader';
import { ColladaLoader } from 'three/examples/jsm/loaders/ColladaLoader';
import { FBXLoader } from 'three/examples/jsm/loaders/FBXLoader';
import { GLTF, GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader';
import { STLLoader } from 'three/examples/jsm/loaders/STLLoader.js';
import { MaterialBuilder } from '../Builders/MaterialBuilder';
import { Box3, Group, LoadingManager, Mesh, Object3D, Scene, TextureLoader, Vector3 } from 'three';
import { ObjectLookup } from '../Utils/ObjectLookup';
//import { GUI } from 'dat.gui';

//const gui = new GUI();

export class Loaders {
    
    //private LoadedObjectComplete(uuid: string) {
    //    DotNet.invokeMethodAsync('BlazorThreeJS', 'LoadedObjectComplete', uuid);
    //}

    private assignPosition(object: Group | Object3D, transform: any) {
        const { x, y, z } = transform.position;
        object.position.x = x;
        object.position.y = y;
        object.position.z = z;
        return object;
    }

    private assignRotation(object: Group | Object3D, transform: any) {
        const { x, y, z } = transform.rotation;
        object.rotation.x = x;
        object.rotation.y = y;
        object.rotation.z = z;
        return object;
    }

    private assignScale(object: Group | Object3D, transform: any) {
        const { x, y, z } = transform.scale;
        object.scale.x = x * object.scale.x;
        object.scale.y = y * object.scale.y;
        object.scale.z = z * object.scale.z;
        return object;
    }

    private setGLTFSceneProps(gltfScene: Group, guid: string, options: any): Group {
        gltfScene.name = `name-${guid}`;

        const group = new Group();
        group.uuid = options.uuid;

        var transform = options.transform;
        if (Boolean(options.pivot) && Boolean(transform)) {
            this.assignPosition(gltfScene, { position: transform.pivot });
        }
        group.add(gltfScene);
        if ( Boolean(transform) ) {
            this.assignScale(group, transform);
            this.assignPosition(group, transform);
            this.assignRotation(group, transform);
        }
        return group;
    }

    // private addDebuggerWindow(url: string, group: Group) {
    //     const guiName = url.split('/').reverse()[0];
    //     const gltfFolder = gui.addFolder(guiName);
    //     gltfFolder.add(group.rotation, 'x', 0, Math.PI * 2);
    //     gltfFolder.add(group.rotation, 'y', 0, Math.PI * 2);
    //     gltfFolder.add(group.rotation, 'z', 0, Math.PI * 2);
    //     gltfFolder.open();
    // }

    private loadGltf(url: string, guid: string, member: any, onComplete: Function) {
        
        console.log('inside loadGltf', url, guid, member);
        var found = ObjectLookup.findGLTF(url);

        if (Boolean(found) == false) {
            console.log('gltf is not loaded', url);
            const loader = new GLTFLoader();
            loader.load(url, 
                (gltf: GLTF) => {
                    console.log('gltf is now loaded', gltf);

                    //animationCallBack(gltf);
                    ObjectLookup.casheGLTF(url, gltf);

                    const clone = gltf.scene; //.clone();
                    const group = this.setGLTFSceneProps(clone, guid, member);

                    const box = new Box3().setFromObject(group);
                    const size = box.getSize(new Vector3());
                    group.userData = { isGLTFGroup: true, url, uuid: guid, size };
                    // console.log('userData', group.userData);

                    onComplete(group);
            }, 
            (xhr) => {}, 
            (error) => {
                console.error('Error loading GLTF', error);
            });

        } else {
            // Found GLTF by URL or GUID so we don't want to load it again.
            var gltf = found as GLTF;
            if (Boolean(gltf)) {
                console.log('gltf is found in cashe', gltf);

                const clone = gltf.scene.clone();
                const group = this.setGLTFSceneProps(clone, guid, member);

                const box = new Box3().setFromObject(group);
                const size = box.getSize(new Vector3());
                group.userData = { isGLTFGroup: true, url, uuid: guid, size: size };
                // console.log('clone userData', group.userData);
                onComplete(group);
            }
        }
    }



    // private loadFbx(url: string, guid: string, options: any, onComplerCallback: Function) {
    //     const loader = new FBXLoader();
    //     loader.load(url, (object) => {
    //         const group = this.setGLTFSceneProps(object, guid, options);
    //         ObjectLookup.casheGroup(guid, group);
    //         onComplerCallback(group);
    //     });
    // }

    // private loadCollada(url: string, guid: string, options: any, onComplerCallback: Function) {

    //     const manager = new LoadingManager(() => {

    //     });
    //     const loader = new ColladaLoader(manager);
    //     loader.load(url, (obj) => {
    //         var source = obj.scene.clone();
    //         const group = this.setGLTFSceneProps(source, guid, options);
    //         ObjectLookup.casheGroup(guid, group);
    //         onComplerCallback(group);
    //     });
    // }

    // private loadOBJ(objUrl: string, textureUrl: string, guid: string, options: any) {
    //     let object: Object3D;
    //     const manager = new LoadingManager(() => {
    //         if (textureUrl) {
    //             object.traverse((child: any) => {
    //                 if (child.isMesh) child.material.map = texture;
    //             });
    //         }
    //         scene.add(object);
    //         object.uuid = guid;
    //         this.ReceiveLoadedCallback(containerId, guid);
    //     });

    //     const textureLoader = new TextureLoader(manager);
    //     const texture = textureLoader.load(textureUrl);

    //     const loader = new OBJLoader(manager);
    //     loader.load(objUrl, (obj) => {
    //         object = obj;
    //     });
    //     return object;
    // }

    // private loadStl(scene: Scene, url: string, guid: string, containerId: string, materialSettings: any, options: any) {
    //     let mesh: Mesh;
    //     const loader = new STLLoader();
    //     loader.load(url, function (geometry) {
    //         // const material = new MeshPhongMaterial( { color: 0xff5533, specular: 0x111111, shininess: 200 } );
    //         // const material = new MeshStandardMaterial({
    //         //   color: 0xff5533,
    //         // });
    //         const material = MaterialBuilder.buildMaterial(materialSettings);
    //         mesh = new Mesh(geometry, material);
    //         mesh.uuid = guid;

    //         // mesh.position.set( 0, - 0.25, 0.6 );
    //         // mesh.rotation.set( 0, - Math.PI / 2, 0 );
    //         // mesh.scale.set( 0.5, 0.5, 0.5 );

    //         // mesh.castShadow = true;
    //         // mesh.receiveShadow = true;

    //         scene.add(mesh);
    //         this.callDotNet(containerId, guid);
    //     });
    // }

    public import3DModel(member: any, onComplete: Function) {
        const format = member.format;
        let uuid = member.uuid;
        let url = member.url;


        console.log('In loader import3DModel', member);

        if (format == 'Gltf') {
            console.log('Calling loadGltf', member);
            this.loadGltf(url, uuid, member, onComplete);
        }

        // else if (format == 'Obj') {
        //     this.loadOBJ(scene, objUrl, textureUrl, guid, containerId, settings);
        // }
        // else if (format == 'Collada') {
        //     this.loadCollada(scene, objUrl, guid, containerId, settings);
        // }
        // else if (format == 'Fbx') {
        //     this.loadFbx(scene, objUrl, guid, containerId, settings);
        // }

        // else if (format == 'Stl') {
        //     this.loadStl(scene, objUrl, guid, containerId, material, settings);
        // }
    }



}
