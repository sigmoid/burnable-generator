using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Peridot;
using Peridot.UI;

public class CreateBurnableUI
{
    private IUIElement root;

    private SpriteFont _font;
    private Action _onActivated;

    public int Iterations { get; private set; } = 100;

    public float Tolerance { get; private set; } = 0.1f;

    public CreateBurnableUI(SpriteFont font, Action onActivated)
    {
        _onActivated = onActivated;
        _font = font;
    }

    public IUIElement GetUIElement()
    {
        return root;
    }

    public void Create()
    {
        VerticalLayoutGroup vLayout = new VerticalLayoutGroup(new Microsoft.Xna.Framework.Rectangle(Core.GraphicsDevice.Viewport.Width - 550, Core.GraphicsDevice.Viewport.Height - 350, 500, 100), 5);

        Label toleranceLabel = new Label(new Microsoft.Xna.Framework.Rectangle(0, 0, 150, 60), "Tolerance:", _font, Color.White);
        TextInput toleranceInput = new TextInput(new Microsoft.Xna.Framework.Rectangle(0, 0, 300, 60), _font, "0.1");

        TextInput iterationsInput = new TextInput(new Microsoft.Xna.Framework.Rectangle(0, 0, 300, 60), _font, "100");
        iterationsInput.OnTextChanged += (text) =>
        {
            if (int.TryParse(text, out int value))
            {
                Iterations = value;
            }
        };
        toleranceInput.OnTextChanged += (text) =>
        {
            if(float.TryParse(text, out float value))
            {
                Tolerance = value;
            }
        };
        HorizontalLayoutGroup hLayout = new HorizontalLayoutGroup(new Microsoft.Xna.Framework.Rectangle(10, 10, 00, 60), 5);
        hLayout.AddChild(toleranceLabel);
        hLayout.AddChild(toleranceInput);

        var hLayout2 = new HorizontalLayoutGroup(new Microsoft.Xna.Framework.Rectangle(10, 80, 400, 60), 5);
        hLayout2.AddChild(new Label(new Microsoft.Xna.Framework.Rectangle(0, 0, 150, 60), "Iterations:", _font, Color.White));
        hLayout2.AddChild(iterationsInput);

        Button ActivateButton = new Button(new Microsoft.Xna.Framework.Rectangle(0, 0, 150, 80), "Create Burnable",  _font, Color.Gray, Color.LightGray, Color.White, () =>
        {
            _onActivated?.Invoke();
        });

        vLayout.AddChild(hLayout);
        vLayout.AddChild(hLayout2);
        vLayout.AddChild(ActivateButton);

        root = vLayout;
    }
}