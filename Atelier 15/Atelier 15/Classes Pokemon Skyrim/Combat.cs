using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        public bool GetPokemonEstChangé => MainMenu.PokémonChangé;

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
            }
        }
        
        /// <summary>
        /// Constructeur d'un combat entre le joueur et un Trainer adversaire.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="positionBox">La position de la boite d'affichage du texte</param>
        /// <param name="user">Le joueur (de type Player.cs)</param>
        /// <param name="opponent">L'adversaire (de type Trainer.cs)</param>
        /// <param name="intervalMAJ">L'intervalle de mise à jour du combat</param>
        public Combat(Game game, Vector2 positionBox, Player user, Trainer opponent, float intervalMAJ)
            : base(game)
        {
            PositionBox = positionBox;
            IntervalMAJ = intervalMAJ;
            UserTrainer = user;
            OpponentTrainer = opponent;
            EstOpponentSauvage = false;
        }
        /// <summary>
        /// Constructeur d'un combat entre le joueur et un Pokémon sauvage.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="positionBox">La position de la boite d'affichage du texte</param>
        /// <param name="user">Le joueur (de type Player.cs)</param>
        /// <param name="opponent">L'adversaire (de type Pokemon.cs)</param>
        /// <param name="intervalMAJ">L'intervalle de mise à jour du combat</param>
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
            GamePad.SetVibration(PlayerIndex.One, 1, 1);

            Générateur = new Random();
            MainMenu = new BattleMenu(Game, PositionBox, new Vector2(Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage), IntervalMAJ, UserPokemon, UserTrainer);
            Game.Components.Add(MainMenu);
            
            AfficherMessagesInitialisation();
            AjouterLesTextesFixes();
            
            MainMenu.BattleMenuState = BattleMenuState.MAIN;
            CombatState = CombatState.BATTLE_MENU;
            base.Initialize();
        }
        /// <summary>
        /// Fonction qui affiche les messages du début d'un combat.
        /// </summary>
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
        /// <summary>
        /// Fonction qui ajoute les textes de vie et de nom des pokémons en combat.
        /// </summary>
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
        }

        

        #region GérerTransition
            /// <summary>
            /// Fonction de l'update qui gère les transitions entre les états et ce que ceux-ci font.
            /// </summary>
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
                default:
                    CombatState = CombatState.BATTLE_MENU;
                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
                    break;
            }
        }
        /// <summary>
        /// Fonction de transition qui attend que le joueur fasse son choix dans le menu du combat.
        /// </summary>
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
        /// <summary>
        /// Fonction de transition qui fait l'action choisie et renvoie à un autre état selon le cas.
        /// </summary>
        void GérerTransitionIN_BATTLE()
        {
            if (MainMenu.ItemPokeballEstUtilisé)
                LancerUnePokeball();
            else if (MainMenu.TentativeFuite)
                EssayerFuir();
            else if (MainMenu.PokémonChangé)
            {
                MainMenu.PokémonChangé = false;
                ChangerUserPokémon(MainMenu.NuméroChoisi);
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
                    if (UserPokemon.EstEnVie && OpponentPokemon.EstEnVie)
                    {
                        MainMenu.BattleMenuState = BattleMenuState.MAIN;
                        CombatState = CombatState.BATTLE_MENU;
                    }
                    else
                        CombatState = CombatState.VERIFY_OUTCOME;
                }
            }
        }

        /// <summary>
        /// Fonction qui fait les attaques entre les deux Pokémons et qui s'assure qu'ils sont encore en vie.
        /// </summary>
        void EffectuerLeRound()
        {
            if (UserPokemon.Speed >= OpponentPokemon.Speed)
            {
                if (!TourOpponentComplété && OpponentPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_OPPONENT;

                if (!OpponentPokemon.EstEnVie || !UserPokemon.EstEnVie)//Parce que le combat pourrait finir entre les attaques des deux pokémons
                    CombatState = CombatState.VERIFY_OUTCOME;

                if (!TourUserComplété && UserPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_USER;
            }
            else
            {
                if (!TourUserComplété && UserPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_USER;

                if (!OpponentPokemon.EstEnVie || !UserPokemon.EstEnVie)
                    CombatState = CombatState.VERIFY_OUTCOME;

                if (!TourOpponentComplété && OpponentPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_OPPONENT;
            }
        }
        /// <summary>
        /// Fonction qui s'effectue quand le joueur sélectionne l'objet "Pokeball".
        /// </summary>
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
        /// <summary>
        /// Fonction pour tenter d'attraper un Pokémon sauvage qui peut servir aussi hors combat.
        /// </summary>
        /// <param name="joueur">Le trainer qui tente d'attraper le Pokémon</param>
        /// <param name="opponent">Le Pokémon que l'on veut attraper</param>
        public void EssayerAttraperWildPokemon(Trainer joueur, Pokemon opponent)
        {
            bool valeurFormule = EffectuerFormuleGenI(opponent);
            if (valeurFormule)
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Gotcha! " + opponent.Nom + " was caught!", IntervalMAJ);
                Game.Components.Add(message);

                joueur.AddPokemon(opponent);

                if (EnCombat)
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
        /// <summary>
        /// Fonction qui vérifie si le Pokémon s'est fait attraper ou non selon la formule originale.
        /// </summary>
        /// <param name="opponent">Le Pokémon que le joueur tente d'attraper</param>
        /// <returns>La valeur s'il est attrapé ou non</returns>
        bool EffectuerFormuleGenI(Pokemon opponent)
        {
            bool estAttrapé = false;

            if (!estAttrapé)
            {
                int m = Générateur.Next(0, 256);
                int f = (opponent.MaxHp * opponent.CatchRate * 4) / (opponent.HP * 12);
                //La possibilité de division entière est voulue, la source de l'algorithme le spécifie.
                if (f >= m)
                    estAttrapé = true;
            }
            return estAttrapé;
        }

        /// <summary>
        /// Fonction de transition qui fait le tour du joueur et renvoie à l'état IN_BATTLE pour compléter le tour.
        /// </summary>
        void GérerTransitionTOUR_USER()
        {
            GamePad.SetVibration(PlayerIndex.One, 1, 0);
            NomOpponentPokemon.Visible = true;
            VieOpponentPokemon.Visible = true;
            if (UserPokemon.EstEnVie)
            {
                EffectuerTourUser(MainMenu.NuméroChoisi);
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
                    ChangerUserPokémon(MainMenu.NuméroChoisi);
                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
                    CombatState = CombatState.BATTLE_MENU;
                }
            }
        }
        /// <summary>
        /// Fonction de transition qui fait le tour de l'adversaire et renvoie à l'état IN_BATTLE pour compléter le tour.
        /// </summary>
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
        /// <summary>
        /// Fonction de transition qui vérifie l'état du combat, s'il doit finir, continuer et comment procéder si l'on continue.
        /// </summary>
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

                    MainMenu.BackLock = true; //Pour forcer le joueur à changer de pokemon sans pouvoir revenir dans le menu.
                    MainMenu.BattleMenuState = BattleMenuState.POKEMON;
                    CombatState = CombatState.TOUR_USER;
                }
            }
        }
        /// <summary>
        /// Fonction de transition de victoire qui mène à la fin du combat.
        /// </summary>
        void GérerTransitionVICTORY()
        {
            if (!EstOpponentSauvage)
            {
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Trainer " + OpponentTrainer.Nom + " was defeated! ", IntervalMAJ);
                Game.Components.Add(messageA);
                //On peut ajouter un gain d'argent pour le joueur ici si l'on ajoute des magasins d'objets.
            }
            else
            {
                AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Wild " + OpponentPokemon.Nom + " fainted!", IntervalMAJ);
                Game.Components.Add(messageB);
            }
            DonnerExp();
            CombatState = CombatState.END;
        }
        /// <summary>
        /// Fonction pour attribuer les points d'expérience au Pokémon du joueur.
        /// </summary>
        private void DonnerExp()
        {
            float exp = OpponentPokemon.GiveExp();
            UserPokemon.GainExp(exp);
        }
        /// <summary>
        /// Fonction de transition de défaite qui mène à la fin du combat.
        /// </summary>
        void GérerTransitionDEFEAT()
        {
            if (!EstOpponentSauvage)
            {
                AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "Trainer " + OpponentTrainer.Nom + " won!", IntervalMAJ);
                Game.Components.Add(messageA); 
            }
            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "You are out of pokemon. Please heal yourself to continue.", IntervalMAJ);
            Game.Components.Add(messageB);
            CombatState = CombatState.END;
        }

        /// <summary>
        /// Fonction de transition mettant fin au combat.
        /// </summary>
        void GérerTransitionEND()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);

            UserTrainer.MettreEnPremièrePosition(UserPokemon);
            EnCombat = false;
            ÀDétruire = true;
        }
        #endregion

        /// <summary>
        /// Fonction qui affiche le message de l'attaque aléatoire de l'adversaire et l'effectue.
        /// </summary>
        void EffectuerTourOpponent()
        {
            int nbAléatoire = Générateur.Next(0, OpponentPokemon.NbAttaques);
            string préfixe = "";
            if (EstOpponentSauvage)
                préfixe = "Wild ";
            else
                préfixe = "Foe ";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, préfixe + OpponentPokemon.Nom + " used " + OpponentPokemon[nbAléatoire].ToString() + "!", IntervalMAJ);
            Game.Components.Add(message);
            EffectuerAttaque(OpponentPokemon, UserPokemon, OpponentPokemon[nbAléatoire]);
        }
        /// <summary>
        /// Fonction qui affiche le message de l'attaque choisie par le joueur et l'effectue.
        /// </summary>
        /// <param name="attaqueSélectionnée">Numéro de l'attaque sélectionnée</param>
        void EffectuerTourUser(int attaqueSélectionnée)
        {
            string messageTour = UserPokemon.Nom + " used " + UserPokemon[attaqueSélectionnée].ToString() + "!";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
            Game.Components.Add(message);
            EffectuerAttaque(UserPokemon, OpponentPokemon, UserPokemon[attaqueSélectionnée]);
        }

        /// <summary>
        /// Fonction qui change le pokémon du joueur.
        /// </summary>
        /// <param name="pokémonSélectionné">Le numéro en inventaire du pokémon choisi</param>
        void ChangerUserPokémon(int pokémonSélectionné)
        {
            UserPokemon = UserTrainer[pokémonSélectionné];

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
        /// <summary>
        /// Fonction qui change le Pokémon de l'adversaire s'il lui en reste qui sont vivants.
        /// </summary>
        void ChangerOpponentPokemon()
        {
            TourOpponentComplété = false;
            TourUserComplété = false;
            AfficheurTexte messageA = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, OpponentTrainer.Nom + "'s " + OpponentPokemon.Nom + " fainted!", IntervalMAJ);
            Game.Components.Add(messageA);

            OpponentPokemon = OpponentTrainer.NextPokemonEnVie();
            NomOpponentPokemon.RemplacerMessage(OpponentPokemon.ToString());
            VieOpponentPokemon.RemplacerMessage(OpponentPokemon.VieToString());

            string messageTour = OpponentTrainer.Nom + " send out " + OpponentPokemon.Nom + "!";
            AfficheurTexte messageB = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
            Game.Components.Add(messageB);
        }

        /// <summary>
        /// Fonction qui sert à déterminer si le joueur peut fuir le combat ou non.
        /// </summary>
        void EssayerFuir()
        {
            MainMenu.TentativeFuite = false;
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

        /// <summary>
        /// Fonction qui exécute l'attaque entre deux Pokémons.
        /// </summary>
        /// <param name="attaquant">Le Pokémon qui attaque</param>
        /// <param name="opposant">Le Pokémon qui se fait attaquer</param>
        /// <param name="attaqueChoisie">L'attaque qui a été choisie</param>
        void EffectuerAttaque(Pokemon attaquant, Pokemon opposant, Attaque attaqueChoisie)
        {
            int nombreAléatoire = Générateur.Next(0, 101);
            if (nombreAléatoire <= attaqueChoisie.Accuracy)//attaque!
            {
                //on va faire une attaque classique, ensuite on applique pardessus l'effet plus complexe peu importe l'attaque
                if (attaqueChoisie.EstUneAttaqueSpéciale() && attaqueChoisie.EstUneAttaqueAvecBasePowerValide())
                    opposant.Défendre(CalculPointsDamageSpécial(attaquant, opposant, attaqueChoisie));

                else if (attaqueChoisie.EstUneAttaquePhysique() && attaqueChoisie.EstUneAttaqueAvecBasePowerValide())
                    opposant.Défendre(CalculPointsDamagePhysique(attaquant, opposant, attaqueChoisie));

                Attaque.AppliquerEffetAttaque(attaquant, opposant, attaqueChoisie);
            }
            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "It missed!", IntervalMAJ);
                Game.Components.Add(message);
            }
        }
        /// <summary>
        /// Calcul des points de damage si l'attaque utilise des facultés "Physiques".
        /// </summary>
        /// <param name="attaquant">Le Pokémon qui attaque</param>
        /// <param name="opposant">Le Pokémon qui se fait attaquer</param>
        /// <param name="attaqueChoisie">L'attaque qui a été choisie</param>
        /// <returns>Les points de damage</returns>
        int CalculPointsDamagePhysique(Pokemon attaquant, Pokemon opposant, Attaque attaqueChoisie)// s'il y a un base power
        {
            float damage;

            float multiplicateurType = attaqueChoisie.GetTypeMultiplier(opposant.Type1, opposant.Type2);
            AfficherMessageMultiplicateur(multiplicateurType);

            damage = ((2 * attaquant.Level / 5f + 2) * attaqueChoisie.Power * (attaquant.Attack / (float)opposant.Defense) / 50f) * multiplicateurType;

            return CalculerDamageMinimal(damage);
        }
        /// <summary>
        /// Calcul des points de damage si l'attaque utilise des facultés "Spéciales".
        /// </summary>
        /// <param name="attaquant">Le Pokémon qui attaque</param>
        /// <param name="opposant">Le Pokémon qui se fait attaquer</param>
        /// <param name="attaqueChoisie">L'attaque qui a été choisie</param>
        /// <returns>Les points de damage</returns>
        int CalculPointsDamageSpécial(Pokemon attaquant, Pokemon opposant, Attaque attaqueChoisie)
        {
            float damage;

            float multiplicateurType = attaqueChoisie.GetTypeMultiplier(opposant.Type1, opposant.Type2);
            AfficherMessageMultiplicateur(multiplicateurType);

            damage = ((2 * attaquant.Level / 5 + 2) * attaqueChoisie.Power * (attaquant.SpecialAttack / (float)opposant.SpecialDefense) / 50) * multiplicateurType;
   
            return CalculerDamageMinimal(damage);
        }
        /// <summary>
        /// Fonction qui affiche un message d'effet si le multiplicateur de type est différent de 100%.
        /// </summary>
        /// <param name="multiplicateurType">Le multiplicateur par rapport au type, 1.00 étant 100%</param>
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

        /// <summary>
        /// Fonction pour que le damage soit égal à 1 s'il est plus petit que 1 mais non égal à 0.
        /// </summary>
        /// <param name="damage">Les points de damage à arrondir</param>
        /// <returns></returns>
        private int CalculerDamageMinimal(float damage)
        {
            int damageInt = (int)(damage * 100);
            if (damageInt < 100 && damageInt != 0)
                damage = 1;
            return (int)damage;
        }
    }
}
