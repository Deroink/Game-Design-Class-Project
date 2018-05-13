using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

using MonoGame.Extended;

using GameDesignTestArea.AudioClasses;

using System.Text;
using System.Collections.Generic;
using System;

namespace GameDesignTestArea
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // The cursor sprite.
        CursorSprite theCursor;

        // The map sprite(s).
        MapSprite testMap;

        // Keyboard State variables.
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // The player/ally sprite(s).
        List<Character> listOfPlayerSprites;
        Character soldier1;
        Character soldier2;
        Character soldier3;
        Character archer1;

        // The enemy sprite(s).
        Character enemySoldier1;
        Character enemySoldier2;
        Character enemySoldier3;
        Character enemySoldier4;
        Character enemyArcher1;
        Character enemyArcher2;

        // Other stuff.
        Sprite titleLogoSprite;
        Sprite actionMenu;
        Sprite pauseMenu;
        Sprite menuPointer;
        Sprite PlayerPhaseNotification;
        Sprite EnemyPhaseNotification;
        Sprite HpBox;
        Sprite VictoryMessage;

        private SpriteFont font;

        public double timeSinceLastKeyPress = 0.0;
        MovementSquare movementSquare;
        Sprite WaitingCharacterSquare;

        int oldCharacterX = 0;
        int oldCharacterY = 0;

        bool cursorHasASprite;
        bool playerPhaseNotificationWasShown = false;
        bool enemyPhaseNotificationWasShown = false;

        bool isAttacking = false;
        int timesAttacked = 0;
        int attackTimer = 0;

        StringBuilder currentText = new StringBuilder();
        string fullText;
        int count = 0;
        float timer = 0.0f;


        int positionInDrawing = 0;
        int WaitTime = 0;
        bool isCleared = false;

        Random random = new Random();

        // Game State, used for determining the kind of input to check for, and handle input.
        enum GameState
        {
            // State for when the game is starting up; no matter what, the game always goes to the main menu first.
            MainMenuOne,
            MainMenuTwo,
            // State for when the game is in the main mode of selecting and moving units.
            NormalPlay,
            // State for when the game is in a menu - paused, after moving a unit, etc.
            ActionMenu,
            // For when the z button is pressed on a tile with no sprite, or enter is pressed.
            PauseMenu,
            // When you select for a character to attack someone.
            AttackSelection,
            // State for when the game is in enemy phase mode.
            EnemyPhase,
            // Victory state
            Victory,
            Defeat
        };
        GameState gameState;

        // Actions for after moving a character. Attack lets the character... attack, then calls a wait function.
        // Wait ends the character's action, setting a bool to true so they can't be selected.
        // Cancel does the same thing as pressing X.
        enum ActionMenuState
        {
            Attack,
            Wait,
            BackToMove,
        };
        ActionMenuState actionMenuState;

        /// <summary>
        /// Actions for when the z button is pressed on a tile where there is no sprite.
        /// Units (NOT IMPLEMENTED) - Shows the list of allied units.
        /// Items (NOT IMPLEMENTED) - Shows the list of items the team has.
        /// Status (NOT IMPLEMENTED) - Shows status of the battle - # of units on each 
        /// Options (NOT IMPLEMENTED) - Shows some options that the player can change.
        /// EndTurn (NOT IMPLEMENTED) - Changes the game over to enemy phase.
        /// </summary>
        enum PauseMenuState
        {
            Units,
            Items,
            Status,
            Options,
            EndTurn
        };
        PauseMenuState pauseMenuState;


        // Panos's button release variables.
        //bool blnPPressed = false;
        //bool blnPPrevState = false;
        //bool blnPUp = false;

        /// <summary>
        /// 
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600

            };
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(500, 200);
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameState = new GameState();
            //gameState = GameState.MainMenuOne;
            gameState = GameState.MainMenuOne;

            // state of the action menu. Remember, after the action menu closes in any fashion, the values should be reset.
            actionMenuState = new ActionMenuState();
            actionMenuState = ActionMenuState.Attack;

            pauseMenuState = new PauseMenuState();
            pauseMenuState = PauseMenuState.Units;


            theCursor = new CursorSprite();
            testMap = new MapSprite();

            // Player Character Sprites.
            listOfPlayerSprites = new List<Character>();
            soldier1 = new Character();
            soldier2 = new Character();
            soldier3 = new Character();
            archer1 = new Character();

            // Enemy Character Sprites.
            enemySoldier1 = new Character();
            enemySoldier2 = new Character();
            enemySoldier3 = new Character();
            enemySoldier4 = new Character();
            enemyArcher1 = new Character();
            enemyArcher2 = new Character();

            movementSquare = new MovementSquare();


            WaitingCharacterSquare = new Sprite();


            /*
             * 
             *      UI Initialization - Player/Enemy Phase notifications, menus, etc.
             *
             * 
             */

            // Arrow for pointing at menus.
            menuPointer = new Sprite
            {
                Position = new Vector2(5, 5),
            };

            titleLogoSprite = new Sprite
            {
                IndividualScale = 0.15f,
            };


            // Action Menu - Used for having characters write new positions, attack enemy units, etc.
            actionMenu = new Sprite();
            // Pause menu - used for moving the game forward.
            pauseMenu = new Sprite();



            // Player and Enemy phase notifications.
            PlayerPhaseNotification = new Sprite
            {
                IndividualScale = 0.5f,
                Position = new Vector2(100, 15)
            };
            EnemyPhaseNotification = new Sprite
            {
                IndividualScale = 0.5f,
                Position = new Vector2(100, 15)
            };

            // Victory and Defeat notifications.
            VictoryMessage = new Sprite
            {
                Position = new Vector2(100, 100)
            };


            // Unit HP boxes.
            HpBox = new Sprite
            {
                Scale = 0.6f
            };




            // Soldier 1 defaults.
            soldier1.SetUnitAssetName("MyArt/allySoldier");
            soldier1.InitializeCharacter();

            // Soldier 2 defaults.
            soldier2.SetUnitAssetName("MyArt/allySoldier");
            soldier2.InitializeCharacter();

            // Soldier 3 defaults.
            soldier3.SetUnitAssetName("MyArt/allySoldier");
            soldier3.InitializeCharacter();

            // Archer 1 defaults.
            archer1.SetUnitAssetName("MyArt/allyArcher");
            archer1.InitializeCharacter();

            // Add the player sprites to the list.
            listOfPlayerSprites.Add(soldier1);
            listOfPlayerSprites.Add(soldier2);
            listOfPlayerSprites.Add(soldier3);


            // Enemy Soldier 1 defaults.
            enemySoldier1.SetUnitAssetName("MyArt/enemySoldier");
            enemySoldier1.InitializeCharacter();

            // Enemy Soldier 2 defaults.
            enemySoldier2.SetUnitAssetName("MyArt/enemySoldier");
            enemySoldier2.InitializeCharacter();

            // Enemy Soldier 3 defaults.
            enemySoldier3.SetUnitAssetName("MyArt/enemySoldier");
            enemySoldier3.InitializeCharacter();

            // Enemy Soldier 4 defaults.
            enemySoldier4.SetUnitAssetName("MyArt/enemySoldier");
            enemySoldier4.InitializeCharacter();

            // Enemy Archer 1 
            enemyArcher1.SetUnitAssetName("MyArt/enemyArcher");
            enemyArcher1.InitializeCharacter();

            enemyArcher2.SetUnitAssetName("MyArt/enemyArcher");
            enemyArcher2.InitializeCharacter();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            theCursor.LoadContent(this.Content);
            testMap.LoadContent(this.Content, "MyArt/testMap");

            // Player Character Sprites Load
            soldier1.LoadContent(this.Content);
            soldier2.LoadContent(this.Content);
            soldier3.LoadContent(this.Content);
            archer1.LoadContent(this.Content);

            // Enemy Character Sprites Load
            enemySoldier1.LoadContent(this.Content);
            enemySoldier2.LoadContent(this.Content);
            enemySoldier3.LoadContent(this.Content);
            enemySoldier4.LoadContent(this.Content);
            enemyArcher1.LoadContent(this.Content);
            enemyArcher2.LoadContent(this.Content);

            // UI load
            actionMenu.LoadContent(this.Content, "MyArt/ActionMenu", 0, 0);
            pauseMenu.LoadContent(this.Content, "MyArt/PauseMenu", 0, 0);
            menuPointer.LoadContent(this.Content, "MyArt/TempMenuPointer", 0, 33);
            titleLogoSprite.LoadContent(this.Content, "MyArt/Logo", 100, 33);

            // Load Square textures, like movement and waiting.
            WaitingCharacterSquare.LoadContent(this.Content, "MyArt/WaitingCharacterSquare", 0, 0);
            movementSquare.LoadContent(this.Content);

            // UI for Phase change.
            PlayerPhaseNotification.LoadContent(this.Content, "MyArt/PlayerPhaseNotification", 100, 100);
            EnemyPhaseNotification.LoadContent(this.Content, "MyArt/EnemyPhaseNotification", 100, 100);


            // Victory/DefeatSprites
            VictoryMessage.LoadContent(this.Content, "MyArt/VictoryMessage", 0, 0);

            // Font.
            font = Content.Load<SpriteFont>("Arial");

            // HP Box.
            HpBox.LoadContent(this.Content, "MyArt/HpBox", 0, 0);

            // Use this block to assign map array ranges to the map.
            testMap.SetMapDataArrayRange(9, 7);

            SetCharacterRandom(soldier1);
            SetCharacterRandom(soldier2);
            SetCharacterRandom(soldier3);
            SetCharacterRandom(archer1);

            SetCharacterRandom(enemySoldier1);
            SetCharacterRandom(enemySoldier2);
            SetCharacterRandom(enemySoldier3);
            SetCharacterRandom(enemySoldier4);
            SetCharacterRandom(enemyArcher1);
            SetCharacterRandom(enemyArcher2);
        }

        void SetCharacterRandom(Character character)
        {
            while (true)
            {
                int x = random.Next(0, testMap.GetPositionXLimit());
                int y = random.Next(0, testMap.GetPositionYLimit());

                if (testMap.characterMapDataArray[x, y] == null)
                {
                    character.UpdatePosition(x, y);
                    testMap.characterMapDataArray[x, y] = character;
                    break;
                }
            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Check for certain presses on the keyboard.
            CheckForKeyPresses(gameTime, testMap);

            // Set the sprite position at the logical level to the sprite.
            // TODO: Efficiency. Get this to work under certain conditions, it doesn't need to call this so often.
            if (soldier1.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(soldier1);
            }
            if (soldier2.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(soldier2);
            }
            if (soldier3.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(soldier3);
            }
            if (archer1.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(archer1);
            }


            if (enemySoldier1.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(enemySoldier1);
            }
            if (enemySoldier2.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(enemySoldier2);
            }
            if (enemySoldier3.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(enemySoldier3);
            }
            if (enemySoldier4.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(enemySoldier4);
            }
            if (enemyArcher1.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(enemyArcher1);
            }
            if (enemyArcher2.IsAlive == true)
            {
                testMap.SetCharacterAtPosition(enemyArcher2);
            }


            if (testMap.GetNumberOfEnemies() <= 0)
            {
                if (gameState != GameState.Victory)
                {
                    gameState = GameState.Victory;
                    currentText.Clear();
                    count = 0;
                }
            }

            if (testMap.GetNumberOfAllies() <= 0)
            {
                if (gameState != GameState.Defeat)
                {
                    gameState = GameState.Defeat;
                    currentText.Clear();
                    count = 0;
                }
            }

            // Panos code.
            {
                ///*
                //    bool blnPPressed = false;
                //    bool blnPPrevState = false;
                //    bool blnPUp = false;
                // */
                //if (blnPPrevState != blnPPressed && blnPPressed == false)
                //{
                //    blnPUp = true;
                //}

                //blnPPrevState = blnPPressed;

                //if (blnPUp == true) {
                //    if (theCursor.isSpriteHeld == true)
                //    {
                //        // Then, wherever the cursor is at the time of pressing z again, set the held sprite's position to that new spot.                    
                //        theCursor.SetHeldSpriteToCursorPosition(testMap);
                //        theCursor.ResetHeldSpriteToNull();

                //    }
                //    blnPUp = false;
                //}
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();


            switch (gameState)
            {
                case GameState.MainMenuOne:
                    {
                        if (startTimer == false)
                        {
                            switch (positionInDrawing)
                            {
                                case 0:
                                    {
                                        MenuStrings(gameTime, "A Game Developed by a Team of Me", 200, 200);
                                        break;
                                    }
                                case 1:
                                    {
                                        if (WaitTime <= 500)
                                        {
                                            MenuStrings(gameTime, "A Game Developed by a Team of Me", 200, 200);
                                            positionInDrawing = 1;
                                            WaitTime += gameTime.ElapsedGameTime.Milliseconds;
                                        }
                                        else if (isCleared == false)
                                        {
                                            count = 0;
                                            currentText.Clear();
                                            isCleared = true;
                                        }
                                        else
                                        {
                                            MenuStrings(gameTime, "Programmed by Derek Dailey", 250, 400);
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        if (WaitTime <= 1100)
                                        {
                                            MenuStrings(gameTime, "Programmed by Derek Dailey", 250, 400);
                                            positionInDrawing = 2;
                                            WaitTime += gameTime.ElapsedGameTime.Milliseconds;
                                            isCleared = false;
                                        }
                                        else if (isCleared == false)
                                        {
                                            count = 0;
                                            currentText.Clear();
                                            isCleared = true;
                                        }
                                        else
                                        {
                                            MenuStrings(gameTime, "Directed by also me", 280, 300);
                                        }

                                        break;
                                    }
                                case 3:
                                    {
                                        if (WaitTime <= 1700)
                                        {
                                            MenuStrings(gameTime, "Directed by also me", 280, 300);
                                            positionInDrawing = 3;
                                            WaitTime += gameTime.ElapsedGameTime.Milliseconds;
                                            isCleared = false;
                                        }
                                        else if (isCleared == false)
                                        {
                                            count = 0;
                                            currentText.Clear();
                                            isCleared = true;
                                        }
                                        else
                                        {
                                            MenuStrings(gameTime, "\"Art\" dummy textures by me as well because I haven't practiced art enough\nto justify actually replacing the textures yet", 80, 300);
                                        }

                                        break;
                                    }
                                case 4:
                                    {
                                        if (WaitTime <= 2300)
                                        {
                                            MenuStrings(gameTime, "\"Art\" dummy textures by me as well because I haven't practiced art enough\nto justify actually replacing the textures yet", 80, 300);
                                            positionInDrawing = 4;
                                            WaitTime += gameTime.ElapsedGameTime.Milliseconds;
                                            isCleared = false;
                                        }
                                        else if (isCleared == false)
                                        {
                                            count = 0;
                                            currentText.Clear();
                                            isCleared = true;
                                        }
                                        else
                                        {
                                            MenuStrings(gameTime, "sounds done by no one because there aren't any sounds in the game now :(", 80, 200);
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        if (WaitTime <= 3000)
                                        {
                                            MenuStrings(gameTime, "sounds done by no one because there aren't any sounds in the game now :(", 80, 200);
                                            WaitTime += gameTime.ElapsedGameTime.Milliseconds;
                                        }
                                        else
                                        {
                                            WaitTime = 0;
                                            timer = 0;
                                            startTimer = true;
                                        }
                                        break;
                                    }
                            }


                            spriteBatch.DrawString(font, "Press 'Z' To Skip", new Vector2(300, 500), Color.White);

                        }
                        break;
                    }
                case GameState.MainMenuTwo:
                    {
                        // Draw the title.
                        if (startTimer == false)
                        {
                            // Draw title screen
                            titleLogoSprite.Draw(this.spriteBatch);

                            // Draw arrow, points to next text line
                            menuPointer.Draw(this.spriteBatch);

                            spriteBatch.DrawString(font, "You are the tactician of the army of green dudes at a notable disadvantage.\n     Find the way to turn this fight around, using your tactical prowess!", new Vector2(70, 10), Color.White);

                            spriteBatch.DrawString(font, "Start Game", new Vector2(310, 500), Color.White);

                            spriteBatch.DrawString(font, "Press 'Z' to Confirm", new Vector2(290, 550), Color.White);
                        }

                        break;
                    }
                case GameState.NormalPlay:
                    {
                        // Draw the map, and grid lines over it.
                        DrawMapAndGrid(testMap);
                        // Reset menu pointer back to neutral position.
                        pauseMenuState = PauseMenuState.Units;
                        actionMenuState = ActionMenuState.Attack;
                        menuPointer.y = 0;
                        menuPointer.Position = new Vector2(0, 0);

                        if (playerPhaseNotificationWasShown == true)
                        {
                            // If there is a sprite held...
                            if (theCursor.isSpriteHeld == true)
                            {
                                // Then, draw movement squares relative to the position of the sprite and the movement.
                                cursorHasASprite = true;
                                movementSquare.DrawWithinCharacterRange(theCursor.heldCharacter, testMap, this.spriteBatch, cursorHasASprite);
                                timeSinceLastKeyPress = 0;
                            }
                            // Otherwise, if the cursor is hovering over a unit...
                            else if (testMap.IsThereASpriteAtThePosition(theCursor.x, theCursor.y))
                            {
                                // If the cursor is not waiting, and they're alive...
                                if (testMap.characterMapDataArray[theCursor.x, theCursor.y].IsWaiting != true)
                                {
                                    timeSinceLastKeyPress += gameTime.ElapsedGameTime.Milliseconds;
                                    if (timeSinceLastKeyPress >= 500)
                                    {
                                        cursorHasASprite = false;
                                        movementSquare.DrawWithinCharacterRange(testMap.characterMapDataArray[theCursor.x, theCursor.y], testMap, this.spriteBatch, cursorHasASprite);
                                        // Draw the HP box above it
                                        // Creating the timer, because we don't need to initialize the position every time.
                                        if (timeSinceLastKeyPress <= 550)
                                        {
                                            Vector2 temp = testMap.characterMapDataArray[theCursor.x, theCursor.y].Position;
                                            HpBox.Position = new Vector2(temp.X - 35, temp.Y - 35);
                                        }
                                        HpBox.Draw(this.spriteBatch);
                                        spriteBatch.DrawString(font, (testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP + "/" + testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterMaxHP), new Vector2(HpBox.Position.X + 35, HpBox.Position.Y + 6), Color.Black);

                                    }
                                }
                            }
                            else
                            {
                                timeSinceLastKeyPress = 0;
                            }

                            // Call function to draw player sprites.
                            DrawPlayerSprites();
                            // Call function to draw enemy sprites.
                            DrawEnemySprites();
                            // Call function to draw the instructions.
                            DrawInstructions(spriteBatch);

                            theCursor.Draw(this.spriteBatch);
                        }
                        else
                        {
                            timeSinceLastKeyPress += gameTime.ElapsedGameTime.Milliseconds;
                            if (timeSinceLastKeyPress < 1000)
                            {
                                PlayerPhaseNotification.Draw(this.spriteBatch);
                            }
                            else
                            {
                                // No longer showing player phase notification - Reset enemy phase notification.
                                playerPhaseNotificationWasShown = true;
                                enemyPhaseNotificationWasShown = false;

                                // Reset the player units back to being able to move again.
                                soldier1.IsWaiting = false;
                                soldier2.IsWaiting = false;
                                soldier3.IsWaiting = false;
                                archer1.IsWaiting = false;

                                // Reset timer.
                                timeSinceLastKeyPress = 0;
                            }
                        }


                        break;
                    }
                case GameState.AttackSelection:
                    {
                        // Draw the map, and grid lines over it.
                        DrawMapAndGrid(testMap);

                        // Do the same thing as normal play, but restrict cursor movement.

                        // Otherwise, if the cursor is hovering over a unit...
                        if (testMap.IsThereASpriteAtThePosition(theCursor.x, theCursor.y))
                        {
                            // Draw their HP box.
                            timeSinceLastKeyPress += gameTime.ElapsedGameTime.Milliseconds;
                            if (timeSinceLastKeyPress >= 500)
                            {
                                // Draw the HP box above it
                                // Creating the timer, because we don't need to initialize the position every time.
                                if (timeSinceLastKeyPress <= 550)
                                {
                                    Vector2 temp = testMap.characterMapDataArray[theCursor.x, theCursor.y].Position;
                                    HpBox.Position = new Vector2(temp.X - 35, temp.Y - 35);
                                }
                                HpBox.Draw(this.spriteBatch);
                                spriteBatch.DrawString(font, (testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP + "/" + testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterMaxHP), new Vector2(HpBox.Position.X + 35, HpBox.Position.Y + 6), Color.Black);

                            }

                        }
                        // Call function to draw player sprites.
                        DrawPlayerSprites();
                        // Call function to draw enemy sprites.
                        DrawEnemySprites();
                        // Draw the cursor.
                        // Call function to draw the instructions.
                        DrawInstructions(spriteBatch);
                        theCursor.Draw(this.spriteBatch);

                        break;
                    }
                case GameState.ActionMenu:
                    {
                        // Draw the map, and grid lines over it.
                        DrawMapAndGrid(testMap);

                        // Draw the Sprites, and the cursor, so the menu overlays them.

                        // Call function to draw player sprites.
                        DrawPlayerSprites();
                        // Call function to draw enemy sprites.
                        DrawEnemySprites();

                        theCursor.Draw(this.spriteBatch);


                        // Draw menu
                        actionMenu.Draw(this.spriteBatch);
                        // TODO: Draw Text to overlay the menu
                        // Draw the arrow to point at the menu.
                        menuPointer.Draw(this.spriteBatch);
                        // Call function to draw the instructions.
                        DrawInstructions(spriteBatch);

                        break;
                    }
                case GameState.PauseMenu:
                    {
                        // Draw the map, and grid lines over it.
                        DrawMapAndGrid(testMap);

                        // Draw the Sprites, and the cursor, so the menu overlays them.

                        // Call function to draw player sprites.
                        DrawPlayerSprites();
                        // Call function to draw enemy sprites.
                        DrawEnemySprites();

                        theCursor.Draw(this.spriteBatch);

                        // Draw menu
                        pauseMenu.Draw(this.spriteBatch);
                        // TODO: Draw Text to overlay the menu
                        // Draw the arrow to point at the menu.
                        menuPointer.Draw(this.spriteBatch);
                        // Call function to draw the instructions.
                        DrawInstructions(spriteBatch);

                        break;
                    }
                case GameState.EnemyPhase:
                    {
                        // Draw the map, and grid lines over it.
                        DrawMapAndGrid(testMap);

                        if (enemyPhaseNotificationWasShown == true)
                        {
                            // Call the AI handler for enemy phase.
                            EnemyPhaseHandler(gameTime, testMap);

                            // Call function to draw player sprites.
                            DrawPlayerSprites();
                            // Call function to draw enemy sprites.
                            DrawEnemySprites();
                            // Call function to draw the instructions.
                            DrawInstructions(spriteBatch);

                        }
                        else
                        {
                            timeSinceLastKeyPress += gameTime.ElapsedGameTime.Milliseconds;
                            if (timeSinceLastKeyPress < 1000)
                            {
                                EnemyPhaseNotification.Draw(this.spriteBatch);
                            }
                            else
                            {
                                enemyPhaseNotificationWasShown = true;
                                playerPhaseNotificationWasShown = false;
                                timeSinceLastKeyPress = 0;
                            }
                        }
                        break;
                    } // End EnemyPhase Draw
                case GameState.Victory:
                    {
                        // Draw the map, and grid lines over it.
                        DrawMapAndGrid(testMap);

                        VictoryMessage.Draw(this.spriteBatch);
                        fullText = "You won... But at what cost?";
                        timer += gameTime.ElapsedGameTime.Milliseconds;
                        if (timer >= 50 && currentText.Length < fullText.Length)
                        {
                            currentText.Append((char)fullText[count]);
                            //spriteBatch.DrawString(font, currentText, new Vector2(VictoryMessage.Position.X + 200, VictoryMessage.Position.Y + 200), Color.Black);
                            timer = 0;
                            count++;
                        }
                        spriteBatch.DrawString(font, currentText, new Vector2(VictoryMessage.Position.X + 150, VictoryMessage.Position.Y + 220), Color.Black);
                        break;
                    }
                case GameState.Defeat:
                    {
                        // Draw the map, and grid lines over it.
                        DrawMapAndGrid(testMap);

                        VictoryMessage.Draw(this.spriteBatch);
                        fullText = "you actually lost lmao";
                        timer += gameTime.ElapsedGameTime.Milliseconds;
                        if (timer >= 50 && currentText.Length < fullText.Length)
                        {
                            currentText.Append((char)fullText[count]);
                            //spriteBatch.DrawString(font, currentText, new Vector2(VictoryMessage.Position.X + 200, VictoryMessage.Position.Y + 200), Color.Black);
                            timer = 0;
                            count++;
                        }
                        spriteBatch.DrawString(font, currentText, new Vector2(VictoryMessage.Position.X + 200, VictoryMessage.Position.Y + 200), Color.Black);
                        break;
                    }
            }

            // If a unit is attacking another that isn't on the same team...
            if (isAttacking == true)
            {
                Vector2 temp = testMap.characterMapDataArray[theCursor.x, theCursor.y].Position;
                HpBox.Position = new Vector2(temp.X - 35, temp.Y - 35);


                if (attackTimer < 1000)
                {
                    // Draw the target's HP box before it's affected 
                    HpBox.Draw(this.spriteBatch);
                    spriteBatch.DrawString(font, (testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP + "/" + testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterMaxHP), new Vector2(HpBox.Position.X + 35, HpBox.Position.Y + 6), Color.Black);
                }
                else
                {

                    // Reduce the target's HP. Do this only once.
                    if (timesAttacked < 1)
                    {
                        HpBox.Draw(this.spriteBatch);
                        spriteBatch.DrawString(font, (testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP + "/" + testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterMaxHP), new Vector2(HpBox.Position.X + 35, HpBox.Position.Y + 6), Color.Black);
                        testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP -= theCursor.heldCharacter.CharacterAttack;
                        if (testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP > 0)
                        {
                            // Hit sound effect.
                        }
                        else if (testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP <= 0)
                        {
                            testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP = 0;
                            // Killing blow sound effect.
                            // Death sound
                            // Set their alive truth value to false.
                            testMap.characterMapDataArray[theCursor.x, theCursor.y].IsAlive = false;
                        }
                        timesAttacked++;
                    }

                    // Draw the target's HP box after it's affected
                    else
                    {
                        HpBox.Draw(spriteBatch);
                        spriteBatch.DrawString(font, (testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterRemainingHP + "/" + testMap.characterMapDataArray[theCursor.x, theCursor.y].CharacterMaxHP), new Vector2(HpBox.Position.X + 35, HpBox.Position.Y + 6), Color.Black);
                    }
                    if (attackTimer > 2000)
                    {
                        isAttacking = false;
                        attackTimer = 0;
                        timesAttacked = 0;
                        theCursor.heldCharacter.IsWaiting = true;
                        theCursor.ResetHeldCharacterToNull();

                        if (gameState == GameState.AttackSelection)
                        {
                            gameState = GameState.NormalPlay;
                            testMap.DeleteDeadCharacters();
                        }
                        else if (gameState == GameState.EnemyPhase)
                        {
                            testMap.DeleteDeadCharacters();
                        }
                    }
                }

                attackTimer += gameTime.ElapsedGameTime.Milliseconds;
            }

            spriteBatch.End();



            base.Draw(gameTime);
        }

        /// <summary>
        /// Takes the passed map, draws it using the MapSprite draw function, then creates a grid based on the map boundaries and global scale.
        /// </summary>
        /// <param name="currentMap"></param>
        void DrawMapAndGrid(MapSprite currentMap)
        {
            // Draw the map itself.
            currentMap.Draw(this.spriteBatch);
            // Draw grid over everything... Except menus, but we'll get to that later.
            // TODO: Make adjustments until it lines up to grid.
            // TEST: Draw a line
            for (int i = 0; i <= currentMap.GetPositionXLimit() + 1; i++)
            {
                int bottom = currentMap.GetPositionYLimit();
                // Vertical lines; doesn't quite reach the bottom yet.
                spriteBatch.DrawLine(new Vector2(i * (32 * currentMap.Scale) + 1, 0), new Vector2(i * (32 * currentMap.Scale), ((bottom + 1) * 32 * currentMap.Scale)), Color.White, 3.0f);
            }
            
            for (int i = 0; i <= currentMap.GetPositionYLimit() + 1; i++)
            {
                int edge = currentMap.GetPositionXLimit();

                // Horizontal lines.
                spriteBatch.DrawLine(new Vector2(0, i * (32 * currentMap.Scale) + 1), new Vector2((edge + 1) * 32 * currentMap.Scale, i * (32 * currentMap.Scale)), Color.White, 3.0f);
            }
        }

        /// <summary>
        /// Take a string representing the desired full text, and draw it letter-by-letter on the screen at the specified position.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="fullText"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void MenuStrings(GameTime gameTime, string fullText, float x, float y)
        {
            if (fullText != Convert.ToString(currentText))
            {
                timer += gameTime.ElapsedGameTime.Milliseconds;
                if (timer >= 50 && currentText.Length < fullText.Length)
                {
                    currentText.Append((char)fullText[count]);
                    //spriteBatch.DrawString(font, currentText, new Vector2(VictoryMessage.Position.X + 200, VictoryMessage.Position.Y + 200), Color.Black);
                    timer = 0;
                    count++;
                    //theCursor.yerDoneSound.Play();
                }
            }
            else
            {
                positionInDrawing++;
            }
            spriteBatch.DrawString(font, currentText, new Vector2(x, y), Color.White);
        }

        /// <summary>
        /// Checks for any input from the player's keyboard.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        void CheckForKeyPresses(GameTime gameTime, MapSprite currentMap)
        {
            // Panos code for button release - logic may be useful for something later.
            {
                //if (currentKeyboardState.IsKeyDown(Keys.P))
                //{
                //    blnPPressed = true;
                //}
                //else
                //{
                //    blnPPressed = false;
                //}
            }

            switch (gameState)
            {
                case GameState.NormalPlay:
                    {
                        NormalPlayKeyHandling(gameTime, currentMap);
                        break;
                    }
                case GameState.AttackSelection:
                    {
                        AttackSelectionKeyHandler(gameTime, currentMap);
                        break;
                    }
                case GameState.ActionMenu:
                    {
                        ActionMenuKeyHandling(gameTime, currentMap);
                        break;
                    }
                case GameState.PauseMenu:
                    {
                        PauseMenuKeyHandling(gameTime, currentMap);
                        break;
                    }
                case GameState.MainMenuOne:
                    {
                        currentKeyboardState = Keyboard.GetState();
                        if (currentKeyboardState.IsKeyDown(Keys.Z) && currentKeyboardState != previousKeyboardState)
                        {
                            startTimer = true;
                        }
                        if (startTimer == true)
                        {
                            timer += gameTime.ElapsedGameTime.Milliseconds;
                        }
                        if (timer >= 1000)
                        {
                            gameState = GameState.MainMenuTwo;
                            startTimer = false;
                            timer = 0;
                            titleLogoSprite.Position.X = 190;
                            titleLogoSprite.Position.Y = 100;

                            menuPointer.Position.X = 210;
                            menuPointer.Position.Y = 493;
                        }
                        previousKeyboardState = currentKeyboardState;
                        break;
                    }
                case GameState.MainMenuTwo:
                    {
                        currentKeyboardState = Keyboard.GetState();
                        if (currentKeyboardState.IsKeyDown(Keys.Z) && currentKeyboardState != previousKeyboardState)
                        {
                            startTimer = true;
                        }
                        if (startTimer == true)
                        {
                            timer += gameTime.ElapsedGameTime.Milliseconds;
                        }
                        if (timer >= 1000)
                        {
                            gameState = GameState.NormalPlay;
                            startTimer = false;
                            timer = 0;
                        }
                        previousKeyboardState = currentKeyboardState;
                        break;
                    }
            }
        }

        bool startTimer = false;

        /// <summary>
        /// The keys to be used when moving the cursor and characters around the map.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        void NormalPlayKeyHandling(GameTime gameTime, MapSprite currentMap)
        {
            currentKeyboardState = Keyboard.GetState();

            // If Z is pressed down at the time of the check, execute the process for selecting/de-selecting a sprite.
            if (currentKeyboardState.IsKeyDown(Keys.Z))
            {

                timeSinceLastKeyPress += gameTime.ElapsedGameTime.TotalMilliseconds;

                // If there is currently a sprite held...
                if ((theCursor.isSpriteHeld == true && currentKeyboardState != previousKeyboardState))
                {
                    // Then, set the sprite to that new position, if it is where a movement square was drawn.
                    if (currentMap.movementSquareMapDataArray[theCursor.x, theCursor.y] == true)
                    {
                        oldCharacterX = theCursor.heldCharacter.x;
                        oldCharacterY = theCursor.heldCharacter.y;
                        theCursor.SetHeldCharacterToCursorPosition(currentMap);
                        //theCursor.ResetHeldCharacterToNull();
                        currentMap.ResetMovementSquareMapData();
                        gameState = GameState.ActionMenu;
                        menuPointer.Position.X = 5;
                        menuPointer.Position.Y = 10;
                    }
                    else
                    {
                        // TODO: Error sound of some sort.
                    }

                }
                // Otherwise, attempt to grab a sprite from the selected position.
                else
                {
                    // If the keyboard state is not equal to previous, the button is not still held down. Therefore, proceed.
                    // If the sprite is not an enemy, select it.
                    // Additionally, if the sprite has already moved, it can't be selected again, until the next player phase starts again.
                    if (currentMap.IsThereASpriteAtThePosition(theCursor.x, theCursor.y) == true)
                    {
                        Character temp = currentMap.characterMapDataArray[theCursor.x, theCursor.y];

                        if (currentKeyboardState != previousKeyboardState)
                        {
                            if (temp.IsEnemyUnit != true && temp.IsWaiting != true)
                            {
                                theCursor.GetCharacterAtCurrentCursorPosition(currentMap);
                            }
                            else
                            {
                                gameState = GameState.PauseMenu;
                                menuPointer.Position.X = 5;
                                menuPointer.Position.Y = 10;
                            }
                        }

                    }
                    else if (currentKeyboardState != previousKeyboardState)
                    {
                        // Change the game state to a pause menu.
                        gameState = GameState.PauseMenu;
                        menuPointer.Position.X = 5;
                        menuPointer.Position.Y = 10;
                    }
                }
            }
            // If X is pressed while there is a sprite held, release the sprite
            else if (currentKeyboardState.IsKeyDown(Keys.X) && theCursor.isSpriteHeld == true)
            {
                theCursor.ResetHeldCharacterToNull();
            }
            // Else, if any of the directional keys are pressed down, execute the movement function.
            else
            {
                Vector2 temp = theCursor.Position;
                theCursor.CheckKeyboardState(gameTime, testMap);
                if (temp != theCursor.Position)
                {
                    timeSinceLastKeyPress = 0;
                }
            }


            // regardless, set previous state to the current one.
            previousKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// The menu that shows after a character moves/is moved.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        void ActionMenuKeyHandling(GameTime gameTime, MapSprite currentMap)
        {
            currentKeyboardState = Keyboard.GetState();

            // If z was pressed, handle actions.
            if (currentKeyboardState.IsKeyDown(Keys.Z) && currentKeyboardState != previousKeyboardState)
            {
                // Handler to have the character attack
                if (actionMenuState == ActionMenuState.Attack)
                {
                    // Change gameState enum to AttackSelection, which will restrict cursor movement, to moving spaces equal to the character's attack range.
                    gameState = GameState.AttackSelection;
                }
                // Handler to have the character wait
                if (actionMenuState == ActionMenuState.Wait)
                {
                    theCursor.heldCharacter.IsWaiting = true;
                    gameState = GameState.NormalPlay;
                    theCursor.ResetHeldCharacterToNull();
                }
                // Handler to have the character wait
                // TODO: Fix bug where it is still having the character moveable.
                if (actionMenuState == ActionMenuState.BackToMove)
                {
                    theCursor.x = oldCharacterX;
                    theCursor.y = oldCharacterY;
                    theCursor.Update(gameTime);
                    theCursor.SetHeldCharacterToCursorPosition(currentMap);

                    gameState = GameState.NormalPlay;

                    // Resets the pointer to the starting position of the action menu.
                    menuPointer.y = 0;
                    menuPointer.Position = new Vector2(menuPointer.x * 33 + 5, menuPointer.y * 33 + 5);

                    // Resets the selected state to the starting option, attack.
                    actionMenuState = ActionMenuState.Attack;
                }

                //theCursor.ResetHeldCharacterToNull();
                //gameState = CurrentStateOfGame.NormalPlay;
            }

            // If x is pressed in the menu, go back to normal play - revert game state to normal play.
            if (currentKeyboardState.IsKeyDown(Keys.X) && currentKeyboardState != previousKeyboardState)
            {
                theCursor.x = oldCharacterX;
                theCursor.y = oldCharacterY;
                theCursor.Update(gameTime);
                theCursor.SetHeldCharacterToCursorPosition(currentMap);

                gameState = GameState.NormalPlay;
                // Resets the pointer to the starting position of the action menu.
                menuPointer.y = 0;
                menuPointer.Position = new Vector2(menuPointer.x * 33 + 5, menuPointer.y * 33 + 5);

                // Resets the selected state to the starting option, attack.
                actionMenuState = ActionMenuState.Attack;
            }

            // If a directional key is pressed, move the pointer down.
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.Up))
            {
                // If the up arrow was the pressed key, move the pointer up through the menu.
                if (currentKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState != previousKeyboardState)
                {
                    // If the up arrow is pressed and the pointer is at the top, move it to the bottom option. Otherwise, increment.
                    if (actionMenuState == ActionMenuState.Attack || menuPointer.y <= 0)
                    {
                        menuPointer.y = 2;
                        actionMenuState = ActionMenuState.BackToMove;
                    }
                    else
                    {
                        menuPointer.y--;
                        actionMenuState--;
                    }
                    menuPointer.Position = new Vector2(5, (5 + (menuPointer.y * 20)) * menuPointer.Scale);
                    // Menu move sound.

                }
                else if (currentKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState != previousKeyboardState)
                {
                    if (actionMenuState == ActionMenuState.BackToMove || menuPointer.y >= 2)
                    {
                        menuPointer.y = 0;
                        actionMenuState = ActionMenuState.Attack;
                    }
                    else
                    {
                        menuPointer.y++;
                        actionMenuState++;
                    }
                    menuPointer.Position = new Vector2(5, (5 + (menuPointer.y * 20)) * menuPointer.Scale);
                    // TODO: Menu movement sound.
                }
            }

            previousKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// The menu that shows after the z/enter button is pressed on an empty tile.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        void PauseMenuKeyHandling(GameTime gameTime, MapSprite currentMap)
        {
            currentKeyboardState = Keyboard.GetState();

            // INCOMPLETE - If Z is pressed within the menu, handle actions.
            if (currentKeyboardState.IsKeyDown(Keys.Z) && currentKeyboardState != previousKeyboardState)
            {
                // Switch case for PauseMenu Hovered option enumeration. Descending order.
                switch (pauseMenuState)
                {
                    case PauseMenuState.Units:
                        {
                            // Low Priority Task.
                            // Show the list of units on the allied party.
                            break;
                        }
                    case PauseMenuState.Items:
                        {
                            // Low Priority Task
                            // Show Items of all units in the allied party.
                            break;
                        }
                    case PauseMenuState.Status:
                        {
                            // Low Priority Task
                            // Display number of units left on each side, as well as the status of alive units.
                            break;
                        }
                    case PauseMenuState.Options:
                        {
                            // Low Priority Task
                            // Display Options
                            break;
                        }
                    case PauseMenuState.EndTurn:
                        {
                            // High Priority Task
                            // Set GameState to Enemy phase.
                            gameState = GameState.EnemyPhase;

                            soldier1.IsWaiting = false;
                            soldier2.IsWaiting = false;
                            soldier3.IsWaiting = false;
                            archer1.IsWaiting = false;

                            enemyPhaseNotificationWasShown = false;
                            timeSinceLastKeyPress = 0;
                            break;
                        }
                }

            }

            // If X is pressed, cancel out of the menu, reverting the game state to normal.
            if (currentKeyboardState.IsKeyDown(Keys.X) && currentKeyboardState != previousKeyboardState)
            {
                gameState = GameState.NormalPlay;
                // Resets the pointer to the starting position of the action menu.
                menuPointer.y = 0;
                menuPointer.Position = new Vector2(menuPointer.x * 33 + 5, menuPointer.y * 33 + 5);

                // Resets the selected state to the starting option, units
                pauseMenuState = PauseMenuState.Units;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.Up))
            {
                // If the up arrow was the pressed key, move the pointer up through the menu.
                if (currentKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState != previousKeyboardState)
                {
                    // If the up arrow is pressed and the pointer is at the top, move it to the bottom option. Otherwise, increment.
                    if (pauseMenuState == PauseMenuState.Units || menuPointer.y <= 0)
                    {
                        menuPointer.y = 4;
                        pauseMenuState = PauseMenuState.EndTurn;
                    }
                    else
                    {
                        menuPointer.y--;
                        pauseMenuState--;
                    }
                    // Change the X to be a flat value.
                    menuPointer.Position = new Vector2(5, (5 + (menuPointer.y * 20)) * menuPointer.Scale);
                    // TODO: Menu move sound.

                }
                else if (currentKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState != previousKeyboardState)
                {
                    if (pauseMenuState == PauseMenuState.EndTurn || menuPointer.y >= 4)
                    {
                        menuPointer.y = 0;
                        pauseMenuState = PauseMenuState.Units;
                    }
                    else
                    {
                        menuPointer.y++;
                        pauseMenuState++;
                    }
                    menuPointer.Position = new Vector2(5, (5 + (menuPointer.y * 20)) * menuPointer.Scale);
                    //TODO: Menu movement sound of some sort.
                }
            }


            previousKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// When the game state is changed to enemy phase.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        void EnemyPhaseHandler(GameTime gameTime, MapSprite currentMap)
        {
            //bool enemyWasRelocated = false;

            if (isAttacking == false)
            {
                timeSinceLastKeyPress += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastKeyPress > 500 && timeSinceLastKeyPress < 1000)
                {
                    if (enemySoldier1.IsAlive == true && enemySoldier1.IsWaiting == false)
                    {
                        HaveEnemyAttack(enemySoldier1, currentMap, gameTime);
                        timeSinceLastKeyPress = 0;
                    }
                    else if (enemySoldier2.IsAlive == true && enemySoldier2.IsWaiting == false)
                    {
                        HaveEnemyAttack(enemySoldier2, currentMap, gameTime);
                        timeSinceLastKeyPress = 0;
                    }
                    else if (enemySoldier3.IsAlive == true && enemySoldier3.IsWaiting == false)
                    {
                        HaveEnemyAttack(enemySoldier3, currentMap, gameTime);
                        timeSinceLastKeyPress = 0;
                    }
                    else if (enemySoldier4.IsAlive == true && enemySoldier4.IsWaiting == false)
                    {
                        HaveEnemyAttack(enemySoldier4, currentMap, gameTime);
                        timeSinceLastKeyPress = 0;
                    }
                    else if (enemyArcher1.IsAlive == true && enemyArcher1.IsWaiting == false)
                    {
                        HaveEnemyAttack(enemyArcher1, currentMap, gameTime);
                        timeSinceLastKeyPress = 0;
                    }
                    else if (enemyArcher2.IsAlive == true && enemyArcher2.IsWaiting == false)
                    {
                        HaveEnemyAttack(enemyArcher2, currentMap, gameTime);
                        timeSinceLastKeyPress = 0;
                    }
                }
            }



            // End enemy phase after a certain amount of time passed in which no units have moved.
            if (timeSinceLastKeyPress > 2000)
            {
                gameState = GameState.NormalPlay;
                playerPhaseNotificationWasShown = false;

                actionMenuState = ActionMenuState.Attack;
                pauseMenuState = PauseMenuState.Units;

                //isAttacking = false;
                timeSinceLastKeyPress = 0;

                //theCursor.ResetHeldCharacterToNull();

                enemySoldier1.IsWaiting = false;
                enemySoldier2.IsWaiting = false;
                enemySoldier3.IsWaiting = false;
                enemySoldier4.IsWaiting = false;
                enemyArcher1.IsWaiting = false;
                enemyArcher2.IsWaiting = false;
            }

        }

        /// <summary>
        /// Basic Enemy AI, searches through the combined movement and attack range of the given enemy, prioritizes targets that it can hit, based on the potential targets'
        /// remaining HP.
        /// </summary>
        /// <param name="attackingEnemy"></param>
        /// <param name="currentMap"></param>
        /// <param name="gameTime"></param>
        void HaveEnemyAttack(Character attackingEnemy, MapSprite currentMap, GameTime gameTime)
        {
            if (attackingEnemy.IsWaiting == false)
            {
                // TODO: Change find target to find multiple targets, get their positions, and assign it to a list.
                List<Character> listDiscoveredTargets = new List<Character>();
                Character PriorityTarget = new Character();

                listDiscoveredTargets = currentMap.FindTargets(attackingEnemy);
                int minHP = 99;

                bool enemyCanGoToRightOfTarget = false;
                bool enemyCanGoToLeftOfTarget = false;
                bool enemyCanGoToAboveOfTarget = false;
                bool enemyCanGoToBelowOfTarget = false;

                // Sort through the targets, pick one to prioritize by closest to dying
                foreach (Character unit in listDiscoveredTargets)
                {
                    if (unit.CharacterRemainingHP < minHP)
                    {
                        // If we found a higher-priority target, reset where they can go.
                        if (unit != PriorityTarget)
                        {
                            enemyCanGoToRightOfTarget = false;
                            enemyCanGoToLeftOfTarget = false;
                            enemyCanGoToAboveOfTarget = false;
                            enemyCanGoToBelowOfTarget = false;
                        }

                        // Check to the right of the target, desired position equal to the target's position x + enemy's range, target's position y.
                        //      If the enemy can move to that desired position, then set a flag to true.
                        if ((currentMap.IsThereASpriteAtThePosition(unit.x + attackingEnemy.CharacterAttackRange, unit.y) == false
                            || currentMap.characterMapDataArray[unit.x + attackingEnemy.CharacterAttackRange, unit.y] == attackingEnemy))
                        {
                            int currentEnemyXPosition = attackingEnemy.x;
                            int currentEnemyYPosition = attackingEnemy.y;
                            int desiredXPosition = unit.x + attackingEnemy.CharacterAttackRange;
                            int desiredYPosition = unit.y;

                            // If the desired position is within the boundaries of the map, it's a legal move thus far.
                            if (desiredXPosition <= currentMap.GetPositionXLimit())
                            {
                                // Check for the number of spaces it would take to get to that position.

                                int positionalDifference = Math.Abs(Math.Abs(desiredXPosition - currentEnemyXPosition) + Math.Abs(desiredYPosition - currentEnemyYPosition));

                                // Finally, if that position is within the movement range of the character, we can set the character to that position, validating the target
                                //     attacking.
                                if (positionalDifference <= attackingEnemy.CharacterMovement)
                                {
                                    minHP = unit.CharacterRemainingHP;
                                    PriorityTarget = unit;

                                    // Flag to tell it to move to the right of the sprite.
                                    enemyCanGoToRightOfTarget = true;
                                }
                            }

                        }
                        // Check below the target, desired position equal to the target's position x, target's position y  + enemy's range.
                        //      If the enemy can move to that desired position, then set a flag to true.
                        if ((currentMap.IsThereASpriteAtThePosition(unit.x, unit.y + attackingEnemy.CharacterAttackRange) == false
                            || currentMap.characterMapDataArray[unit.x, unit.y + attackingEnemy.CharacterAttackRange] == attackingEnemy))
                        {
                            int currentEnemyXPosition = attackingEnemy.x;
                            int currentEnemyYPosition = attackingEnemy.y;
                            int desiredXPosition = unit.x;
                            int desiredYPosition = unit.y + attackingEnemy.CharacterAttackRange;

                            // If the desired position is within the boundaries of the map, it's a legal move thus far.
                            if (desiredYPosition <= currentMap.GetPositionYLimit())
                            {
                                // Check for the number of spaces it would take to get to that position.

                                int positionalDifference = Math.Abs(Math.Abs(desiredXPosition - currentEnemyXPosition) + Math.Abs(desiredYPosition - currentEnemyYPosition));

                                // Finally, if that position is within the movement range of the character, we can set the character to that position, validating the target
                                //     attacking.
                                if (positionalDifference <= attackingEnemy.CharacterMovement)
                                {
                                    minHP = unit.CharacterRemainingHP;
                                    PriorityTarget = unit;

                                    // Flag to tell it to move to the right of the sprite.
                                    enemyCanGoToBelowOfTarget = true;
                                }
                            }
                        }
                        // Check to the left of the target, desired position equal to the target's position x - enemy's range., target's position y
                        //      If the enemy can move to that desired position, then set a flag to true.
                        if ((currentMap.IsThereASpriteAtThePosition(unit.x - attackingEnemy.CharacterAttackRange, unit.y) == false
                            || currentMap.characterMapDataArray[unit.x - attackingEnemy.CharacterAttackRange, unit.y] == attackingEnemy))
                        {
                            int currentEnemyXPosition = attackingEnemy.x;
                            int currentEnemyYPosition = attackingEnemy.y;
                            int desiredXPosition = unit.x - attackingEnemy.CharacterAttackRange;
                            int desiredYPosition = unit.y;

                            // If the desired position is within the boundaries of the map, it's a legal move thus far.
                            if (desiredXPosition >= 0)
                            {
                                // Check for the number of spaces it would take to get to that position.

                                int positionalDifference = Math.Abs(Math.Abs(desiredXPosition - currentEnemyXPosition) + Math.Abs(desiredYPosition - currentEnemyYPosition));

                                // Finally, if that position is within the movement range of the character, we can set the character to that position, validating the target
                                //     attacking.
                                if (positionalDifference <= attackingEnemy.CharacterMovement)
                                {
                                    minHP = unit.CharacterRemainingHP;
                                    PriorityTarget = unit;

                                    // Flag to tell it to move to the right of the sprite.
                                    enemyCanGoToLeftOfTarget = true;
                                }
                            }
                        }
                        // Check above the target, desired position equal to the target's position x, target's position y  - enemy's range.
                        //      If the enemy can move to that desired position, then set a flag to true.
                        if ((currentMap.IsThereASpriteAtThePosition(unit.x, unit.y - attackingEnemy.CharacterAttackRange) == false
                            || currentMap.characterMapDataArray[unit.x, unit.y - attackingEnemy.CharacterAttackRange] == attackingEnemy))
                        {
                            int currentEnemyXPosition = attackingEnemy.x;
                            int currentEnemyYPosition = attackingEnemy.y;
                            int desiredXPosition = unit.x;
                            int desiredYPosition = unit.y - attackingEnemy.CharacterAttackRange;

                            // If the desired position is within the boundaries of the map, it's a legal move thus far.
                            if (desiredYPosition >= 0)
                            {
                                // Check for the number of spaces it would take to get to that position.

                                int positionalDifference = Math.Abs(Math.Abs(desiredXPosition - currentEnemyXPosition) + Math.Abs(desiredYPosition - currentEnemyYPosition));

                                // Finally, if that position is within the movement range of the character, we can set the character to that position, validating the target
                                //     attacking.
                                if (positionalDifference <= attackingEnemy.CharacterMovement)
                                {
                                    minHP = unit.CharacterRemainingHP;
                                    PriorityTarget = unit;

                                    // Flag to tell it to move to the right of the sprite.
                                    enemyCanGoToAboveOfTarget = true;
                                }
                            }
                        }
                    }
                }
                // Find out where to move with the prioritized target. Start with the right of the character. If that's not available, or not in range,
                //      try below. If that isn't available, left. If that isn't applicable, above. Otherwise, move on to a different character. Or, do nothing.
                //      May need function to calculate the difference between the current character's position and the target's position.

                // If there is an actual target selected, execute the attacking part.
                if (PriorityTarget.assetName != null)
                {
                    if (enemyCanGoToRightOfTarget == true)
                    {
                        int tempX = attackingEnemy.x;
                        int tempY = attackingEnemy.y;

                        currentMap.characterMapDataArray[PriorityTarget.x + attackingEnemy.CharacterAttackRange, PriorityTarget.y] = attackingEnemy;

                        attackingEnemy.x = PriorityTarget.x + attackingEnemy.CharacterAttackRange;
                        attackingEnemy.y = PriorityTarget.y;
                        attackingEnemy.Position.X = ((attackingEnemy.x * 32) * attackingEnemy.Scale);
                        attackingEnemy.Position.Y = ((attackingEnemy.y * 32) * attackingEnemy.Scale);

                        currentMap.DeleteSpriteAtPosition(tempX, tempY);

                        theCursor.x = PriorityTarget.x;
                        theCursor.y = PriorityTarget.y;
                        theCursor.Update(gameTime);
                        theCursor.heldCharacter = attackingEnemy;

                        isAttacking = true;
                        return;

                    }
                    // If the enemy cannot go to the right of the target, but can go below the target, do that instead.
                    else if (enemyCanGoToBelowOfTarget == true)
                    {
                        int tempX = attackingEnemy.x;
                        int tempY = attackingEnemy.y;

                        // Get the desired position and set the enemy to that position.
                        currentMap.characterMapDataArray[PriorityTarget.x, PriorityTarget.y + attackingEnemy.CharacterAttackRange] = attackingEnemy;

                        attackingEnemy.x = PriorityTarget.x;
                        attackingEnemy.y = PriorityTarget.y + attackingEnemy.CharacterAttackRange;
                        attackingEnemy.Position.X = ((attackingEnemy.x * 32) * attackingEnemy.Scale);
                        attackingEnemy.Position.Y = ((attackingEnemy.y * 32) * attackingEnemy.Scale);

                        currentMap.DeleteSpriteAtPosition(tempX, tempY);

                        theCursor.x = PriorityTarget.x;
                        theCursor.y = PriorityTarget.y;
                        theCursor.Update(gameTime);
                        theCursor.heldCharacter = attackingEnemy;

                        isAttacking = true;
                        return;
                    }
                    // If the enemy cannot go to the right or below the target, but can go to the left of target, do that instead.
                    else if (enemyCanGoToLeftOfTarget == true)
                    {
                        int tempX = attackingEnemy.x;
                        int tempY = attackingEnemy.y;

                        currentMap.characterMapDataArray[PriorityTarget.x - attackingEnemy.CharacterAttackRange, PriorityTarget.y] = attackingEnemy;

                        attackingEnemy.x = PriorityTarget.x - attackingEnemy.CharacterAttackRange;
                        attackingEnemy.y = PriorityTarget.y;
                        attackingEnemy.Position.X = ((attackingEnemy.x * 32) * attackingEnemy.Scale);
                        attackingEnemy.Position.Y = ((attackingEnemy.y * 32) * attackingEnemy.Scale);

                        currentMap.DeleteSpriteAtPosition(tempX, tempY);

                        theCursor.x = PriorityTarget.x;
                        theCursor.y = PriorityTarget.y;
                        theCursor.Update(gameTime);
                        theCursor.heldCharacter = attackingEnemy;

                        isAttacking = true;
                        return;
                    }
                    // If the enemy cannot go to the right, below, or left of the target, but can go above the target, do that instead.
                    else if (enemyCanGoToAboveOfTarget == true)
                    {
                        int tempX = attackingEnemy.x;
                        int tempY = attackingEnemy.y;

                        // Get the desired position and set the enemy to that position.
                        currentMap.characterMapDataArray[PriorityTarget.x, PriorityTarget.y - attackingEnemy.CharacterAttackRange] = attackingEnemy;

                        attackingEnemy.x = PriorityTarget.x;
                        attackingEnemy.y = PriorityTarget.y - attackingEnemy.CharacterAttackRange;
                        attackingEnemy.Position.X = ((attackingEnemy.x * 32) * attackingEnemy.Scale);
                        attackingEnemy.Position.Y = ((attackingEnemy.y * 32) * attackingEnemy.Scale);

                        currentMap.DeleteSpriteAtPosition(tempX, tempY);

                        theCursor.x = PriorityTarget.x;
                        theCursor.y = PriorityTarget.y;
                        theCursor.Update(gameTime);
                        theCursor.heldCharacter = attackingEnemy;

                        isAttacking = true;
                        return;
                    }
                    // If they can't do anything, just make them wait,
                    else
                    {
                        attackingEnemy.IsWaiting = true;
                        return;
                    }
                }
                else
                {
                    // No targets discovered, enemy will not do anything.
                    attackingEnemy.IsWaiting = true;
                    return;
                }
            }

        }

        /// <summary>
        /// Does the same thing that Normal Key handling does, but restricts cursor movement.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        void AttackSelectionKeyHandler(GameTime gameTime, MapSprite currentMap)
        {
            if (isAttacking != true)
            {
                currentKeyboardState = Keyboard.GetState();
                timeSinceLastKeyPress = 0;

                // If Z is pressed, attack the target at the cursor location, if there is one.
                if (currentKeyboardState.IsKeyDown(Keys.Z) && currentKeyboardState != previousKeyboardState)
                {
                    // If there is a sprite at the selected target, and the target isn't themselves or a teammate...
                    if (currentMap.IsThereASpriteAtThePosition(theCursor.x, theCursor.y) == true
                        && currentMap.characterMapDataArray[theCursor.x, theCursor.y] != theCursor.heldCharacter
                        && (theCursor.heldCharacter.IsEnemyUnit != testMap.characterMapDataArray[theCursor.x, theCursor.y].IsEnemyUnit))
                    {
                        isAttacking = true;
                    }
                    else
                    {
                        // TODO: Error sound.
                    }
                }

                // If X is pressed, revert to action menu.
                if (currentKeyboardState.IsKeyDown(Keys.X) && currentKeyboardState != previousKeyboardState)
                {
                    gameState = GameState.ActionMenu;
                }



                // If the character has a range of 2, they are able to attack diagonally, and priority is given to that.
                // If the player presses down-right, move the cursor down and to the right.
                if (theCursor.heldCharacter.CharacterAttackRange == 2
                    && currentKeyboardState != previousKeyboardState
                    && currentKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (theCursor.x + 1 <= currentMap.GetPositionXLimit() && theCursor.y + 1 <= currentMap.GetPositionYLimit())
                    {
                        theCursor.x = theCursor.heldCharacter.x + 1;
                        theCursor.Position.X = (theCursor.x * 32 * theCursor.Scale);

                        theCursor.y = theCursor.heldCharacter.y + 1;
                        theCursor.Position.Y = theCursor.y * 32 * theCursor.Scale;
                    }
                    else
                    {
                        // TODO: Error sound.
                    }
                }
                // Otherwise, if the player presses up-right and has 2 range, move the cursor up and to the right.
                else if (theCursor.heldCharacter.CharacterAttackRange == 2
                    && currentKeyboardState != previousKeyboardState
                    && currentKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (theCursor.x + 1 <= currentMap.GetPositionXLimit() && theCursor.y - 1 >= 0)
                    {
                        theCursor.x = theCursor.heldCharacter.x + 1;
                        theCursor.Position.X = (theCursor.x * 32 * theCursor.Scale);

                        theCursor.y = theCursor.heldCharacter.y - 1;
                        theCursor.Position.Y = theCursor.y * 32 * theCursor.Scale;
                    }
                    else
                    {
                        // TODO: Error sound.
                    }
                }
                // Otherwise, if the player presses down-left and has 2 range, move the cursor down and to the left.
                else if (theCursor.heldCharacter.CharacterAttackRange == 2
                    && currentKeyboardState != previousKeyboardState
                    && currentKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (theCursor.x - 1 >= 0 && theCursor.y + 1 <= currentMap.GetPositionYLimit())
                    {
                        theCursor.x = theCursor.heldCharacter.x - 1;
                        theCursor.Position.X = (theCursor.x * 32 * theCursor.Scale);

                        theCursor.y = theCursor.heldCharacter.y + 1;
                        theCursor.Position.Y = theCursor.y * 32 * theCursor.Scale;
                    }
                    else
                    {
                        // TODO: Error sound.
                    }
                }
                // Otherwise, if the player presses up-left and has 2 range, move the cursor up and to the left.
                else if (theCursor.heldCharacter.CharacterAttackRange == 2
                    && currentKeyboardState != previousKeyboardState
                   && currentKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (theCursor.x - 1 >= 0 && theCursor.y - 1 >= 0)
                    {
                        theCursor.x = theCursor.heldCharacter.x - 1;
                        theCursor.Position.X = (theCursor.x * 32 * theCursor.Scale);

                        theCursor.y = theCursor.heldCharacter.y - 1;
                        theCursor.Position.Y = theCursor.y * 32 * theCursor.Scale;
                    }
                    else
                    {
                        // TODO: Error sound.
                    }
                }
                // Otherwise, assume the player is only pressing one key, or does not have 2 range.
                else
                {
                    // If the left key was pressed...
                    if (currentKeyboardState.IsKeyDown(Keys.Left) && currentKeyboardState != previousKeyboardState)
                    {
                        // If the cursor movement would not go off the left-side boundary...
                        if (theCursor.heldCharacter.x - theCursor.heldCharacter.CharacterAttackRange >= 0)
                        {
                            // ...Then, move the cursor to the left, equivalent to the held character's attack range.
                            theCursor.x = theCursor.heldCharacter.x - theCursor.heldCharacter.CharacterAttackRange;
                            theCursor.Position.X = (theCursor.x * 32 * theCursor.Scale);

                            theCursor.y = theCursor.heldCharacter.y;
                            theCursor.Position.Y = (theCursor.heldCharacter.y * 32 * theCursor.Scale);
                        }
                        else
                        {
                            // TOOD: Error sound.
                        }
                    }
                    // Otherwise, if the right key is pressed...
                    else if (currentKeyboardState.IsKeyDown(Keys.Right) && currentKeyboardState != previousKeyboardState)
                    {
                        // If the cursor movement would not go off the left-side boundary...
                        if (theCursor.heldCharacter.x + theCursor.heldCharacter.CharacterAttackRange <= currentMap.GetPositionXLimit())
                        {
                            // ...Then, move the cursor to the right, equivalent to the held character's attack range.
                            theCursor.x = theCursor.heldCharacter.x + theCursor.heldCharacter.CharacterAttackRange;
                            theCursor.Position.X = (theCursor.x * 32 * theCursor.Scale);

                            theCursor.y = theCursor.heldCharacter.y;
                            theCursor.Position.Y = (theCursor.heldCharacter.y * 32 * theCursor.Scale);
                        }
                        else
                        {
                            // TODO: Error sound.
                        }
                    }
                    // Otherwise, if the up arrow key is pressed...
                    else if (currentKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState != previousKeyboardState)
                    {
                        if (theCursor.heldCharacter.y - theCursor.heldCharacter.CharacterAttackRange >= 0)
                        {
                            // ...Then, move the cursor up, equivalent to the held character's attack range.
                            theCursor.x = theCursor.heldCharacter.x;
                            theCursor.Position.X = (theCursor.heldCharacter.x * 32 * theCursor.Scale);

                            theCursor.y = theCursor.heldCharacter.y - theCursor.heldCharacter.CharacterAttackRange;
                            theCursor.Position.Y = (theCursor.y * 32 * theCursor.Scale);
                        }
                        else
                        {
                            // TODO: Error sound.
                        }
                    }
                    // Otherwise, if the down arrow key is pressed...
                    else if (currentKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState != previousKeyboardState)
                    {
                        if (theCursor.heldCharacter.y + theCursor.heldCharacter.CharacterAttackRange <= currentMap.GetPositionYLimit())
                        {
                            // ...Then, move the cursor down, equivalent to the held character's attack range.
                            theCursor.x = theCursor.heldCharacter.x;
                            theCursor.Position.X = (theCursor.heldCharacter.x * 32 * theCursor.Scale);

                            theCursor.y = theCursor.heldCharacter.y + theCursor.heldCharacter.CharacterAttackRange;
                            theCursor.Position.Y = (theCursor.y * 32 * theCursor.Scale);
                        }
                        else
                        {
                            // TODO: Error sound.
                        }
                    }
                }


                previousKeyboardState = Keyboard.GetState();
            }

        } // End Attack Handler

        /// <summary>
        /// Draw each of the sprites associated with the player.
        /// TODO: Make 2 lists of player sprites, one concerning 'active' sprites, and one concerning all of them, then turn this function to draw only active sprites.
        /// </summary>
        void DrawPlayerSprites()
        {
            // If the player soldier is alive...
            if (soldier1.IsAlive == true)
            {
                // Then, draw their sprite.
                soldier1.Draw(this.spriteBatch);
                // If the player unit is waiting...
                if (soldier1.IsWaiting == true)
                {
                    // Then, draw a grey square over them to represent their waiting status.
                    WaitingCharacterSquare.Position = new Vector2(soldier1.x * 32 * soldier1.Scale, soldier1.y * 32 * soldier1.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                } // End soldier wait check.

            } // End soldier alive check.
            // If the player soldier is alive...
            if (soldier2.IsAlive == true)
            {
                // Then, draw their sprite.
                soldier2.Draw(this.spriteBatch);
                // If the player unit is waiting...
                if (soldier2.IsWaiting == true)
                {
                    // Then, draw a grey square over them to represent their waiting status.
                    WaitingCharacterSquare.Position = new Vector2(soldier2.x * 32 * soldier2.Scale, soldier2.y * 32 * soldier2.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                } // End soldier wait check.

            } // End soldier alive check.
            // If the player soldier is alive...
            if (soldier3.IsAlive == true)
            {
                // Then, draw their sprite.
                soldier3.Draw(this.spriteBatch);
                // If the player unit is waiting...
                if (soldier3.IsWaiting == true)
                {
                    // Then, draw a grey square over them to represent their waiting status.
                    WaitingCharacterSquare.Position = new Vector2(soldier3.x * 32 * soldier3.Scale, soldier3.y * 32 * soldier3.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                } // End soldier wait check.

            } // End soldier alive check.

            if (archer1.IsAlive == true)
            {
                // Then, draw their sprite.
                archer1.Draw(this.spriteBatch);
                // If the player unit is waiting...
                if (archer1.IsWaiting == true)
                {
                    // Then, draw a grey square over them to represent their waiting status.
                    WaitingCharacterSquare.Position = new Vector2(archer1.x * 32 * archer1.Scale, archer1.y * 32 * archer1.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                }

            }
        } // End DrawPlayerSprites function.

        /// <summary>
        /// 
        /// </summary>
        void DrawEnemySprites()
        {
            if (enemySoldier1.IsAlive == true)
            {
                enemySoldier1.Draw(this.spriteBatch);
                // If the enemy is waiting...
                if (enemySoldier1.IsWaiting == true)
                {
                    // Then, draw a grey square to represent them waiting.
                    WaitingCharacterSquare.Position = new Vector2(enemySoldier1.x * 32 * enemySoldier1.Scale, enemySoldier1.y * 32 * enemySoldier1.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                }
            } // End Enemy Soldier 1 Drawing.
            if (enemySoldier2.IsAlive == true)
            {
                enemySoldier2.Draw(this.spriteBatch);
                // If the enemy is waiting...
                if (enemySoldier2.IsWaiting == true)
                {
                    // Then, draw a grey square to represent them waiting.
                    WaitingCharacterSquare.Position = new Vector2(enemySoldier2.x * 32 * enemySoldier2.Scale, enemySoldier2.y * 32 * enemySoldier2.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                }
            } // End Enemy Soldier 2 Drawing.
            if (enemySoldier3.IsAlive == true)
            {
                enemySoldier3.Draw(this.spriteBatch);
                // If the enemy is waiting...
                if (enemySoldier3.IsWaiting == true)
                {
                    // Then, draw a grey square to represent them waiting.
                    WaitingCharacterSquare.Position = new Vector2(enemySoldier3.x * 32 * enemySoldier3.Scale, enemySoldier3.y * 32 * enemySoldier3.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                }
            }  // End Enemy Soldier 2 Drawing.

            if (enemySoldier4.IsAlive == true)
            {
                enemySoldier4.Draw(this.spriteBatch);
                // If the enemy is waiting...
                if (enemySoldier4.IsWaiting == true)
                {
                    // Then, draw a grey square to represent them waiting.
                    WaitingCharacterSquare.Position = new Vector2(enemySoldier4.x * 32 * enemySoldier4.Scale, enemySoldier4.y * 32 * enemySoldier4.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                }
            }  // End Enemy Soldier 2 Drawing.
            if (enemyArcher1.IsAlive == true)
            {
                // Then, draw their sprite.
                enemyArcher1.Draw(this.spriteBatch);
                // If the player unit is waiting...
                if (enemyArcher1.IsWaiting == true)
                {
                    // Then, draw a grey square over them to represent their waiting status.
                    WaitingCharacterSquare.Position = new Vector2(enemyArcher1.x * 32 * enemyArcher1.Scale, enemyArcher1.y * 32 * enemyArcher1.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                }
            }
            if (enemyArcher2.IsAlive == true)
            {
                // Then, draw their sprite.
                enemyArcher2.Draw(this.spriteBatch);
                // If the player unit is waiting...
                if (enemyArcher2.IsWaiting == true)
                {
                    // Then, draw a grey square over them to represent their waiting status.
                    WaitingCharacterSquare.Position = new Vector2(enemyArcher2.x * 32 * enemyArcher2.Scale, enemyArcher2.y * 32 * enemyArcher2.Scale);
                    WaitingCharacterSquare.Draw(this.spriteBatch);
                }
            }
        } // End DrawEnemySprites function.

        /// <summary>
        /// Instruction text describing the controls. Drawn on multiple game states.
        /// </summary>
        /// <param name="spriteBatch"></param>
        void DrawInstructions(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, "           Instructions (controls):\nZ to select/confirm                X to cancel" +
                "\nUse the arrow keys to move the cursor", new Vector2(10, 525), Color.White);
        }
    } // End Class Game1
} // End GameDesign namespace.
