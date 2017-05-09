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

        Pokemon UserPokemon { get; set; } //Le BattleMenu doit avoir accès aux attaques du pokémon... Garder en mémoire l'indice du pkm en jeu?
        public int NoPokédexUserPokemon => UserPokemon.PokedexNumber;//pour afficher un modèle de son numéro
        Pokemon OpponentPokemon { get; set; }
        public int NoPokédexOpponentPokemon => UserPokemon.PokedexNumber;
        AccessBaseDeDonnée Database { get; set; }
        RessourcesManager<Song> GestionnaireDeChansons { get; set; }
        //AccessBaseDeDonnée Database { get; set; }
        //bool EnCombat { get; set; }
        public bool EstOpponentSauvage { get; private set; }
        bool UserPkmPrioritaire { get; set; }
        bool TourComplété => TourUserComplété && TourOpponentComplété;/*{ get; set; }*/
        bool TourUserComplété { get; set; }
        bool TourOpponentComplété { get; set; }

        CombatState CombatState { get; set; }
        Vector2 PositionBox { get; set; }
        BattleMenu MainMenu { get; set; }
        Song tuneCombat { get; set; }

        TexteFixe NomUserPokemon { get; set; }
        TexteFixe NomOpponentPokemon { get; set; }
        Vector2 PositionInfoUserPokemon { get; set; }
        TexteFixe VieUserPokemon { get; set; }
        TexteFixe VieOpponentPokemon { get; set; }
        Vector2 PositionInfoOpponentPokemon { get; set; }

        Random Générateur { get; set; }

        public static bool Wait { get; set; }

        public static bool EnCombat { get; set; }
        static Combat()
        {
            Wait = false;
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
            }
        }

        public int LargeurBox { get; private set; }

        public bool GetPokemonEstChangé => MainMenu.PokémonChangé;

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
        //public Combat(Game game, Vector2 positionBox, Player user, Trainer opponent, float intervalMAJ)
        //    : base(game)
        //{
        //    PositionBox = positionBox;
        //    IntervalMAJ = intervalMAJ;
        //    UserTrainer = user;
        //    OpponentTrainer = opponent;
        //    EstOpponentSauvage = false;
        //}
        public Combat(Game game, Vector2 positionBox, Player user, Pokemon opponent, float intervalMAJ)
            : base(game)
        {
            PositionBox = positionBox;
            IntervalMAJ = intervalMAJ;
            UserTrainer = user;
            OpponentPokemon = opponent;
            EstOpponentSauvage = true;
        }
        public override void Initialize()//Ouverture du combat. Tout ce qui doit être fait avant "Combat Menu"
        {
            EnCombat = true;
            //GestionnaireDeChansons = Game.Services.GetService(typeof(RessourcesManager<Song>)) as RessourcesManager<Song>;
            //GestionnaireDeChansons = new RessourcesManager<Song>(Game, "Songs");
            //tuneCombat = GestionnaireDeChansons.Find("tuneCombat");
            //if (EnCombat)
            //{
            //    MediaPlayer.Stop();
            //    MediaPlayer.Play(tuneCombat);
            //}

            Générateur = new Random();
            LargeurBox = Game.Window.ClientBounds.Width / Cadre.TAILLE_TILE;
            UserPokemon = UserTrainer.NextPokemonEnVie();

            PositionInfoUserPokemon = new Vector2(Game.Window.ClientBounds.Width - (UserPokemon.ToString().Count() + 3) * Cadre.TAILLE_TILE, Game.Window.ClientBounds.Height - Cadre.TAILLE_TILE * 9);
            PositionInfoOpponentPokemon = new Vector2(Cadre.TAILLE_TILE, Game.Window.ClientBounds.Height / 10);
            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
            CombatState = CombatState.INI;

            if (EstOpponentSauvage)
            {
                AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Wild " + OpponentPokemon.Nom + " appeared!", IntervalMAJ);
                Game.Components.Add(message);//Message opponent
            }
            else
            {
                OpponentPokemon = OpponentTrainer.NextPokemonEnVie();
                AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Trainer " + OpponentTrainer.Nom + " wants to battle! " + "Trainer " + OpponentTrainer.Nom + " send out " + OpponentPokemon.Nom + "!", IntervalMAJ);
                Game.Components.Add(message);//Message opponent
            }


            AfficheurTexte messageB = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, UserTrainer.Nom + ": Go, " + UserPokemon.Nom + "!", IntervalMAJ);
            Game.Components.Add(messageB);//Message pokemon user


            AjouterLesTextesFixes();

            MainMenu = new BattleMenu(Game, PositionBox, new Vector2(LargeurBox, Cadre.HAUTEUR_BOX_STANDARD), IntervalMAJ, UserPokemon, UserTrainer);
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
        public override void Update(GameTime gameTime)//mise à jour des tours tant que en vie (both trainer et son pokémon)
        {
            //GérerÉtats(); //Passer gameTime si l'on doit animer qqch
            Wait = AfficheurTexte.MessageEnCours;
            if (!Wait)
                GérerTransitions();//si y a pas de message en cours on peut procéder

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
        void GérerTransitions() //ici on vérifie la condition qui change l'état (et on le change au besoin)
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
        void GérerTransitionIN_BATTLE()
        {
            if (MainMenu.ItemPokeball)
            {
                //if (EstOpponentSauvage)
                TryCatchWildPokemon(UserTrainer, OpponentPokemon);
                //else
                //{
                //    AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "You can't catch a trainer's pokemon!", IntervalMAJ);
                //    Game.Components.Add(message);
                //    CombatState = CombatState.TOUR_OPPONENT;
                //}
            }
            if (MainMenu.ItemGreatBall)
            {
                if (EstOpponentSauvage)
                    TryCatchWildPokemonEfficace(UserTrainer, OpponentPokemon);
                else
                {
                    AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "You can't catch a trainer's pokemon!", IntervalMAJ);
                    Game.Components.Add(message);
                    CombatState = CombatState.TOUR_OPPONENT;
                }
            }
            if (MainMenu.ItemMasterBall)
            {
                if (EstOpponentSauvage)
                    CatchWildPokemon(UserTrainer, OpponentPokemon);
                else
                {
                    AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "You can't catch a trainer's pokemon!", IntervalMAJ);
                    Game.Components.Add(message);
                    CombatState = CombatState.TOUR_OPPONENT;
                }
            }
            //else if(MainMenu.NuméroChoisi == 1) Aucune idée comment formuler ça pour faire un full heal xD
            //{
            //    UserPokemon.HP = UserPokemon.RétablirStats();
            //}
            else
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

                if (TourOpponentComplété && TourUserComplété)
                {
                    TourUserComplété = false;
                    TourOpponentComplété = false;
                    if (UserPokemon.EstEnVie && OpponentPokemon.EstEnVie) //si les deux sont en vie après s'être battus, retour au MainMenu
                    {
                        MainMenu.BattleMenuState = BattleMenuState.MAIN;
                        CombatState = CombatState.BATTLE_MENU;
                    }

                    else //si l'un des deux est mort
                        CombatState = CombatState.VERIFY_OUTCOME;
                }
            }

        }
        public void TryCatchWildPokemon(Trainer joueur, Pokemon opponent)
        {
            bool valeurFormule = EffectuerFormuleGenI(opponent);
            if (valeurFormule)
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Gotcha! " + opponent.Nom + " was caught!", IntervalMAJ);
                Game.Components.Add(message);

                joueur.AddPokemon(opponent);//on ajoute directement la référence dans la liste du joueur sans copies


                if (EnCombat)
                {
                    MainMenu.ItemPokeball = false;
                    CombatState = CombatState.END;
                }
            }

            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "The wild " + opponent.Nom + " broke free!", IntervalMAJ);
                Game.Components.Add(message);

                if (EnCombat)
                {
                    MainMenu.ItemPokeball = false;
                    CombatState = CombatState.TOUR_OPPONENT;
                }
            }
        }
        public void TryCatchWildPokemonEfficace(Trainer joueur, Pokemon opponent)
        {
            bool valeurFormule2 = EffectuerFormuleGenIGREATBALL(opponent);

            if (valeurFormule2)
            {
                AfficheurTexte message2 = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Gotcha! " + opponent.Nom + " was caught!", IntervalMAJ);
                Game.Components.Add(message2);

                joueur.AddPokemon(opponent);//on ajoute directement la référence dans la liste du joueur sans copies

                if (EnCombat)
                {
                    MainMenu.ItemGreatBall = false;
                    CombatState = CombatState.END;
                }
            }

            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "The wild " + opponent.Nom + " broke free!", IntervalMAJ);
                Game.Components.Add(message);

                if (EnCombat)
                {
                    MainMenu.ItemGreatBall = false;
                    CombatState = CombatState.TOUR_OPPONENT;
                }
            }
        }
        public void CatchWildPokemon(Trainer joueur, Pokemon opponent)
        {

            AfficheurTexte message2 = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Gotcha! " + opponent.Nom + " was caught!", IntervalMAJ);
            Game.Components.Add(message2);

            joueur.AddPokemon(opponent);//on ajoute directement la référence dans la liste du joueur sans copies

            if (EnCombat)
            {
                MainMenu.ItemMasterBall = false;
                CombatState = CombatState.END;
            }

        }
        private bool EffectuerFormuleGenI(Pokemon opponent)
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
        private bool EffectuerFormuleGenIGREATBALL(Pokemon opponent)
        {

            bool estAttrapé = false;

            if (!estAttrapé)
            {
                int m = Générateur.Next(0, 256);
                int f = (opponent.MaxHp * /*255 * */opponent.CatchRate * 4) / (opponent.HP * 12); //Laisser la division entière d'après le site de la formule

                if (f >= m / 2) //DEUX FOIS PLUS EFFICACE
                    estAttrapé = true;
            }
            return estAttrapé;
        }

        void GérerTransitionTOUR_USER()
        {
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
            EffectuerTourOpponent();
            VieUserPokemon.RemplacerMessage(UserPokemon.VieToString());
            TourOpponentComplété = true;
            CombatState = CombatState.IN_BATTLE;
        }
        void GérerTransitionVERIFY_OUTCOME()
        {
            //L'un des deux est mort donc nous sommes arrivé ici. (on doit assurer à 100% qu'on change le state ici parce que sinon on va aller à défaut, soit END)
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
                    MainMenu.BackLock = true; //pour forcer à switch de pokemon
                    MainMenu.BattleMenuState = BattleMenuState.POKEMON;
                    CombatState = CombatState.TOUR_USER;
                }
            }
        }
        void GérerTransitionVICTORY()
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
            bool aAugmentéDeNiveau;
            float exp = OpponentPokemon.GiveExp();
            aAugmentéDeNiveau = UserPokemon.GainExp(exp);
            AfficheurTexte messageC = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, UserPokemon.Nom + " gained " + ((int)exp).ToString() + " exp.", IntervalMAJ);
            Game.Components.Add(messageC);
            if (aAugmentéDeNiveau)
            {
                AfficheurTexte messageD = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, UserPokemon.Nom + " grew to level " + UserPokemon.Level + "!", IntervalMAJ);
                Game.Components.Add(messageD);
            }
        }
        void GérerTransitionDEFEAT()
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
        void GérerTransitionEND()
        {
            //GameState = Jeu3D; ??
            //Détruire le component?

            EnCombat = false;
            ÀDétruire = true;

        }
        #endregion
        void EffectuerTourUser()
        {
            if (MainMenu.AttaqueUtilisée)
                //EffectuerAttaque(UserPokemon, OpponentPokemon, numéroAttaqueChoisie
                EffectuerAttaque(MainMenu.NuméroChoisi);
            else if (MainMenu.ItemUtilisé)
                UtilierItem(MainMenu.NuméroChoisi);
            else if (MainMenu.PokémonChangé)
                ChangerPokémon(MainMenu.NuméroChoisi);
            else if (MainMenu.TentativeFuite)
                EssayerFuir();
        }
        void EffectuerTourOpponent()
        {
            //choisir une attaque aléatoire
            int nbAléatoire = Générateur.Next(0, OpponentPokemon.NbAttaques);
            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, "Wild " + OpponentPokemon.Nom + " used " + OpponentPokemon[nbAléatoire].ToString() + "!", IntervalMAJ);
            Game.Components.Add(message);//Message opponent
            EffectuerAttaque(OpponentPokemon, UserPokemon, OpponentPokemon[nbAléatoire]);

        }
        void EffectuerAttaque(int numéroChoisi)
        {

            string messageTour = UserPokemon.Nom + " used " + UserPokemon[numéroChoisi].ToString() + "!";
            AfficheurTexte message = new AfficheurTexte(Game, new Vector2(PositionBox.X, PositionBox.Y), LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, messageTour, IntervalMAJ);
            Game.Components.Add(message);
            EffectuerAttaque(UserPokemon, OpponentPokemon, UserPokemon[numéroChoisi]);

        }
        void UtilierItem(int numéroChoisi)
        {
            string messageTour = "User used item " + numéroChoisi.ToString() + ".";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, messageTour, IntervalMAJ);
            //Game.Components.Add(message);
            //Ensuite on fait l'effet de l'item
        }
        void ChangerPokémon(int numéroChoisi)
        {
            UserPokemon = UserTrainer[numéroChoisi];//ligne de code qui switch de pokémon
            NomUserPokemon.RemplacerMessage(UserPokemon.ToString());
            VieUserPokemon.RemplacerMessage(UserPokemon.VieToString());

            string messageTour = UserTrainer.Nom + " send out " + UserPokemon.Nom + "!";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, messageTour, IntervalMAJ);
            Game.Components.Add(message);

            //faire le reste du code de switch pokemon
        }
        void ChangerOpponentPokemon()
        {
            TourOpponentComplété = false;
            TourUserComplété = false;
            AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, OpponentTrainer.Nom + "'s " + OpponentPokemon.Nom + " fainted!", IntervalMAJ);
            Game.Components.Add(messageA);

            OpponentPokemon = OpponentTrainer.NextPokemonEnVie();//ligne de code qui switch de pokémon
            NomOpponentPokemon.RemplacerMessage(OpponentPokemon.ToString());
            VieOpponentPokemon.RemplacerMessage(OpponentPokemon.VieToString());

            string messageTour = OpponentTrainer.Nom + " send out " + OpponentPokemon.Nom + "!";
            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, LargeurBox, Cadre.HAUTEUR_BOX_STANDARD, messageTour, IntervalMAJ);
            Game.Components.Add(messageB);
        }
        void EssayerFuir()
        {
            MainMenu.TentativeFuite = false;
            //random selon des probabilités et level des deux pokemons. pour l'instant on doit qu'y réussi à tout coup
            if (!EstOpponentSauvage)
            {
                TourUserComplété = true;
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
            int nombreAléatoire = Générateur.Next(0, 101);
            if (nombreAléatoire <= atk.Accuracy)//attaque!
            {
                //on va faire une attaque classique, ensuite on applique pardessus l'effet plus complexe peu importe l'attaque
                if (atk.EstUneAttaqueSpéciale() && atk.EstUneAttaqueAvecBasePowerValide())
                    opposant.Défendre(CalculPointsDamageSpécial(attaquant, opposant, atk));

                else if (atk.EstUneAttaquePhysique() && atk.EstUneAttaqueAvecBasePowerValide())
                    opposant.Défendre(CalculPointsDamagePhysique(attaquant, opposant, atk));

                //ExécuterEffet(attaquant, opposant, atk);
            }
            //opposant.Défendre(CalculerPointsDamage(attaqueChoisie, attaquant, opposant));
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
            //MessageBox: "It's super effective!", "It's very effective!", "It's not very effective.", "It has no effect at all."
            damage = ((2 * attaquant.Level / 5f + 2) * atk.Power * (attaquant.Attack / (float)opposant.Defense) / 50f) * multiplicateurType;
            if (damage < 1)
                damage = 1;
            return (int)damage;
        }
        int CalculPointsDamageSpécial(Pokemon attaquant, Pokemon opposant, Attaque atk)
        {
            float damage;

            float multiplicateurType = atk.GetTypeMultiplier(opposant.Type1, opposant.Type2);
            //MessageBox: "It's super effective!", "It's very effective!", "It's not very effective.", "It has no effect at all."
            damage = ((2 * attaquant.Level / 5 + 2) * atk.Power * (attaquant.SpecialAttack / (float)opposant.SpecialDefense) / 50) * multiplicateurType;
            if (damage < 1)
                damage = 1;
            return (int)damage;
        }
        private int CalculerPointsDamage(int attaqueChoisie, Pokemon attaquant, Pokemon opposant)
        {
            return ((2 * attaquant.Level / 5 + 2) * /*atk.Power*/50 * (attaquant.Attack / opposant.Defense) / 50 + 2) * /*multiplicateurType*/1;
        }
    }
}
