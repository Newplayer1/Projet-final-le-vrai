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


namespace AtelierXNA.Classes_Pokemon_Skyrim
{
    public class Pokeball : SphèreTexturée
    {

        public Pokeball(Game jeu, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float rayon, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, échelleInitiale, rotationInitiale, positionInitiale, rayon, charpente, nomTexture, intervalleMAJ)
        {

        }


        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

    }
}
