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
        entity.uuid = options.uuid;
        
        return entity;
    }

    public static CreateMesh(options: any): Object3D | null {

        console.log('MeshBuilder.CreateMesh', options);
        if ( !Boolean(options.geometry) || !Boolean(options.material) )
            return null;

        let result = this.ConstructMesh(options);
        if (!Boolean(result)) {
            return null;
        }
        if (!Boolean(options.pivot)) {
            console.log('MeshBuilder.CreateMesh', result);
            return result;
        }

        const group = new Group();
        group.uuid = options.uuid;
        var list = [result] as any;
        group.add(list);

        console.log('MeshBuilder.CreateMesh Group', group);
        return group;

    }
    public static ApplyMeshTransform(options: any, entity: Object3D): Object3D {

        console.log('MeshBuilder.RefreshMesh', options);
        if (Boolean(options.pivot)) {
            Transforms.setPosition(entity, options.pivot);
        }
        Transforms.setTransform(entity, options.transform);
        return entity;
    }
}
