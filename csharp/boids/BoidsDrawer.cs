using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace boids
{
    public class BoidsDrawer : Game
    {
        public static int[] Dimensions = { 1200, 720 };
        private int numBoids = 100;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BoidCollection _boids;
        private Texture2D _whiteRectangle;
        private Vector2 _boidSize = new Vector2(7f, 7f);
        private Color _bgColor = new Color(30, 30, 30);
        private Color _fgColor = new Color(100, 100, 170);

        public BoidsDrawer()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = Dimensions[0];
            _graphics.PreferredBackBufferHeight = Dimensions[1];
            _graphics.ApplyChanges();

            _boids = new BoidCollection(numBoids);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            _whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            _whiteRectangle.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            _boids.Move();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bgColor);

            _spriteBatch.Begin();

            float[] angles = _boids.GetAngles();
            float[][] boidPositions = _boids.GetPositions();

            for (var i = 0; i < boidPositions.Length; i++)
            {
                _spriteBatch.Draw(
                    _whiteRectangle,        // texture
                    new Vector2(
                        boidPositions[i][0],
                        boidPositions[i][1]
                        ), // Position
                    null,                   // Source rectangle
                    _fgColor,               // Color
                    angles[i],                  // Rotation
                    new Vector2(0.5f, 0.5f),// Origin
                    _boidSize,              // Scale
                    SpriteEffects.None,     // Effects
                    0f                      // layerDepth
                );
            }

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
