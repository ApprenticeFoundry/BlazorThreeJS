import { AmbientLight, Object3D, PointLight } from 'three';
import { Transforms } from '../Utils/Transforms';

export class LightBuilder {
    static BuildAmbientLight(options: any, parent?: Object3D) {
        const light = new AmbientLight(options.color, options.intensity);
        light.uuid = options.uuid;
        Transforms.setTransform(light, options.transform);
        parent?.add(light);
        return light;
    }

    static BuildPointLight(options: any, parent?: Object3D) {
        const light = new PointLight(options.color, options.intensity, options.distance, options.decay);
        light.uuid = options.uuid;
        Transforms.setTransform(light, options.transform);
        parent?.add(light);
        return light;
    }
}
