using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace boids
{
    public class BoidsDrawer : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Boid _boid;
        private Texture2D _whiteRectangle;
        private Vector2 _boidSize = new Vector2(10f, 10f);
        private Color _bgColor = new Color(30, 30, 30);
        private Color _fgColor = new Color(100, 100, 170);

        public BoidsDrawer()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _boid = new Boid();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            _boid.Move();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bgColor);

            _spriteBatch.Begin();

            float angle = _boid.GetAngle();

            float[] boidPos = _boid.GetPosition();
            _spriteBatch.Draw(
                _whiteRectangle,        // texture
                new Vector2(boidPos[0], boidPos[1]), // Position
                null,                   // Source rectangle
                _fgColor,               // Color
                angle,                  // Rotation
                new Vector2(0.5f, 0.5f),// Origin
                _boidSize,              // Scale
                SpriteEffects.None,     // Effects
                0f                      // layerDepth
            );

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
