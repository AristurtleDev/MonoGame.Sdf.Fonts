using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Sdf.Fonts;

namespace SdfFontExample;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private BitmapFont _bitmapFont;
    private Effect _sdfEffect;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        //  Load the .fnt file from disk
        using (FileStream stream = TitleContainer.OpenStream("Content/cute-dino.fnt") as FileStream)
        {
            _bitmapFont = BitmapFont.FromStream(GraphicsDevice, stream);
        }

        //  Load the SDF font shader effect
        _sdfEffect = Content.Load<Effect>("sdf-effect");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        //  When drawing the Distance Field Font, ensure you use the SDF Effect shader or it will not render properly
        //  You will need to provide the Spread value that you set in Hiero as well as the scale that you are rendering
        //  the font at.
        _sdfEffect.Parameters["Spread"].SetValue(4.0f);
        _sdfEffect.Parameters["Scale"].SetValue(1.0f);

        _spriteBatch.Begin(effect: _sdfEffect);
        _spriteBatch.DrawString(_bitmapFont, "Hello World", Vector2.Zero, Color.White);
        _spriteBatch.End();

        //  For instance, if we were now to render it at a scale of 5, we would change the scale for this batch to five.
        _sdfEffect.Parameters["Scale"].SetValue(5.0f);
        _spriteBatch.Begin(effect: _sdfEffect);
        _spriteBatch.DrawString(_bitmapFont, "Hello World", new Vector2(0, 100), Color.White, 0.0f, Vector2.Zero, scale: 5.0f, 0);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
