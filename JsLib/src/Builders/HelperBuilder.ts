import {
    ArrowHelper,
    AxesHelper,
    BoxHelper,
    GridHelper,
    Object3D,
    Plane,
    PlaneHelper,
    PointLight,
    PointLightHelper,
    PolarGridHelper,
    Vector3,
} from 'three';
import { Transforms } from '../Utils/Transforms';

export class HelperBuilder {
    static BuildHelper(options: any, source: Object3D): Object3D {
        if (options.type == 'ArrowHelper') {
            const dir = new Vector3(options.dir.x, options.dir.y, options.dir.z);
            dir.normalize();
            const origin = new Vector3(options.origin.x, options.origin.y, options.origin.z);
            const arrow = new ArrowHelper(
                dir,
                origin,
                options.length,
                options.color,
                options.headLength,
                options.headWidth
            );
            arrow.uuid = options.uuid;

            // transitions are not aplicable here??? todo: investigate this
            return arrow;
        }

        if (options.type == 'AxesHelper') {
            const axes = new AxesHelper(options.size);
            axes.uuid = options.uuid;
            Transforms.setTransform(axes, options.transform);
            return axes;
        }

        if (options.type == 'BoxHelper') {
            const box = new BoxHelper(source, options.color);
            box.uuid = options.uuid;
            Transforms.setTransform(box, options.transform);
            return box;
        }

        if (options.type == 'GridHelper') {
            const grid = new GridHelper(options.size, options.divisions, options.colorCenterLine, options.colorGrid);
            grid.uuid = options.uuid;
            Transforms.setTransform(grid, options.transform);
            return grid;
        }

        if (options.type == 'PolarGridHelper') {
            const grid = new PolarGridHelper(
                options.radius,
                options.radials,
                options.circles,
                options.divisions,
                options.color1,
                options.color2
            );
            grid.uuid = options.uuid;
            Transforms.setTransform(grid, options.transform);
            return grid;
        }

        if (options.type == 'PlaneHelper') {
            let { x, y, z } = options.plane.normal;
            let normal = new Vector3(x, y, z);
            let plane = new Plane(normal, options.plane.constant);

            const planeHelper = new PlaneHelper(plane, options.size, options.color);
            planeHelper.uuid = options.uuid;;
            Transforms.setTransform(planeHelper, options.transform);
            return planeHelper;
        }

        if (options.type == 'PointLightHelper') {
            var light = source as PointLight;
            var color = options.color || light.color;

            const plight = new PointLightHelper(light, options.sphereSize, color);
            plight.uuid = options.uuid;
            Transforms.setTransform(plight, options.transform);
            return plight;
        }
    }
}
