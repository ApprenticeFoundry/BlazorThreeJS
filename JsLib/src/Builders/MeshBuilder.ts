import { BufferGeometry, Material, Mesh, Object3D, Group } from 'three';
import { Transforms } from '../Utils/Transforms';
import { GeometryBuilder } from './GeometryBuilder';
import { MaterialBuilder } from './MaterialBuilder';

export interface MeshCreationResult {
    mesh: Mesh | null;
    geometry: BufferGeometry | null;
    material: Material | null;
    pivotGroup?: Group | null;  // Optional pivot group when pivot is non-zero
    entity: Object3D | null;    // The entity to add to scene (Group or Mesh)
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
                material: null,
                entity: null
            };

        
        try {
            const geometry = this.ConstructGeometry(options);
            const material = this.ConstructMaterial(options);
            const mesh = new Mesh(geometry, material);
    
            mesh.name = options.name;
            mesh.uuid = options.uuid;

            // Check if we need to create a pivot group
            const hasPivot = Transforms.hasPivot(options.transform);
            
            if (hasPivot) {
                // Create pivot group and add mesh as child with offset
                const pivotGroup = Transforms.createPivotGroup(mesh, options.transform.pivot);
                pivotGroup.uuid = options.uuid; // Use same UUID as the original object
                
                return {
                    mesh,
                    geometry,
                    material,
                    pivotGroup,
                    entity: pivotGroup  // Return group as the entity to add to scene
                };
            } else {
                // No pivot - return mesh directly (unchanged behavior)
                return {
                    mesh,
                    geometry,
                    material,
                    entity: mesh  // Return mesh as the entity to add to scene
                };
            }
        } catch (error) {
            console.error('MeshBuilder.CreateMesh', error);
            return {
                mesh: null,
                geometry: null,
                material: null,
                entity: null
            };
        }
    }


    public static ApplyMeshTransform(options: any, entity: Object3D): Object3D {

        //console.log('MeshBuilder.ApplyMeshTransform', options);
        try {
            if (Transforms.isPivotGroup(entity)) {
                // For pivot groups, apply transforms to the group (not the mesh)
                // The mesh offset is already handled in createPivotGroup
                Transforms.setTransform(entity, options.transform);
            } else {
                // For direct meshes, apply transforms normally (unchanged behavior)
                Transforms.setTransform(entity, options.transform);
            }
            return entity;       
        } catch (error) {
            console.error('MeshBuilder.ApplyMeshTransform', error);
            return entity;
            
        }
    }
}
