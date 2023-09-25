import { Texture, TextureLoader } from 'three';

export class TextureBuilder {
    static buildTexture(options: any) {
        if (!Boolean(options)) return null;
        if (options.type == 'Texture') {
            let texture = new Texture();
            if (options.textureUrl) {
                texture = new TextureLoader().load(options.textureUrl);
                texture.uuid = options.uuid;
                texture.wrapS = options.wrapS;
                texture.wrapT = options.wrapT;
                texture.repeat = options.repeat;
                texture.offset = options.offset;
                texture.center = options.center;
                texture.rotation = options.rotation;
                return texture;
            }
            return null; // if no texture needs to be loaded, return null
        }
    }
}
