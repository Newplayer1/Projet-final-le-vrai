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

    public class Combat : Microsoft.Xna.Framework.GameComponent
    {
        Trainer User { get; set; }
        Trainer Opponent { get; set; }

        public bool EnCombat { get; set; }
        public bool Est

        public Combat(Game game, Trainer user, Trainer opponent)
            : base(game)
        {
            User = user;
            Opponent = opponent;
        }//Faire second constructeur pour WILD BATTLE et bool est wild à true si constructeur wild


        public override void Initialize()//Ouverture du combat. Tout ce qui doit être fait avant "Main menu"
        {
            EnCombat = true; //GameState = Battle

            User.Throw(PokemonsSurLui{0});

            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }
    }
}
