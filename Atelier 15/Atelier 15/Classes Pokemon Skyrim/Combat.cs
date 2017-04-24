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

//        AccessBaseDeDonnée Database { get; set; }
//        bool EnCombat { get; set; }
//        bool EstOpponentSauvage { get; set; }
//        bool UserPkmPrioritaire { get; set; }
//        bool TourComplété { get; set; }

//        CombatState CombatState { get; set; }
//        Vector2 PositionBox { get; set; }
//        BattleMenu MainMenu { get; set; }
//        public static bool Wait { get; set; }
//        static Combat()
//        {
//            Wait = false;
//        }

//        bool àDétruire;
//        public bool ÀDétruire
//        {
//            get
//            {
//                return àDétruire;
//            }

//            set
//            {
//                àDétruire = value;
//                MainMenu.ÀDétruire = value;
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

//        public override void Initialize()//Ouverture du combat. Tout ce qui doit être fait avant "Combat Menu"
//        {
//            CombatState = CombatState.INI;//changer la position de la caméra icitte
//            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
//            EnCombat = true;
//            //GameState = Combat
//            UserPkmPrioritaire = true;
//            //Créer le MainMenu mais le garder invisible?
//            MenuBattle = new BattleMenu(Game, new Vector2(2, 300), new Vector2(32, 6), Atelier.INTERVALLE_STANDARDS);
//            if (!EstOpponentSauvage)
//            {
//                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, (int)Dimensions.X, (int)Dimensions.Y, "temp: User threw an item!", IntervalMAJ);
//                Game.Components.Add(message);
//                OpponentPokemon = LancerPokémon(0, OpponentTrainer); //lance son premier pokémon
//            }//si est sauvage, on a déjà décidé du OpponentPokemon dans le constructeur
//            UserPokemon = LancerPokémon(0, UserTrainer);//envoie le premier pokémon de l'inventaire.

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

//        Pokemon LancerPokémon(int index, Trainer trainer)
//        {
//            while (!trainer.PokemonsSurLui[index].EstEnVie)
//            {
//                index++;
//            }
//            trainer.Throw(index);

//            return trainer.PokemonsSurLui[index];
//        }


//        public override void Update(GameTime gameTime)//mise à jour des tours tant que en vie (both trainer et son pokémon)
//        {
//            GérerÉtats(); //Passer gameTime si l'on doit animer qqch
//            Wait = AfficheurTexte.MessageEnCours;
//            if (!Wait)
//                GérerTransitions();
//            base.Update(gameTime);//Utile?
//        }

//        #region GérerÉtats
//        void GérerÉtats() //ici on exécute ce que l'on fait à un tel état
//        {
//            switch (CombatState)
//            {
//                case CombatState.BATTLE_MENU:
//                    GérerBATTLE_MENU();
//                    break;
//                case CombatState.IN_BATTLE:
//                    GérerIN_BATTLE();
//                    break;
//                //case CombatState.VERIFY_OUTCOME:
//                //    GérerVERIFY_ALIVE();
//                //    break;
//                case CombatState.VICTORY:
//                    GérerVICTORY();
//                    break;
//                case CombatState.DEFEAT:
//                    GérerDEFEAT();
//                    break;
//                case CombatState.END:
//                    GérerEND();
//                    break;
//            }
//        }
//        void GérerBATTLE_MENU()
//        {
//        }
//        void GérerIN_BATTLE()
//        {
//            EffectuerTourUser();
//            EffectuerTourOpponent();
//            TourComplété = true; // simple, pour que ça run
//        }
//        void GérerVICTORY()
//        {
//        }
//        void GérerDEFEAT()
//        {
//        }
//        void GérerEND()
//        {
//        }
//        #endregion

//        #region GérerTransition
//        void GérerTransitions() //ici on vérifie la condition qui change l'état (et on le change au besoin)
//        {
//            switch (CombatState)
//            {
//                case CombatState.BATTLE_MENU:
//                    GérerTransitionBATTLE_MENU();
//                    break;
//                case CombatState.IN_BATTLE:
//                    GérerTransitionIN_BATTLE();
//                    break;
//                case CombatState.VERIFY_OUTCOME:
//                    GérerTransitionVERIFY_OUTCOME();
//                    break;
//                case CombatState.VICTORY:
//                    GérerTransitionVICTORY();
//                    break;
//                case CombatState.DEFEAT:
//                    GérerTransitionDEFEAT();
//                    break;
//                case CombatState.END: //default
//                    GérerTransitionEND();
//                    break;
//            }
//        }
//        void GérerTransitionBATTLE_MENU()
//        {
//            if (MainMenu.BattleMenuState == BattleMenuState.READY) //? //has reached READY state
//                CombatState = CombatState.IN_BATTLE;
//        }
//        void GérerTransitionIN_BATTLE()//MAIN_MENUSTATE.ÉtatDuMenu == MAIN_MENUSTATE.READY?
//        {
//            if (UserPokemon.EstEnVie)
//                EffectuerTourUser();
//            if (OpponentPokemon.EstEnVie)
//                EffectuerTourOpponent();

