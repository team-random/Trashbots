using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Trashbots1
{
    public class Trashbot
    {
        public static ContentManager Content;
        public static SpriteBatch spriteBatch;

        private class Suction
        {
            static Texture2D spriteSheet;
            int index = 0;
            int frameTime = 200;
            int lastFrameTime = 0;
            Rectangle rectangle;
            Vector2 center;
            Dimension singleSprite;

            public bool IsOn = false;

            public Suction()
            {
                spriteSheet = Content.Load<Texture2D>("sucksheet");
                singleSprite = new Dimension(spriteSheet.Width / Util.ColumnsInSuckSpriteSheet, spriteSheet.Height / Util.LinesInSuckSpriteSheet);
                center = new Vector2(singleSprite.width / 2, singleSprite.height);
            }
            public void Start(GameTime gameTime)
            {
                Update(gameTime);
                IsOn = true;
            }
            public void Stop()
            {
                IsOn = false;
                index = 0;
            }
            public void Update(GameTime gameTime)
            {
                if (IsOn)
                {
                    lastFrameTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (lastFrameTime >= frameTime)
                    {
                        if (index > 3)
                        {
                            index = 0;
                        }
                        rectangle = new Rectangle((int)(index * singleSprite.width), 0,
                            (int)singleSprite.width, (int)singleSprite.height);
                        index++;
                        lastFrameTime -= frameTime;
                    }
                }
            }
            public void Draw(Color color, float rotationAngle, Vector2 position)
            {
                if (IsOn)
                    spriteBatch.Draw(spriteSheet, position, rectangle, color, rotationAngle, center, scale, SpriteEffects.None, 0f);
            }
        }
        class Dimension
        {
            public float width;
            public float height;
            public Dimension(float width, float height)
            {
                this.width = width;
                this.height = height;
            }
        }

        static Texture2D texture;
        static Dimension scaledTexture;
        private static float scale = 0.15f;

        float rotationAngle;
        Vector2 center;
        Vector2 position;
        Vector2 positionMove;
        PlayerIndex player;
        Suction suckAction;
        Color color;
        GamePadState previousState;

        public Trashbot(PlayerIndex player)
        {
            this.player = player;
            texture = Content.Load<Texture2D>("trashbot");
            scaledTexture = new Dimension(texture.Width * scale, texture.Height * scale);
            center = new Vector2(texture.Width / 2, texture.Height / 2);
            suckAction = new Suction();
            setInitialPosition();
            previousState = GamePad.GetState(player);

        }

        private void setInitialPosition()
        {
            //TODO: Waht's gonna be the initial position for each bot??
            position = new Vector2(200);
        }

        bool slowing = false;
        bool triggerPressed = false;
        float triggerForce;
        float brake = 1.0f;
        bool buttonPressed = false;

        public void Update(GameTime gameTime)
        {
            // Get the game pad state.
            GamePadState currentState = GamePad.GetState(player);
            if (currentState.IsConnected)
            {
                #region Rotation
                if (currentState.ThumbSticks.Left.Y == 0)
                {
                    if (currentState.ThumbSticks.Left.X == 1)
                    {
                        rotationAngle = MathHelper.PiOver2;
                    }
                    if (currentState.ThumbSticks.Left.X == -1)
                    {
                        rotationAngle = -MathHelper.PiOver2;
                    }

                }
                else
                {
                    rotationAngle = (float)Math.Atan((double)(currentState.ThumbSticks.Left.X / currentState.ThumbSticks.Left.Y));
                    if (currentState.ThumbSticks.Left.Y < 0)
                    {
                        rotationAngle += MathHelper.Pi;
                    }
                }
                #endregion
                #region Movement
                if (currentState.Triggers.Right > 0)
                {
                    triggerPressed = true;
                    positionMove.X = (float)Math.Sin(rotationAngle);
                    positionMove.Y = -(float)Math.Cos(rotationAngle);
                    positionMove *= currentState.Triggers.Right * 10;
                    triggerForce = currentState.Triggers.Right;
                    position += positionMove;

                    brake = 1.0f;
                    slowing = false;

                }
                else
                {
                    if (triggerPressed) //Just released
                    {
                        slowing = true;
                        if (triggerForce < 0.2)
                            positionMove *= 4;
                    }

                    if (slowing)
                    {
                        if (brake > 0)
                        {
                            position += positionMove * brake;
                            brake -= 0.05f;
                        }
                        else
                        {
                            brake = 1.0f;
                            slowing = false;
                        }
                    }

                    triggerPressed = false;
                }
                #endregion
                #region Bound Movement
                if (position.X < 0 + scaledTexture.width / 2)
                {
                    position.X = 0 + scaledTexture.width / 2;
                }
                if (position.X > Util.ScreenWidth - scaledTexture.width / 2)
                {
                    position.X = Util.ScreenWidth - scaledTexture.width / 2;
                }
                if (position.Y < 0 + scaledTexture.width / 2)
                {
                    position.Y = 0 + scaledTexture.width / 2;
                }
                if (position.Y > Util.ScreenHeight - scaledTexture.height / 2)
                {
                    position.Y = Util.ScreenHeight - scaledTexture.height / 2;
                }
                #endregion
                #region Suck Action
                if (currentState.Buttons.A == ButtonState.Pressed && previousState.Buttons.A==ButtonState.Released)
                {
                    color = Color.Green;
                }
                if (currentState.Buttons.B == ButtonState.Pressed && previousState.Buttons.B == ButtonState.Released)
                {
                    color = Color.Red;
                }
                if (currentState.Buttons.Y == ButtonState.Pressed && previousState.Buttons.Y == ButtonState.Released)
                {
                    color = Color.Yellow;
                }
                if (currentState.Buttons.X == ButtonState.Pressed && previousState.Buttons.X == ButtonState.Released)
                {
                    color = Color.Blue;
                }
                if (currentState.IsButtonDown(Buttons.A) || currentState.IsButtonDown(Buttons.B) || currentState.IsButtonDown(Buttons.X) || currentState.IsButtonDown(Buttons.Y))
                {   
                    if (!buttonPressed)
                        suckAction.Start(gameTime);
                    else
                        suckAction.Update(gameTime);
                    buttonPressed = true;
                }else
                {
                    buttonPressed = false;
                    suckAction.Stop();
                }
               

                #endregion

                previousState = currentState;
            }
        }

        public void Draw()
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotationAngle, center, scale, SpriteEffects.None, 0f);
            suckAction.Draw(color, rotationAngle, new Vector2(position.X, position.Y));
        }
    }
}
