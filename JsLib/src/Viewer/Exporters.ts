import { GLTFExporter } from 'three/examples/jsm/exporters/GLTFExporter';
import { ColladaExporter } from 'three/examples/jsm/exporters/ColladaExporter';
import { OBJExporter } from 'three/examples/jsm/exporters/OBJExporter';

export class Exporters {
    static save(blob: Blob | MediaSource, filename: string) {
        const link = document.createElement('a');
        link.style.display = 'none';
        // document.body.appendChild( link ); // Firefox workaround, see #6594
        link.href = URL.createObjectURL(blob);
        link.download = filename;
        link.click();
    }

    static saveString(text: string, filename: string) {
        this.save(new Blob([text], { type: 'text/plain' }), filename);
    }

    static saveArrayBuffer(buffer: BlobPart, filename: string) {
        this.save(new Blob([buffer], { type: 'application/octet-stream' }), filename);
    }

    static exportOBJ(input: any) {
        const objExporter = new OBJExporter();
        const result = objExporter.parse(input);
        this.saveString(result, 'scene.obj');
    }

    static exportGLTF(input: any) {
        const gltfExporter = new GLTFExporter();
        gltfExporter.parse(
            input,
            (result) => {
                if (result instanceof ArrayBuffer) {
                    this.saveArrayBuffer(result, 'scene.glb');
                } else {
                    const output = JSON.stringify(result, null, 2);
                    this.saveString(output, 'scene.gltf');
                }
            },
            (evt: ErrorEvent) => {
                console.error('Problem exporting as GLTF: ', evt.message);
            }
        );
    }

    static exportCollada(input: any) {
        const exporter = new ColladaExporter();
        const result = exporter.parse(input, undefined, {
            // upAxis: 'Y_UP',
            // unitName: 'millimeter',
            // unitMeter: 0.001,
        });
        this.saveString(result.data, 'scene.dae');
    }
}
