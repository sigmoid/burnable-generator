using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Peridot;
using Peridot.Components;
using Peridot.EntityComponentScene.Serialization;
using Peridot.Graphics;
using Peridot.UI;

namespace burnable_generator;

public class Game1 : Core
{
    LoadImageUI _loadImageUI;
    CreateBurnableUI _createBurnableUI;
    MeshPropertiesUI _meshPropertiesUI;
    SaveDataUI _saveDataUI;

    Slider _previewSlider;

    Sprite _loadedImageSprite = null;
    Texture2D _loadedTexture = null;
    Texture2D _loadedSDFTexture = null;

    SpriteBatch _spriteBatch;
    SpriteBatch _uiBatch;
    SpriteFont _font;


    Effect _burnDecayEffect;

    BurnableData _burnableData = new BurnableData();

    Texture2D _pixel;

    public Game1()
: base("Burnable Generator", 1920, 1080, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _uiBatch = new SpriteBatch(GraphicsDevice);
        Core.Camera.CenterOn(new Vector2(0, 0));
    }


    protected override void LoadContent()
    {
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _font = Content.Load<SpriteFont>("fonts/JosefinSans");
        _burnDecayEffect = Content.Load<Effect>("shaders/burn-decay");

        CreateUI();

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkSlateGray);

        _spriteBatch.Begin(effect: _burnDecayEffect);
        _burnDecayEffect.Parameters["mainTexture"].SetValue(_loadedTexture);
        _burnDecayEffect.Parameters["sdfTexture"].SetValue(_loadedSDFTexture);
        _burnDecayEffect.Parameters["burnAmount"].SetValue(_previewSlider.Value);
        _burnDecayEffect.CurrentTechnique = _burnDecayEffect.Techniques["BurnDecay"];
        _burnDecayEffect.CurrentTechnique.Passes[0].Apply();

        _loadedImageSprite?.Draw(_spriteBatch, new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));
        _spriteBatch.End();

        if (_burnableData.Meshes != null && _burnableData.Meshes.Count > 0)
        {
            float t = _previewSlider.Value;
            float closestT = 0;
            foreach (var m in _burnableData.Meshes)
            {
                if (MathF.Abs(m.Amount - t) < MathF.Abs(closestT - t))
                {
                    closestT = m.Amount;
                }
            }

            var mesh = _burnableData.Meshes.FirstOrDefault(m => m.Amount == closestT);
            _meshPropertiesUI.SetMesh(mesh);
            DrawMesh(mesh);
        }

        _uiBatch.Begin();
        Core.UISystem.Draw(_uiBatch);
        _uiBatch.End();
    }

    private void LoadImage(Texture2D loadedTexture)
    {
        _loadedImageSprite = new Sprite(new TextureRegion(loadedTexture, 0, 0, loadedTexture.Width, loadedTexture.Height));
        _loadedImageSprite.Origin = new Vector2(loadedTexture.Width / 2, loadedTexture.Height / 2);

        _loadedSDFTexture = CreateSDF.ConvertImageToSDF(loadedTexture);

    }

    private void GenerateBurnableData()
    {
        var iterations = _createBurnableUI.Iterations;
        var tolerance = _createBurnableUI.Tolerance;
        _burnableData = new BurnableData();
        _burnableData.Meshes = new List<Mesh>();

        for (int i = 0; i < iterations; i++)
        {
            float t = (float)i / (iterations - 1);

            var mesh = MeshGenerator.GenerateMesh(_loadedSDFTexture, t, true, tolerance);
            mesh.Amount = t;

            _burnableData.Meshes.Add(mesh);
        }
    }


    private void DrawMesh(Mesh mesh)
    {
        _spriteBatch.Begin();
        for (int i = 0; i < mesh.Vertices.Count; i += 2)
        {
            if (i + 1 < mesh.Vertices.Count)
            {
                var v1 = mesh.Vertices[i];
                var v2 = mesh.Vertices[i + 1];

                Vector2 p1 = new Vector2(v1.X, v1.Y);
                Vector2 p2 = new Vector2(v2.X, v2.Y);

                DrawLine(p1 * new Vector2(_loadedImageSprite.Width, _loadedImageSprite.Height) + new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2) - new Vector2(_loadedImageSprite.Width / 2, _loadedImageSprite.Height / 2),
                p2 * new Vector2(_loadedImageSprite.Width, _loadedImageSprite.Height) + new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2) - new Vector2(_loadedImageSprite.Width / 2, _loadedImageSprite.Height / 2),
                Color.OrangeRed, 5);
            }
        }
        _spriteBatch.End();
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 2f)
    {
        float distance = Vector2.Distance(start, end);
        float angle = (float)MathF.Atan2(end.Y - start.Y, end.X - start.X);

        Vector2 origin = new Vector2(0f, 0.5f);
        Vector2 scale = new Vector2(distance, thickness);

        _spriteBatch.Draw(_pixel, start, null, color, angle, origin, scale, SpriteEffects.None, 0);
    }

    private void CreateUI()
    {
        CreateLoadImageUI();
        CreateBurnableUI();
        CreatePreviewSlider();
        CreateMeshPropertiesUI();
        CreateSaveDataUI();
    }

    private void CreateLoadImageUI()
    {
        _loadImageUI = new LoadImageUI(_font, (loadedTexture) =>
        {
            LoadImage(loadedTexture);
        });
        _loadImageUI.Create();
        Core.UISystem.AddElement(_loadImageUI.GetUIElement());
    }

    private void CreateBurnableUI()
    {
        _createBurnableUI = new CreateBurnableUI(_font, () =>
        {
            GenerateBurnableData();
        });
        _createBurnableUI.Create();
        Core.UISystem.AddElement(_createBurnableUI.GetUIElement());
    }

    private void CreatePreviewSlider()
    {
        _previewSlider = new Slider(new Rectangle(Core.GraphicsDevice.Viewport.Width / 2 - 300, Core.GraphicsDevice.Viewport.Height - 50, 600, 30), 0, 1, 0.0f);
        Core.UISystem.AddElement(_previewSlider);
    }

    private void CreateMeshPropertiesUI()
    {
        _meshPropertiesUI = new MeshPropertiesUI(_font);
        _meshPropertiesUI.Create();
        Core.UISystem.AddElement(_meshPropertiesUI.GetUIElement());
    }

    private void CreateSaveDataUI()
    {
        _saveDataUI = new SaveDataUI(_font, () =>
        {
            var filePath = _saveDataUI.GetFilePath();
            var meshData = _burnableData;

            var serializedString = JsonSerializer.Serialize(meshData);
            // if path does not exist, create it
            var directory = System.IO.Path.GetDirectoryName(filePath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            System.IO.File.WriteAllText(filePath, serializedString);

            _saveDataUI.SetSuccessMessage("Data saved to " + filePath);
        });
        _saveDataUI.Create();

        Core.UISystem.AddElement(_saveDataUI.GetUIElement());
    }

}
