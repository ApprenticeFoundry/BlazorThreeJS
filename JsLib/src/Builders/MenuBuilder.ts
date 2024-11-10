import { Color, Scene } from 'three';
import ThreeMeshUI, { BlockOptions } from 'three-mesh-ui';

const FontFamily = '/assets/Roboto-msdf.json';
const FontImage = '/assets/Roboto-msdf.png';

class MenuBuilderClass {
    private Menus: any[] = [];
    public elementPanels: Map<string, ThreeMeshUI.Block> = new Map<string, ThreeMeshUI.Block>();
    public elementButtons: Map<string, ThreeMeshUI.Block> = new Map<string, ThreeMeshUI.Block>();

    private buttonOptions() {
        const buttonOptions = {
            width: 0.75,
            height: 0.15,
            justifyContent: 'center',
            offset: 0.05,
            margin: 0.02,
            borderRadius: 0.075,
        };

        return buttonOptions;
    }

    private buttonHoveredState() {
        // Options for component.setupState().
        // It must contain a 'state' parameter, which you will refer to with component.setState( 'name-of-the-state' ).

        const hoveredStateAttributes = {
            state: 'hovered',
            attributes: {
                offset: 0.035,
                backgroundColor: new Color(0x999999),
                backgroundOpacity: 1,
                fontColor: new Color(0xffffff),
            },
        };

        return hoveredStateAttributes;
    }

    private buttonIdleState() {
        const idleStateAttributes = {
            state: 'idle',
            attributes: {
                offset: 0.035,
                backgroundColor: new Color(0x666666),
                backgroundOpacity: 0.3,
                fontColor: new Color(0xffffff),
            },
        };

        return idleStateAttributes;
    }

    private buttonSelectedState() {
        const selectedAttributes = {
            offset: 0.02,
            backgroundColor: new Color(0x777777),
            fontColor: new Color(0x222222),
        };

        return selectedAttributes;
    }

    private MakePanel(scene: Scene, menuOptions: any) {
        const { width, height } = menuOptions;
        const blockOptions: BlockOptions = {
            width,
            height,
            // justifyContent: 'center',
            // contentDirection: 'row',
            fontFamily: FontFamily,
            fontTexture: FontImage,
            fontSize: 0.07,
            padding: 0.02,
            borderRadius: 0.05,
        };

        const container = new ThreeMeshUI.Block(blockOptions);

        const { x: posX, y: posY, z: posZ } = menuOptions.position;
        container.position.set(posX, posY, posZ);

        const { x: rotX, y: rotY, z: rotZ } = menuOptions.rotation;
        container.rotation.set(rotX, rotY, rotZ);

        scene.add(container);

        menuOptions.buttons.forEach((button: any) => {
            const panelButton = this.EstablishButton(button);
            container.add(panelButton);
        });

        this.elementPanels.set(menuOptions.uuid, container);
    }

    private EstablishButton(buttonOption: any): ThreeMeshUI.Block {
        const uiButton = new ThreeMeshUI.Block(this.buttonOptions());
        uiButton.uuid = buttonOption.uuid;
        uiButton.add(new ThreeMeshUI.Text({ content: buttonOption.text }));

        uiButton['setupState']({
            state: 'selected',
            attributes: this.buttonSelectedState(),
            onSet: () => {
                console.log(`MenuBuilder.MakePanel Clicked Button ${buttonOption.text}, ${buttonOption.uuid}`);
            },
        });
        uiButton['setupState'](this.buttonHoveredState());
        uiButton['setupState'](this.buttonIdleState());

        this.elementButtons.set(uiButton.uuid, uiButton);

        return uiButton;
    }

    private RemoveMenus(scene: Scene, options: any) {
        this.elementPanels.forEach((item) => {
            scene.remove(item);
        });
        this.elementButtons.forEach((item) => {
            scene.remove(item);
        });
    }

    private GetMenuOptionss(options: any) {
        const filterOptions = Boolean(options.scene) ? options.scene : options;
        if (Boolean(filterOptions.children)) {
            this.Menus = filterOptions.children.filter((item: any) => {
                return item.type === 'Menu';
            });
        }
    }

    public BuildMenus(scene: Scene, options: any) {
        this.RemoveMenus(scene, options);
        this.GetMenuOptionss(options);
        this.Menus.forEach((menuOptions) => {
            this.MakePanel(scene, menuOptions);
        });
    }
}

export const MenuBuilder = new MenuBuilderClass();
