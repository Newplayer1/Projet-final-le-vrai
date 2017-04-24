//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using AtelierXNA.Classes_Pokemon_Skyrim;

//namespace AtelierXNA
//{
//    enum CombatState { INI, BATTLE_MENU, IN_BATTLE, VERIFY_OUTCOME, VICTORY, DEFEAT, END }
//    public class Combat : Microsoft.Xna.Framework.GameComponent, IDestructible
//    {
//        float IntervalMAJ { get; set; }
//        Player UserTrainer { get; set; }
//        Player OpponentTrainer { get; set; }

//        Pokemon UserPokemon { get; set; }
//        Pokemon OpponentPokemon { get; set; }

//        AccessBaseDeDonn�e Database { get; set; }
//        bool EnCombat { get; set; }
//        bool EstOpponentSauvage { get; set; }
//        bool UserPkmPrioritaire { get; set; }
//        bool TourCompl�t� { get; set; }

//        CombatState CombatState { get; set; }
//        Vector2 PositionBox { get; set; }
//        BattleMenu MainMenu { get; set; }
//        public static bool Wait { get; set; }
//        static Combat()
//        {
//            Wait = false;
//        }

//        bool �D�truire;
//        public bool �D�truire
//        {
//            get
//            {
//                return �D�truire;
//            }

//            set
//            {
//                �D�truire = value;
//                MainMenu.�D�truire = value;
//            }
//        }

//        public Combat(Game game, Vector2 positionBox, Trainer user, Trainer opponentTrainer)
//            : base(game)
//        {
//            PositionBox = positionBox;
//            UserTrainer = user;
//            OpponentTrainer = opponentTrainer;
//            EstOpponentSauvage = false;
//        }
//        public Combat(Game game, Vector2 positionBox, Trainer user, Pokemon wildPokemon)
//            : base(game)
//        {
//            PositionBox = positionBox;
//            UserTrainer = user;
//            //OpponentTrainer = null;
//            OpponentPokemon = wildPokemon;
//            EstOpponentSauvage = true;
//        }
//        public Combat(Game game, Vector2 positionBox, Player user, Player opponent, float intervalMAJ)
//            : base(game)
//        {
//            PositionBox = positionBox;
//            IntervalMAJ = intervalMAJ;
//            UserTrainer = user;
//            OpponentTrainer = opponent;
//        }

//        public override void Initialize()//Ouverture du combat. Tout ce qui doit �tre fait avant "Combat Menu"
//        {
//            CombatState = CombatState.INI;//changer la position de la cam�ra icitte
//            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
//            EnCombat = true;
//            //GameState = Combat
//            UserPkmPrioritaire = true;
//            //Cr�er le MainMenu mais le garder invisible?
//            MenuBattle = new BattleMenu(Game, new Vector2(2, 300), new Vector2(32, 6), Atelier.INTERVALLE_STANDARDS);
//            if (!EstOpponentSauvage)
//            {
//                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, (int)Dimensions.X, (int)Dimensions.Y, "temp: User threw an item!", IntervalMAJ);
//                Game.Components.Add(message);
//                OpponentPokemon = LancerPok�mon(0, OpponentTrainer); //lance son premier pok�mon
//            }//si est sauvage, on a d�j� d�cid� du OpponentPokemon dans le constructeur
//            UserPokemon = LancerPok�mon(0, UserTrainer);//envoie le premier pok�mon de l'inventaire.

//            si pas sauvage, message du trainer, ensuite message du pokemon opponent
//            ajouter le pokemon opponent, OpponentPokemon = wildPokemon;
//            OpponentPokemon = OpponentTrainer[0];
//            UserPokemon = UserTrainer[0];
//            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: Wild " + OpponentPokemon.Nom + " appeared!", IntervalMAJ);
//            Game.Components.Add(message);//Message opponent


//            AfficheurTexte messageB = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: Go, " + UserTrainer[0].Nom + "!", IntervalMAJ);
//            Game.Components.Add(messageB);//Message pokemon user


//            MainMenu = new BattleMenu(Game, PositionBox, new Vector2(Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD), IntervalMAJ);
//            Game.Components.Add(MainMenu);

