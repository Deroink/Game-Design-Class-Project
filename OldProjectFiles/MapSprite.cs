using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

// This namespace is used for debugging purposes; delete when it's no longer needed.
using System.Windows.Forms;

namespace GameDesignTestArea
{
    public class MapSprite : Sprite
    {
        // This 2D array serves as metadata to the game; it gives a description of what it is.
        // For example, it might specify one position is a string, while another is forest, or water.
        char[,] mapMetaDataArray;
        // Used for finding the sprite.
        public Character[,] characterMapDataArray;
        // Array used for indicating where movement squares have been drawn.
        public bool[,] movementSquareMapDataArray;
        // Used for limits of arrays of data associated with the map, but track the individual values for conditional purposes.
        int positionXLimit = 0;
        int positionYLimit = 0;

        int numberOfAllies = 0;
        int numberOfEnemies = 0;

        
        
        /// <summary>
        /// LoadContent is used to set the map name and position.
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="mapName"></param>
        public void LoadContent(ContentManager contentManager, string mapName)
        { 
            base.LoadContent(contentManager, mapName, 0, 0);
            base.IndividualScale = 0.25f;
        }

        /// <summary>
        /// Set the mapLiteralDataArray and mapMetaDataArray's range, used to ease creating and loading new maps.
        /// Obviously, these will have individual values, but they should have the same exact range.
        /// </summary>
        /// <param name="iLimit"></param>
        /// <param name="jLimit"></param>
        public void SetMapDataArrayRange(int iLimit, int jLimit)
        {
            positionXLimit = iLimit;
            positionYLimit = jLimit;
            mapMetaDataArray = new char[iLimit, jLimit];
            characterMapDataArray = new Character[iLimit + 1, jLimit + 1];
            movementSquareMapDataArray = new bool[iLimit + 1, jLimit + 1];
            ResetMovementSquareMapData();
        }

        /// <summary>
        /// Returns the x (horizontal) value associated with the array boundaries of the map.
        /// </summary>
        /// <returns></returns>
        public int GetPositionXLimit()
        {
            return positionXLimit;
        }

        /// <summary>
        /// Returns the y (vertical) value associated with the vertical boundaries of the map.
        /// </summary>
        /// <returns></returns>
        public int GetPositionYLimit()
        {
            return positionYLimit;
        }



        public void ResetMovementSquareMapData()
        {
            for (int i = 0; i <= GetPositionXLimit(); i++)
            {
                for (int j = 0; j <= GetPositionYLimit(); j++)
                {
                    movementSquareMapDataArray[i, j] = false;
                }
            }
        }



