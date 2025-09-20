using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Peridot;
using Peridot.Components;
using Peridot.EntityComponentScene.Serialization;
using Peridot.Graphics;

namespace burnable_generator;

public class Game1 : Core
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    LoadImageUI _loadImageUI;
    Sprite _loadedImageSprite = null;
    TextureRegion _loadedImageRegion = null;

    Entity _previewEntity = null;

    SpriteFont _font;
    public Game1()
    : base("Burnable Generator", 1920, 1080, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        Core.Camera.CenterOn(new Vector2(0, 0));
    }


    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("fonts/JosefinSans");

        _loadImageUI = new LoadImageUI(_font, (loadedTexture) =>
        {
            _loadedImageRegion = new TextureRegion(loadedTexture, 0, 0, loadedTexture.Width, loadedTexture.Height);
            _loadedImageSprite = new Sprite(_loadedImageRegion);
            var newEntity = new Entity();
            var spriteComponent = new SpriteComponent(_loadedImageSprite);
            newEntity.AddComponent(spriteComponent);
            newEntity.Position = new Vector2(0, 0);
            Core.CurrentScene.RemoveEntity(_previewEntity);
            _previewEntity = newEntity;
            Core.CurrentScene.AddEntity(_previewEntity);
        });
        _loadImageUI.Create();

        Core.UISystem.AddElement(_loadImageUI.GetUIElement());

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}