//            MainMenu.BattleMenuState = BattleMenuState.MAIN;
//            CombatState = CombatState.BATTLE_MENU;
//            base.Initialize();
//        }

//        Pokemon LancerPok�mon(int index, Trainer trainer)
//        {
//            while (!trainer.PokemonsSurLui[index].EstEnVie)
//            {
//                index++;
//            }
//            trainer.Throw(index);

//            return trainer.PokemonsSurLui[index];
//        }


//        public override void Update(GameTime gameTime)//mise � jour des tours tant que en vie (both trainer et son pok�mon)
//        {
//            G�rer�tats(); //Passer gameTime si l'on doit animer qqch
//            Wait = AfficheurTexte.MessageEnCours;
//            if (!Wait)
//                G�rerTransitions();
//            base.Update(gameTime);//Utile?
//        }

//        #region G�rer�tats
//        void G�rer�tats() //ici on ex�cute ce que l'on fait � un tel �tat
//        {
//            switch (CombatState)
//            {
//                case CombatState.BATTLE_MENU:
//                    G�rerBATTLE_MENU();
//                    break;
//                case CombatState.IN_BATTLE:
//                    G�rerIN_BATTLE();
//                    break;
//                //case CombatState.VERIFY_OUTCOME:
//                //    G�rerVERIFY_ALIVE();
//                //    break;
//                case CombatState.VICTORY:
//                    G�rerVICTORY();
//                    break;
//                case CombatState.DEFEAT:
//                    G�rerDEFEAT();
//                    break;
//                case CombatState.END:
//                    G�rerEND();
//                    break;
//            }
//        }
//        void G�rerBATTLE_MENU()
//        {
//        }
//        void G�rerIN_BATTLE()
//        {
//            EffectuerTourUser();
//            EffectuerTourOpponent();
//            TourCompl�t� = true; // simple, pour que �a run
//        }
//        void G�rerVICTORY()
//        {
//        }
//        void G�rerDEFEAT()
//        {
//        }
//        void G�rerEND()
//        {
//        }
//        #endregion

//        #region G�rerTransition
//        void G�rerTransitions() //ici on v�rifie la condition qui change l'�tat (et on le change au besoin)
//        {
//            switch (CombatState)
//            {
//                case CombatState.BATTLE_MENU:
//                    G�rerTransitionBATTLE_MENU();
//                    break;
//                case CombatState.IN_BATTLE:
//                    G�rerTransitionIN_BATTLE();
//                    break;
//                case CombatState.VERIFY_OUTCOME:
//                    G�rerTransitionVERIFY_OUTCOME();
//                    break;
//                case CombatState.VICTORY:
//                    G�rerTransitionVICTORY();
//                    break;
//                case CombatState.DEFEAT:
//                    G�rerTransitionDEFEAT();
//                    break;
//                case CombatState.END: //default
//                    G�rerTransitionEND();
//                    break;
//            }
//        }
//        void G�rerTransitionBATTLE_MENU()
//        {
//            if (MainMenu.BattleMenuState == BattleMenuState.READY) //? //has reached READY state
//                CombatState = CombatState.IN_BATTLE;
//        }
//        void G�rerTransitionIN_BATTLE()//MAIN_MENUSTATE.�tatDuMenu == MAIN_MENUSTATE.READY?
//        {
//            if (UserPokemon.EstEnVie)
//                EffectuerTourUser();
//            if (OpponentPokemon.EstEnVie)
//                EffectuerTourOpponent();

//            TourCompl�t� = true; // simple, pour que �a run
//            if (TourCompl�t�)
//            {
//                if (UserPokemon.EstEnVie && OpponentPokemon.EstEnVie) //si les deux sont en vie apr�s s'�tre battus, retour au MainMenu
//                {
//                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
//                    CombatState = CombatState.BATTLE_MENU;
//                }

//                else //si l'un des deux est mort
//                    CombatState = CombatState.VERIFY_OUTCOME;

//                CombatState = CombatState.BATTLE_MENU;
//                MainMenu.BattleMenuState = BattleMenuState.MAIN;