//            TourComplété = true; // simple, pour que ça run
//            if (TourComplété)
//            {
//                if (UserPokemon.EstEnVie && OpponentPokemon.EstEnVie) //si les deux sont en vie après s'être battus, retour au MainMenu
//                {
//                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
//                    CombatState = CombatState.BATTLE_MENU;
//                }

//                else //si l'un des deux est mort
//                    CombatState = CombatState.VERIFY_OUTCOME;

//                CombatState = CombatState.BATTLE_MENU;
//                MainMenu.BattleMenuState = BattleMenuState.MAIN;

//                on va juste retourner au menu quand le tour est fait pour l'instant
//                CombatState = CombatState.VICTORY;//direct pour l'instant, je veux juste créer un combat, faire mon tour pis dire que j'ai gagné, pour pouvoir run le projet
//            }
//        }
//        void GérerTransitionVERIFY_OUTCOME()
//        {
//            L'un des deux est mort donc nous sommes arrivé ici. (on doit assurer à 100% qu'on change le state ici parce que sinon on va aller à défaut, soit END)
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
//        void GérerTransitionVICTORY()
//        {
//            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "Amazing, such strength!", IntervalMAJ);
//            Game.Components.Add(message);
//            CombatState = CombatState.END;
//        }
//        void GérerTransitionDEFEAT()
//        {
//            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "Wow, what a weakling.", IntervalMAJ);
//            Game.Components.Add(message);
//            CombatState = CombatState.END;
//        }
//        void GérerTransitionEND()
//        {
//            GameState = Jeu3D; ??
//            Détruire le component?
//            ÀDétruire = true;

//        }
//        #endregion

//        void EffectuerTourUser()
//        {
//            if (MainMenu.AttaqueUtilisée)
//                EffectuerAttaque(UserPokemon, OpponentPokemon, numéroAttaqueChoisie
//                EffectuerAttaque(MainMenu.NuméroChoisi);
//            else if (MainMenu.ItemUtilisé)
//                UtilierItem(MainMenu.NuméroChoisi);
//            else if (MainMenu.PokémonChangé)
//                ChangerPokémon(MainMenu.NuméroChoisi);
//            else if (MainMenu.TentativeFuite)
//                EssayerFuir();
//        }



//        void EffectuerTourOpponent()
//        {
//            choisir une attaque aléatoire
//            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: Wild " + OpponentPokemon.Nom + " used attack 0!", IntervalMAJ);
//            Game.Components.Add(message);//Message opponent
//            EffectuerAttaque(OpponentPokemon, UserPokemon, 0);
//        }

//        void EffectuerAttaque(int numéroChoisi)
//        {
//            string messageTour = UserPokemon.Nom + " used attack " + numéroChoisi.ToString() + "!";
//            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: " + messageTour, IntervalMAJ);
//            Game.Components.Add(message);

//            EffectuerAttaque(UserPokemon, OpponentPokemon, numéroChoisi);
//        }
//        void UtilierItem(int numéroChoisi)
//        {
//            string messageTour = "User used item " + numéroChoisi.ToString() + ".";
//            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: " + messageTour, IntervalMAJ);
//            Game.Components.Add(message);
//            Ensuite on fait l'effet de l'item
//        }
//        void ChangerPokémon(int numéroChoisi)
//        {
//            string messageTour = "User switched with pokemon " + numéroChoisi.ToString() + ".";
//            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: " + messageTour, IntervalMAJ);
//            Game.Components.Add(message);
//            faire le reste du code de switch pokemon
//        }
//            void EssayerFuir()
//        {
//                random selon des probabilités et level des deux pokemons.pour l'instant on doit qu'y réussi à tout coup
//               AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Atelier.LARGEUR_BOX_STANDARD, Atelier.HAUTEUR_BOX_STANDARD, "temp: Got away safely!", IntervalMAJ);
//                Game.Components.Add(message);
//                CombatState = CombatState.END;//On met fin au combat
//            }
//            void EffectuerAttaque(Pokemon attaquant, Pokemon opposant, int attaqueChoisie)//Maybe, je sais pas trop, reformuler?

//            {
//                opposant.Défendre(CalculerPointsDamage(attaqueChoisie, attaquant, opposant));
//            }

//        private int CalculerPointsDamage(int attaqueChoisie, Pokemon attaquant, Pokemon opposant)
//        {
//            return ((2 * attaquant.Level / 5 + 2) * /*atk.Power*/20 * (attaquant.Attack / opposant.Defense) / 50 + 2) * /*multiplicateurType*/1;
//        }
//    }
//}
