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
    class MovementSquare : Sprite
    {
        const string MOVEMENT_SQUARE_ASSET_NAME = "MyArt/movementSquare";

        public void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager, MOVEMENT_SQUARE_ASSET_NAME, x, y);
        }

        /// <summary>
        /// This method uses the movement properties of the passed character, the given map's boundaries, and the cursor, in order to draw the movement squares based on
        /// the character sprite's given position, allowing the visual indication of where the cursor can move the character to.
        /// </summary>
        /// <param name="currentCharacter"></param>
        /// <param name="CurrentMap"></param>
        /// <param name="spriteBatch"></param>
        public void DrawWithinCharacterRange(Character currentCharacter, MapSprite CurrentMap, SpriteBatch spriteBatch, bool cursorGrabbedSprite)
        {
            base.x = currentCharacter.x;
            base.y = currentCharacter.y;
            base.Position = new Vector2(currentCharacter.x * 32 * Scale, currentCharacter.y * 32 * Scale);

            int theMovementLimit = currentCharacter.CharacterMovement;
            
            // The following routine loops through the map squares, and draws a square in the appropriate movement range of the selected sprite character.
            for (int rowY = 0; rowY <= CurrentMap.GetPositionYLimit(); rowY++)
            {
                for (int colX = 0; colX <= CurrentMap.GetPositionXLimit(); colX++)
                {
                    // Algorithm to draw the blue squares based on the location relevant to the passed character's movement space.
                    
                    // The following variables are used simply to make ease of typing since it's used a few times.
                    int movementLeft = currentCharacter.CharacterMovement;
                    // The xLimit and yLimit variables represent the absolute value on either side of the square.
                    int xLimitFromSprite = Math.Abs(currentCharacter.x - colX);
                    int yLimitFromSprite = Math.Abs(currentCharacter.y - rowY);
                    
                    // This conditional has it draw, when the routine is finished, a square with a "radius" of the character's movement spaces.
                    if (xLimitFromSprite <= movementLeft && yLimitFromSprite <= movementLeft)
                    {
                        // Refining our original conditional statement, this will draw the squares such that the most a character can move is up to the combined values
                        //      of the x and y limitations, effectively making the max movement in ANY direction, or path, the character sprite's movement.
                        if (Math.Abs(xLimitFromSprite + yLimitFromSprite) <= movementLeft)
                        {
                            // Now that we have the basic framework drawing of the movement squares, we can use situational conditionals,
                            // such as if there is a sprite in the way. The convenience of the movement squares is also later used to say that we can
                            // move to where there is a square drawn.
                            if (CurrentMap.IsThereASpriteAtThePosition(colX, rowY) == false || CurrentMap.characterMapDataArray[colX, rowY] == currentCharacter)
                            {
                                base.Draw(spriteBatch, colX, rowY);

                                // If this method is called via hover, do NOT set it to true. Only if it's grabbed.
                                if(cursorGrabbedSprite == true)
                                {
                                    CurrentMap.movementSquareMapDataArray[colX, rowY] = true;
                                }

                            } // End if-statement checking for sprites.
                        } // End if statement checking for total movement values within the square that was "drawn".
                    } // End if statement for refining the drawing down to a square around the character based on the passed character's movement.
                } // End nested for-loop going through the columns of the map.
            } // End for loop going through the rows of the map.
        } // End DrawWithinCharacterRange method.


    } // End MovementSquare class.
} // End GameDesignTestArea namespace.
