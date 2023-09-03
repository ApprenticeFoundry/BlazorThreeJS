import {
    BoxGeometry,
    CapsuleGeometry,
    CircleGeometry,
    ConeGeometry,
    CylinderGeometry,
    TubeGeometry,
    DodecahedronGeometry,
    IcosahedronGeometry,
    OctahedronGeometry,
    PlaneGeometry,
    RingGeometry,
    SphereGeometry,
    TetrahedronGeometry,
    TorusGeometry,
    TorusKnotGeometry,
    LineCurve3,
    Group,
    BufferGeometry,
    Mesh,
    Line,
    LineBasicMaterial,
    MeshBasicMaterial,
} from 'three';

export class GeometryBuilder {
    static buildGeometry(options: any, requestedMaterial: any) {
        if (options.type == 'BoxGeometry') {
            const geometry = new BoxGeometry(
                options.width,
                options.height,
                options.depth,
                options.widthSegments,
                options.heightSegments,
                options.depthSegments
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'CapsuleGeometry') {
            const geometry = new CapsuleGeometry(
                options.radius,
                options.length,
                options.capSubdivisions,
                options.radialSegments
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'CircleGeometry') {
            const geometry = new CircleGeometry(
                options.radius,
                options.segments,
                options.thetaStart,
                options.thetaLength
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'ConeGeometry') {
            const geometry = new ConeGeometry(
                options.radius,
                options.height,
                options.radialSegments,
                options.heigthSegments,
                options.openEnded,
                options.thetaStart,
                options.thetaLength
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'CylinderGeometry') {
            const geometry = new CylinderGeometry(
                options.radiusTop,
                options.radiusBottom,
                options.height,
                options.radialSegments,
                options.heigthSegments,
                options.openEnded,
                options.thetaStart,
                options.thetaLength
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'LineGeometry') {
            // https://threejs.org/docs/#api/en/materials/LineBasicMaterial.linewidth
            // Due to limitations of the OpenGL Core Profile with the WebGL renderer on most platforms linewidth will always be 1 regardless of the set value.
            const material = new LineBasicMaterial({ color: requestedMaterial.color || 'yellow', linewidth: 8 });
            const geometry = new BufferGeometry().setFromPoints(options.path);
            const line = new Line(geometry, material);

            const group = new Group();
            group.add(line);
            return group;
        }

        if (options.type == 'TubeGeometry') {
            const group = new Group();

            for (let i = 0; i < options.path.length - 1; i++) {
                const tubePath = new LineCurve3(options.path[i], options.path[i + 1]);
                const geometry = new TubeGeometry(
                    tubePath,
                    options.tubularSegments,
                    options.radius,
                    options.radialSegments,
                    options.closed
                );
                const material = new MeshBasicMaterial({ color: requestedMaterial.color || 'orange' });
                geometry.uuid = options.uuid;
                const mesh = new Mesh(geometry, material);
                group.add(mesh);
            }
            return group;
        }
        if (options.type == 'DodecahedronGeometry') {
            const geometry = new DodecahedronGeometry(options.radius, options.detail);
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'IcosahedronGeometry') {
            const geometry = new IcosahedronGeometry(options.radius, options.detail);
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'OctahedronGeometry') {
            const geometry = new OctahedronGeometry(options.radius, options.detail);
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'PlaneGeometry') {
            const geometry = new PlaneGeometry(
                options.width,
                options.height,
                options.widthSegments,
                options.heightSegments
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'RingGeometry') {
            const geometry = new RingGeometry(
                options.innerRadius,
                options.outerRadius,
                options.thetaSegments,
                options.phiSegments,
                options.thetaStart,
                options.thetaLength
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'SphereGeometry') {
            const geometry = new SphereGeometry(
                options.radius,
                options.widthSegments,
                options.heightSegments,
                options.phiStart,
                options.phiLength,
                options.thetaStart,
                options.thetaLength
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'TetrahedronGeometry') {
            const geometry = new TetrahedronGeometry(options.radius, options.detail);
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'TorusGeometry') {
            const geometry = new TorusGeometry(
                options.radius,
                options.tube,
                options.radialSegments,
                options.tubularSegments,
                options.arc
            );
            geometry.uuid = options.uuid;
            return geometry;
        }

        if (options.type == 'TorusKnotGeometry') {
            const geometry = new TorusKnotGeometry(
                options.radius,
                options.tube,
                options.tubularSegments,
                options.radialSegments,
                options.p,
                options.pq
            );
            geometry.uuid = options.uuid;
            return geometry;
        }
    }
}
