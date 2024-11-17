import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls';
import { BoxLineGeometry } from 'three/examples/jsm/geometries/BoxLineGeometry';
import { Loaders } from './Loaders';
import { SceneBuilder } from '../Builders/SceneBuilder';
import { CameraBuilder } from '../Builders/CameraBuilder';
import { Transforms } from '../Utils/Transforms';
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
} from 'three';

import { SceneState } from '../Utils/SceneState';
import ThreeMeshUI from 'three-mesh-ui';
import { MenuBuilder } from '../Builders/MenuBuilder';
import { GLTF, GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader';

export class Viewer3D {
    private options: any;
    private container: any;
    private renderer: WebGLRenderer;
    private scene: Scene;
    private camera: OrthographicCamera | PerspectiveCamera;
    private controls: OrbitControls;
    private mouse: Vector2 = new Vector2();
    private raycaster: Raycaster = new Raycaster();
    private uiElementSelectState = false;
    private lastSelectedGuid = null;
    private animationMixers: Array<AnimationMixer> = [];
    private clock: Clock;

    private INTERSECTED: any = null;
    private HasLoaded = false;

    public loadViewer(json: string) {
        if ( this.HasLoaded ) return;
        this.HasLoaded = true;

        const options = JSON.parse(json);
        this.clock = new Clock();

        this.setListeners();

        let container = document.getElementById(options.viewerSettings.containerId) as HTMLDivElement;

        if (!container) {
            console.warn('Container not found');
            return;
        }

        this.options = options;
        this.container = container;

        this.scene = new Scene();
        this.setScene();
        this.setCamera();

        this.renderer = new WebGLRenderer({
            antialias: this.options.viewerSettings.webGLRendererSettings.antialias,
            preserveDrawingBuffer: true
        });

        const requestedWidth = this.options.viewerSettings.width;
        const requestedHeight = this.options.viewerSettings.height;
        if (Boolean(requestedWidth) && Boolean(requestedHeight)) {
            this.renderer.setSize(requestedWidth, requestedHeight, true);
        }
        else {
            this.renderer.domElement.style.width = '100%';
            this.renderer.domElement.style.height = '100%';
        }

        // this.renderer.domElement.onclick = (event) => {
        //     if (this.options.viewerSettings.canSelect == true) {
        //         this.selectObject(event);
        //     }
        //     if (this.options.camera.animateRotationSettings.stopAnimationOnOrbitControlMove == true) {
        //         this.options.camera.animateRotationSettings.animateRotation = false;
        //     }
        // };

        this.container.appendChild(this.renderer.domElement);

        // this.addTestText('How do we pass text values?');

        this.setOrbitControls();
        this.onResize();

        const animate = () => {
            window.requestAnimationFrame(animate);
            this.render();
        };
        animate();
        //this.render();
    }

    private render() {
        ThreeMeshUI.update();
        this.updateUIElements();
        // this.selectObject();

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
        this.renderer.render(this.scene, this.camera);
    }

    private setListeners() {
        window.addEventListener('pointermove', (event: PointerEvent) => {
            let canvas = this.renderer.domElement;

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

        this.renderer.setSize(
            this.container.offsetWidth,
            this.container.offsetHeight,
            false // required
        );
    }

    public setScene() {
        // console.log('in setScene this.options=', this.options);
        this.scene.background = new Color(this.options.scene.backGroundColor);
        this.scene.uuid = this.options.scene.uuid;
        this.addFloor();
        // this.addAxes();  we should control this from FoundryBlazor by default
        //this.addRoom();

        if (Boolean(this.options.scene.children)) {
            this.options.scene.children.forEach((childOptions: any) => {
                const child = SceneBuilder.BuildPeripherals(this.scene, childOptions);
                if (child) {
                    this.scene.add(child);
                }
            });
        }
        // Add cached meshes.
        SceneState.renderToScene(this.scene, this.options);
    }

    public updateScene(options: string) {
        const sceneOptions = JSON.parse(options);
        //console.log('updateScene sceneOptions=', sceneOptions);
        this.options.scene = sceneOptions;
        SceneState.refreshScene(this.scene, sceneOptions);
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

    public showCurrentCameraInfo() {
        console.log('Current camera info:', this.camera);
        console.log('Orbit controls info:', this.controls);
    }

    private setOrbitControls() {
        this.controls = new OrbitControls(this.camera, this.renderer.domElement);
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

    public import3DModel(options: string) {
        const settings = JSON.parse(options);
        const loaders = new Loaders();
        return loaders.import3DModel(this.scene, settings, this.options.viewerSettings.containerId, (model: GLTF) => {
            this.playGltfAnimation(model);
        });
    }

    public clone3DModel(sourceGuid: string, options: string) {
        const settings = JSON.parse(options);
        const loaders = new Loaders();
        return loaders.clone3DModel(this.scene, sourceGuid, settings, this.options.viewerSettings.containerId);
    }

    public moveObject(object3D: Object3D): boolean {
        const moved = SceneState.moveObject(this.scene, object3D);
        return Boolean(moved);
    }

    public getSceneItemByGuid(guid: string) {
        let item = this.scene.getObjectByProperty('uuid', guid);
        const json = {
            uuid: item.uuid,
            type: item.type,
            name: item.name,
            children: item.type == 'Group' ? this.iterateGroup(item.children) : [],
        };
        return JSON.stringify(json);
    }

    private iterateGroup(children: any[]) {
        let result = [];
        for (let i = 0; i < children.length; i++) {
            result.push({
                uuid: children[i].uuid,
                type: children[i].type,
                name: children[i].name,
                children: children[i].type == 'Group' ? this.iterateGroup(children[i].children) : [],
            });
        }
        return result;
    }

    private findRootGuid(item: Object3D<ThreeEvent>): Object3D<ThreeEvent> {
        const userData = item.userData;
        if (userData.isGLTFGroup) return item;

        if (item.parent !== null) return this.findRootGuid(item.parent);
        return null;
    }

    private selectObject() {
        let intersect: any = null;

        if (this.mouse.x !== null && this.mouse.y !== null) {
            this.raycaster.setFromCamera(this.mouse, this.camera);
            intersect = this.raycast(Array.from(MenuBuilder.elementButtons.values()));
        }

        const intersects = this.raycaster.intersectObjects(this.scene.children, true);

        // Ignore object selection if this is a UI element.  UI elements are handled in updateUIElements
        if (intersect && intersect.object.isUI) {
            return;
        } else {
            if (intersects.length === 0) {
                // this.INTERSECTED = null;
                // DotNet.invokeMethodAsync(
                //     'BlazorThreeJS',
                //     'ReceiveSelectedObjectUUID',
                //     this.options.viewerSettings.containerId,
                //     null
                // );
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

    public deleteByUuid(uuid: string) {
        return SceneState.removeItem(this.scene, uuid);
    }

    public clearScene() {
        const self = this;
        SceneState.clearScene(this.scene, this.options.scene, function onClearScene() {
            self.setScene();
            self.setOrbitControls();
        });
    }

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

    private addAxes() {
        // const axesHelper = new AxesHelper(3);
        // this.scene.add(axesHelper);

        const url = 'assets/fiveMeterAxis.glb';

        const loader = new GLTFLoader();
        loader.loadAsync(url).then((model: GLTF) => {
            this.scene.add(model.scene);
            this.playGltfAnimation(model);
        });
    }

    private onObjectSelected(uuid: string) {
        DotNet.invokeMethodAsync('BlazorThreeJS', 'OnClickButton', this.options.viewerSettings.containerId, uuid);
    }

    private updateUIElements() {
        // Find closest intersecting object
        let intersect: any = null;

        if (this.mouse.x !== null && this.mouse.y !== null) {
            this.raycaster.setFromCamera(this.mouse, this.camera);

            // intersect = this.raycastUIElements();
            intersect = this.raycast(Array.from(MenuBuilder.elementButtons.values()));
        }

        // Update non-targeted buttons state
        MenuBuilder.elementButtons.forEach((obj) => {
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
