import { BufferGeometry, Group, Mesh, Object3D } from 'three';
import { Transforms } from '../Utils/Transforms';
import { GeometryBuilder } from './GeometryBuilder';
import { MaterialBuilder } from './MaterialBuilder';


export class MeshBuilder {
    public static ConstructMesh(options: any): Mesh | Group | null {
        const geometry = GeometryBuilder.buildGeometry(options.geometry, options.material);

        if (geometry['isGroup'])
            return geometry as Group;
        

        const item = geometry as BufferGeometry;
        const material = MaterialBuilder.buildMaterial(options.material);
        const entity = new Mesh(item, material);

        entity.name = options.name;
        entity.uuid = options.uuid;
        
        return entity;
    }

    public static CreateMesh(options: any): Mesh | null {

        //console.log('MeshBuilder.CreateMesh', options);
        if ( !Boolean(options.geometry) || !Boolean(options.material) )
            return null;

        let result = this.ConstructMesh(options);
        return result;
    }

    public static ApplyMeshMaterial(options: any, entity: Mesh): Object3D {

        //console.log('MeshBuilder.ApplyMeshTransform', options);
        const material = MaterialBuilder.buildMaterial(options.material);
        entity.material = material;
        return entity;
    }

    public static ApplyMeshTransform(options: any, entity: Object3D): Object3D {

        //console.log('MeshBuilder.ApplyMeshTransform', options);
        Transforms.setTransform(entity, options.transform);
        return entity;
    }
}
