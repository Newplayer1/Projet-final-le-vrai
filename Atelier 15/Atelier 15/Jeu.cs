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


namespace AtelierXNA
{
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        public Jeu(Game game)
            : base(game)
        {

        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            G�rerTransition();//tout ce qui a rapport avec l'affichage des choses 
            //Ex: quand on rentre dans un combat de pok�mons
            G�rer�tat(); // tout ce qui a rapport avec le changement d'�tat
            // Ex: Le pokemon est vivant et a un final hit qui le tue ... il passe maintenant mort
            base.Update(gameTime);
        }
    }
}
