using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Trashbots1
{
    public class Trashbots : Microsoft.Xna.Framework.Game
    {
        private enum GameStates
        {
            Menu, Play
        }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Trashbot trashbot1;

        GameStates gameState = GameStates.Menu;

        public Trashbots()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = Util.ScreenWidth;
            graphics.PreferredBackBufferHeight = Util.ScreenHeight;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Trashbot.Content = Content;
            Trashbot.spriteBatch = spriteBatch;
            ItemPool.Content = Content;
            ItemPool.spriteBatch = spriteBatch;
            font = Content.Load<SpriteFont>("font");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            switch(gameState)
            {
                case GameStates.Menu:
                //if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                //{
                    trashbot1 = new Trashbot(PlayerIndex.One);
                    gameState = GameStates.Play;
                    ItemPool.Start();
                //}
                break;
                case GameStates.Play:
                    trashbot1.Update(gameTime);
                    ItemPool.Update();
                break;
            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (gameState == GameStates.Play)
            {
                ItemPool.Draw();
                trashbot1.Draw();
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
