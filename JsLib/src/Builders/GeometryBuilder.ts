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
    Curve,
    BufferGeometry,
    Mesh,
    Line,
    LineBasicMaterial,
    MeshBasicMaterial,
    CatmullRomCurve3,
    Vector3,
    CurvePath,
} from 'three';

// For straight line segments, we should use a special curve class
// that combines LineCurve3 segments but still extends Curve<Vector3>
class CustomLinePath extends Curve<Vector3> {
    private points: Vector3[];
    private lineSegments: LineCurve3[] = [];
    private totalLength: number = 0;
    private cumulativeLengths: number[] = [0];
  
    constructor(points: Vector3[]) {
      super();
      this.points = points;
      
      // Create line segments and calculate total length
      for (let i = 0; i < points.length - 1; i++) {
        const segment = new LineCurve3(points[i], points[i + 1]);
        this.lineSegments.push(segment);
        
        const segmentLength = segment.getLength();
        this.totalLength += segmentLength;
        this.cumulativeLengths.push(this.totalLength);
      }
    }
  
    getPoint(t: number, optionalTarget?: Vector3): Vector3 {
      const target = optionalTarget || new Vector3();
      
      if (t <= 0) {
        return target.copy(this.points[0]);
      }
      if (t >= 1) {
        return target.copy(this.points[this.points.length - 1]);
      }
      
      // Find which segment this point falls on
      const targetLength = t * this.totalLength;
      let segmentIndex = 0;
      
      for (let i = 0; i < this.cumulativeLengths.length; i++) {
        if (targetLength < this.cumulativeLengths[i]) {
          segmentIndex = i - 1;
          break;
        }
      }
      
      // Map t to the segment's local t parameter
      const segmentLength = this.cumulativeLengths[segmentIndex + 1] - this.cumulativeLengths[segmentIndex];
      const segmentT = (targetLength - this.cumulativeLengths[segmentIndex]) / segmentLength;
      
      // Get the point on the segment
      return this.lineSegments[segmentIndex].getPoint(segmentT, target);
    }
  }

export class GeometryBuilder {
    static buildGeometry(options: any): BufferGeometry {
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

        if (options.type == 'BoundaryGeometry') {
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

        // if (options.type == 'LineGeometryxx') {
        //     // https://threejs.org/docs/#api/en/materials/LineBasicMaterial.linewidth
        //     // Due to limitations of the OpenGL Core Profile with the WebGL renderer on most platforms linewidth will always be 1 regardless of the set value.
        //     const material = new LineBasicMaterial({ color: requestedMaterial.color || 'yellow', linewidth: 8 });
        //     const geometry = new BufferGeometry().setFromPoints(options.path);
        //     const line = new Line(geometry, material);

        //     const group = new Group();
        //     group.add(line);
        //     return group;
        // }

        if (options.type == 'LineGeometry') {
            const geometry = new BufferGeometry().setFromPoints(options.path);
            return geometry;
        }



        if (options.type == 'TubeGeometry') {
            // Validate the path
            if (!Array.isArray(options.path) || options.path.length < 2) {
                throw new Error('Invalid path for TubeGeometry. Path must be an array of at least two Vector3 points.');
            }
                        
            // Ensure all points are Vector3 instances
            const path = options.path.map((point: any) => new Vector3(point.x, point.y, point.z));


            // Create our custom curve with straight line segments
            const linePath = new CustomLinePath(path);

            //const curve = new RoundedLineCurve3(path, 0.05); 
            //const curve = new CatmullRomCurve3(path);
            const geometry = new TubeGeometry(
                linePath,
                options.tubularSegments || 64,
                options.radius || 0.5,
                options.radialSegments || 8,
                options.closed || false
            );
            geometry.uuid = options.uuid;
            return geometry;
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
