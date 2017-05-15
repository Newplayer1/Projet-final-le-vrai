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
        AccessBaseDeDonn�e Database { get; set; }
        Random G�n�rateur { get; set; }
        Vector2 PositionBox { get; set; }
        BattleMenu MainMenu { get; set; }
        
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
        public bool GetPokemonEstChang� => MainMenu.Pok�monChang�;

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
            }
        }
        
        /// <summary>
        /// Constructeur d'un combat entre le joueur et un Trainer adversaire.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="positionBox">La position de la boite d'affichage du texte</param>
        /// <param name="user">Le joueur (de type Player.cs)</param>
        /// <param name="opponent">L'adversaire (de type Trainer.cs)</param>
        /// <param name="intervalMAJ">L'intervalle de mise � jour du combat</param>
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
        /// Constructeur d'un combat entre le joueur et un Pok�mon sauvage.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="positionBox">La position de la boite d'affichage du texte</param>
        /// <param name="user">Le joueur (de type Player.cs)</param>
        /// <param name="opponent">L'adversaire (de type Pokemon.cs)</param>
        /// <param name="intervalMAJ">L'intervalle de mise � jour du combat</param>
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

            G�n�rateur = new Random();
            MainMenu = new BattleMenu(Game, PositionBox, new Vector2(Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage), IntervalMAJ, UserPokemon, UserTrainer);
            Game.Components.Add(MainMenu);
            
            AfficherMessagesInitialisation();
            AjouterLesTextesFixes();
            
            MainMenu.BattleMenuState = BattleMenuState.MAIN;
            CombatState = CombatState.BATTLE_MENU;
            base.Initialize();
        }
        /// <summary>
        /// Fonction qui affiche les messages du d�but d'un combat.
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
        /// Fonction qui ajoute les textes de vie et de nom des pok�mons en combat.
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
                G�rerTransitions();
        }

        

        #region G�rerTransition
            /// <summary>
            /// Fonction de l'update qui g�re les transitions entre les �tats et ce que ceux-ci font.
            /// </summary>
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
                default:
                    CombatState = CombatState.BATTLE_MENU;
                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
                    break;
            }
        }
        /// <summary>
        /// Fonction de transition qui attend que le joueur fasse son choix dans le menu du combat.
        /// </summary>
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
        /// <summary>
        /// Fonction de transition qui fait l'action choisie et renvoie � un autre �tat selon le cas.
        /// </summary>
        void G�rerTransitionIN_BATTLE()
        {
            if (MainMenu.ItemPokeballEstUtilis�)
                LancerUnePokeball();
            else if (MainMenu.TentativeFuite)
                EssayerFuir();
            else if (MainMenu.Pok�monChang�)
            {
                MainMenu.Pok�monChang� = false;
                ChangerUserPok�mon(MainMenu.Num�roChoisi);
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
        /// Fonction qui fait les attaques entre les deux Pok�mons et qui s'assure qu'ils sont encore en vie.
        /// </summary>
        void EffectuerLeRound()
        {
            if (UserPokemon.Speed >= OpponentPokemon.Speed)
            {
                if (!TourOpponentCompl�t� && OpponentPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_OPPONENT;

                if (!OpponentPokemon.EstEnVie || !UserPokemon.EstEnVie)//Parce que le combat pourrait finir entre les attaques des deux pok�mons
                    CombatState = CombatState.VERIFY_OUTCOME;

                if (!TourUserCompl�t� && UserPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_USER;
            }
            else
            {
                if (!TourUserCompl�t� && UserPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_USER;

                if (!OpponentPokemon.EstEnVie || !UserPokemon.EstEnVie)
                    CombatState = CombatState.VERIFY_OUTCOME;

                if (!TourOpponentCompl�t� && OpponentPokemon.EstEnVie)
                    CombatState = CombatState.TOUR_OPPONENT;
            }
        }
        /// <summary>
        /// Fonction qui s'effectue quand le joueur s�lectionne l'objet "Pokeball".
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
        /// Fonction pour tenter d'attraper un Pok�mon sauvage qui peut servir aussi hors combat.
        /// </summary>
        /// <param name="joueur">Le trainer qui tente d'attraper le Pok�mon</param>
        /// <param name="opponent">Le Pok�mon que l'on veut attraper</param>
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
        /// <summary>
        /// Fonction qui v�rifie si le Pok�mon s'est fait attraper ou non selon la formule originale.
        /// </summary>
        /// <param name="opponent">Le Pok�mon que le joueur tente d'attraper</param>
        /// <returns>La valeur s'il est attrap� ou non</returns>
        bool EffectuerFormuleGenI(Pokemon opponent)
        {
            bool estAttrap� = false;

            if (!estAttrap�)
            {
                int m = G�n�rateur.Next(0, 256);
                int f = (opponent.MaxHp * opponent.CatchRate * 4) / (opponent.HP * 12);
                //La possibilit� de division enti�re est voulue, la source de l'algorithme le sp�cifie.
                if (f >= m)
                    estAttrap� = true;
            }
            return estAttrap�;
        }

        /// <summary>
        /// Fonction de transition qui fait le tour du joueur et renvoie � l'�tat IN_BATTLE pour compl�ter le tour.
        /// </summary>
        void G�rerTransitionTOUR_USER()
        {
            GamePad.SetVibration(PlayerIndex.One, 1, 0);
            NomOpponentPokemon.Visible = true;
            VieOpponentPokemon.Visible = true;
            if (UserPokemon.EstEnVie)
            {
                EffectuerTourUser(MainMenu.Num�roChoisi);
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
                    ChangerUserPok�mon(MainMenu.Num�roChoisi);
                    MainMenu.BattleMenuState = BattleMenuState.MAIN;
                    CombatState = CombatState.BATTLE_MENU;
                }
            }
        }
        /// <summary>
        /// Fonction de transition qui fait le tour de l'adversaire et renvoie � l'�tat IN_BATTLE pour compl�ter le tour.
        /// </summary>
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
        /// <summary>
        /// Fonction de transition qui v�rifie l'�tat du combat, s'il doit finir, continuer et comment proc�der si l'on continue.
        /// </summary>
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

                    MainMenu.BackLock = true; //Pour forcer le joueur � changer de pokemon sans pouvoir revenir dans le menu.
                    MainMenu.BattleMenuState = BattleMenuState.POKEMON;
                    CombatState = CombatState.TOUR_USER;
                }
            }
        }
        /// <summary>
        /// Fonction de transition de victoire qui m�ne � la fin du combat.
        /// </summary>
        void G�rerTransitionVICTORY()
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
        /// Fonction pour attribuer les points d'exp�rience au Pok�mon du joueur.
        /// </summary>
        private void DonnerExp()
        {
            float exp = OpponentPokemon.GiveExp();
            UserPokemon.GainExp(exp);
        }
        /// <summary>
        /// Fonction de transition de d�faite qui m�ne � la fin du combat.
        /// </summary>
        void G�rerTransitionDEFEAT()
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
        void G�rerTransitionEND()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);

            UserTrainer.MettreEnPremi�rePosition(UserPokemon);
            EnCombat = false;
            �D�truire = true;
        }
        #endregion

        /// <summary>
        /// Fonction qui affiche le message de l'attaque al�atoire de l'adversaire et l'effectue.
        /// </summary>
        void EffectuerTourOpponent()
        {
            int nbAl�atoire = G�n�rateur.Next(0, OpponentPokemon.NbAttaques);
            string pr�fixe = "";
            if (EstOpponentSauvage)
                pr�fixe = "Wild ";
            else
                pr�fixe = "Foe ";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, pr�fixe + OpponentPokemon.Nom + " used " + OpponentPokemon[nbAl�atoire].ToString() + "!", IntervalMAJ);
            Game.Components.Add(message);
            EffectuerAttaque(OpponentPokemon, UserPokemon, OpponentPokemon[nbAl�atoire]);
        }
        /// <summary>
        /// Fonction qui affiche le message de l'attaque choisie par le joueur et l'effectue.
        /// </summary>
        /// <param name="attaqueS�lectionn�e">Num�ro de l'attaque s�lectionn�e</param>
        void EffectuerTourUser(int attaqueS�lectionn�e)
        {
            string messageTour = UserPokemon.Nom + " used " + UserPokemon[attaqueS�lectionn�e].ToString() + "!";
            AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, messageTour, IntervalMAJ);
            Game.Components.Add(message);
            EffectuerAttaque(UserPokemon, OpponentPokemon, UserPokemon[attaqueS�lectionn�e]);
        }

        /// <summary>
        /// Fonction qui change le pok�mon du joueur.
        /// </summary>
        /// <param name="pok�monS�lectionn�">Le num�ro en inventaire du pok�mon choisi</param>
        void ChangerUserPok�mon(int pok�monS�lectionn�)
        {
            UserPokemon = UserTrainer[pok�monS�lectionn�];

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
        /// Fonction qui change le Pok�mon de l'adversaire s'il lui en reste qui sont vivants.
        /// </summary>
        void ChangerOpponentPokemon()
        {
            TourOpponentCompl�t� = false;
            TourUserCompl�t� = false;
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
        /// Fonction qui sert � d�terminer si le joueur peut fuir le combat ou non.
        /// </summary>
        void EssayerFuir()
        {
            MainMenu.TentativeFuite = false;
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

        /// <summary>
        /// Fonction qui ex�cute l'attaque entre deux Pok�mons.
        /// </summary>
        /// <param name="attaquant">Le Pok�mon qui attaque</param>
        /// <param name="opposant">Le Pok�mon qui se fait attaquer</param>
        /// <param name="attaqueChoisie">L'attaque qui a �t� choisie</param>
        void EffectuerAttaque(Pokemon attaquant, Pokemon opposant, Attaque attaqueChoisie)
        {
            int nombreAl�atoire = G�n�rateur.Next(0, 101);
            if (nombreAl�atoire <= attaqueChoisie.Accuracy)//attaque!
            {
                //on va faire une attaque classique, ensuite on applique pardessus l'effet plus complexe peu importe l'attaque
                if (attaqueChoisie.EstUneAttaqueSp�ciale() && attaqueChoisie.EstUneAttaqueAvecBasePowerValide())
                    opposant.D�fendre(CalculPointsDamageSp�cial(attaquant, opposant, attaqueChoisie));

                else if (attaqueChoisie.EstUneAttaquePhysique() && attaqueChoisie.EstUneAttaqueAvecBasePowerValide())
                    opposant.D�fendre(CalculPointsDamagePhysique(attaquant, opposant, attaqueChoisie));

                Attaque.AppliquerEffetAttaque(attaquant, opposant, attaqueChoisie);
            }
            else
            {
                AfficheurTexte message = new AfficheurTexte(Game, PositionBox, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, "It missed!", IntervalMAJ);
                Game.Components.Add(message);
            }
        }
        /// <summary>
        /// Calcul des points de damage si l'attaque utilise des facult�s "Physiques".
        /// </summary>
        /// <param name="attaquant">Le Pok�mon qui attaque</param>
        /// <param name="opposant">Le Pok�mon qui se fait attaquer</param>
        /// <param name="attaqueChoisie">L'attaque qui a �t� choisie</param>
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
        /// Calcul des points de damage si l'attaque utilise des facult�s "Sp�ciales".
        /// </summary>
        /// <param name="attaquant">Le Pok�mon qui attaque</param>
        /// <param name="opposant">Le Pok�mon qui se fait attaquer</param>
        /// <param name="attaqueChoisie">L'attaque qui a �t� choisie</param>
        /// <returns>Les points de damage</returns>
        int CalculPointsDamageSp�cial(Pokemon attaquant, Pokemon opposant, Attaque attaqueChoisie)
        {
            float damage;

            float multiplicateurType = attaqueChoisie.GetTypeMultiplier(opposant.Type1, opposant.Type2);
            AfficherMessageMultiplicateur(multiplicateurType);

            damage = ((2 * attaquant.Level / 5 + 2) * attaqueChoisie.Power * (attaquant.SpecialAttack / (float)opposant.SpecialDefense) / 50) * multiplicateurType;
   
            return CalculerDamageMinimal(damage);
        }
        /// <summary>
        /// Fonction qui affiche un message d'effet si le multiplicateur de type est diff�rent de 100%.
        /// </summary>
        /// <param name="multiplicateurType">Le multiplicateur par rapport au type, 1.00 �tant 100%</param>
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
        /// Fonction pour que le damage soit �gal � 1 s'il est plus petit que 1 mais non �gal � 0.
        /// </summary>
        /// <param name="damage">Les points de damage � arrondir</param>
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
