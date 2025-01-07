using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
namespace MinecraftAM
{
    public class Compass
    {
        protected Texture2D compass;
        public void LoadContent(ContentManager Content)
        {
            compass = Content.Load<Texture2D>("compass");
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (compass == null)
            {
                Console.WriteLine("Compass Draw error: compass == null");
                return;
            }
            if (spriteBatch == null)
            {
                Console.WriteLine("Compass Draw error: spriteBatch == null");
                return;
            }
            if (Globals.userPlayer == null)
            {
                Console.WriteLine("Compass Draw error: Globals.userPlayer == null");
                return;
            }
            if (AMSettings.bRotate)
            {
                int newWidth;
                if (Globals.screenWidth < Globals.screenHeight)
                    newWidth = Globals.screenWidth / 10;
                else
                    newWidth = Globals.screenHeight / 10;
                newWidth = Math.Max(newWidth, 25);
                int distanceFromEdge = newWidth == 25 ? 10 : 25;
                spriteBatch.Draw(compass, new Rectangle(Globals.screenWidth - newWidth / 2 - distanceFromEdge,
                    Globals.screenHeight - newWidth / 2 - distanceFromEdge,
                    newWidth, newWidth), null, Color.White, -(float)Globals.userPlayer.rotation, new Vector2(compass.Width / 2, compass.Height / 2), SpriteEffects.None, 0);
            }
        }
    }
}