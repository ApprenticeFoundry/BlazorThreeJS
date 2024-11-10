import { BufferGeometry, Color, Group, Mesh, Scene } from 'three';
import ThreeMeshUI, { BlockOptions } from 'three-mesh-ui';
import { GeometryBuilder } from './GeometryBuilder';
import { MaterialBuilder } from './MaterialBuilder';

const FontFamily = '/assets/Roboto-msdf.json';
const FontImage = '/assets/Roboto-msdf.png';

class PanelGroupBuilderClass {
    private PanelGroups: any[] = [];
    // public elementPanels: Map<string, ThreeMeshUI.Block> = new Map<string, ThreeMeshUI.Block>();
    public elementPanels: Map<string, Group> = new Map<string, Group>();

    private MakePanel(scene: Scene, panelGroupOptions: any) {
        const { width, height, color } = panelGroupOptions;
        const blockOptions: BlockOptions = {
            width,
            height,
            textType: 'MSDF',
            textAlign: 'left',
            fontFamily: FontFamily,
            fontTexture: FontImage,
            padding: 0.02,
            borderRadius: 0.05,
            backgroundColor: new Color(color),
        };

        const group = new Group();
        group.uuid = panelGroupOptions.uuid;

        const container = new ThreeMeshUI.Block(blockOptions);

        const { x: posX, y: posY, z: posZ } = panelGroupOptions.position;
        group.position.set(posX, posY, posZ);

        const { x: rotX, y: rotY, z: rotZ } = panelGroupOptions.rotation;
        group.rotation.set(rotX, rotY, rotZ);

        console.log('MakePanel ready to add group');
        group.add(container);

        panelGroupOptions.textLines.forEach((textLine: any) => {
            const text = this.EstablishTextLine(textLine);
            container.add(text);
        });

        panelGroupOptions.meshes?.forEach((options: any) => {
            const entity = this.EstablishMesh(options);
            group.add(entity);
        });

        panelGroupOptions.textPanels.forEach((options: any) => {
            const panel = this.EstablishPanel(options);
            group.add(panel);
        });

        scene.add(group);

        this.elementPanels.set(panelGroupOptions.uuid, group);
    }

    private EstablishTextLine(textLine: string): ThreeMeshUI.Text {
        const text = new ThreeMeshUI.Text({
            content: `${textLine}\n`,
            fontSize: 0.15,
            backgroundColor: 'yellow',
        });
        return text;
    }

    private EstablishPanel(textPanelOptions: any): ThreeMeshUI.Block {
        const { width, height, color } = textPanelOptions;
        const blockOptions: BlockOptions = {
            width,
            height,
            textType: 'MSDF',
            textAlign: 'left',
            fontFamily: FontFamily,
            fontTexture: FontImage,
            padding: 0.02,
            borderRadius: 0.05,
            backgroundColor: new Color(color),
        };

        const panel = new ThreeMeshUI.Block(blockOptions);

        const { x: posX, y: posY, z: posZ } = textPanelOptions.position;
        panel.position.set(posX, posY, posZ);

        textPanelOptions.textLines.forEach((textLine: any) => {
            const text = this.EstablishTextLine(textLine);
            panel.add(text);
        });

        return panel;
    }

    private EstablishMesh(options: any) {
        const geometry = GeometryBuilder.buildGeometry(options.geometry, options.material);
        const material = MaterialBuilder.buildMaterial(options.material);

        if (geometry.type === 'Group') {
            return geometry as Group;
        } else {
            const item = geometry as BufferGeometry;
            const entity = new Mesh(item, material);
            return entity;
        }
    }

    private RemovePanels(scene: Scene, options: any) {
        this.elementPanels.forEach((item) => {
            scene.remove(item);
        });
    }

    private GetTextPanelOptions(options: any) {
        const filterOptions = Boolean(options.scene) ? options.scene : options;
        if (Boolean(filterOptions.children)) {
            this.PanelGroups = filterOptions.children.filter((item: any) => {
                return item.type === 'PanelGroup';
            });
        }
    }

    public BuildPanelGroup(scene: Scene, options: any) {
        this.RemovePanels(scene, options);
        this.GetTextPanelOptions(options);
        this.PanelGroups.forEach((options) => {
            this.MakePanel(scene, options);
        });
    }
}

export const PanelGroupBuilder = new PanelGroupBuilderClass();
