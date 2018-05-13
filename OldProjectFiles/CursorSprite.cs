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
    public class CursorSprite : Sprite
    {
        // Path/Name of the Cursor Sprite.
        const string CURSOR_ASSET_NAME = "MyArt/tempCursorSprite";

        // Keyboard states, used to track what key is pressed and trigger events based on the pressed key
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        // Used for teacking repeat movements of the keyboard.
        double timeSinceRepeatedButtonPress = 0.0;
        bool firstMovementDelayHasHappened = false;
        
        // Variables associated with transporting sprites around.
        public bool isSpriteHeld = false;
        public Character heldCharacter = null;

        // Random variable used for various tasks.
        Random nextRandom = new Random();

        /// <summary>
        /// This enum value is essentially used for helping move the cursor around. Future-proofing, if we need to disable
        /// for any reason.
        /// </summary>
        enum State
        {
            PlayerPhase,
            EnemyPhase
        }
        State currentState = State.PlayerPhase;


        /// <summary>
        /// Loads the sprite for the cursor, as well as the sounds associated with the cursor; including collisions.
        /// </summary>
        /// <param name="theContentManager"></param>
        public void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager, CURSOR_ASSET_NAME, x, y);
        }

        /// <summary>
        ///  Although it is a sprite, it should be able to move anywhere. Function to move the sprite cursor. Basically equivalent to
        ///  what we have in the main class, since technically the only thing moving by our keyboard input is the cursor;
        ///  Character movement will be done via relocation/pathfinding.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="theMap"></param>
        public void CheckKeyboardState(GameTime gameTime, MapSprite theMap)
        {
            // Get current keyboard state.
            currentKeyboardState = Keyboard.GetState();

            // If either directional button is pressed down at the previous get statement, call the update movement function.
            if (currentKeyboardState.IsKeyDown(Keys.Up) == true || currentKeyboardState.IsKeyDown(Keys.Down) == true ||
                currentKeyboardState.IsKeyDown(Keys.Right) == true || currentKeyboardState.IsKeyDown(Keys.Left) == true)
            {
                // If current key press is equal to previous, the button is still held down.
                // Therefore, handle this by delaying.
                // If they are not equal, that means a different button was pressed, or it's the first press.
                if (currentKeyboardState == previousKeyboardState)
                {
                    timeSinceRepeatedButtonPress += gameTime.ElapsedGameTime.TotalMilliseconds;

                    // First delay - pause is slightly longer than future delays.
                    if (timeSinceRepeatedButtonPress >= 500 && firstMovementDelayHasHappened == false)
                    {
                        firstMovementDelayHasHappened = true;
                        timeSinceRepeatedButtonPress = 0;
                    }
                    // Second delay - pause is lower than the first delay.
                    else if (timeSinceRepeatedButtonPress >= 50 && firstMovementDelayHasHappened == true)
                    {
                        // Update sprite's position... If they're not on any of the boundaries.
                        if (IsTheCursorMoveLegal(theMap) == true)
                        {
                            // Move cursor, and play cursor move sound.
                            Update(gameTime);
                        }
                        else
                        {
                            // Play some error sound.
                        }
                        timeSinceRepeatedButtonPress = 0;
                    }
                }
                else
                {
                    if (IsTheCursorMoveLegal(theMap) == true)
                    {
                        // Play cursor move sound.
                        Update(gameTime);
                    }
                    else
                    {
                        // Play some error sound.
                    }
                    firstMovementDelayHasHappened = false;
                }
            }
            // Set previous keyboard state to the state we just input.
            previousKeyboardState = currentKeyboardState;
        } // End CheckKeyboardState function.

        /// <summary>
        /// This is called whenever properties of the Cursor is updated, such as the position, most commonly.
        /// </summary>
        /// <param name="theGameTime"></param>
        public void Update(GameTime theGameTime)
        {
            KeyboardState CurrentKeyboardState = Keyboard.GetState();
            UpdateCursorMovement(CurrentKeyboardState);
            previousKeyboardState = CurrentKeyboardState;

            Position = new Vector2(((x * 32)) * Scale, ((y * 32)) * Scale);
        } // End Update Function

        /// <summary>
        /// Move the cursor based on the key that was pressed at the time of being called.
        /// </summary>
        /// <param name="aCurrentKeyboardState"></param>
        private void UpdateCursorMovement(KeyboardState aCurrentKeyboardState)
        {
            if (currentState == State.PlayerPhase)
            {
                if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true)
                {
                    x = x - 1;
                }
                else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
                {
                    x = x + 1;
                }
                if (aCurrentKeyboardState.IsKeyDown(Keys.Up) == true)
                {
                    y = y - 1;
                }
                else if (aCurrentKeyboardState.IsKeyDown(Keys.Down) == true)
                {
                    y = y + 1;
                }
            }
        } // End UpdateCursorMovement

        /// <summary>
        /// Checks if the cursor movement goes outside of the boundaries. 
        /// If the move would go outside the boundary, return false,
        /// thus preventing the player from pressing the key.
        /// </summary>
        /// <param name="currentMap"></param>
        /// <returns></returns>
        private bool IsTheCursorMoveLegal(MapSprite currentMap)
        {
            // We do corner checks first, to prevent data conflict that would likly bug up the controls.

            // If we are on the upper-right corner of the map...
            if (x >= currentMap.GetPositionXLimit() && y <= 0)
            {
                // ...Then, only allow input if the right key AND the up key is NOT pressed.
                if (currentKeyboardState.IsKeyUp(Keys.Right) && currentKeyboardState.IsKeyUp(Keys.Up))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Or, if we are on the lower-right corner of the map...
            else if (x >= currentMap.GetPositionXLimit() && y >= currentMap.GetPositionYLimit())
            {
                // ...Then, only allow input if the right key AND the down key is NOT pressed.
                if (currentKeyboardState.IsKeyUp(Keys.Right) && currentKeyboardState.IsKeyUp(Keys.Down))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Or, if we are on the upper-left corner of the map...
            else if (x <= 0 && y <= 0)
            {
                // ...Then, only allow input if the left key AND the up key is NOT pressed.
                if (currentKeyboardState.IsKeyUp(Keys.Left) && currentKeyboardState.IsKeyUp(Keys.Up))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Or, if we are on the lower-left corner of the map...
            else if (x <= 0 && y >= currentMap.GetPositionYLimit())
            {
                // ...Then, only allow input if the left key AND the down key is NOT pressed.
                if (currentKeyboardState.IsKeyUp(Keys.Left) && currentKeyboardState.IsKeyUp(Keys.Down))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Or, if we are on the right-side boundary of the map...
            else if (x >= currentMap.GetPositionXLimit())
            {
                // ...Then, only allow input if the right key is NOT pressed.
                if (currentKeyboardState.IsKeyUp(Keys.Right))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Or, if we are on the left-side boundary of the map...
            else if (x <= 0)
            {
                // ...Then, only allow input if the left arrow key is NOT pressed.
                if (currentKeyboardState.IsKeyUp(Keys.Left))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Or, if we are on the bottom-side boundary of the map...
            else if (y >= currentMap.GetPositionYLimit())
            {
                // ...Then, only allow input if the down arrow key is NOT pressed.
                if (currentKeyboardState.IsKeyUp(Keys.Down))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Or, if we are on the top-side boundary of the map...
            else if (y <= 0)
            {
                // ...Then, only allow input if the up arrow key is NOT pressed.
                if (currentKeyboardState.IsKeyUp(Keys.Up))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Otherwise, we can allow the map movement to happen.
            else
            {
                return true;
            }
        } // End IsCursorMoveLegal function.

        /// <summary>
        /// This function is called when the cursor is checking if there is a sprite at the given position, before attempting to get it.
        /// First, it checks the map logical data layer, to see if there is a sprite at the current location.
        /// Second, if it is determined there is a sprite at the cursor location, it sets the current held sprite to the sprite at the given location
        /// TODO: Once available, call a function to draw the movement range available for the selected character.
        /// </summary>
        /// <param name="currentMap"></param>
        public void GetCharacterAtCurrentCursorPosition(MapSprite currentMap)
        {
            if (currentMap.IsThereASpriteAtThePosition(x, y) == true)
            {
                // get the sprite at the position
                heldCharacter = currentMap.GetCharacter(x, y);
                // set sprite is held to true
                isSpriteHeld = true;

                // TODO: Sound indicating the character is held.
            }
            else
            {
                // TODO: Sound that indicates error.
            }
        } // End GetSpriteAtCurrentPosition function.

        /// <summary>
        /// When this function is called, under the assumption a sprite is currently held at the time and was placed,
        /// this sets the held sprite back to null, and the flag for checking if a sprite is held is set back to false.
        /// Generally, this is called when a sprite is placed, or the player cancels their sprite-based actions.
        /// </summary>
        public void ResetHeldCharacterToNull()
        {
            // TODO: Sound to confirm reset here.
            heldCharacter = null;
            isSpriteHeld = false;
        } // End ResetSpriteHeld function.

        /// <summary>
        /// If there is a sprite currently held, when this function is called.
        /// </summary>
        /// <param name="currentMap"></param>
        public void SetHeldCharacterToCursorPosition(MapSprite currentMap)
        {
            // call a map function to delete the sprite at the old position
            currentMap.DeleteSpriteAtPosition(heldCharacter.x, heldCharacter.y);

            // call a map function to add the sprite at the new position
            currentMap.ChangeCharacterPosition(x, y, heldCharacter);
            heldCharacter.UpdatePosition(x, y);
        } // End SetHeldSpriteToCursorPosition.
    } // End class CursorSprite class
} // End GameDesignProject namespace.