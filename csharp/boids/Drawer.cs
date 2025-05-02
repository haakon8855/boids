using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boids.DataStructures;
using Boids.Models;

namespace Boids;

public class Drawer : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private BoidCollection Boids { get; set; }
    private Texture2D WhiteRectangle { get; set; }
    private Vector2 _boidSize = new Vector2(7f, 7f);
    private Color _bgColor = new Color(30, 30, 30);
    private Color _fgColor = new Color(100, 100, 170);
    private Config Config { get; set; }

    public Drawer(Config config)
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Config = config;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = Config.Dimensions.Width;
        _graphics.PreferredBackBufferHeight = Config.Dimensions.Height;
        _graphics.ApplyChanges();

        Boids = new BoidCollection(Config);

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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        Boids.Move();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_bgColor);

        _spriteBatch.Begin();

        var angles = Boids.Angles;
        var boidPositions = Boids.Positions;

        for (var i = 0; i < boidPositions.Length; i++)
        {
            _spriteBatch.Draw(
                texture: WhiteRectangle,
                position: new Vector2(
                    (float)boidPositions[i][0],
                    (float)boidPositions[i][1]
                ),
                sourceRectangle: null,
                color: _fgColor,
                rotation: (float)angles[i],
                origin: new Vector2(0.5f, 0.5f),
                scale: _boidSize,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}