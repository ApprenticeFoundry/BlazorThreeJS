import {

    LineCurve3,
    Curve,
    Vector3,
    Quaternion,
} from 'three';

export class RoundedLineCurve3 extends Curve<Vector3> {
    segments: any[];
    bendRadius: number;
    points: any;
    constructor(points, bendRadius = 1) {
      super();
      this.points = points;
      this.bendRadius = bendRadius;
      this.segments = [];
      
      this._processPoints();
    }
    
    _processPoints() {
      if (this.points.length < 2) return;
      
      // Calculate segments and rounded corners
      for (let i = 0; i < this.points.length - 1; i++) {
        // Add straight segment
        this.segments.push({
          type: 'line',
          start: i === 0 ? this.points[0] : null, // Will be calculated later for corners
          end: i === this.points.length - 2 ? this.points[i+1] : null // Will be calculated for corners
        });
        
        // Add corner if not the last point
        if (i < this.points.length - 2) {
          const p0 = this.points[i];
          const p1 = this.points[i+1];
          const p2 = this.points[i+2];
          
          // Calculate directions
          const dir1 = new Vector3().subVectors(p1, p0).normalize();
          const dir2 = new Vector3().subVectors(p2, p1).normalize();
          
          // Calculate angle between segments
          const angle = Math.acos(dir1.dot(dir2));
          
          // Skip if points are collinear (no corner needed)
          if (Math.abs(angle) < 0.01 || Math.abs(angle - Math.PI) < 0.01) {
            this.segments[this.segments.length-1].end = p1;
            continue;
          }
          
          // Calculate corner parameters
          const cornerLength = this.bendRadius * Math.tan(angle / 2);
          
          // Ensure corner length doesn't exceed segment length
          const seg1Length = p0.distanceTo(p1);
          const seg2Length = p1.distanceTo(p2);
          const safeCornerLength = Math.min(cornerLength, seg1Length * 0.49, seg2Length * 0.49);
          
          // Calculate corner start and end points
          const cornerStart = new Vector3().addVectors(
            p1, dir1.clone().multiplyScalar(-safeCornerLength)
          );
          const cornerEnd = new Vector3().addVectors(
            p1, dir2.clone().multiplyScalar(safeCornerLength)
          );
          
          // Update previous segment end
          this.segments[this.segments.length-1].end = cornerStart;
          
          // Add corner segment
          this.segments.push({
            type: 'corner',
            center: p1,
            start: cornerStart,
            end: cornerEnd,
            startDir: dir1.clone(),
            endDir: dir2.clone(),
            angle: angle
          });
        }
      }
    }
    
    getPoint(t) {
      if (this.segments.length === 0) return new Vector3();
      
      // Determine which segment t falls within
      const segmentCount = this.segments.length;
      const segmentT = segmentCount * t;
      const segmentIndex = Math.min(Math.floor(segmentT), segmentCount - 1);
      const localT = segmentT - segmentIndex;
      
      const segment = this.segments[segmentIndex];
      
      if (segment.type === 'line') {
        // Simple linear interpolation for line segments
        return new Vector3().lerpVectors(segment.start, segment.end, localT);
      } else if (segment.type === 'corner') {
        // Arc interpolation for corners
        const rotationAxis = new Vector3().crossVectors(
          segment.startDir, segment.endDir
        ).normalize();
        
        // Handle parallel but opposite directions
        if (rotationAxis.lengthSq() < 0.001) {
          rotationAxis.set(0, 1, 0);
        }
        
        // Interpolate angle based on t
        const currentAngle = (Math.PI - segment.angle) * localT;
        
        // Create quaternion for rotation
        const q = new Quaternion().setFromAxisAngle(rotationAxis, currentAngle);
        
        // Get vector from center to start of corner
        const radius = segment.start.distanceTo(segment.center);
        const startVector = new Vector3().subVectors(segment.start, segment.center);
        
        // Rotate vector and add to center
        const rotated = startVector.clone().applyQuaternion(q);
        return new Vector3().addVectors(segment.center, rotated);
      }
      
      return new Vector3();
    }
  }