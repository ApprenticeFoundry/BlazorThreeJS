import { MeshStandardMaterial, MeshPhongMaterial } from 'three';
import { TextureBuilder } from './TextureBuilder';

export class MaterialBuilder {
    static buildMaterial(options: any) {
        if (options.type == 'MeshStandardMaterial') {
            const material = new MeshStandardMaterial({
                color: options.color,
                opacity: options.opacity,
                transparent: options.transparent,
                wireframe: options.wireframe,
                flatShading: options.flatShading,
                metalness: options.metalness,
                roughness: options.roughness,
            });
            material.uuid = options.uuid;
            if ( Boolean(options.map) )
                material.map = TextureBuilder.buildTexture(options.map);
            return material;
        }
        if (options.type == 'MeshPhongMaterial') {
            const material = new MeshPhongMaterial({
                color: options.color,
                opacity: options.opacity,
                transparent: options.transparent,
                wireframe: options.wireframe,
                flatShading: options.flatShading,
            });
            material.uuid = options.uuid;
            if ( Boolean(options.map) )
                material.map = TextureBuilder.buildTexture(options.map);
            return material;
        }        
    }
}
