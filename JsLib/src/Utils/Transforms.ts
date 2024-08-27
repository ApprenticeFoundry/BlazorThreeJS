import { Euler, Object3D, Vector3 } from 'three';

export class Transforms {
    static setPosition(object3d: Object3D, position: Vector3) {
        let { x, y, z } = position;
        if (Boolean(object3d))
            object3d.position.set(x, y, z);
    }

    static setRotation(object3d: Object3D, eulerOptions: Euler) {
        let { x, y, z, order } = eulerOptions;
        if (Boolean(object3d))
            object3d.setRotationFromEuler(new Euler(x, y, z, order));
    }

    static setScale(object3d: Object3D, scale: Vector3) {
        let { x, y, z } = scale;
        if (Boolean(object3d))
            object3d.scale.set(x, y, z);
    }
}
