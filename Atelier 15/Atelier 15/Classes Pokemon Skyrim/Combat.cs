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
    enum CombatState { INI, BATTLE_MENU, IN_BATTLE, TOUR_USER, TOUR_OPPONENT, VERIFY_OUTCOME, VICTORY, DEFEAT, END }
    public class Combat : Microsoft.Xna.Framework.GameComponent, IDestructible
    {
        float IntervalMAJ { get; set; }
        Trainer UserTrainer { get; set; }
        Trainer OpponentTrainer { get; set; }

        Pokemon UserPokemon { get; set; } //Le BattleMenu doit avoir acc�s aux attaques du pok�mon... Garder en m�moire l'indice du pkm en jeu?
        public int NoPok�dexUserPokemon => UserPokemon.PokedexNumber;//pour afficher un mod�le de son num�ro
        Pokemon OpponentPokemon { get; set; }
        public int NoPok�dexOpponentPokemon => UserPokemon.PokedexNumber;
        AccessBaseDeDonn�e Database { get; set; }
        //AccessBaseDeDonn�e Database { get; set; }
        //bool EnCombat { get; set; }
        public bool EstOpponentSauvage { get; private set; }
        bool UserPkmPrioritaire { get; set; }
        bool TourCompl�t� => TourUserCompl�t� && TourOpponentCompl�t�;/*{ get; set; }*/
        bool TourUserCompl�t� { get; set; }
        bool TourOpponentCompl�t� { get; set; }

        CombatState CombatState { get; set; }
        Vector2 PositionBox { get; set; }
        BattleMenu MainMenu { get; set; }

        TexteFixe NomUserPokemon { get; set; }
        TexteFixe NomOpponentPokemon { get; set; }
        Vector2 PositionInfoUserPokemon { get; set; }
        TexteFixe VieUserPokemon { get; set; }
        TexteFixe VieOpponentPokemon { get; set; }
        Vector2 PositionInfoOpponentPokemon { get; set; }

        Random G�n�rateur { get; set; }

        public static bool Wait { get; set; }

        public static bool EnCombat { get; set; }
        static Combat()
        {
            Wait = false;
            EnCombat = false;
        }

        bool �D�truire;
        public bool �D�truire
        {
            get
            {
                return �D�truire;
            }

            set
            {
                �D�truire = value;
                MainMenu.�D�truire = value;
                NomOpponentPokemon.�D�truire = value;
                NomUserPokemon.�D�truire = value;
                VieOpponentPokemon.�D�truire = value;
                VieUserPokemon.�D�truire = value;
            }
        }

        public int LargeurBox { get; private set; }

        //public Combat(Game game, Vector2 positionBox, Trainer user, Trainer opponentTrainer)
        //    : base(game)
        //{
        //    PositionBox = positionBox;
        //    UserTrainer = user;
        //    OpponentTrainer = opponentTrainer;
        //    EstOpponentSauvage = false;
        //}
        //public Combat(Game game, Vector2 positionBox, Trainer user, Pokemon wildPokemon)
        //    : base(game)
        //{
        //    PositionBox = positionBox;
        //    UserTrainer = user;
        //    //OpponentTrainer = null;
        //    OpponentPokemon = wildPokemon;
        //    EstOpponentSauvage = true;
        //}
        public Combat(Game game, Vector2 positionBox, Player user, Trainer opponent, float intervalMAJ)
            : base(game)
        {
            PositionBox = positionBox;
            IntervalMAJ = intervalMAJ;
            UserTrainer = user;
            OpponentTrainer = opponent;
            EstOpponentSauvage = false;
        }
        public Combat(Game game, Vector2 positionBox, Player user, Pokemon opponent, float intervalMAJ)
            : base(game)
        {
            PositionBox = positionBox;
            IntervalMAJ = intervalMAJ;
            UserTrainer = user;
            OpponentPokemon = opponent;
            EstOpponentSauvage = true;
        }

        public override void Initialize()//Ouverture du combat. Tout ce qui doit �tre fait avant "Combat Menu"
        {
            EnCombat = true;
            G�n�rateur = new Random();
            LargeurBox = Game.Window.ClientBounds.Width / Cadre.TAILLE_TILE;
            UserPokemon = UserTrainer.NextPokemonEnVie();

            PositionInfoUserPokemon = new Vector2(Game.Window.ClientBounds.Width - (UserPokemon.ToString().Count() + 3) * Cadre.TAILLE_TILE, Game.Window.ClientBounds.Height - Cadre.TAILLE_TILE * 9);
            PositionInfoOpponentPokemon = new Vector2(Cadre.TAILLE_TILE, Game.Window.ClientBounds.Height/10);
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            CombatState = CombatState.INI;//changer la position de la cam�ra icitte
                                          //Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
                                          //EnCombat = true;
                                          ////GameState = Combat
                                          //UserPkmPrioritaire = true;
                                          ////Cr�er le MainMenu mais le garder invisible?
                                          //MenuBattle = new BattleMenu(Game, new Vector2(2, 300), new Vector2(32, 6), Atelier.INTERVALLE_STANDARDS);
                                          //if (!EstOpponentSauvage)
                                          //{
                                          //    AfficheurTexte message = new AfficheurTexte(Game, PositionBox, (int)Dimensions.X, (int)Dimensions.Y, "User threw an item!", IntervalMAJ);
                                          //    Game.Components.Add(message);
                                          //    OpponentPokemon = LancerPok�mon(0, OpponentTrainer); //lance son premier pok�mon
                                          //}//si est sauvage, on a d�j� d�cid� du OpponentPokemon dans le constructeur
                                          //UserPokemon = LancerPok�mon(0, UserTrainer);//envoie le premier pok�mon de l'inventaire.

            //si pas sauvage, message du trainer, ensuite message du pokemon opponent
            //ajouter le pokemon opponent, OpponentPokemon = wildPokemon;

            if (EstOpponentSauvage)
            {
                AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Wild " + OpponentPokemon.Nom + " appeared!", IntervalMAJ);
                Game.Components.Add(message);//Message opponent
            }
            else
            {
                OpponentPokemon = OpponentTrainer[0];
                AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Trainer "+ OpponentTrainer.Nom + " wants to battle! " + "Trainer " + OpponentTrainer.Nom + " send out "+ OpponentPokemon.Nom + "!", IntervalMAJ);
                Game.Components.Add(message);//Message opponent
            }
            

            AfficheurTexte messageB = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, UserTrainer.Nom + ": Go, " + UserTrainer[0].Nom + "!", IntervalMAJ);
            Game.Components.Add(messageB);//Message pokemon user


            AjouterLesTextesFixes();
            
            MainMenu = new BattleMenu(Game, PositionBox, new Vector2(LargeurBox, Cadre.HAUTEUR_BOX_STANDARD),IntervalMAJ, UserPokemon, UserTrainer);
            Game.Components.Add(MainMenu);

            MainMenu.BattleMenuState = BattleMenuState.MAIN;
            CombatState = CombatState.BATTLE_MENU;
            base.Initialize();
        }

        private void AjouterLesTextesFixes()
        {
            NomOpponentPokemon = new TexteFixe(Game, PositionInfoOpponentPokemon, OpponentPokemon.ToString());
            Game.Components.Add(NomOpponentPokemon);

            NomUserPokemon = new TexteFixe(Game, PositionInfoUserPokemon, UserPokemon.ToString());
            Game.Components.Add(NomUserPokemon);

            VieOpponentPokemon = new TexteFixe(Game, new Vector2(PositionInfoOpponentPokemon.X, PositionInfoOpponentPokemon.Y + Cadre.TAILLE_TILE), OpponentPokemon.VieToString());
            Game.Components.Add(VieOpponentPokemon);

            VieUserPokemon = new TexteFixe(Game, new Vector2(PositionInfoUserPokemon.X, PositionInfoUserPokemon.Y + Cadre.TAILLE_TILE), UserPokemon.VieToString());
            Game.Components.Add(VieUserPokemon);
            

            NomOpponentPokemon.Visible = false;
            NomUserPokemon.Visible = false;
            VieOpponentPokemon.Visible = false;
            VieUserPokemon.Visible = false;
        }

        public override void Update(GameTime gameTime)//mise � jour des tours tant que en vie (both trainer et son pok�mon)
        {
            //G�rer�tats(); //Passer gameTime si l'on doit animer qqch
            Wait = AfficheurTexte.MessageEnCours;
            if (!Wait)
                G�rerTransitions();//si y a pas de message en cours on peut proc�der

            base.Update(gameTime);//Utile?
        }

        //#region G�rer�tats
        //void G�rer�tats() //ici on ex�cute ce que l'on fait � un tel �tat
        //{
        //    switch (CombatState)
        //    {
        //        case CombatState.BATTLE_MENU:
        //            G�rerBATTLE_MENU();
        //            break;
        //        case CombatState.IN_BATTLE:
        //            G�rerIN_BATTLE();
        //            break;
        //        //case CombatState.VERIFY_OUTCOME:
        //        //    G�rerVERIFY_ALIVE();
        //        //    break;
        //        case CombatState.VICTORY:
        //            G�rerVICTORY();
        //            break;
        //        case CombatState.DEFEAT:
        //            G�rerDEFEAT();
        //            break;
        //        case CombatState.END:
        //            G�rerEND();
        //            break;
        //    }
        //}
        //void G�rerBATTLE_MENU()
        //{
        //}
        //void G�rerIN_BATTLE()
        //{
        //    EffectuerTourUser();
        //    EffectuerTourOpponent();
        //    TourCompl�t� = true; // simple, pour que �a run
        //}
        //void G�rerVICTORY()
        //{
        //}
        //void G�rerDEFEAT()
        //{
        //}
        //void G�rerEND()
        //{
        //}
        //#endregion

        #region G�rerTransition
        void G�rerTransitions() //ici on v�rifie la condition qui change l'�tat (et on le change au besoin)
        {
            switch (CombatState)
            {
                case CombatState.BATTLE_MENU:
                    G�rerTransitionBATTLE_MENU();
                    break;
                case CombatState.IN_BATTLE:
                    G�rerTransitionIN_BATTLE();
                    break;
                case CombatState.TOUR_USER:
                    G�rerTransitionTOUR_USER();
                    break;
                case CombatState.TOUR_OPPONENT:
                    G�rerTransitionTOUR_OPPONENT();
                    break;
                case CombatState.VERIFY_OUTCOME:
                    G�rerTransitionVERIFY_OUTCOME();
                    break;
                case CombatState.VICTORY:
                    G�rerTransitionVICTORY();
                    break;
                case CombatState.DEFEAT:
                    G�rerTransitionDEFEAT();
                    break;
                case CombatState.END: //default?
                    G�rerTransitionEND();
                    break;
            }
        }
        void G�rerTransitionBATTLE_MENU()
        {
            NomOpponentPokemon.Visible = true;
            NomUserPokemon.Visible = true;
            VieOpponentPokemon.Visible = true;
            VieUserPokemon.Visible = true;

            if (MainMenu.BattleMenuState == BattleMenuState.READY) //has reached READY state
                CombatState = CombatState.IN_BATTLE;
            if (MainMenu.TentativeFuite && !EstOpponentSauvage)//possible de gosser ici pour pas faire manquer de tour si c'est un trainer
            {
                EssayerFuir();
            }
        }
        void G�rerTransitionIN_BATTLE()
        {
            if (UserPokemon.Speed >= OpponentPokemon.Speed)
            {
                if (!TourOpponentCompl�t� && OpponentPokemon.EstEnVie) //�a attaque en sens inverse, user avant opponent. Tr�s contre-intuitif, comment �a se fait?
                    CombatState = CombatState.TOUR_OPPONENT;//EffectuerTourOpponent();

                if (!OpponentPokemon.EstEnVie || !UserPokemon.EstEnVie)//Parce que le combat pourrait finir entre les attaques des deux pok�mons
                    CombatState = CombatState.VERIFY_OUTCOME;

                if (!TourUserCompl�t� && UserPokemon.EstEnVie) 
                    CombatState = CombatState.TOUR_USER;//EffectuerTourUser();
            }
            else
            {
                if (!TourUserCompl�t� && UserPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_USER;

                if (!OpponentPokemon.EstEnVie || !UserPokemon.EstEnVie)//Parce que le combat pourrait finir entre les attaques des deux pok�mons
                    CombatState = CombatState.VERIFY_OUTCOME;

                if (!TourOpponentCompl�t� && OpponentPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_OPPONENT;//EffectuerTourOpponent();
            }

            if (TourOpponentCompl�t� && TourUserCompl�t�)
            {
                TourUserCompl�t� = false;
                TourOpponentCompl�t� = false;
                if (UserPokemon.EstEnVie && OpponentPokemon.EstEnVie) //si les deux sont en vie apr�s s'�tre battus, retour au MainMenu
                {
                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
                    CombatState = CombatState.BATTLE_MENU;
                }

                else //si l'un des deux est mort
                    CombatState = CombatState.VERIFY_OUTCOME;
            }
        }

        void G�rerTransitionTOUR_USER()
        {
            if (UserPokemon.EstEnVie)
            {
                EffectuerTourUser();
                VieOpponentPokemon.RemplacerMessage(OpponentPokemon.VieToString());
                TourUserCompl�t� = true;
                if (CombatState != CombatState.END)
                    CombatState = CombatState.IN_BATTLE;
            }
            else
            {
                if (MainMenu.Pok�monChang�)
                {
                    TourOpponentCompl�t� = false;
                    TourUserCompl�t� = false;
                    ChangerPok�mon(MainMenu.Num�roChoisi);
                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
                    CombatState = CombatState.BATTLE_MENU;
                }
            }
        }
        void G�rerTransitionTOUR_OPPONENT()
        {
            EffectuerTourOpponent();
            VieUserPokemon.RemplacerMessage(UserPokemon.VieToString());
            TourOpponentCompl�t� = true;
            CombatState = CombatState.IN_BATTLE;
        }
        void G�rerTransitionVERIFY_OUTCOME()
        {
            //L'un des deux est mort donc nous sommes arriv� ici. (on doit assurer � 100% qu'on change le state ici parce que sinon on va aller � d�faut, soit END)
            if (UserPokemon.EstEnVie)
            {
                if (EstOpponentSauvage || !(OpponentTrainer.EstEnVie))
                    CombatState = CombatState.VICTORY;
                else
                {
                    DonnerExp();
                    ChangerOpponentPokemon();
                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
                    CombatState = CombatState.BATTLE_MENU;
                }
                    
            }
            else//userPokemon est dead
            {
                if (!UserTrainer.EstEnVie)
                    CombatState = CombatState.DEFEAT;
                else
                {
                    NomUserPokemon.Visible = false;
                    VieUserPokemon.Visible = false;
                    AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, UserPokemon.Nom + " fainted!", IntervalMAJ);
                    Game.Components.Add(message);
                    MainMenu.BackLock = true; //pour forcer � switch de pokemon
                    MainMenu.BattleMenuState = BattleMenuState.POKEMON;
                    CombatState = CombatState.TOUR_USER;
                }
            }
        }


        void G�rerTransitionVICTORY()
        {

            if (!EstOpponentSauvage)
            {
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Amazing, such strength!", IntervalMAJ);
                Game.Components.Add(messageA);
            }
            else
            {
                AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Wild " + OpponentPokemon.Nom + " fainted!", IntervalMAJ);
                Game.Components.Add(messageB);
            }
            DonnerExp();
            CombatState = CombatState.END;
        }

        private void DonnerExp()
        {
            int exp = OpponentPokemon.GiveExp();
            UserPokemon.GainExp(exp);
            AfficheurTexte messageC = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, UserPokemon.Nom + " gained " + exp.ToString() + " exp.", IntervalMAJ);
            Game.Components.Add(messageC);
        }

        void G�rerTransitionDEFEAT()
        {
            if (!EstOpponentSauvage)
            {
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Wow, what a weakling.", IntervalMAJ);
                Game.Components.Add(messageA);
            }

            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Player is out of pokemon. Go to the Pokemon Center.", IntervalMAJ);
            Game.Components.Add(messageB);
            CombatState = CombatState.END;
        }
        void G�rerTransitionEND()
        {
            //GameState = Jeu3D; ??
            //D�truire le component?
            
            EnCombat = false;
            �D�truire = true;

        }
        #endregion

        void EffectuerTourUser()
        {
            if (MainMenu.AttaqueUtilis�e)
                //EffectuerAttaque(UserPokemon, OpponentPokemon, num�roAttaqueChoisie
                EffectuerAttaque(MainMenu.Num�roChoisi);
            else if (MainMenu.ItemUtilis�)
                UtilierItem(MainMenu.Num�roChoisi);
            else if (MainMenu.Pok�monChang�)
                ChangerPok�mon(MainMenu.Num�roChoisi);
            else if (MainMenu.TentativeFuite)
                EssayerFuir();
        }



        void EffectuerTourOpponent()
        {
            //choisir une attaque al�atoire
            int nbAl�atoire = G�n�rateur.Next(0, OpponentPokemon.NbAttaques);
            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Wild "+OpponentPokemon.Nom+" used " + OpponentPokemon[nbAl�atoire].ToString() + "!", IntervalMAJ);
            Game.Components.Add(message);//Message opponent
            EffectuerAttaque(OpponentPokemon, UserPokemon, OpponentPokemon[nbAl�atoire]);
            
        }

        void EffectuerAttaque(int num�roChoisi)
        {
            
            string messageTour = UserPokemon.Nom +" used " + UserPokemon[num�roChoisi].ToString() + "!";
            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, messageTour, IntervalMAJ);
            Game.Components.Add(message);
            EffectuerAttaque(UserPokemon, OpponentPokemon, UserPokemon[num�roChoisi]);
            
        }
        void UtilierItem(int num�roChoisi)
        {
            string messageTour = "User used item " + num�roChoisi.ToString() + ".";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, messageTour, IntervalMAJ);
            Game.Components.Add(message);
            //Ensuite on fait l'effet de l'item
        }
        void ChangerPok�mon(int num�roChoisi)
        {
            UserPokemon = UserTrainer[num�roChoisi];//ligne de code qui switch de pok�mon
            NomUserPokemon.RemplacerMessage(UserPokemon.ToString());
            VieUserPokemon.RemplacerMessage(UserPokemon.VieToString());

            string messageTour = UserTrainer.Nom + " send out " + UserPokemon.Nom + "!";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, messageTour, IntervalMAJ);
            Game.Components.Add(message);

            //faire le reste du code de switch pokemon
        }

        void ChangerOpponentPokemon()
        {
            TourOpponentCompl�t� = false;
            TourUserCompl�t� = false;
            AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, OpponentTrainer.Nom + "'s " + OpponentPokemon.Nom + " fainted!", IntervalMAJ);
            Game.Components.Add(messageA);

            OpponentPokemon = OpponentTrainer.NextPokemonEnVie();//ligne de code qui switch de pok�mon
            NomOpponentPokemon.RemplacerMessage(OpponentPokemon.ToString());
            VieOpponentPokemon.RemplacerMessage(OpponentPokemon.VieToString());

            string messageTour = OpponentTrainer.Nom + " send out " + OpponentPokemon.Nom + "!";
            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, messageTour, IntervalMAJ);
            Game.Components.Add(messageB);
        }
        void EssayerFuir()
        {
            MainMenu.TentativeFuite = false;
            //random selon des probabilit�s et level des deux pokemons. pour l'instant on doit qu'y r�ussi � tout coup
            if (!EstOpponentSauvage)
            {
                TourUserCompl�t� = true;
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "There's no running from a trainer battle!", IntervalMAJ);
                Game.Components.Add(message);
                MainMenu.BattleMenuState = BattleMenuState.MAIN;
                CombatState = CombatState.BATTLE_MENU;
            }
            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Got away safely!", IntervalMAJ);
                Game.Components.Add(message);
                CombatState = CombatState.END;//On met fin au combat
            }

        }
        void EffectuerAttaque(Pokemon attaquant, Pokemon opposant, int attaqueChoisie)//Maybe, je sais pas trop, reformuler?
        {
            Attaque atk = new Attaque(Game, attaqueChoisie);
            int nombreAl�atoire = G�n�rateur.Next(0, 101);
            if (nombreAl�atoire <= atk.Accuracy)//attaque!
            {
                //on va faire une attaque classique, ensuite on applique pardessus l'effet plus complexe peu importe l'attaque
                if (atk.EstUneAttaqueSp�ciale() && atk.EstUneAttaqueAvecBasePowerValide())
                    opposant.D�fendre(CalculPointsDamageSp�cial(attaquant, opposant, atk));

                else if (atk.EstUneAttaquePhysique() && atk.EstUneAttaqueAvecBasePowerValide())
                    opposant.D�fendre(CalculPointsDamagePhysique(attaquant, opposant, atk));

                //Ex�cuterEffet(attaquant, opposant, atk);
            }
            //opposant.D�fendre(CalculerPointsDamage(attaqueChoisie, attaquant, opposant));
        }
        void EffectuerAttaque(Pokemon attaquant, Pokemon opposant, Attaque attaqueChoisie)//Maybe, je sais pas trop, reformuler?
        {
            int nombreAl�atoire = G�n�rateur.Next(0, 101);
            //if (nombreAl�atoire <= attaqueChoisie.Accuracy)//attaque!
            //{
                //on va faire une attaque classique, ensuite on applique pardessus l'effet plus complexe peu importe l'attaque
                if (attaqueChoisie.EstUneAttaqueSp�ciale() && attaqueChoisie.EstUneAttaqueAvecBasePowerValide())
                    opposant.D�fendre(CalculPointsDamageSp�cial(attaquant, opposant, attaqueChoisie));

                else if (attaqueChoisie.EstUneAttaquePhysique() && attaqueChoisie.EstUneAttaqueAvecBasePowerValide())
                    opposant.D�fendre(CalculPointsDamagePhysique(attaquant, opposant, attaqueChoisie));

                //Ex�cuterEffet(attaquant, opposant, atk);
            //}
            //opposant.D�fendre(CalculerPointsDamage(attaqueChoisie, attaquant, opposant));
        }
        int CalculPointsDamagePhysique(Pokemon attaquant, Pokemon opposant, Attaque atk)// s'il y a un base power
        {
            float damage;

            float multiplicateurType = atk.GetTypeMultiplier(opposant.Type1, opposant.Type2);
            //MessageBox: "It's super effective!", "It's very effective!", "It's not very effective.", "It has no effect at all."
            damage = ((2 * attaquant.Level / 5f + 2) * atk.Power * (attaquant.Attack / (float)opposant.Defense) / 50f) * multiplicateurType;

            return (int)damage;
        }

        int CalculPointsDamageSp�cial(Pokemon attaquant, Pokemon opposant, Attaque atk)
        {
            float damage;

            float multiplicateurType = atk.GetTypeMultiplier(opposant.Type1, opposant.Type2);
            //MessageBox: "It's super effective!", "It's very effective!", "It's not very effective.", "It has no effect at all."
            damage = ((2 * attaquant.Level / 5 + 2) * atk.Power * (attaquant.SpecialAttack / (float)opposant.SpecialDefense) / 50) * multiplicateurType;

            return (int)damage;
        }
        private int CalculerPointsDamage(int attaqueChoisie, Pokemon attaquant, Pokemon opposant)
        {
            return ((2 * attaquant.Level / 5 + 2) * /*atk.Power*/50 * (attaquant.Attack / opposant.Defense) / 50 + 2) * /*multiplicateurType*/1;
        }
    }
}
