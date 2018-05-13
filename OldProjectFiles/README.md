# Section Purpose

Since there were a lot of files that ultimately went unused in the first iteration of the project, I'm going to hold the old files that did get used here, so people can still see this, but it doesn't directly interfere with the actual project, while I'm working on making the newer, more efficient (and ever-so-slightly more graceful) version of the project. Once everything is up, I'll work towards explaining this project's version.

## Classes

#### Program.cs
- Auto-generated class via MonoGame. Runs the game.
#### Game1.cs
- The "main" class for the game's runtime.
- Handles nearly every non-image task; input handling, enemy AI, menu navigation, character selection and actions, updating sprite parameters (such as position, if you moved a sprite), drawing sprites to the screen, and the general game state.
#### Sprite.cs 
- Class generally meant to be used for inheriting, but has the parameters to function on its own.
- Holds the parameters that all sprites should always share; a position, textures, path names, image scaling, etc.
#### Character.cs
- Inherits from Sprite.cs
- Utilizes the parameters from Sprite.cs, while assigning properties unique to character sprites;  RPG stats, such as HP, attack, or movement.
#### MapSprite.cs  
- Inherits from Sprite.cs
- Utilizes the parameters from Sprite.cs, while using its own unique properties, such as map boundaries, to account for where the player can and cannot go.
#### CursorSprite.cs
- Inherits from Sprite.cs
- Utilizes parameters from inherited class, but creates its own properties for input handling, as well as character selection, such as "holding" a character when selecting a movement location for the character.
#### MovementSquare.cs
- Inherits from Sprite.cs
- Utlizes parameters from inherited class, but is generally focused on the contained algorithm to draw out movement squares that determine where a selected character can move, to give the player an ease-of-use, visually speaking, so they can understand at a glance where they can move.
