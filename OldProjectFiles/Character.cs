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
    public class Character : Sprite
    {
        // Properties representing common stats, that directly affect combat - HP, attack, etc.
        public int CharacterMaxHP { get; set; }
        public int CharacterRemainingHP { get; set; }
        public int CharacterAttack { get; set; }
        public int CharacterDefense { get; set; }
        public int CharacterDexterity { get; set; }

        // Properties affecting random factors - hit chance, speed, etc.
        public int CharacterHitChance { get; set; }
        public int CharacterSpeed { get; set; }

        // Properties representing this character's class, movement range from class, and attack range.
        public string CharacterClass { get; set; }
        public int CharacterMovement { get; set; }
        public int CharacterAttackRange { get; set; }

        // Properties representing what a character is, such as whether they are a player unit or not, for collision purposes.
        public bool IsEnemyUnit { get; set; }
        public bool IsWaiting { get; set; }
        public bool IsAlive { get; set; }
        

        // All characters have an asset name, in some cases, multiple.
        /// <summary>
        /// Sets the unit asset name. Used for loading content for new objects.
        /// </summary>
        /// <param name="newAssetName"></param>
        public void SetUnitAssetName(string newAssetName)
        {
            base.assetName = newAssetName;
        }

        /// <summary>
        /// Used to load content for individual characters; Retrieves the current character's asset name, it's starting position, and passes it to the parent Sprite class.
        /// </summary>
        /// <param name="theContentManager"></param>
        public void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager, assetName, x, y);
        }

        /// <summary>
        /// Used to update the x and y value of the Sprite, as well as the Position vector, of the parent Sprite class.
        /// Note that x and y are generally used for representation on the logical layer of data,
        /// whereas the Position vector is used for a visual representation of where a sprite is.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void UpdatePosition(int x, int y)
        {
            base.x = x;
            base.y = y;
            base.Position = new Vector2(((x * 32)) * Scale, ((y * 32)) * Scale);
        }

        /// <summary>
        /// Initializes a character's stats, like movement or HP, based on their asset name.
        /// TODO: Find out if it would be more efficient to make classes for each character, which inherit from Character, and thus, Sprite.
        /// </summary>
        public void InitializeCharacter()
        {
            if (base.assetName != null)
            {
                if (assetName.Equals("MyArt/allySoldier"))
                {
                    base.IndividualScale = 0.08f;

                    CharacterAttack = 12;

                    CharacterMaxHP = 20;
                    CharacterRemainingHP = 20;
                    CharacterAttackRange = 1;
                    CharacterMovement = 3;
                    IsEnemyUnit = false;
                    IsWaiting = false;
                    IsAlive = true;
                } // End Ally Soldier1 Initialization.
                else if(assetName.Equals("MyArt/enemySoldier"))
                {
                    // Create individual scaling for texture.
                    base.IndividualScale = 0.08f;

                    CharacterAttack = 12;
                    CharacterMaxHP = 20;
                    CharacterRemainingHP = 20;
                    CharacterAttackRange = 1;
                    CharacterMovement = 3;
                    IsEnemyUnit = true;
                    IsWaiting = false;
                    IsAlive = true;
                }
                else if (assetName.Equals("MyArt/allyArcher"))
                {
                    // Create individual scaling for texture.
                    base.IndividualScale = 0.08f;
                    //base.Scale = IndividualScale;

                    CharacterAttack = 8;
                    CharacterMaxHP = 12;
                    CharacterRemainingHP = 12;
                    CharacterAttackRange = 2;
                    CharacterMovement = 3;
                    IsEnemyUnit = false;
                    IsWaiting = false;
                    IsAlive = true;
                }
                else if (assetName.Equals("MyArt/enemyArcher"))
                {
                    // Create individual scaling for texture.
                    base.IndividualScale = 0.08f;
                    //base.Scale = IndividualScale;

                    CharacterAttack = 8;
                    CharacterMaxHP = 12;
                    CharacterRemainingHP = 12;
                    CharacterAttackRange = 2;
                    CharacterMovement = 3;
                    IsEnemyUnit = true;
                    IsWaiting = false;
                    IsAlive = true;
                }
            } // End check for null assetName value.
        } // End InitializeCharacter function.
    } // End class Character.
} // End GameDesignTestArea namespace.