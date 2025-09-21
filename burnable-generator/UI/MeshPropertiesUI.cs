
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Peridot;
using Peridot.UI;

public class MeshPropertiesUI
{
    private Label _vertexCountLabel;

    SpriteFont _font;

    public MeshPropertiesUI(SpriteFont font)
    {
        _font = font;
    }

    public void Create()
    {
        _vertexCountLabel = new Label(new Rectangle(Core.GraphicsDevice.Viewport.Width - 200, Core.GraphicsDevice.Viewport.Height/2-25, 200, 50), "Vertex Count: 0", _font, Color.White);
    }

    public void SetMesh(Mesh mesh)
    {
        _vertexCountLabel.SetText($"Vertex Count: {mesh.Vertices.Count}");
    }

    public IUIElement GetUIElement()
    {
        return _vertexCountLabel;
    }
}