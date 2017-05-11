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
    enum CombatState { BATTLE_MENU, IN_BATTLE, TOUR_USER, TOUR_OPPONENT, VERIFY_OUTCOME, VICTORY, DEFEAT, END }
    public class Combat : Microsoft.Xna.Framework.GameComponent, IDestructible
    {
        float IntervalMAJ { get; set; }
        CombatState CombatState { get; set; }
        AccessBaseDeDonn�e Database { get; set; }
        Random G�n�rateur { get; set; }
        Vector2 PositionBox { get; set; }
        BattleMenu MainMenu { get; set; }

        RessourcesManager<Song> GestionnaireDeChansons { get; set; }
        //Player LeJoueur { get; set; } Faut utiliser UserTrainer ici.
        Song tuneCombat { get; set; }


        Trainer UserTrainer { get; set; }
        Pokemon UserPokemon { get; set; }
        public int NoPok�dexUserPokemon => UserPokemon.PokedexNumber;
        TexteFixe NomUserPokemon { get; set; }
        TexteFixe VieUserPokemon { get; set; }
        Vector2 PositionInfoUserPokemon { get; set; }
        bool TourUserCompl�t� { get; set; }

        Trainer OpponentTrainer { get; set; }
        Pokemon OpponentPokemon { get; set; }
        public int NoPok�dexOpponentPokemon => OpponentPokemon.PokedexNumber;
        TexteFixe NomOpponentPokemon { get; set; }
        TexteFixe VieOpponentPokemon { get; set; }
        Vector2 PositionInfoOpponentPokemon { get; set; }
        bool TourOpponentCompl�t� { get; set; }

        public bool EstOpponentSauvage { get; private set; }
        bool TourCompl�t� => TourUserCompl�t� && TourOpponentCompl�t�;


        
        public static bool EnCombat { get; set; }
        static Combat()
        {
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
                //peut-�tre que faire une liste de composantes "combat composantes" serait plus appropri�
            }
        }


        public bool GetPokemonEstChang� => MainMenu.Pok�monChang�;


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
        public override void Initialize()
        {
            EnCombat = true;
            UserPokemon = UserTrainer.NextPokemonEnVie();
            //GestionnaireDeChansons = Game.Services.GetService(typeof(RessourcesManager<Song>)) as RessourcesManager<Song>;
            //GestionnaireDeChansons = new RessourcesManager<Song>(Game, "Songs");
            //tuneCombat = GestionnaireDeChansons.Find("tuneCombat");
            //if (EnCombat)
            //{
            //    MediaPlayer.Stop();
            //    MediaPlayer.Play(tuneCombat);
            //}
            GamePad.SetVibration(PlayerIndex.One, 1, 1);
            G�n�rateur = new Random();
            

            
            MainMenu = new BattleMenu(Game, PositionBox, new Vector2(Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage), IntervalMAJ, UserPokemon, UserTrainer);
            Game.Components.Add(MainMenu);

            //LeJoueur = Game.Services.GetService(typeof(Player)) as Player;
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;

            AfficherMessagesInitialisation();
            AjouterLesTextesFixes();



            MainMenu.BattleMenuState = BattleMenuState.MAIN;
            CombatState = CombatState.BATTLE_MENU;
            base.Initialize();
        }




        private void AfficherMessagesInitialisation()
        {
            if (EstOpponentSauvage)
            {
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Wild " + OpponentPokemon.Nom + " appeared!", IntervalMAJ);
                Game.Components.Add(messageA);
            }
            else
            {
                OpponentPokemon = OpponentTrainer.NextPokemonEnVie();
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Trainer " + OpponentTrainer.Nom + " wants to battle!", IntervalMAJ);
                Game.Components.Add(messageA);
                AfficheurTexte messageA2 = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Trainer " + OpponentTrainer.Nom + " send out " + OpponentPokemon.Nom + "!", IntervalMAJ);
                Game.Components.Add(messageA2);
            }

            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, UserTrainer.Nom + ": Go, " + UserPokemon.Nom + "!", IntervalMAJ);
            Game.Components.Add(messageB);
        }
        private void AjouterLesTextesFixes()
        {
            PositionInfoUserPokemon = new Vector2(Game.Window.ClientBounds.Width - (UserPokemon.ToString().Count() + 3) * Cadre.TAILLE_TILE, Game.Window.ClientBounds.Height - Cadre.TAILLE_TILE * 9);
            PositionInfoOpponentPokemon = new Vector2(Cadre.TAILLE_TILE, Game.Window.ClientBounds.Height / 10);

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
        public override void Update(GameTime gameTime)
        {
            if (!AfficheurTexte.MessageEnCours)
                G�rerTransitions();

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
        void G�rerTransitions()
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
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
            NomOpponentPokemon.Visible = true;
            NomUserPokemon.Visible = true;
            VieOpponentPokemon.Visible = true;
            VieUserPokemon.Visible = true;

            if (MainMenu.BattleMenuState == BattleMenuState.READY)
                CombatState = CombatState.IN_BATTLE;

        }
        void G�rerTransitionIN_BATTLE()
        {
            if (MainMenu.ItemPokeballEstUtilis�)
                LancerUnePokeball();
            else if (MainMenu.TentativeFuite)
                EssayerFuir();
            else if (MainMenu.Pok�monChang�)
            {
                MainMenu.Pok�monChang� = false;
                ChangerPok�mon(MainMenu.Num�roChoisi);
                TourUserCompl�t� = true;
                CombatState = CombatState.TOUR_OPPONENT;
            }
            else
            {
                EffectuerLeRound();

                if (TourCompl�t�)
                {
                    TourUserCompl�t� = false;
                    TourOpponentCompl�t� = false;
                    if (UserPokemon.EstEnVie && OpponentPokemon.EstEnVie) //si les deux sont en vie apr�s s'�tre battus, retour au MainMenu
                    {
                        MainMenu.BattleMenuState = BattleMenuState.MAIN;
                        CombatState = CombatState.BATTLE_MENU;
                    }
                    else
                        CombatState = CombatState.VERIFY_OUTCOME;
                }
            }
        }

        void EffectuerLeRound()
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

        }

        void LancerUnePokeball()
        {
            if (EstOpponentSauvage)
                EssayerAttraperWildPokemon(UserTrainer, OpponentPokemon);
            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "You can't catch a trainer's pokemon!", IntervalMAJ);
                Game.Components.Add(message);
                CombatState = CombatState.TOUR_OPPONENT;
            }
        }

        public void EssayerAttraperWildPokemon(Trainer joueur, Pokemon opponent)
        {
            bool valeurFormule = EffectuerFormuleGenI(opponent);
            if (valeurFormule)
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Gotcha! " + opponent.Nom + " was caught!", IntervalMAJ);
                Game.Components.Add(message);

                joueur.AddPokemon(opponent);//on ajoute directement la r�f�rence dans la liste du joueur sans copies
                
                if (EnCombat)//Parce qu'on pourrait vouloir l'utiliser hors combat
                {
                    MainMenu.ItemPokeballEstUtilis� = false;
                    CombatState = CombatState.END;
                }
            }
            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "The wild " + opponent.Nom + " broke free!", IntervalMAJ);
                Game.Components.Add(message);

                if (EnCombat)
                {
                    MainMenu.ItemPokeballEstUtilis� = false;
                    TourUserCompl�t� = true;
                    CombatState = CombatState.TOUR_OPPONENT;
                }
            }
        }
        public void CatchWildPokemon(Trainer joueur, Pokemon opponent)
        {

            AfficheurTexte message2 = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Gotcha! " + opponent.Nom + " was caught!", IntervalMAJ);
            Game.Components.Add(message2);

            joueur.AddPokemon(opponent);//on ajoute directement la r�f�rence dans la liste du joueur sans copies

            if (EnCombat)
            {
                MainMenu.ItemMasterBall = false;
                CombatState = CombatState.END;
            }

        }
        bool EffectuerFormuleGenI(Pokemon opponent)
        {
            //Formule de pok�mon g�n�ration I (La forme tr�s algorythmique s'applique bien � la programmation, je vais la reprendre ligne pour ligne) http://bulbapedia.bulbagarden.net/wiki/Catch_rate
            //note: la formule gen II s'appliquerait bien aussi, mais le calcul de "b" est fastidieux
            /*
             1. g�n�rer un nombre "n" al�atoire (pok�ball = 0 � 255, greatball = 0 � 200 et ultraball = 0 � 150)
             2. return true if (((Sleep || Frozen) && n < 25) || ((Paralyzed || Burned || Poisoned) && n < 12)) //(sleep ou fozen et n < 25 = true; par, burn, psn et n < 12 = true; )
             3. si N (ou N - 25 ou - 12 selon status) <= catchrate, true
             4. si valeur = false, g�n�rer M entre 0 et 255
             5. Calculer F = (HPmaxOpp * 255 * 4)/(HPcurrent * Ball) et ball = 8 si greatball, 12 otherwise.
             6. si F >= M, true

            Le reste de l'algorithme est juste pour indiquer combien de fois la balle shake si le pok�mon n'est pas attrap� (on fait pas pour l'instant)
            
             
             */
            bool estAttrap� = false; //attrape pas, � moins que l'on le dit.
            //int n = G�n�rateur.Next(0, 256); //on ne fera que les pok�balls pour l'instant, et ici �a d�pend si y a un status uniquement 
            //if (n <= OpponentPokemon.CatchRate)
            //    estAttrap� = true;

            if (!estAttrap�)
            {
                int m = G�n�rateur.Next(0, 256);
                int f = (opponent.MaxHp * /*255 * */opponent.CatchRate * 4) / (opponent.HP * 12); //Laisser la division enti�re d'apr�s le site de la formule

                if (f >= m)
                    estAttrap� = true;
            }
            return estAttrap�;
        }

        void G�rerTransitionTOUR_USER()
        {
            GamePad.SetVibration(PlayerIndex.One, 1, 0);
            NomOpponentPokemon.Visible = true;
            VieOpponentPokemon.Visible = true;
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
            GamePad.SetVibration(PlayerIndex.One, 0, 1);
            NomUserPokemon.Visible = true;
            VieUserPokemon.Visible = true;
            EffectuerTourOpponent();
            VieUserPokemon.RemplacerMessage(UserPokemon.VieToString());
            TourOpponentCompl�t� = true;
            CombatState = CombatState.IN_BATTLE;
        }
        void G�rerTransitionVERIFY_OUTCOME()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
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
            else
            {
                if (!UserTrainer.EstEnVie)
                    CombatState = CombatState.DEFEAT;
                else
                {
                    NomUserPokemon.Visible = false;
                    VieUserPokemon.Visible = false;
                    AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, UserPokemon.Nom + " fainted!", IntervalMAJ);
                    Game.Components.Add(message);

                    MainMenu.BackLock = true; //pour forcer � changer de pokemon
                    MainMenu.BattleMenuState = BattleMenuState.POKEMON;
                    CombatState = CombatState.TOUR_USER;
                }
            }
        }
        void G�rerTransitionVICTORY()
        {
            if (!EstOpponentSauvage)
            {
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Amazing, such strength!", IntervalMAJ);
                Game.Components.Add(messageA);
            }
            else
            {
                AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Wild " + OpponentPokemon.Nom + " fainted!", IntervalMAJ);
                Game.Components.Add(messageB);
            }
            DonnerExp();
            CombatState = CombatState.END;
        }
        private void DonnerExp()
        {
            bool aAugment�DeNiveau;
            float exp = OpponentPokemon.GiveExp();
            AfficheurTexte messageC = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, UserPokemon.Nom + " gained " + ((int)exp).ToString() + " exp.", IntervalMAJ);
            Game.Components.Add(messageC);
            aAugment�DeNiveau = UserPokemon.GainExp(exp);

        }
        void G�rerTransitionDEFEAT()
        {
            if (!EstOpponentSauvage)
            {
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Wow, what a weakling.", IntervalMAJ);
                Game.Components.Add(messageA);
            }
            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Player is out of pokemon. Go to the Pokemon Center.", IntervalMAJ);
            Game.Components.Add(messageB);
            CombatState = CombatState.END;
        }
        void G�rerTransitionEND()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);

            UserTrainer.MettreEnPremi�rePosition(UserPokemon);
            EnCombat = false;
            �D�truire = true;
        }
        #endregion




        void EffectuerTourUser()
        {
            if (MainMenu.AttaqueUtilis�e)
                EffectuerAttaque(MainMenu.Num�roChoisi);
            else if (MainMenu.ItemUtilis�)
                UtilierItem(MainMenu.Num�roChoisi);
            else if (MainMenu.Pok�monChang�)
                ChangerPok�mon(MainMenu.Num�roChoisi);
            //else if (MainMenu.TentativeFuite)
            //    EssayerFuir();
        }
        void EffectuerTourOpponent()
        {
            //choisir une attaque al�atoire
            int nbAl�atoire = G�n�rateur.Next(0, OpponentPokemon.NbAttaques);
            AfficheurTexte message;
            if (EstOpponentSauvage)
                message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Wild " + OpponentPokemon.Nom + " used " + OpponentPokemon[nbAl�atoire].ToString() + "!", IntervalMAJ);
            else
                message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Foe " + OpponentPokemon.Nom + " used " + OpponentPokemon[nbAl�atoire].ToString() + "!", IntervalMAJ);

            Game.Components.Add(message);
            EffectuerAttaque(OpponentPokemon, UserPokemon, OpponentPokemon[nbAl�atoire]);
        }
        void EffectuerAttaque(int num�roChoisi)
        {
            string messageTour = UserPokemon.Nom + " used " + UserPokemon[num�roChoisi].ToString() + "!";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
            Game.Components.Add(message);
            EffectuerAttaque(UserPokemon, OpponentPokemon, UserPokemon[num�roChoisi]);
        }
        void UtilierItem(int num�roChoisi)
        {
            if (!MainMenu.ItemPokeballEstUtilis�)
            {
                string messageTour = "User used item " + num�roChoisi.ToString() + ".";
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
                Game.Components.Add(message);
            }
            
        }
        void ChangerPok�mon(int num�roChoisi)
        {
            UserPokemon = UserTrainer[num�roChoisi];
            PositionInfoUserPokemon = new Vector2(Game.Window.ClientBounds.Width - (UserPokemon.ToString().Count() + 3) * Cadre.TAILLE_TILE, Game.Window.ClientBounds.Height - Cadre.TAILLE_TILE * 9);
            NomUserPokemon.Visible = false;
            VieUserPokemon.Visible = false;
            NomUserPokemon.RemplacerPosition(PositionInfoUserPokemon);
            NomUserPokemon.RemplacerMessage(UserPokemon.ToString());
            VieUserPokemon.RemplacerPosition(new Vector2(PositionInfoUserPokemon.X, PositionInfoUserPokemon.Y + Cadre.TAILLE_TILE));
            VieUserPokemon.RemplacerMessage(UserPokemon.VieToString());

            string messageTour = UserTrainer.Nom + " send out " + UserPokemon.Nom + "!";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
            Game.Components.Add(message);
            
        }
        void ChangerOpponentPokemon()
        {
            TourOpponentCompl�t� = false;
            TourUserCompl�t� = false;
            AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, OpponentTrainer.Nom + "'s " + OpponentPokemon.Nom + " fainted!", IntervalMAJ);
            Game.Components.Add(messageA);

            OpponentPokemon = OpponentTrainer.NextPokemonEnVie();//ligne de code qui switch de pok�mon
            NomOpponentPokemon.RemplacerMessage(OpponentPokemon.ToString());
            VieOpponentPokemon.RemplacerMessage(OpponentPokemon.VieToString());

            string messageTour = OpponentTrainer.Nom + " send out " + OpponentPokemon.Nom + "!";
            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
            Game.Components.Add(messageB);
        }
        void EssayerFuir()
        {
            MainMenu.TentativeFuite = false;
            //random selon des probabilit�s et level des deux pokemons. pour l'instant on doit qu'y r�ussi � tout coup
            if (!EstOpponentSauvage)
            {
                TourUserCompl�t� = true;
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "There's no running from a trainer battle!", IntervalMAJ);
                Game.Components.Add(message);
                MainMenu.BattleMenuState = BattleMenuState.MAIN;
                CombatState = CombatState.BATTLE_MENU;
            }
            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Got away safely!", IntervalMAJ);
                Game.Components.Add(message);
                CombatState = CombatState.END;
            }
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
            AfficherMessageMultiplicateur(multiplicateurType);

            damage = ((2 * attaquant.Level / 5f + 2) * atk.Power * (attaquant.Attack / (float)opposant.Defense) / 50f) * multiplicateurType;

            int damageInt = (int)(damage * 100);
            if (damageInt < 100 && damageInt != 0)
                damage = 1;
            return (int)damage;
        }

        

        int CalculPointsDamageSp�cial(Pokemon attaquant, Pokemon opposant, Attaque atk)
        {
            float damage;

            float multiplicateurType = atk.GetTypeMultiplier(opposant.Type1, opposant.Type2);
            AfficherMessageMultiplicateur(multiplicateurType);

            damage = ((2 * attaquant.Level / 5 + 2) * atk.Power * (attaquant.SpecialAttack / (float)opposant.SpecialDefense) / 50) * multiplicateurType;

            int damageInt = (int)(damage * 100);
            if (damageInt < 100 && damageInt != 0)
                damage = 1;
            return (int)damage;
        }
        private void AfficherMessageMultiplicateur(float multiplicateurType)
        {
            int pourcentageMultiplicatif = (int)(multiplicateurType * 100);

            if (pourcentageMultiplicatif == 0)
            {
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "It had no effect.", IntervalMAJ);
                Game.Components.Add(messageA);
            }
            else if (pourcentageMultiplicatif < 100)
            {
                AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "It's not very effective...", IntervalMAJ);
                Game.Components.Add(messageB);
            }
            else if (pourcentageMultiplicatif > 100)
            {
                AfficheurTexte messageC = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "It's super effective!", IntervalMAJ);
                Game.Components.Add(messageC);
            }
        }
    }
}
