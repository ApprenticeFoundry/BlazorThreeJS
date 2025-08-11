import { BufferGeometry, Material, Mesh, Object3D } from 'three';
import { Transforms } from '../Utils/Transforms';
import { GeometryBuilder } from './GeometryBuilder';
import { MaterialBuilder } from './MaterialBuilder';

export interface MeshCreationResult {
    mesh: Mesh | null;
    geometry: BufferGeometry | null;
    material: Material | null;
}

export class MeshBuilder {
    public static ConstructGeometry(options: any): BufferGeometry | null {
        try {
            const geometry = GeometryBuilder.buildGeometry(options.geometry);
            geometry.name = options.name;
            geometry.uuid = options.uuid;
            return geometry;          
        } catch (error) {
            console.error('MeshBuilder.ConstructGeometry', error);
            return null;
        }

    }

    public static ConstructMaterial(options: any): Material | null {
        try {
            const material = MaterialBuilder.buildMaterial(options.material);
            material.name = options.name;
            material.uuid = options.uuid;
            
            return material;
        } catch (error) {
            console.error('MeshBuilder.ConstructMaterial', error);
            return null;
        }
    }

    public static CreateMesh(options: any): MeshCreationResult {

        //console.log('MeshBuilder.CreateMesh', options);
        if ( !Boolean(options.geometry) || !Boolean(options.material) )
            return {
                mesh: null,
                geometry: null,
                material: null
            };

        
        try {
            const geometry = this.ConstructGeometry(options);
            const material = this.ConstructMaterial(options);
            const mesh = new Mesh(geometry, material);
    
            mesh.name = options.name;
            mesh.uuid = options.uuid;
    
            return {
                mesh,
                geometry,
                material
            };
        } catch (error) {
            console.error('MeshBuilder.CreateMesh', error);
            return {
                mesh: null,
                geometry: null,
                material: null
            };
        }
    }


    public static ApplyMeshTransform(options: any, entity: Object3D): Object3D {

        //console.log('MeshBuilder.ApplyMeshTransform', options);
        try {
            Transforms.setTransform(entity, options.transform);
            return entity;       
        } catch (error) {
            console.error('MeshBuilder.ApplyMeshTransform', error);
            return entity;
            
        }
    }
}
