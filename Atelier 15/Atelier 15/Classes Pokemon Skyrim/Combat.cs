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

    public class Combat : Microsoft.Xna.Framework.GameComponent
    {
        Trainer UserTrainer { get; set; }
        Trainer OpponentTrainer { get; set; }

        Pokemon UserPokemon { get; set; }
        Pokemon OpponentPokemon { get; set; }
        
        AccessBaseDeDonn�e Database { get; set; }
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

        public override void Initialize()//Ouverture du combat. Tout ce qui doit �tre fait avant "Combat Menu"
        {
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
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
            while (!trainer.PokemonsSurLui[index].EstEnVie)
            {
                index++;
            }
            trainer.Throw(index);

            return trainer.PokemonsSurLui[index];
        }


        public override void Update(GameTime gameTime)//mise � jour des tours tant que en vie (both trainer et son pok�mon)
        {
            bool UserPkmPrioritaire;
            

            while (UserTrainer.EstEnVie() && OpponentTrainer.EstEnVie())
            {
                //UserPkmPrioritaire = true;
                while (UserPokemon.EstEnVie && OpponentPokemon.EstEnVie)
                {
                    //Syst�me de tours entre pkmns ici
                    AfficherMenuAttaques(); //On va commencer par savoir comment choisir une attaque, apr�s on fera un menu pour fight/bag/pokemons/run

                    if (UserPokemon.Speed < OpponentPokemon.Speed)//le tour du joueur sauf si prouv� du contraire
                        UserPkmPrioritaire = false;
                    else
                        UserPkmPrioritaire = true;
                    //Important de garder dans la boucle, si la vitesse est chang�e par une attaque effect, et que l'adversaire ou le user devient plus rapide, il a droit de frapper deux fois


                    
                    if (UserPkmPrioritaire)  //Si le pok�mon User est plus rapide
                    {
                        OpponentPokemon.D�fendre(CalculPointsDamage(UserPokemon, OpponentPokemon, num�roAttaqueChoisie));//fonctions temporaires, � modifier pour calculer les points de dommages avec la formule
                        UserPokemon.D�fendre(CalculPointsDamage(OpponentPokemon, UserPokemon, OpponentPokemon.AttaqueAl�atoire()));
                        UserPkmPrioritaire = false;//fin de son tour
                    }
                    else   //Si le pok�mon adverse est le plus rapide
                    {
                        UserPokemon.D�fendre(CalculPointsDamage(OpponentPokemon, UserPokemon, OpponentPokemon.AttaqueAl�atoire())); // c'est le tour du pok�mon adverse
                        OpponentPokemon.D�fendre(CalculPointsDamage(UserPokemon, OpponentPokemon, num�roAttaqueChoisie));
                        UserPkmPrioritaire = true;
                    }

                }
                if (!OpponentPokemon.EstEnVie)//sorti de la boucle de combat: l'un des deux est mort
                {
                    //Message/animation/whatever, opponent has been defeated!
                    UserPokemon.GainExp(OpponentPokemon.Level * 10);

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

            //Trainer has been defeated! ou Opponent has been defeated!
            if (UserTrainer.EstEnVie())
            {//le User a gagn�
                //Changement de la toune?
                //Gagne du cash, ptit message genre "wow le ptit con m'a battu holyshit man"
            }
            else
            {//Le User a perdu
                //Donne du cash, ptit message genre "wow t'es ben faible ptit con"
                //User wrap au pok�mon center (une m�thode de t�l�portation dans le trainer pour le d�placer perhaps?)
            }

            //Le combat est fini
            EnCombat = false;//Ou on pourrait changer le gamestate?
            base.Update(gameTime);//Utile?
        }

        int CalculPointsDamage(Pokemon attaquant, Pokemon opposant, int attaqueChoisie)// s'il y a un damage
        {
            float damage;
            //devra changer selon le type de l'attaque et les deux types de l'opponent
            Attaque Atk = new Attaque(attaqueChoisie);
            float multiplicateurType = Atk.GetTypeMultiplier(opposant.Type1, opposant.Type2);

            damage = ((2 * attaquant.Level / 5 + 2) * Atk.Power * (attaquant.Attack / opposant.Defense) / 50 + 2) * multiplicateurType;
            return (int)damage;
        }
        void AppliquerEffet()
        {

        }



        /*
         * 
                //Fonction access direct
        public int AttaquesStatsPower(int attaqueNumber)
        {
            return int.Parse(AccessDonn�esAttaqueStats(attaqueNumber)[2]);
        }
        */
        /*
         Damage = ( (2*UserPokemon/5 + 2) * PowerDeLAttaque * (UserAttack/OpponentDefense))/50 + 2) * type
	        Level : Level de UserPokemon
	        PowerDeLAttaque : La force de l'attaque (genre de 10 � 120)
	        UserAttack : le stat d'attaque (ou super attack) du user
	        OpponentDefense: le stat de d�fense (ou de super defense) de l'opponent
	        type : 0 (ineffective); 0.25, 0.5 (not very effective); 1 (normally effective); 2 or 4 (very effective)
		        Selon la ligne du type de l'attaque, va chercher le type de l'opponent sur la ligne. (dual-type: multiplie les facteurs)

         */
    }
}
