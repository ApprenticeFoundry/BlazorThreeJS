import { Color, Scene } from 'three';
import ThreeMeshUI, { BlockOptions } from 'three-mesh-ui';

const FontFamily = '/assets/Roboto-msdf.json';
const FontImage = '/assets/Roboto-msdf.png';

class TextPanelBuilderClass {
    private TextPanels: any[] = [];
    public elementPanels: Map<string, ThreeMeshUI.Block> = new Map<string, ThreeMeshUI.Block>();

    private MakePanel(scene: Scene, textPanelOptions: any) {
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

        const container = new ThreeMeshUI.Block(blockOptions);

        const { x: posX, y: posY, z: posZ } = textPanelOptions.position;
        container.position.set(posX, posY, posZ);

        const { x: rotX, y: rotY, z: rotZ } = textPanelOptions.rotation;
        container.rotation.set(rotX, rotY, rotZ);

        scene.add(container);

        textPanelOptions.textLines.forEach((textLine: any) => {
            const text = this.EstablishTextLine(textLine);
            container.add(text);
        });

        this.elementPanels.set(textPanelOptions.uuid, container);
    }

    private EstablishTextLine(textLine: string): ThreeMeshUI.Text {
        const text = new ThreeMeshUI.Text({
            content: `${textLine}\n`,
            fontSize: 0.15,
            backgroundColor: 'yellow',
        });
        return text;
    }

    private RemoveTextPanels(scene: Scene, options: any) {
        this.elementPanels.forEach((item) => {
            scene.remove(item);
        });
    }

    private GetTextPanelOptions(options: any) {
        const filterOptions = Boolean(options.scene) ? options.scene : options;
        if (Boolean(filterOptions.children)) {
            this.TextPanels = filterOptions.children.filter((item: any) => {
            return item.type === 'TextPanel';
        });
        }
    }

    public BuildTextPanels(scene: Scene, options: any) {
        this.RemoveTextPanels(scene, options);
        this.GetTextPanelOptions(options);
        this.TextPanels.forEach((menuOptions) => {
            this.MakePanel(scene, menuOptions);
        });
    }
}

export const TextPanelBuilder = new TextPanelBuilderClass();
