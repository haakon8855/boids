using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boids.DataStructures;
using Boids.Models;

namespace Boids;

public class Monogame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private BoidCollection Boids { get; set; }
    private Texture2D WhiteRectangle { get; set; }
    private Configuration Configuration { get; set; }

    public Monogame(Configuration configuration)
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Configuration = configuration;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = Configuration.Window.Width;
        _graphics.PreferredBackBufferHeight = Configuration.Window.Height;
        _graphics.ApplyChanges();

        Boids = new BoidCollection(Configuration);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        WhiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
        WhiteRectangle.SetData([Color.White]);
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
        GraphicsDevice.Clear(new Color(
            Configuration.Window.BackgroundColor[0],
            Configuration.Window.BackgroundColor[1],
            Configuration.Window.BackgroundColor[2])
        );

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
                color: new Color(
                    Configuration.Window.BoidColor[0],
                    Configuration.Window.BoidColor[1],
                    Configuration.Window.BoidColor[2]
                ),
                rotation: (float)angles[i],
                origin: new Vector2(0.5f, 0.5f),
                scale: Configuration.Boids.BoidSize,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}