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
        Trainer UserTrainer { get; set; }
        Trainer OpponentTrainer { get; set; }

        Pokemon UserPokemon { get; set; }
        Pokemon OpponentPokemon { get; set; }
        

        public bool EnCombat { get; set; }
        public bool EstOpponentSauvage { get; set; }


        public Combat(Game game, Trainer user, Trainer opponentTrainer)
            : base(game)
        {
            UserTrainer = user;
            OpponentTrainer = opponentTrainer;
            EstOpponentSauvage = false;
        }
        public Combat(Game game, Trainer user, Pokemon wildPokemon)
            : base(game)
        {
            UserTrainer = user;
            OpponentTrainer = null;
            OpponentPokemon = wildPokemon;
            EstOpponentSauvage = true;
        }

        public override void Initialize()//Ouverture du combat. Tout ce qui doit �tre fait avant "Main menu"
        {
            EnCombat = true; //GameState = Battle

            if (!EstOpponentSauvage)
            {
                OpponentPokemon = LancerPok�mon(0, OpponentTrainer); //lance son premier pok�mon
            }
            UserPokemon = LancerPok�mon(0, UserTrainer);//envoie le premier pok�mon de l'inventaire.

            base.Initialize();
        }

        Pokemon LancerPok�mon(int index, Trainer trainer)
        {
            while (!trainer.PokemonsSurLui[index].EstEnVie())
            {
                index++;
            }
            trainer.Throw(index);

            return trainer.PokemonsSurLui[index];
        }


        public override void Update(GameTime gameTime)//mise � jour des tours tant que en vie(both trainer et son pok�mon
        {
            //Ouvrir menu principal d'un combat
            while (UserTrainer.EstEnVie() && OpponentTrainer.EstEnVie())
            {
                while (UserPokemon.EstEnVie() && OpponentPokemon.EstEnVie())
                {
                    //Syst�me de tours ici
                    AfficherMenuAttaques(); //On va commencer par savoir comment choisir une attaque, apr�s on fera un menu pour fight/bag/pokemons/run
                    if (UserPokemon.Speed < OpponentPokemon.Speed)
                    {//Opponent attaquerait en premier
                    }//la fonction attaquer devra prendre en intrant l'index de l'attaque s�lectionn�e et le target.
                    else
                    {//User attaquerait en premier
                    }
                }
                if (!OpponentPokemon.EstEnVie())
                {
                    //Message/animation/whatever, opponent has been defeated!
                    CalculExpPourUser();

                    OpponentPokemon = LancerPok�mon(0, OpponentTrainer); //Throw next pokemon
                }
                else
                {
                    //trainer pokemon fainted! Tu dois choisir un autre pok�mon dans ton inventaire
                    //Ouvrir inventaire, s�lectionner un index
                    int prochainPokemon = S�lectionnerUnPok�monEnInventaire();

                    UserPokemon = LancerPok�mon(prochainPokemon, UserTrainer);
                }
            }
            base.Update(gameTime);
        }
    }
}