//                on va juste retourner au menu quand le tour est fait pour l'instant
//                CombatState = CombatState.VICTORY;//direct pour l'instant, je veux juste cr�er un combat, faire mon tour pis dire que j'ai gagn�, pour pouvoir run le projet
//            }
//        }
//        void G�rerTransitionVERIFY_OUTCOME()
//        {
//            L'un des deux est mort donc nous sommes arriv� ici. (on doit assurer � 100% qu'on change le state ici parce que sinon on va aller � d�faut, soit END)
//            if (UserPokemon.EstEnVie)
//            {
//                if (EstOpponentSauvage || !(OpponentTrainer.EstEnVie))
//                    CombatState = CombatState.VICTORY;
//            }
//            else//userPokemon est dead
//            {
//                if (!UserTrainer.EstEnVie)
//                    CombatState = CombatState.DEFEAT;
//            }


//        }
//        void G�rerTransitionVICTORY()
//        {
//            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "Amazing, such strength!", IntervalMAJ);
//            Game.Components.Add(message);
//            CombatState = CombatState.END;
//        }
//        void G�rerTransitionDEFEAT()
//        {
//            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "Wow, what a weakling.", IntervalMAJ);
//            Game.Components.Add(message);
//            CombatState = CombatState.END;
//        }
//        void G�rerTransitionEND()
//        {
//            GameState = Jeu3D; ??
//            D�truire le component?
//            �D�truire = true;

//        }
//        #endregion

//        void EffectuerTourUser()
//        {
//            if (MainMenu.AttaqueUtilis�e)
//                EffectuerAttaque(UserPokemon, OpponentPokemon, num�roAttaqueChoisie
//                EffectuerAttaque(MainMenu.Num�roChoisi);
//            else if (MainMenu.ItemUtilis�)
//                UtilierItem(MainMenu.Num�roChoisi);
//            else if (MainMenu.Pok�monChang�)
//                ChangerPok�mon(MainMenu.Num�roChoisi);
//            else if (MainMenu.TentativeFuite)
//                EssayerFuir();
//        }



//        void EffectuerTourOpponent()
//        {
//            choisir une attaque al�atoire
//            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: Wild " + OpponentPokemon.Nom + " used attack 0!", IntervalMAJ);
//            Game.Components.Add(message);//Message opponent
//            EffectuerAttaque(OpponentPokemon, UserPokemon, 0);
//        }

//        void EffectuerAttaque(int num�roChoisi)
//        {
//            string messageTour = UserPokemon.Nom + " used attack " + num�roChoisi.ToString() + "!";
//            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: " + messageTour, IntervalMAJ);
//            Game.Components.Add(message);

//            EffectuerAttaque(UserPokemon, OpponentPokemon, num�roChoisi);
//        }
//        void UtilierItem(int num�roChoisi)
//        {
//            string messageTour = "User used item " + num�roChoisi.ToString() + ".";
//            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: " + messageTour, IntervalMAJ);
//            Game.Components.Add(message);
//            Ensuite on fait l'effet de l'item
//        }
//        void ChangerPok�mon(int num�roChoisi)
//        {
//            string messageTour = "User switched with pokemon " + num�roChoisi.ToString() + ".";
//            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: " + messageTour, IntervalMAJ);
//            Game.Components.Add(message);
//            faire le reste du code de switch pokemon
//        }
//            void EssayerFuir()
//        {
//                random selon des probabilit�s et level des deux pokemons.pour l'instant on doit qu'y r�ussi � tout coup
//               AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: Got away safely!", IntervalMAJ);
//                Game.Components.Add(message);
//                CombatState = CombatState.END;//On met fin au combat
//            }
//            void EffectuerAttaque(Pokemon attaquant, Pokemon opposant, int attaqueChoisie)//Maybe, je sais pas trop, reformuler?

//            {
//                opposant.D�fendre(CalculerPointsDamage(attaqueChoisie, attaquant, opposant));
//            }

//        private int CalculerPointsDamage(int attaqueChoisie, Pokemon attaquant, Pokemon opposant)
//        {
//            return ((2 * attaquant.Level / 5 + 2) * /*atk.Power*/20 * (attaquant.Attack / opposant.Defense) / 50 + 2) * /*multiplicateurType*/1;
//        }
//    }
//}
