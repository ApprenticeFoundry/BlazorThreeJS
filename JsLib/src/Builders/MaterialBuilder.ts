import { MeshStandardMaterial } from 'three';
import { TextureBuilder } from './TextureBuilder';

export class MaterialBuilder {
    static buildMaterial(options: any) {
        if (options.type == 'MeshStandardMaterial') {
            const material = new MeshStandardMaterial({
                color: options.color,
                flatShading: options.flatShading,
                metalness: options.metalness,
                roughness: options.roughness,
                wireframe: options.wireframe,
            });
            material.uuid = options.uuid;
            if ( Boolean(options.map) )
                material.map = TextureBuilder.buildTexture(options.map);
            return material;
        }
    }
}
