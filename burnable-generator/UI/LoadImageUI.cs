using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Peridot;
using Peridot.UI;

public class LoadImageUI
{
    private IUIElement root;
    private string _imagePath;
    private Texture2D _loadedTexture;
    private Action<Texture2D> _onImageLoaded;
    private SpriteFont _font;
    private Label _infoLabel;

    public LoadImageUI(SpriteFont font, Action<Texture2D> onImageLoaded)
    {
        _imagePath = string.Empty;
        _loadedTexture = null;
        _font = font;
        _onImageLoaded = onImageLoaded;
    }

    public IUIElement GetUIElement()
    {
        return root;
    }

    public void Create()
    {
        VerticalLayoutGroup vLayout = new VerticalLayoutGroup(new Microsoft.Xna.Framework.Rectangle(25, 25, 900, 100), 5);
        HorizontalLayoutGroup hLayout = new HorizontalLayoutGroup(new Microsoft.Xna.Framework.Rectangle(10, 10, 900, 50), 5);
        TextInput filePathInput = new TextInput(new Microsoft.Xna.Framework.Rectangle(0, 0, 700, 60), _font, "Enter image file path...");

        filePathInput.OnTextChanged += (text) =>
        {
            _imagePath = text;
        };

        filePathInput.OnEnterPressed += (text) =>
        {
            _imagePath = text;
            LoadImage();
        };

        Button loadButton = new Button(new Microsoft.Xna.Framework.Rectangle(0, 0, 100, 60), "Load",  _font, Color.Gray, Color.LightGray, Color.White, () =>
        {
            string filePath = _imagePath;
            LoadImage();
        });
        hLayout.AddChild(filePathInput);
        hLayout.AddChild(loadButton);
        vLayout.AddChild(hLayout);

        _infoLabel = new Label(new Microsoft.Xna.Framework.Rectangle(10, 70, 300, 20), "", _font, Color.Red);
        vLayout.AddChild(_infoLabel);

        root = vLayout;
    }


    private void LoadImage()
    {
        if (string.IsNullOrEmpty(_imagePath))
        {
            System.Console.WriteLine("Image path is empty.");
            return;
        }

        try
        {
            using (var stream = System.IO.File.OpenRead(_imagePath))
            {
                _loadedTexture = Texture2D.FromStream(Core.GraphicsDevice, stream);
            }

            _infoLabel.SetText("");
            _onImageLoaded?.Invoke(_loadedTexture);
        }
        catch (System.Exception ex)
        {
            _infoLabel.SetText("Failed to load image.");
            System.Console.WriteLine($"Failed to load image: {ex.Message}");
        }
    }
}