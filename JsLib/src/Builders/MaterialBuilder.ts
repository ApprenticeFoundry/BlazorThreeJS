import { MeshStandardMaterial } from 'three';
import { TextureBuilder } from './TextureBuilder';

export class MaterialBuilder {
    static buildMaterial(options: any) {
        if (options.type == 'MeshStandardMaterial') {
            let map = TextureBuilder.buildTexture(options.map);

            const material = new MeshStandardMaterial({
                color: options.color,
                flatShading: options.flatShading,
                metalness: options.metalness,
                roughness: options.roughness,
                wireframe: options.wireframe,
                map: map,
            });
            material.uuid = options.uuid;
            return material;
        }
    }
}
