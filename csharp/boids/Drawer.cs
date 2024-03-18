using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boids.DataStructures;

namespace Boids;

public class Drawer : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static int[] Dimensions { get; private set; } = { 1200, 720 };
    public int NumBoids { get; private set; } = 250;
    public BoidCollection Boids { get; private set; }
    public Texture2D WhiteRectangle { get; private set; }
    private Vector2 _boidSize = new Vector2(7f, 7f);
    private Color _bgColor = new Color(30, 30, 30);
    private Color _fgColor = new Color(100, 100, 170);

    public Drawer()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = Dimensions[0];
        _graphics.PreferredBackBufferHeight = Dimensions[1];
        _graphics.ApplyChanges();

        Boids = new BoidCollection(NumBoids);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        WhiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
        WhiteRectangle.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Boids.Move();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_bgColor);

        _spriteBatch.Begin();

        var angles = Boids.Angles();
        var boidPositions = Boids.Positions();

        for (var i = 0; i < boidPositions.Length; i++)
        {
            _spriteBatch.Draw(
                WhiteRectangle,            // texture
                new Vector2(
                    (float)boidPositions[i][0],
                    (float)boidPositions[i][1]
                    ),                     // Position
                null,                      // Source rectangle
                _fgColor,                  // Color
                (float)angles[i],          // Rotation
                new Vector2(0.5f, 0.5f),   // Origin
                _boidSize,                 // Scale
                SpriteEffects.None,        // Effects
                0f                         // layerDepth
            );
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
