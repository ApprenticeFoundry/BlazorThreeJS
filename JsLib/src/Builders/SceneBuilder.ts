import { Group, Mesh, Scene, Vector3 } from 'three';
import { HelperBuilder } from './HelperBuilder';
import { LightBuilder } from './LightBuilder';
import { MeshBuilder } from './MeshBuilder';
import { Text } from 'troika-three-text';
import { SceneState } from '../Utils/SceneState';
import { Transforms } from '../Utils/Transforms';
import { MenuBuilder } from './MenuBuilder';
import { TextPanelBuilder } from './TextPanelBuilder';
import { PanelGroupBuilder } from './PanelGroupBuilder';

export class SceneBuilder {
    static BuildPeripherals(scene: Scene, options: any) {
        if (options.type == 'AmbientLight') {
            return LightBuilder.BuildAmbientLight(options);
        }

        if (options.type == 'PointLight') {
            return LightBuilder.BuildPointLight(options);
        }

        if (options.type.includes('Helper')) {
            return HelperBuilder.BuildHelper(options, scene);
        }

        return null;
    }

    static BuildChildOBSOLITE(scene: Scene, options: any): Text | Mesh | null {
        console.log('BuildChild', options);

        
        if ( Boolean(options.geometry) && Boolean(options.material) ) {
            if (options.type == 'Mesh') {
                return MeshBuilder.CreateMesh(options);
            }
    
            if (options.type == 'Group') {
                return MeshBuilder.CreateMesh(options);
            }
        }


        if (options.type === 'Menu') {
            console.log('BuildChild We have a menu');
            return null;
        }

        return null;

    }

    static UpdateChild(options: any, scene: Scene) {
        console.log('UpdateChild', options);
        // if (options.type == 'Text3D') {
        //     const label = SceneState.findLabel(options.uuid);

        //     const { position: pos } = options;
        //     label.position.x = pos.x;
        //     label.position.y = pos.y;
        //     label.position.z = pos.z;
        //     label.color = options.color;
        //     label.text = options.text;
        //     // label.sync();

        //     return null;
        // }

        // if (options.type == 'Mesh') {
        //     const object3D = SceneState.findPrimitive(options.uuid);
        //     if (Boolean(object3D)) {
        //         Transforms.setPosition(object3D, options.position);
        //         Transforms.setRotation(object3D, options.rotation);
        //         Transforms.setScale(object3D, options.scale);
        //     }
        //     // SceneState.establishPrimitive(options.uuid, object3D);
        //     return null;
        // }

        // if (options.type.includes('Group')) {
        //     const object3D = SceneState.findGLTFByGuid(options.uuid);
        //     if (Boolean(object3D)) {
        //         // const sceneGLTF = object3D.scene;
        //         const sceneGLTF = object3D;
        //         Transforms.setPosition(sceneGLTF, options.position);
        //         Transforms.setRotation(sceneGLTF, options.rotation);
        //         Transforms.setScale(sceneGLTF, options.scale);
        //     }
        //     // SceneState.addGLTF(scene, options.uuid, object3D);
        //     return null;
        // }
    }

    static BuildMenus(scene: Scene, options: any): null {
        MenuBuilder.BuildMenus(scene, options);
        return null;
    }

    static BuildTextPanels(scene: Scene, options: any): null {
        TextPanelBuilder.BuildTextPanels(scene, options);
        return null;
    }

    static BuildPanelGroups(scene: Scene, options: any): null {
        PanelGroupBuilder.BuildPanelGroup(scene, options);
        return null;
    }
}
