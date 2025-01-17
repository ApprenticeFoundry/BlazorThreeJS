import { Transforms } from '../Utils/Transforms';
import { Text } from 'troika-three-text';
import { Loaders } from './Loaders';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls';
import { BoxLineGeometry } from 'three/examples/jsm/geometries/BoxLineGeometry';
import { ObjectLookup } from '../Utils/ObjectLookup';
import { CameraBuilder } from '../Builders/CameraBuilder';
import { MeshBuilder } from '../Builders/MeshBuilder';
import { MenuBuilder } from '../Builders/MenuBuilder';

import {
    AnimationMixer,
    Clock,
    Color,
    GridHelper,
    LineBasicMaterial,
    LineSegments,
    Object3D,
    OrthographicCamera,
    PerspectiveCamera,
    Raycaster,
    Scene,
    Vector2,
    Vector3,
    WebGLRenderer,
    Event as ThreeEvent,
    Group,
    BoxGeometry,
    CylinderGeometry,
    MeshBasicMaterial,
    Mesh,
} from 'three';


import ThreeMeshUI from 'three-mesh-ui';
import { GLTF, GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader';
import { TextPanelBuilder } from '../Builders/TextPanelBuilder';
import { PanelGroupBuilder } from '../Builders/PanelGroupBuilder';
import { LightBuilder } from '../Builders/LightBuilder';
import { HelperBuilder } from '../Builders/HelperBuilder';

export class Viewer3D {
    private options: any;
    private container: any;
    private settings: any;
    private webGLRenderer: WebGLRenderer;
    private scene: Scene;
    private camera: OrthographicCamera | PerspectiveCamera;
    private controls: OrbitControls;
    private mouse: Vector2 = new Vector2();
    private raycaster: Raycaster = new Raycaster();
    private uiElementSelectState = false;
    private lastSelectedGuid = null;
    private animationMixers: Array<AnimationMixer> = [];
    private clock: Clock;
    private blockTest: Mesh;

    private INTERSECTED: any = null;
    private HasLoaded = false;
    public AnimationRequest: any = null;

    private LoadedObjectComplete(uuid: string) {
        DotNet.invokeMethodAsync('BlazorThreeJS', 'LoadedObjectComplete', uuid);
    }
    private onObjectSelected(uuid: string) {
        DotNet.invokeMethodAsync('BlazorThreeJS', 'OnClickButton', uuid);
    }

    public Initialize3DViewer(spec: string) {
        if ( this.HasLoaded ) return;
        this.HasLoaded = true;
        this.clock = new Clock();

        console.log('In Initialize3DViewer');

        const options = JSON.parse(spec);

        this.setListeners();
        this.settings = options.viewerSettings;

        let container = document.getElementById(this.settings.containerId) as HTMLDivElement;

        if (!container) {
            console.warn('Container not found');
            return;
        }

        this.options = options;
        this.container = container;

        this.scene = new Scene();
        this.InitializeScene(this.scene, options);
        this.setCamera();

        this.webGLRenderer = new WebGLRenderer({
            antialias: this.settings.webGLRendererSettings.antialias,
            preserveDrawingBuffer: true
        });

        const requestedWidth = this.settings.width;
        const requestedHeight = this.settings.height;
        if (Boolean(requestedWidth) && Boolean(requestedHeight)) {
            this.webGLRenderer.setSize(requestedWidth, requestedHeight, true);
        }
        else {
            this.webGLRenderer.domElement.style.width = '100%';
            this.webGLRenderer.domElement.style.height = '100%';
        }

        this.container.appendChild(this.webGLRenderer.domElement);

        // this.renderer.domElement.onclick = (event) => {
        //     if (this.options.viewerSettings.canSelect == true) {
        //         this.selectObject(event);
        //     }
        //     if (this.options.camera.animateRotationSettings.stopAnimationOnOrbitControlMove == true) {
        //         this.options.camera.animateRotationSettings.animateRotation = false;
        //     }
        // };


        // this.addTestText('How do we pass text values?');

        this.setOrbitControls();
        this.onResize();

        //this.blockTest = this.GeomExample();
        this.StartAnimation();

        console.log('Exit Initialize3DViewer');
    }

    public InitializeScene(scene: Scene, options: any) {
        // console.log('in setScene this.options=', this.options);
        scene.background = new Color(options.scene.backGroundColor);
        scene.uuid = options.scene.uuid;

        //add the floor
        const grid = new GridHelper(30, 30, 0x848484, 0x848484);
        scene.add(grid);

        // this.addAxes();  we should control this from FoundryBlazor by default
        //this.addRoom();

        if (Boolean(options.scene.children))
        {
            console.log('In InitializeScene options.scene.children=', options.scene.children);
            this.establish3DChildren(options.scene);
        }
    }

    //clear out animation
    public Finalize3DViewer() 
    {
        console.log('In Finalize3DViewer');
        this.StopAnimation();
    }

    private RenderJS(self: any) 
    {
        if ( self.AnimationRequest == null ) return;

        // if ( this.blockTest != null) {
        //     this.blockTest.rotation.x += 0.01;
        //     this.blockTest.rotation.y += 0.01;
        //     this.blockTest.rotation.z += 0.01;
        // }

        // request another animation frame
        try {
            DotNet.invokeMethodAsync('BlazorThreeJS', 'TriggerAnimationFrame');  
            self.AnimationRequest = window.requestAnimationFrame(() => self.RenderJS(self));
            self.render();
        } catch (error) {
            console.log('Error in RenderJS', error); 
        }
    }

    private render() {
        ThreeMeshUI.update();
        this.updateUIElements();
        this.selectObject();

        for (let i = 1, l = this.scene.children.length; i < l; i++) {
            const item = this.scene.children[i];
            if (item.userData.isTextLabel) {
                item.lookAt(this.camera.position);
            }
        }

        var delta = this.clock.getDelta();
        if (Boolean(this.animationMixers.length)) {
            for (const mixer of this.animationMixers) {
                mixer.update(delta);
            }
        }
        this.webGLRenderer.render(this.scene, this.camera);
    }

    public StartAnimation() {
        console.log('In StartAnimation');
        if (this.AnimationRequest == null)
            this.AnimationRequest = window.requestAnimationFrame(() => {

                this.RenderJS(this);
            });
    }

    public StopAnimation() {
        console.log('In StopAnimation');
        if (this.AnimationRequest != null) 
            window.cancelAnimationFrame(this.AnimationRequest);

        this.AnimationRequest = null;
    }

    public deleteFromScene(uuid: string):boolean {
        let obj = this.scene.getObjectByProperty('uuid', uuid);
        console.log('deleteFromScene obj=', obj);
        if (obj) {
            this.scene.remove(obj);
            return true;
        }
        return false
    }



    private setListeners() {
        window.addEventListener('pointermove', (event: PointerEvent) => {
            let canvas = this.webGLRenderer.domElement;

            this.mouse.x = (event.offsetX / canvas.clientWidth) * 2 - 1;
            this.mouse.y = -(event.offsetY / canvas.clientHeight) * 2 + 1;
        });

        window.addEventListener('pointerdown', () => {
            this.selectObject();
            this.uiElementSelectState = true;
        });

        window.addEventListener('pointerup', () => {
            this.uiElementSelectState = false;
        });
    }

    private onResize() {
        // OrthographicCamera does not have aspect property
        if (this.camera.type === 'PerspectiveCamera') {
            this.camera.aspect = this.container.offsetWidth / this.container.offsetHeight;
        }

        if (this.camera.type === 'OrthographicCamera' && this.options && this.options.camera) {
            this.camera.left = this.options.camera.left;
            this.camera.right = this.options.camera.right;
            // OrthographicCamera does not have aspect property
            // this.camera.left = this.options.camera.left * this.camera.aspect;
            // this.camera.right = this.options.camera.right * this.camera.aspect;
        }

        this.camera.updateProjectionMatrix();

        this.webGLRenderer.setSize(
            this.container.offsetWidth,
            this.container.offsetHeight,
            false // required
        );
    }

    // public updateScene(spec: string) {
    //     //console.log('inside updateScene spec=', spec);
    //     const options = JSON.parse(spec);
    //     //console.log('updateScene sceneOptions=', options);
    //     this.options.scene = options;
    //     this.establish3DChildren(options);
    // }

    public GeomExample():Mesh
    {

        // Create box (parent)
        const boxGeometry = new BoxGeometry(2, 2, 2);
        const boxMaterial = new MeshBasicMaterial({ color: 0x00ff00, wireframe: false });
        const box = new Mesh(boxGeometry, boxMaterial);
        this.scene.add(box);

        // Create cylinder (child)
        const cylinderGeometry = new CylinderGeometry(0.5, 0.5, 3, 32);
        const cylinderMaterial = new MeshBasicMaterial({ color: 0xff0000, wireframe: false });
        const cylinder = new Mesh(cylinderGeometry, cylinderMaterial);

        // Position cylinder relative to box
        cylinder.position.set(1, 2, 1);  // Place cylinder on top of box

        // Add cylinder as child of box
        box.add(cylinder);
        return box;

    }

    public request3DScene(importSettings: string) {
        const options = JSON.parse(importSettings);
        
        console.log('request3DScene importSettings=', options);
        this.establish3DChildren(options);
    }

    //can we be smart here and call the correct method based on the type of object we are adding?
    public establish3DChildren(options: any) {
        
        var members = options.children;
        for (let index = 0; index < members.length; index++) {
            
            //console.log('updateScene element.type=', element.type);
            //console.log('updateScene element=', index, element);
            
            try {
                //add these back in when we have the builders
                //TextPanelBuilder.BuildTextPanels(scene, options);
                //PanelGroupBuilder.BuildPanelGroup(scene, options);
                
                const element = members[index];
                if (element.type == 'AmbientLight') {
                    var ambient = LightBuilder.BuildAmbientLight(options);
                    this.scene.add(ambient);
                } else
                if (element.type == 'PointLight') {
                    var light = LightBuilder.BuildPointLight(options);
                    this.scene.add(light);
                } else
                if ( element.type == 'Text3D' ) {
                    this.establish3DLabel(element);
                } else
                if ( element.type == 'Mesh3D' ) {
                    this.establish3DGeometry(element);
                } else
                if ( element.type == 'Group3D' ) {
                    this.establish3DGroup(element);
                } else
                if ( element.type == 'PanelMenu3D' ) {
                    this.establish3DMenu(element);
                } else
                if ( element.type == 'Model3D' ) {
                    this.establish3DModel(element);
                } else
                if (element.type.includes('Helper')) {
                    const obj = this.scene.getObjectByProperty('uuid', element.uuid);
                    var helper = HelperBuilder.BuildHelper(options, obj);
                    this.scene.add(helper);
                }
            } catch (error) {
                console.log('Error in establish3DChildren', error);
            }
        }

    }

    public setCamera() {
        const builder = new CameraBuilder();
        this.camera = builder.BuildCamera(
            this.options.camera,
            this.container.offsetWidth / this.container.offsetHeight
        );
    }

    public updateCamera(options: string) {
        const newCamera = JSON.parse(options) as OrthographicCamera | PerspectiveCamera;
        this.options.camera = newCamera;
        this.setCamera();
        this.setOrbitControls();
    }

    // public showCurrentCameraInfo() {
    //     console.log('Current camera info:', this.camera);
    //     console.log('Orbit controls info:', this.controls);
    // }

    private setOrbitControls() {
        this.controls = new OrbitControls(this.camera, this.webGLRenderer.domElement);
        this.controls.screenSpacePanning = true;
        this.controls.minDistance = this.options.orbitControls.minDistance;
        this.controls.maxDistance = this.options.orbitControls.maxDistance;
        let { x, y, z } = this.options.camera.lookAt;
        this.controls.target.set(x, y, z);
        this.controls.update();
    }

    private playGltfAnimation(model: GLTF) {
        const animations = model.animations;
        animations?.forEach((animation) => {
            if (Boolean(animation) && Boolean(animation.tracks.length)) {
                const mixer = new AnimationMixer(model.scene);
                this.animationMixers.push(mixer);
                const animationAction = mixer.clipAction(animation);
                animationAction.play();
            }
        });
    }

    public establish3DMenu(options: any): ThreeMeshUI.Block | null {

        const guid = options.uuid;

        var entity = ObjectLookup.findPanel(guid) as ThreeMeshUI.Block;
        var exist = Boolean(entity)
        entity = exist ? entity : MenuBuilder.CreateMenuPanel(options);

        MenuBuilder.RefreshMenuPanel(options, entity);

        if ( !exist )
        {
            this.scene.add(entity);
            ObjectLookup.addPanel(guid, entity);
            this.LoadedObjectComplete(guid);
            console.log('MenuPanel Added to Scene', entity);
        }
        return entity;
    }

    public establish3DGeometry(options: any): Object3D | null {

        const guid = options.uuid;

        var entity = ObjectLookup.findPrimitive(guid) as Object3D;
        var exist = Boolean(entity)
        entity = exist ? entity : MeshBuilder.CreateMesh(options);

        MeshBuilder.RefreshMesh(options, entity);

        if ( options.children && options.children.length > 0 ) {
            for (let index = 0; index < options.children.length; index++) {
                const child = options.children[index];
                this.establish3DChildMesh(child, entity);
            }
        }

        if ( !exist )
        {
            this.scene.add(entity);
            ObjectLookup.addPrimitive(guid, entity);
            this.LoadedObjectComplete(guid);
            console.log('Geometry Added to Scene', entity);
        }
        return entity;
    }

    public establish3DChildMesh(options: any, parent: Object3D): Object3D | null {

        const guid = options.uuid;

        var entity = ObjectLookup.findPrimitive(guid) as Object3D;
        var exist = Boolean(entity)
        if ( !exist ) {
            entity = MeshBuilder.CreateMesh(options);
            ObjectLookup.addPrimitive(guid, entity);
            parent.add(entity);
        }
        
        MeshBuilder.RefreshMesh(options, entity);
        //MeshBuilder.RefreshMesh(options, entity);

        console.log('establish3DChildMesh Child', entity);


        return entity;
    }



    //spec is always a importSettings
    public request3DGeometry(importSettings: string): Object3D | null {
        const options = JSON.parse(importSettings);
        if ( options.type != 'ImportSettings' ) return null;
        
        console.log('request3DGeometry importSettings=', options);
        var member = options.children[0];
        console.log('request3DGeometry members=', member);
        var geometry = this.establish3DGeometry(member);
        return geometry;
    }

    private establish3DGroup(options: any): Group | null {
        console.log('establish3DGroup modelOptions=', options);
        return null;
    }

    private establish3DLabel(options: any): Text | null {
        console.log('establish3DLabel modelOptions=', options);

        const guid = options.uuid;

        var label = ObjectLookup.findLabel(guid) as Text;
        var exist = Boolean(label)
        label = exist ? label : new Text();

        label.uuid = guid;
        label.text = options.text;
        label.color = options.color;
        label.fontSize = options.fontSize;
        label.userData = { isTextLabel: true, };

        const { x, y, z } = options.transform.position  as Vector3;
        label.position.x = x;
        label.position.y = y;
        label.position.z = z;

        // Update the rendering:
        label.sync();
        
        if ( !exist )
        {
            this.scene.add(label);
            ObjectLookup.addLabel(guid, label);
            this.LoadedObjectComplete(guid);
            console.log('label Added to Scene', label);
        }

        return label;
    }

    //spec is always a importSettings
    public request3DLabel(importSettings: string): Text | null {
        const options = JSON.parse(importSettings);
        if ( options.type != 'ImportSettings' ) return null;

        console.log('request3DLabel modelOptions=', options);
        var member = options.children[0];
        console.log('request3DLabel members=', member);
        var label = this.establish3DLabel(member);
        return label;
    }

    public establish3DModel(options: any) {
        console.log('establish3DModel modelOptions=', options);
        

        const loaders = new Loaders();
        loaders.import3DModel(options, (model: GLTF) => this.playGltfAnimation(model),
            (group) => {
                //this.addDebuggerWindow(url, group);
                this.scene.add(group);
                ObjectLookup.addGroup(group.uuid, group);
                this.LoadedObjectComplete(group.uuid);
                console.log('Group Added to Scene', group);
            })
    }

    //spec is always a importSettings
    public request3DModel(importSettings: string) {
        const options = JSON.parse(importSettings);
        if ( options.type != 'ImportSettings' ) return null;

        console.log('request3DModel modelOptions=', options);
        this.establish3DModel(options);
    }



    // public getSceneItemByGuid(guid: string):string {
    //     let item = this.scene.getObjectByProperty('uuid', guid);
    //     const json = {
    //         uuid: item.uuid,
    //         type: item.type,
    //         name: item.name,
    //         children: item.type == 'Group' ? this.iterateGroup(item.children) : [],
    //     };
    //     return JSON.stringify(json);
    // }

    // private iterateGroup(children: any[]) {
    //     let result = [];
    //     for (let i = 0; i < children.length; i++) {
    //         result.push({
    //             uuid: children[i].uuid,
    //             type: children[i].type,
    //             name: children[i].name,
    //             children: children[i].type == 'Group' ? this.iterateGroup(children[i].children) : [],
    //         });
    //     }
    //     return result;
    // }

    private findRootGuid(item: Object3D<ThreeEvent>): Object3D<ThreeEvent> {
        const userData = item.userData;
        if (userData.isGLTFGroup) return item;

        if (item.parent !== null) return this.findRootGuid(item.parent);
        return null;
    }

    private selectObject() {
        let intersect: any = null;
        let allButtons = ObjectLookup.getAllButtons();

        if (this.mouse.x !== null && this.mouse.y !== null) {
            this.raycaster.setFromCamera(this.mouse, this.camera);
            intersect = this.raycast(Array.from(allButtons));
        }

        const intersects = this.raycaster.intersectObjects(this.scene.children, true);

        // Ignore object selection if this is a UI element.  UI elements are handled in updateUIElements
        if (intersect && intersect.object.isUI) {
            return;
        } else {
            if (intersects.length === 0) {
                this.INTERSECTED = null;
                //DotNet.invokeMethodAsync('BlazorThreeJS','ReceiveSelectedObjectUUID', this.INTERSECTED.uuid, size);
                return;
            }

            this.INTERSECTED = null;
            for (let value of intersects) {
                this.INTERSECTED = this.findRootGuid(value.object);
                if (this.INTERSECTED !== null) break;
            }
            if (Boolean(this.INTERSECTED) && Boolean(this.INTERSECTED.userData)) {
                console.log('this.INTERSECTED=', this.INTERSECTED);
                const size: Vector3 = this.INTERSECTED.userData.size;

                // So a better job SRS  2021-09-29
                //DotNet.invokeMethodAsync('BlazorThreeJS', 'ReceiveSelectedObjectUUID', this.INTERSECTED.uuid, size);
            }
        }
    }

    public setCameraPosition(position: Vector3, lookAt: Vector3) {
        Transforms.setPosition(this.camera, position);
        if (lookAt != null && this.controls && this.controls.target) {
            let { x, y, z } = lookAt;
            this.camera.lookAt(x, y, z);
            this.controls.target.set(x, y, z);
        }
    }

    // private getFirstNonHelper(intersects: any) {
    //     for (let i = 0; i < intersects.length; i++) {
    //         if (!intersects[i].object.type.includes('Helper')) {
    //             return intersects[i].object;
    //         }
    //     }
    //     return null;
    // }




    private addRoom() {
        const room = new LineSegments(
            new BoxLineGeometry(30, 30, 30, 30, 30, 30).translate(0, 15, 0),
            new LineBasicMaterial({ color: 0x808080 })
        );
        this.scene.add(room);
    }

    private addFloor() {
        const grid = new GridHelper(30, 30, 0x848484, 0x848484);
        this.scene.add(grid);
    }

    // private addAxes() {
    //     // const axesHelper = new AxesHelper(3);
    //     // this.scene.add(axesHelper);

    //     const url = 'assets/fiveMeterAxis.glb';

    //     const loader = new GLTFLoader();
    //     loader.loadAsync(url).then((model: GLTF) => {
    //         this.scene.add(model.scene);
    //         this.playGltfAnimation(model);
    //     });
    // }



    private updateUIElements() {
        // Find closest intersecting object
        let intersect: any = null;
        let allButtons = ObjectLookup.getAllButtons();

        if (this.mouse.x !== null && this.mouse.y !== null) {
            this.raycaster.setFromCamera(this.mouse, this.camera);

            // intersect = this.raycastUIElements();
            intersect = this.raycast(Array.from(allButtons));
        }

        // Update non-targeted buttons state
        allButtons.forEach((obj) => {
            obj['setState']('idle');
        });
        // Update targeted button state (if any)
        if (intersect && intersect.object.isUI) {
            const currentMouseState = this.uiElementSelectState ? 'selected' : 'hovered';
            if (currentMouseState === 'selected') {
                const uuid = intersect.object?.uuid;
                if (uuid !== this.lastSelectedGuid) {
                    this.lastSelectedGuid = uuid;
                    this.onObjectSelected(uuid);
                    setTimeout(() => {
                        this.lastSelectedGuid = null;
                    }, 1000);
                }
            }
            intersect.object.setState(currentMouseState);
        }
    }

    //

    private raycast(items: any[]) {
        return items.reduce((closestIntersection, obj) => {
            const intersection = this.raycaster.intersectObject(obj, true);

            if (!intersection[0]) return closestIntersection;

            if (!closestIntersection || intersection[0].distance < closestIntersection.distance) {
                intersection[0].object = obj;

                return intersection[0];
            }

            return closestIntersection;
        }, null);
    }
}
