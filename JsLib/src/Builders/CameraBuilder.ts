import { OrthographicCamera, PerspectiveCamera } from 'three';
import { Transforms } from '../Utils/Transforms';

export class CameraBuilder {
    public BuildCamera(options: any, aspect: number) {
        let camera: OrthographicCamera | PerspectiveCamera;
        if (options.type == 'PerspectiveCamera') {
            camera = new PerspectiveCamera(options.fov, aspect, options.near, options.far);
        }

        if (options.type == 'OrthographicCamera') {
            camera = new OrthographicCamera(
                options.left * aspect,
                options.right * aspect,
                options.top,
                options.bottom,
                options.near,
                options.far
            );
            camera.zoom = options.zoom;
        }

        camera.uuid = options.uuid;
        var transform = options.transform;
        Transforms.setTransform(camera, transform);
        let { x, y, z } = options.lookAt;
        camera.lookAt(x, y, z);
        return camera;
    }
}
