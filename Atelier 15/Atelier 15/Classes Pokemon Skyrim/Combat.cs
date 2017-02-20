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

        public override void Initialize()//Ouverture du combat. Tout ce qui doit être fait avant "Main menu"
        {
            EnCombat = true; //GameState = Battle

            if (!EstOpponentSauvage)
            {
                OpponentPokemon = LancerPokémon(0, OpponentTrainer); //lance son premier pokémon
            }
            UserPokemon = LancerPokémon(0, UserTrainer);//envoie le premier pokémon de l'inventaire.


            base.Initialize();
        }

        Pokemon LancerPokémon(int index, Trainer trainer)
        {
            while (!trainer.PokemonsSurLui[index].EstEnVie())
            {
                index++;
            }
            trainer.Throw(index);

            return trainer.PokemonsSurLui[index];
        }


        public override void Update(GameTime gameTime)//mise à jour des tours tant que en vie(both trainer et son pokémon
        {
            //Ouvrir menu principal d'un combat
            while (UserTrainer.EstEnVie() && OpponentTrainer.EstEnVie())
            {
                while (UserPokemon.EstEnVie() && OpponentPokemon.EstEnVie())
                {
                    //Système de tours ici
                    AfficherMenuAttaques(); //On va commencer par savoir comment choisir une attaque, après on fera un menu pour fight/bag/pokemons/run
                    if (UserPokemon.Speed > OpponentPokemon.Speed)
                    {
                        OpponentPokemon.Défendre(UserPokemon.Attaquer());// c'est le tour du pokémon User

                        UserPokemon.ChangerSpeed(0);
                        OpponentPokemon.ChangerSpeed(1);
                    }
                    else
                    {
                        UserPokemon.Défendre(OpponentPokemon.AttaqueAléatoire()); // c'est le tour du pokémon adverse

                        UserPokemon.ChangerSpeed(1);
                        OpponentPokemon.ChangerSpeed(0);
                    }
                }
                if (!OpponentPokemon.EstEnVie())//sorti de la boucle de combat: l'un des deux est mort
                {
                    //Message/animation/whatever, opponent has been defeated!
                    UserPokemon.GainExp(OpponentPokemon.Level * 10);

                    OpponentPokemon = LancerPokémon(0, OpponentTrainer); //Throw next pokemon
                }
                else
                {
                    //trainer pokemon fainted! Tu dois choisir un autre pokémon dans ton inventaire
                    //Ouvrir inventaire, sélectionner un index
                    int prochainPokemon = SélectionnerUnPokémonEnInventaire();

                    UserPokemon = LancerPokémon(prochainPokemon, UserTrainer);
                }

            }

            //Trainer has been defeated! ou Opponent has been defeated!
            if (UserTrainer.EstEnVie())
            {//le User a gagné
                //Changement de la toune?
                //Gagne du cash, ptit message genre "wow le ptit con m'a battu holyshit man"
            }
            else
            {//Le User a perdu
                //Donne du cash, ptit message genre "wow t'es ben faible ptit con"
                //User wrap au pokémon center (une méthode de téléportation dans le trainer pour le déplacer perhaps?)
            }

            //Le combat est fini
            EnCombat = false;
            base.Update(gameTime);
        }
    }
}
