import { OBJLoader } from 'three/examples/jsm/loaders/OBJLoader';
import { ColladaLoader } from 'three/examples/jsm/loaders/ColladaLoader';
import { FBXLoader } from 'three/examples/jsm/loaders/FBXLoader';
import { GLTF, GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader';
import { STLLoader } from 'three/examples/jsm/loaders/STLLoader.js';
import { MaterialBuilder } from '../Builders/MaterialBuilder';
import { Transforms } from '../Utils/Transforms';
import { AnimationMixer, Box3, Group, AnimationClip, LoadingManager, Mesh, Object3D, Scene, TextureLoader, Vector3 } from 'three';
import { ObjectLookup } from '../Utils/ObjectLookup';
//import { GUI } from 'dat.gui';

//const gui = new GUI();

export class Loaders {
    

    private createGLTFGroups(gltfScene: Group, options: any): Group {
        
        const group = new Group();
        group.uuid = options.uuid;
        gltfScene.name = `name-${group.uuid}`;
        group.add(gltfScene);

        
        group.userData = { isGLTFGroup: true, uuid: options.uuid };
        
        Transforms.setTransform(group, options.transform);

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

    private CaptureAnimations(animations: AnimationClip[], group: Group) {

        animations?.forEach((animation) => {
            if (Boolean(animation) && Boolean(animation.tracks.length)) {
                const mixer = new AnimationMixer(group);
                const animationAction = mixer.clipAction(animation);
                animationAction.play();

                ObjectLookup.addMixer(mixer);
            }
        });
    }

    private loadGltf(url: string, guid: string, member: any, onComplete: (gltf: GLTF, group: Group) => void) {
        console.log('inside loadGltf', url, guid, member);

        const loader = new GLTFLoader();
        loader.load(url, 
            (gltf: GLTF) => {
                console.log('gltf is now loaded', gltf);

                ObjectLookup.casheGLTF(url, gltf);

                const clone = gltf.scene;
                const group = this.createGLTFGroups(clone, member);

                onComplete(gltf, group);
        }, 
        (xhr) => {}, 
        (error) => {
            console.error('Error loading GLTF', error);
        });
    }

    public import3DModel(member: any, onComplete: (gltf: GLTF, group: Group) => void) {
        const format = member.format;
        let uuid = member.uuid;
        let url = member.url;

        console.log('In loader import3DModel', member);

        if (format == 'Gltf') {

            var found = ObjectLookup.findGLTF(url);
            if (Boolean(found)) {
                console.log('gltf is found in cashe', found);

                const clone = found.scene.clone();
                const group = this.createGLTFGroups(clone, member);

                this.CaptureAnimations(found.animations,group)
                onComplete(found, group);
            }
            else {
                this.loadGltf(url, uuid, member, (gltf: GLTF, group: Group) =>
                {
                    this.CaptureAnimations(gltf.animations,group)
                    onComplete(gltf, group);
                });
            }
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





}
