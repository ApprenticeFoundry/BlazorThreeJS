import { Euler, Object3D, Vector3, Group, Mesh } from 'three';

export class Transforms {
    // PIVOT UTILITY FUNCTIONS
    
    /**
     * Check if a transform has a non-zero pivot
     * @param transform Transform object with pivot property
     * @returns true if pivot is non-zero, false otherwise
     */
    static hasPivot(transform: any): boolean {
        if (!transform?.pivot) return false;
        const { x, y, z } = transform.pivot;
        return x !== 0 || y !== 0 || z !== 0;
    }

    /**
     * Create a pivot group containing the mesh with proper offset
     * @param mesh The mesh to wrap in a pivot group
     * @param pivot The pivot offset vector
     * @returns Group containing the offset mesh
     */
    static createPivotGroup(mesh: Mesh, pivot: Vector3): Group {
        const pivotGroup = new Group();
        pivotGroup.name = `${mesh.name}_PivotGroup`;
        
        // Position the mesh at the negative pivot offset
        // This makes the group's origin the pivot point
        mesh.position.set(-pivot.x, -pivot.y, -pivot.z);
        
        // Add mesh as child of the pivot group
        pivotGroup.add(mesh);
        
        // Mark the group so we can identify it later
        (pivotGroup as any).isPivotGroup = true;
        (pivotGroup as any).originalMesh = mesh;
        
        return pivotGroup;
    }

    /**
     * Extract the original mesh from a pivot group
     * @param entity Either a direct mesh or a pivot group
     * @returns The mesh object
     */
    static getMeshFromEntity(entity: Object3D): Mesh | null {
        if ((entity as any).isPivotGroup) {
            return (entity as any).originalMesh as Mesh;
        }
        return entity as Mesh;
    }

    /**
     * Check if an entity is a pivot group
     * @param entity Object to check
     * @returns true if entity is a pivot group
     */
    static isPivotGroup(entity: Object3D): boolean {
        return Boolean((entity as any).isPivotGroup);
    }

    static setPosition(object3d: Object3D, position: Vector3) {
        let { x, y, z } = position;
        if (Boolean(object3d))
            object3d.position.set(x, y, z);
    }

    static setRotation(object3d: Object3D, rotation: Euler) {
        let { x, y, z, order } = rotation;
        if (Boolean(object3d))
            object3d.setRotationFromEuler(new Euler(x, y, z, order));
    }

    static setScale(object3d: Object3D, scale: Vector3) {
        let { x, y, z } = scale;
        if (Boolean(object3d))
            object3d.scale.set(x, y, z);
    }
    
    static setTransform(object3d: Object3D, transform: any) {
        if ( !Boolean(transform) ) return;

        // if (transform.pivot) {
        //     Transforms.setPivot(object3d, transform.pivot);
        // }

        if (transform.position) {
            Transforms.setPosition(object3d, transform.position);
        }
        if (transform.rotation) {
            Transforms.setRotation(object3d, transform.rotation);
        }
        if (transform.scale) {
            Transforms.setScale(object3d, transform.scale);
        }
    }
}
