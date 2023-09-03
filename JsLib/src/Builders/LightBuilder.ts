import { AmbientLight, PointLight } from 'three';
import { Transforms } from '../Utils/Transforms';

export class LightBuilder {
    static BuildAmbientLight(options: any) {
        const light = new AmbientLight(options.color, options.intensity);
        Transforms.setPosition(light, options.position);
        return light;
    }

    static BuildPointLight(options: any) {
        const light = new PointLight(options.color, options.intensity, options.distance, options.decay);
        light.uuid = options.uuid;
        Transforms.setPosition(light, options.position);
        return light;
    }
}