        /// <summary>
        /// Check if there is a sprite at the given position
        /// </summary>
        /// <param name="rowX"></param>
        /// <param name="colY"></param>
        /// <returns></returns>
        public bool IsThereASpriteAtThePosition(int rowX, int colY)
        {
            // if the position is within the map boundaries, then check. Otherwise, automatically return false.
            if(rowX >= 0 && rowX <= GetPositionXLimit() && colY >= 0 && colY <= GetPositionYLimit())
            {
                if (characterMapDataArray[rowX, colY] != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Retrieve the character at the specified position of the characterMapDataArray... If there is one. This should not be called
        /// unless the function IsThereASpriteATheCursorPosition was determined to be true.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Character GetCharacter(int x, int y)
        {
            return characterMapDataArray[x, y];
        }

        /// <summary>
        /// Set a character sprite's position on the array
        /// </summary>
        /// <param name="theSprite"></param>
        public void SetCharacterAtPosition(Character theSprite)
        {
            // TODO: Add conditional - if there is already a sprite there, don't set the position there.
            characterMapDataArray[theSprite.x, theSprite.y] = theSprite;
        }

        /// <summary>
        /// Assigns the passed Character to the specified position. Used for transitioning sprites with the cursor.
        /// </summary>
        /// <param name="rowX"></param>
        /// <param name="colY"></param>
        /// <param name="currentCharacter"></param>
        public void ChangeCharacterPosition(int rowX, int colY, Character currentCharacter)
        {
            characterMapDataArray[rowX, colY] = currentCharacter;
        }

        /// <summary>
        /// Removes the character at the specified position. 
        /// Usually is used for removing the character from an old position, when moving a character.
        /// </summary>
        /// <param name="rowX"></param>
        /// <param name="colY"></param>
        public void DeleteSpriteAtPosition(int rowX, int colY)
        {
            characterMapDataArray[rowX, colY] = null;
        }


        int FindNumberOfEnemies()
        {
            int i = 0;
            foreach (Character unit in characterMapDataArray)
            {
                if (unit != null)
                {
                    if (unit.IsEnemyUnit == true)
                    {
                        i++;
                    }
                }
            }

            return i;
        }

        public int GetNumberOfEnemies()
        {
            numberOfEnemies = FindNumberOfEnemies();
            return numberOfEnemies;
        }

        int FindNumberOfAllies()
        {
            int i = 0;
            foreach (Character unit in characterMapDataArray)
            {
                if (unit != null)
                {
                    if (unit.IsEnemyUnit != true)
                    {
                        i++;
                    }
                }
            }

            return i;
        }

        public int GetNumberOfAllies()
        {
            numberOfAllies = FindNumberOfAllies();
            return numberOfAllies;
        }

        public void DeleteDeadCharacters()
        {
            foreach(Character unit in characterMapDataArray)
            {
                if(unit != null && unit.IsAlive == false)
                {
                    DeleteSpriteAtPosition(unit.x, unit.y);
                }
            }
        }



        int xNegativeLimit;
        int xPositiveLimit;
        int yNegativeLimit;
        int yPositiveLimit;
        
        public List<Character> FindTargets(Character attackingEnemy)
        {
            xNegativeLimit = attackingEnemy.x - attackingEnemy.CharacterMovement - attackingEnemy.CharacterAttackRange;
            xPositiveLimit = attackingEnemy.x + attackingEnemy.CharacterMovement + attackingEnemy.CharacterAttackRange;
            yNegativeLimit = attackingEnemy.y - attackingEnemy.CharacterMovement - attackingEnemy.CharacterAttackRange;
            yPositiveLimit = attackingEnemy.y + attackingEnemy.CharacterMovement + attackingEnemy.CharacterAttackRange;

            List<Character> listOfDiscoveredTargets = new List<Character>();

            // For loop to search through the square of the enemy unit's movement.
            for (int rowY = yNegativeLimit; rowY < yPositiveLimit; rowY++)
            {
                for (int colX = xNegativeLimit; colX < xPositiveLimit; colX++)
                {
                    // If the current iteration is within the boundaries of the map...
                    if (rowY >= 0 && rowY <= GetPositionYLimit()
                        && colX >= 0 && colX <= GetPositionXLimit())
                    {
                        // If there is a sprite at that position...
                        if (IsThereASpriteAtThePosition(colX, rowY) == true)
                        {
                            // If it's a player unit (I.E. Not an enemy unit)...
                            if (characterMapDataArray[colX, rowY].IsEnemyUnit != true)
                            {
                                int xLimitFromSprite = Math.Abs(attackingEnemy.x - colX);
                                int yLimitFromSprite = Math.Abs(attackingEnemy.y - rowY);

                                // If it's within their movement range + their attack range...
                                if ((xLimitFromSprite + yLimitFromSprite) <= (attackingEnemy.CharacterMovement + attackingEnemy.CharacterAttackRange))
                                {
                                    // Then, a target is discovered, add it to the potential target list.
                                    listOfDiscoveredTargets.Add(characterMapDataArray[colX, rowY]);


                                    //// If there isn't a sprite already there...
                                    //// TODO: Remove.
                                    //if (IsThereASpriteAtThePosition(colX + 1, rowY) == false || characterMapDataArray[colX + 1, rowY] == attackingEnemy)
                                    //{




                                    //    // Desperation; If time runs out, change to bool and put this code back up.
                                    //    //int tempX = attackingEnemy.x;
                                    //    //int tempY = attackingEnemy.y;

                                    //    //characterMapDataArray[colX + 1, rowY] = attackingEnemy;

                                    //    //attackingEnemy.x = colX + 1;
                                    //    //attackingEnemy.y = rowY;
                                    //    //attackingEnemy.Position.X = (((attackingEnemy.x * 32) + 5) * attackingEnemy.Scale);
                                    //    //attackingEnemy.Position.Y = (((attackingEnemy.y * 32)) * attackingEnemy.Scale);

                                    //    //DeleteSpriteAtPosition(tempX, tempY);

                                    //    //// This accomplished its task in the end, so it can return true;
                                    //    //return true;
                                    //}

                                }
                            }
                        }
                    }

                }
            }

            return listOfDiscoveredTargets;
        } // End FindTargets Method.
   

    } // End Class MapSprite
} // End GameDesign namespace.
