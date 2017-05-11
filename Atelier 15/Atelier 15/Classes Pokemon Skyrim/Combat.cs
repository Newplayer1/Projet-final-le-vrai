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
        AccessBaseDeDonnée Database { get; set; }
        Random Générateur { get; set; }
        Vector2 PositionBox { get; set; }
        BattleMenu MainMenu { get; set; }

        RessourcesManager<Song> GestionnaireDeChansons { get; set; }
        //Player LeJoueur { get; set; } Faut utiliser UserTrainer ici.
        Song tuneCombat { get; set; }


        Trainer UserTrainer { get; set; }
        Pokemon UserPokemon { get; set; }
        public int NoPokédexUserPokemon => UserPokemon.PokedexNumber;
        TexteFixe NomUserPokemon { get; set; }
        TexteFixe VieUserPokemon { get; set; }
        Vector2 PositionInfoUserPokemon { get; set; }
        bool TourUserComplété { get; set; }

        Trainer OpponentTrainer { get; set; }
        Pokemon OpponentPokemon { get; set; }
        public int NoPokédexOpponentPokemon => OpponentPokemon.PokedexNumber;
        TexteFixe NomOpponentPokemon { get; set; }
        TexteFixe VieOpponentPokemon { get; set; }
        Vector2 PositionInfoOpponentPokemon { get; set; }
        bool TourOpponentComplété { get; set; }

        public bool EstOpponentSauvage { get; private set; }
        bool TourComplété => TourUserComplété && TourOpponentComplété;


        
        public static bool EnCombat { get; set; }
        static Combat()
        {
            EnCombat = false;
        }

        bool àDétruire;
        public bool ÀDétruire
        {
            get
            {
                return àDétruire;
            }

            set
            {
                àDétruire = value;
                MainMenu.ÀDétruire = value;
                NomOpponentPokemon.ÀDétruire = value;
                NomUserPokemon.ÀDétruire = value;
                VieOpponentPokemon.ÀDétruire = value;
                VieUserPokemon.ÀDétruire = value;
                //peut-être que faire une liste de composantes "combat composantes" serait plus approprié
            }
        }


        public bool GetPokemonEstChangé => MainMenu.PokémonChangé;


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
            Générateur = new Random();
            

            
            MainMenu = new BattleMenu(Game, PositionBox, new Vector2(Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage), IntervalMAJ, UserPokemon, UserTrainer);
            Game.Components.Add(MainMenu);

            //LeJoueur = Game.Services.GetService(typeof(Player)) as Player;
            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;

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
                GérerTransitions();

            base.Update(gameTime);//Utile?
        }

        //#region GérerÉtats
        //void GérerÉtats() //ici on exécute ce que l'on fait à un tel état
        //{
        //    switch (CombatState)
        //    {
        //        case CombatState.BATTLE_MENU:
        //            GérerBATTLE_MENU();
        //            break;
        //        case CombatState.IN_BATTLE:
        //            GérerIN_BATTLE();
        //            break;
        //        //case CombatState.VERIFY_OUTCOME:
        //        //    GérerVERIFY_ALIVE();
        //        //    break;
        //        case CombatState.VICTORY:
        //            GérerVICTORY();
        //            break;
        //        case CombatState.DEFEAT:
        //            GérerDEFEAT();
        //            break;
        //        case CombatState.END:
        //            GérerEND();
        //            break;
        //    }
        //}
        //void GérerBATTLE_MENU()
        //{
        //}
        //void GérerIN_BATTLE()
        //{
        //    EffectuerTourUser();
        //    EffectuerTourOpponent();
        //    TourComplété = true; // simple, pour que ça run
        //}
        //void GérerVICTORY()
        //{
        //}
        //void GérerDEFEAT()
        //{
        //}
        //void GérerEND()
        //{
        //}
        //#endregion

        #region GérerTransition
        void GérerTransitions()
        {
            switch (CombatState)
            {
                case CombatState.BATTLE_MENU:
                    GérerTransitionBATTLE_MENU();
                    break;
                case CombatState.IN_BATTLE:
                    GérerTransitionIN_BATTLE();
                    break;
                case CombatState.TOUR_USER:
                    GérerTransitionTOUR_USER();
                    break;
                case CombatState.TOUR_OPPONENT:
                    GérerTransitionTOUR_OPPONENT();
                    break;
                case CombatState.VERIFY_OUTCOME:
                    GérerTransitionVERIFY_OUTCOME();
                    break;
                case CombatState.VICTORY:
                    GérerTransitionVICTORY();
                    break;
                case CombatState.DEFEAT:
                    GérerTransitionDEFEAT();
                    break;
                case CombatState.END: //default?
                    GérerTransitionEND();
                    break;
            }
        }
        void GérerTransitionBATTLE_MENU()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
            NomOpponentPokemon.Visible = true;
            NomUserPokemon.Visible = true;
            VieOpponentPokemon.Visible = true;
            VieUserPokemon.Visible = true;

            if (MainMenu.BattleMenuState == BattleMenuState.READY)
                CombatState = CombatState.IN_BATTLE;

        }
        void GérerTransitionIN_BATTLE()
        {
            if (MainMenu.ItemPokeballEstUtilisé)
                LancerUnePokeball();
            else if (MainMenu.TentativeFuite)
                EssayerFuir();
            else if (MainMenu.PokémonChangé)
            {
                MainMenu.PokémonChangé = false;
                ChangerPokémon(MainMenu.NuméroChoisi);
                TourUserComplété = true;
                CombatState = CombatState.TOUR_OPPONENT;
            }
            else
            {
                EffectuerLeRound();

                if (TourComplété)
                {
                    TourUserComplété = false;
                    TourOpponentComplété = false;
                    if (UserPokemon.EstEnVie && OpponentPokemon.EstEnVie) //si les deux sont en vie après s'être battus, retour au MainMenu
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
                if (!TourOpponentComplété && OpponentPokemon.EstEnVie) //ça attaque en sens inverse, user avant opponent. Très contre-intuitif, comment ça se fait?
                    CombatState = CombatState.TOUR_OPPONENT;//EffectuerTourOpponent();

                if (!OpponentPokemon.EstEnVie || !UserPokemon.EstEnVie)//Parce que le combat pourrait finir entre les attaques des deux pokémons
                    CombatState = CombatState.VERIFY_OUTCOME;

                if (!TourUserComplété && UserPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_USER;//EffectuerTourUser();
            }
            else
            {
                if (!TourUserComplété && UserPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_USER;

                if (!OpponentPokemon.EstEnVie || !UserPokemon.EstEnVie)//Parce que le combat pourrait finir entre les attaques des deux pokémons
                    CombatState = CombatState.VERIFY_OUTCOME;

                if (!TourOpponentComplété && OpponentPokemon.EstEnVie)
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

                joueur.AddPokemon(opponent);//on ajoute directement la référence dans la liste du joueur sans copies
                
                if (EnCombat)//Parce qu'on pourrait vouloir l'utiliser hors combat
                {
                    MainMenu.ItemPokeballEstUtilisé = false;
                    CombatState = CombatState.END;
                }
            }
            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "The wild " + opponent.Nom + " broke free!", IntervalMAJ);
                Game.Components.Add(message);

                if (EnCombat)
                {
                    MainMenu.ItemPokeballEstUtilisé = false;
                    TourUserComplété = true;
                    CombatState = CombatState.TOUR_OPPONENT;
                }
            }
        }
        public void CatchWildPokemon(Trainer joueur, Pokemon opponent)
        {

            AfficheurTexte message2 = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Gotcha! " + opponent.Nom + " was caught!", IntervalMAJ);
            Game.Components.Add(message2);

            joueur.AddPokemon(opponent);//on ajoute directement la référence dans la liste du joueur sans copies

            if (EnCombat)
            {
                MainMenu.ItemMasterBall = false;
                CombatState = CombatState.END;
            }

        }
        bool EffectuerFormuleGenI(Pokemon opponent)
        {
            //Formule de pokémon génération I (La forme très algorythmique s'applique bien à la programmation, je vais la reprendre ligne pour ligne) http://bulbapedia.bulbagarden.net/wiki/Catch_rate
            //note: la formule gen II s'appliquerait bien aussi, mais le calcul de "b" est fastidieux
            /*
             1. générer un nombre "n" aléatoire (pokéball = 0 à 255, greatball = 0 à 200 et ultraball = 0 à 150)
             2. return true if (((Sleep || Frozen) && n < 25) || ((Paralyzed || Burned || Poisoned) && n < 12)) //(sleep ou fozen et n < 25 = true; par, burn, psn et n < 12 = true; )
             3. si N (ou N - 25 ou - 12 selon status) <= catchrate, true
             4. si valeur = false, générer M entre 0 et 255
             5. Calculer F = (HPmaxOpp * 255 * 4)/(HPcurrent * Ball) et ball = 8 si greatball, 12 otherwise.
             6. si F >= M, true

            Le reste de l'algorithme est juste pour indiquer combien de fois la balle shake si le pokémon n'est pas attrapé (on fait pas pour l'instant)
            
             
             */
            bool estAttrapé = false; //attrape pas, à moins que l'on le dit.
            //int n = Générateur.Next(0, 256); //on ne fera que les pokéballs pour l'instant, et ici ça dépend si y a un status uniquement 
            //if (n <= OpponentPokemon.CatchRate)
            //    estAttrapé = true;

            if (!estAttrapé)
            {
                int m = Générateur.Next(0, 256);
                int f = (opponent.MaxHp * /*255 * */opponent.CatchRate * 4) / (opponent.HP * 12); //Laisser la division entière d'après le site de la formule

                if (f >= m)
                    estAttrapé = true;
            }
            return estAttrapé;
        }

        void GérerTransitionTOUR_USER()
        {
            GamePad.SetVibration(PlayerIndex.One, 1, 0);
            NomOpponentPokemon.Visible = true;
            VieOpponentPokemon.Visible = true;
            if (UserPokemon.EstEnVie)
            {
                EffectuerTourUser();
                VieOpponentPokemon.RemplacerMessage(OpponentPokemon.VieToString());
                TourUserComplété = true;
                if (CombatState != CombatState.END)
                    CombatState = CombatState.IN_BATTLE;
            }
            else
            {
                if (MainMenu.PokémonChangé)
                {
                    TourOpponentComplété = false;
                    TourUserComplété = false;
                    ChangerPokémon(MainMenu.NuméroChoisi);
                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
                    CombatState = CombatState.BATTLE_MENU;
                }
            }
        }
        void GérerTransitionTOUR_OPPONENT()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 1);
            NomUserPokemon.Visible = true;
            VieUserPokemon.Visible = true;
            EffectuerTourOpponent();
            VieUserPokemon.RemplacerMessage(UserPokemon.VieToString());
            TourOpponentComplété = true;
            CombatState = CombatState.IN_BATTLE;
        }
        void GérerTransitionVERIFY_OUTCOME()
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

                    MainMenu.BackLock = true; //pour forcer à changer de pokemon
                    MainMenu.BattleMenuState = BattleMenuState.POKEMON;
                    CombatState = CombatState.TOUR_USER;
                }
            }
        }
        void GérerTransitionVICTORY()
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
            bool aAugmentéDeNiveau;
            float exp = OpponentPokemon.GiveExp();
            AfficheurTexte messageC = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, UserPokemon.Nom + " gained " + ((int)exp).ToString() + " exp.", IntervalMAJ);
            Game.Components.Add(messageC);
            aAugmentéDeNiveau = UserPokemon.GainExp(exp);

        }
        void GérerTransitionDEFEAT()
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
        void GérerTransitionEND()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);

            UserTrainer.MettreEnPremièrePosition(UserPokemon);
            EnCombat = false;
            ÀDétruire = true;
        }
        #endregion




        void EffectuerTourUser()
        {
            if (MainMenu.AttaqueUtilisée)
                EffectuerAttaque(MainMenu.NuméroChoisi);
            else if (MainMenu.ItemUtilisé)
                UtilierItem(MainMenu.NuméroChoisi);
            else if (MainMenu.PokémonChangé)
                ChangerPokémon(MainMenu.NuméroChoisi);
            //else if (MainMenu.TentativeFuite)
            //    EssayerFuir();
        }
        void EffectuerTourOpponent()
        {
            //choisir une attaque aléatoire
            int nbAléatoire = Générateur.Next(0, OpponentPokemon.NbAttaques);
            AfficheurTexte message;
            if (EstOpponentSauvage)
                message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Wild " + OpponentPokemon.Nom + " used " + OpponentPokemon[nbAléatoire].ToString() + "!", IntervalMAJ);
            else
                message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Foe " + OpponentPokemon.Nom + " used " + OpponentPokemon[nbAléatoire].ToString() + "!", IntervalMAJ);

            Game.Components.Add(message);
            EffectuerAttaque(OpponentPokemon, UserPokemon, OpponentPokemon[nbAléatoire]);
        }
        void EffectuerAttaque(int numéroChoisi)
        {
            string messageTour = UserPokemon.Nom + " used " + UserPokemon[numéroChoisi].ToString() + "!";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
            Game.Components.Add(message);
            EffectuerAttaque(UserPokemon, OpponentPokemon, UserPokemon[numéroChoisi]);
        }
        void UtilierItem(int numéroChoisi)
        {
            if (!MainMenu.ItemPokeballEstUtilisé)
            {
                string messageTour = "User used item " + numéroChoisi.ToString() + ".";
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
                Game.Components.Add(message);
            }
            
        }
        void ChangerPokémon(int numéroChoisi)
        {
            UserPokemon = UserTrainer[numéroChoisi];
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
            TourOpponentComplété = false;
            TourUserComplété = false;
            AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, OpponentTrainer.Nom + "'s " + OpponentPokemon.Nom + " fainted!", IntervalMAJ);
            Game.Components.Add(messageA);

            OpponentPokemon = OpponentTrainer.NextPokemonEnVie();//ligne de code qui switch de pokémon
            NomOpponentPokemon.RemplacerMessage(OpponentPokemon.ToString());
            VieOpponentPokemon.RemplacerMessage(OpponentPokemon.VieToString());

            string messageTour = OpponentTrainer.Nom + " send out " + OpponentPokemon.Nom + "!";
            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
            Game.Components.Add(messageB);
        }
        void EssayerFuir()
        {
            MainMenu.TentativeFuite = false;
            //random selon des probabilités et level des deux pokemons. pour l'instant on doit qu'y réussi à tout coup
            if (!EstOpponentSauvage)
            {
                TourUserComplété = true;
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
            int nombreAléatoire = Générateur.Next(0, 101);
            //if (nombreAléatoire <= attaqueChoisie.Accuracy)//attaque!
            //{
            //on va faire une attaque classique, ensuite on applique pardessus l'effet plus complexe peu importe l'attaque
            if (attaqueChoisie.EstUneAttaqueSpéciale() && attaqueChoisie.EstUneAttaqueAvecBasePowerValide())
                opposant.Défendre(CalculPointsDamageSpécial(attaquant, opposant, attaqueChoisie));

            else if (attaqueChoisie.EstUneAttaquePhysique() && attaqueChoisie.EstUneAttaqueAvecBasePowerValide())
                opposant.Défendre(CalculPointsDamagePhysique(attaquant, opposant, attaqueChoisie));

            //ExécuterEffet(attaquant, opposant, atk);
            //}
            //opposant.Défendre(CalculerPointsDamage(attaqueChoisie, attaquant, opposant));
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

        

        int CalculPointsDamageSpécial(Pokemon attaquant, Pokemon opposant, Attaque atk)
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
