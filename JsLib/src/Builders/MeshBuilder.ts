import { BufferGeometry, Group, Mesh, Scene } from 'three';
import { Transforms } from '../Utils/Transforms';
import { GeometryBuilder } from './GeometryBuilder';
import { MaterialBuilder } from './MaterialBuilder';
import { SceneState } from '../Utils/SceneState';

export class MeshBuilder {
    static BuildMesh(options: any, scene: Scene) {
        const geometry = GeometryBuilder.buildGeometry(options.geometry, options.material);
        const material = MaterialBuilder.buildMaterial(options.material);

        let entity = null;
        if (geometry['isGroup']) {
            entity = geometry;
        } else {
            const item = geometry as BufferGeometry;
            entity = new Mesh(item, material);
            Transforms.setScale(entity, options.scale);
        }

        if (!Boolean(options.pivot)) {
            entity.uuid = options.uuid;
            Transforms.setPosition(entity, options.position);
            Transforms.setRotation(entity, options.rotation);
            SceneState.addPrimitive(entity.uuid, entity);
            return entity;
        } else {
            Transforms.setPosition(entity, options.pivot);
            const group = new Group();
            group.uuid = options.uuid;
            group.add(entity);
            Transforms.setPosition(group, options.position);
            Transforms.setRotation(group, options.rotation);
            Transforms.setScale(group, options.scale);
            SceneState.addPrimitive(group.uuid, entity);
            return group;
        }
    }
}
