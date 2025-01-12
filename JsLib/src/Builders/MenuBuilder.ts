import { Color } from 'three';
import ThreeMeshUI, { BlockOptions } from 'three-mesh-ui';
import { ObjectLookup } from '../Utils/ObjectLookup';

const FontFamily = '/assets/Roboto-msdf.json';
const FontImage = '/assets/Roboto-msdf.png';

class MenuBuilderClass {

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

        return uiButton;
    }

    public CreateMenuPanel(options: any): ThreeMeshUI.Block {
        const { width, height } = options;

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

        options.buttons.forEach((button: any) => {
            const panelButton = this.EstablishButton(button);
            ObjectLookup.addButton(button.uuid, panelButton);
            container.add(panelButton);
        });
        return container;
    }

    public RefreshMenuPanel(options: any, container: ThreeMeshUI.Block): ThreeMeshUI.Block {
        var transform = options.transform;

        const { x: posX, y: posY, z: posZ } = transform.position;
        container.position.set(posX, posY, posZ);

        const { x: rotX, y: rotY, z: rotZ } = transform.rotation;
        container.rotation.set(rotX, rotY, rotZ);
        return container;
    }

}

export const MenuBuilder = new MenuBuilderClass();
