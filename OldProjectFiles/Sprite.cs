using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;

namespace GameDesignTestArea
{
    /// <summary>
    /// The Sprite class is created for inheritance purposes; everything in the game is a sprite of some form, and
    /// this class therefore represents common elements that the sprites all have, such as position and asset names.
    /// </summary>
    public class Sprite
    {

        // Variables; All sprites have an asset name, a position, a size, a scaling rate, a texture/pic associated with it.
        public string assetName;
        public Vector2 Position = new Vector2();
        public Rectangle spriteSize;
        private float individualScale = 1.0f;
        private float scale = 2.0f;
        public Texture2D currentSpriteTexture;

        // This is individual positions outside of the vector, used for logical data purposes; For example, identifying it's position
        // on the current map's data array, which cannot be seen by the player.
        public int x = 0, y = 0;

        /// <summary>
        /// Load the necessary assets for the Sprite. The assetName string should the path/file name for the sprite.
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="newSpriteAssetName"></param>
        /// <param name="startingPositionX"></param>
        /// <param name="startingPositionY"></param>
        public void LoadContent(ContentManager contentManager, string newSpriteAssetName, int startingPositionX, int startingPositionY)
        {
            assetName = newSpriteAssetName;
            currentSpriteTexture = contentManager.Load<Texture2D>(assetName);
            spriteSize = new Rectangle(startingPositionX, startingPositionY, (int)(currentSpriteTexture.Width), (int)(currentSpriteTexture.Height));
        }
        
        /// <summary>
        /// Draw the given sprite to the screen, utilizing the texture, size (and scale), and position.
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(currentSpriteTexture, Position, new Rectangle(0, 0, currentSpriteTexture.Width, currentSpriteTexture.Height), Color.White, 0.0f, Vector2.Zero, (Scale*IndividualScale), SpriteEffects.None, 0);
        }

        // Draw function used for movement squares, parameterized differently.
        public void Draw(SpriteBatch theSpriteBatch, int x, int y)
        {
            Position = new Vector2(((x * 32)+1) * Scale, ((y * 32)+1) * Scale);
            theSpriteBatch.Draw(currentSpriteTexture, Position, new Rectangle(0, 0, currentSpriteTexture.Width, currentSpriteTexture.Height), Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// The Scale property, used to retrieve scale as normal, or if necessesary, change the scale.
        /// A special circumstance of this property is that the sprite size for all sprites will be changed in accordance with the scale,
        /// as well as any movements done.
        /// </summary>
        public float Scale
        {
            get { return this.scale; }
            set
            {
                this.scale = value;
                //Recalculate the Size of the Sprite with the new scale
                //spriteSize = new Rectangle(0, 0, (int)(currentSpriteTexture.Width * Scale), (int)(currentSpriteTexture.Height * Scale));
            }
        }

        public float IndividualScale
        {
            get { return this.individualScale; }
            set
            {
                this.individualScale = Scale * value;
            }
        }
    }
}
