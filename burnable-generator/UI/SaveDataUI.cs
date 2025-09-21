using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Peridot;
using Peridot.UI;

public class SaveDataUI
{
    private IUIElement root;

    private SpriteFont _font;
    private Action _onActivated;
    private string _defaultFilePath = "F:\\Dev\\burnable-generator\\Meshes\\";
    private string _fileName = "burnable_data.json";

    public SaveDataUI(SpriteFont font, Action onActivated)
    {
        _font = font;
        _onActivated = onActivated;
    }

    public IUIElement GetUIElement()
    {
        return root;
    }

    public void Create()
    {
        VerticalLayoutGroup vLayout = new VerticalLayoutGroup(new Rectangle(10, Core.GraphicsDevice.Viewport.Height - 250, 500, 100), 5);
        Label successLabel = new Label(new Rectangle(10, 10, 300, 20), "", _font, Color.Green);
        TextInput filePathInput = new TextInput(new Rectangle(10, 10, 300, 60), _font, _defaultFilePath);
        TextInput fileNameInput = new TextInput(new Rectangle(10, 80, 300, 60), _font, _fileName);
        Button saveButton = new Button(new Rectangle(10, 10, 150, 80), "Save Data", _font, Color.Gray, Color.LightGray, Color.White, () =>
        {
            _onActivated?.Invoke();
        });

        filePathInput.OnTextChanged += (text) =>
        {
            _defaultFilePath = text;
        };
        fileNameInput.OnTextChanged += (text) =>
        {
            _fileName = text;
        };

        vLayout.AddChild(successLabel);
        vLayout.AddChild(filePathInput);
        vLayout.AddChild(fileNameInput);
        vLayout.AddChild(saveButton);
        root = vLayout;
    }

    public void SetSuccessMessage(string message)
    {
        if (root is VerticalLayoutGroup vLayout && vLayout.Children.Count > 0 && vLayout.Children[0] is Label successLabel)
        {
            successLabel.Text = message;
        }
    }

    public string GetFilePath()
    {
        var res = System.IO.Path.Combine(_defaultFilePath, _fileName);
        return res;
    }
}