import { ObjectLookup } from '../Utils/ObjectLookup';
import { MeshBuilder } from '../Builders/MeshBuilder';

import {
    AnimationMixer,
    Object3D,
    Event as ThreeEvent,
    Group,
    BoxGeometry,
    CylinderGeometry,
    MeshBasicMaterial,
    Mesh,
} from 'three';

//import { Text } from 'three-mesh-ui';
import { Text } from 'troika-three-text';
import { LightBuilder } from '../Builders/LightBuilder';
import { Transforms } from './Transforms';
import { GLTF } from 'three/examples/jsm/loaders/GLTFLoader';
import { Loaders } from '../Viewer/Loaders';


export class FactoryClass {
    
    private makers = new Map<string, Function>();

    public constructor() {
        
        this.makers.set('AmbientLight', LightBuilder.BuildAmbientLight);
        this.makers.set('PointLight', LightBuilder.BuildPointLight);
        this.makers.set('Mesh3D', this.establish3DGeometry.bind(this));
        this.makers.set('Model3D', this.establish3DModel.bind(this));
        this.makers.set('Text3D', this.establish3DLabel.bind(this));
        //this.makers.set('Group3D', this.establish3DGroup.bind(this));
        //this.makers.set('PanelMenu3D', this.establish3DMenu.bind(this));


    }

    private LoadedObjectComplete(uuid: string) {
        DotNet.invokeMethodAsync('BlazorThreeJS', 'LoadedObjectComplete', uuid);
    }

   public establish3DGeometry(options: any, parent: Object3D): Object3D | null {

        const guid = options.uuid;

        var entity = ObjectLookup.findPrimitive(guid) as Object3D;
        var exist = Boolean(entity)
        if ( !exist ) {
            entity = MeshBuilder.CreateMesh(options);
            ObjectLookup.addPrimitive(guid, entity);
            parent.add(entity);
        }

        MeshBuilder.ApplyMeshTransform(options, entity);
        this.establish3DChildren(options, entity);


        if ( !exist && parent.type === 'Scene' )
        {
            this.LoadedObjectComplete(guid);
            console.log('Geometry Added to Scene', entity);
        }
        return entity;
    }

    private establish3DLabel(options: any, parent: Object3D): Text | null {
        console.log('establish3DLabel modelOptions=', options);

        const guid = options.uuid;

        var entity = ObjectLookup.findLabel(guid) as Text;
        var exist = Boolean(entity)
        if ( !exist ) {
            entity = new Text();
            ObjectLookup.addLabel(guid, entity);
            parent.add(entity);

            entity.uuid = guid;
            entity.text = options.text;
            entity.color = options.color;
            entity.fontSize = options.fontSize;
            //entity.userData = { isTextLabel: true, };
        }
        
        //Transforms.setTransform(entity, options.transform);
        if ( Boolean(options.transform) ) {
            const { position: pos } = options.transform;
            entity.position.x = pos.x;
            entity.position.y = pos.y;
            entity.position.z = pos.z;
        }

        // Update the rendering:
        entity.sync();

        //MeshBuilder.ApplyMeshTransform(options, entity);
        this.establish3DChildren(options, entity);


        if ( !exist && parent.type === 'Scene' )
        {
            this.LoadedObjectComplete(guid);
            console.log('Text Added to Scene', entity);
        }
        return entity;
    }




    public establish3DModel(options: any, parent: Object3D) {
        console.log('establish3DModel modelOptions=', options);
        

        var model = ObjectLookup.findModel(options.uuid) as Group;
        if (Boolean(model)) {
            Transforms.setTransform(model, options.transform);
            return;
        }

        const loaders = new Loaders();
        loaders.import3DModel(options, (gltf, item) => {
            console.log('Model Added to Scene', item);

            parent.add(item);
            ObjectLookup.addModel(item.uuid, item);
            //this.addDebuggerWindow(url, group);
            if ( parent.type === 'Scene' )
            {
                this.LoadedObjectComplete(item.uuid);
            }
        })
    }


    //can we be smart here and call the correct method based on the type of object we are adding?
    public establish3DChildren(options: any, parent: Object3D) 
    {
        console.log('establish3DChildren options=', options);
        var members = options.children;
        for (let index = 0; index < members.length; index++) {
            
            const element = members[index];
            //console.log('establish3DChildren element.type=', element.type, element);
            //console.log('establish3DChildren element=', index, element);
            
            try {
                //add these back in when we have the builders
                //TextPanelBuilder.BuildTextPanels(scene, options);
                //PanelGroupBuilder.BuildPanelGroup(scene, options);
                

                var funct = this.makers.get(element.type);
                if (funct) 
                    funct(element, parent);
                else
                    console.log('No Constructor for', element.type);
                

                // } else
                // if ( element.type == 'Group3D' ) {
                //     this.establish3DGroup(element);
                // } else
                // if ( element.type == 'PanelMenu3D' ) {
                //     this.establish3DMenu(element);
                // } else

                // } else
                // if (element.type.includes('Helper')) {
                //     const obj = this.scene.getObjectByProperty('uuid', element.uuid);
                //     var helper = HelperBuilder.BuildHelper(options, obj);
                //     this.scene.add(helper);
                // }
            } catch (error) {
                console.log('Error in establish3DChildren', error);
            }
        }    
    }

}

export const Constructors = new FactoryClass();
