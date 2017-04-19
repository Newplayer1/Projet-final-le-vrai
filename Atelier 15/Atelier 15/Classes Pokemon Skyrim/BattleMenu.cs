//Gabriel Paquette, 5 Avril 2017
//Modifications 5 Avril 2017: cr��, ajout des �tats et switch/case
//�tat: En cours, non complet 
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
    public enum BattleMenuState { MAIN, FIGHT, POKEMON, BAG, RUN, READY }
    public class BattleMenu : Microsoft.Xna.Framework.DrawableGameComponent, IDestructible
    {
        public BattleMenuState BattleMenuState { get; set; }
        float IntervalMAJ { get; set; }
        Vector2 Dimensions { get; set; }
        Vector2 Position { get; set; }

        InputManager GestionInput { get; set; }

        AfficheurChoix MainChoix { get; set; }
        AfficheurChoix FightChoix { get; set; }
        AfficheurChoix BagChoix { get; set; }
        AfficheurChoix PokemonChoix { get; set; }
        AfficheurTexte Message { get; set; }
        List<string> ListeChoix { get; set; }
        public List<DrawableGameComponent> ComposantesBattleMenu { get; set; }
        public bool AttaqueUtilis�e  { get; set; }
        public bool ItemUtilis� { get; set; }
        public bool Pok�monChang� { get; set; }
        public bool TentativeFuite { get; set; }
        public int Num�roChoisi { get; private set; }

        /*
         Pokemon UserPokemon { get; set; }
         Trainer UserTrainer { get; set; }
        */

            public static bool Wait { get; set; }
        public static bool EnCombat { get; set; }

        bool �D�truire;
        public bool �D�truire
        {
            get
            {
                return �D�truire;
            }
            set //Pas trop s�r du set
            {
                �D�truire = value;
                foreach (IDestructible item in ComposantesBattleMenu)
                {
                    item.�D�truire = value;
                }
            }
        }

        static BattleMenu()
        {
            Wait = false;
            EnCombat = false;
        }
        public BattleMenu(Game game, Vector2 position, Vector2 dimensions, float intervalMAJ/*, Pokemon userPokemon, Trainer userTrainer*/)//Tout d'abord on a un afficheur de choix pour le user, qui va en branches avec switch case
            : base(game)
        {
            /*
             UserPokemon = userPokemon;
             UserTrainer = userTrainer;
            */
            Dimensions = dimensions;
            Position = position;
            IntervalMAJ = intervalMAJ;
            EnCombat = true;
        }
        public override void Initialize()
        {
            Num�roChoisi = 0;
            InitialiserMainChoix();
            InitialiserFightChoix();
            InitialiserBagChoix();
            InitialiserPokemonChoix();

            Wait = AfficheurTexte.MessageEnCours;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;

            ComposantesBattleMenu = new List<DrawableGameComponent>();
            ComposantesBattleMenu.Add(MainChoix);
            ComposantesBattleMenu.Add(FightChoix);
            ComposantesBattleMenu.Add(BagChoix);
            ComposantesBattleMenu.Add(PokemonChoix);
            DisableComponents();
            foreach (DrawableGameComponent item in ComposantesBattleMenu)
            {
                Game.Components.Add(item);
            }
            //MainChoix.Enabled = true;
            //MainChoix.Visible = true;
            BattleMenuState = BattleMenuState.MAIN;
            base.Initialize();
        }
        
        private void InitialiserMainChoix()
        {
            ListeChoix = new List<string>();
            ListeChoix.Add("FIGHT");
            ListeChoix.Add("BAG");
            ListeChoix.Add("POKEMON");
            ListeChoix.Add("RUN");
            MainChoix = new AfficheurChoix(Game, Position, (int)Dimensions.X, ListeChoix.Count + 2, ListeChoix, IntervalMAJ);
        }
        private void InitialiserFightChoix()
        {
            ListeChoix = new List<string>();
            ListeChoix.Add("Attaque 1");//UserPokemon[0].toString()     //Mettre en boucle (on devra override attaque.tostring)
            ListeChoix.Add("Attaque 2");//UserPokemon[1].toString()
            ListeChoix.Add("Attaque 3");//UserPokemon[2].toString()
            ListeChoix.Add("Attaque 4");//UserPokemon[3].toString()
            FightChoix = new AfficheurChoix(Game, Position, (int)Dimensions.X, ListeChoix.Count + 2, ListeChoix, IntervalMAJ);
        }
        private void InitialiserPokemonChoix()
        {
            ListeChoix = new List<string>();
            ListeChoix.Add("Pok<mon 1");//UserTrainer[0].toString()     //Mettre en boucle (on devra override pokemon.tostring)
            ListeChoix.Add("Pok<mon 2");//UserTrainer[1].toString()     On va devoir pouvoir les modifier, pour le hp/level
            ListeChoix.Add("Pok<mon 3");//UserTrainer[2].toString()
            ListeChoix.Add("Pok<mon 4");//UserTrainer[3].toString()
            ListeChoix.Add("Pok<mon 5");//UserTrainer[4].toString()
            ListeChoix.Add("Pok<mon 6");//UserTrainer[5].toString()
            PokemonChoix = new AfficheurChoix(Game, new Vector2(Position.X + Cadre.TAILLE_TILE * 9, Position.Y - Cadre.TAILLE_TILE * 2), (int)Dimensions.X - 9, ListeChoix.Count + 2, ListeChoix, IntervalMAJ);

        }

        private void InitialiserBagChoix()//Un trainer peut pas avoir plus que 6 sortes d'items sur lui? (+ simple pour afficher)
        {
            ListeChoix = new List<string>();
            ListeChoix.Add("Item 1");//UserTrainerBag[0].toString()     //Mettre en boucle (on devra override pokemon.tostring)
            ListeChoix.Add("Item 2");//UserTrainerBag[1].toString()
            ListeChoix.Add("Item 3");//UserTrainerBag[2].toString()
            ListeChoix.Add("Item 4");//UserTrainerBag[3].toString()
            ListeChoix.Add("Item 5");//UserTrainerBag[4].toString()
            ListeChoix.Add("Item 6");//UserTrainerBag[5].toString()
            BagChoix = new AfficheurChoix(Game, new Vector2(Position.X + Cadre.TAILLE_TILE * 9, Position.Y - Cadre.TAILLE_TILE * 2), (int)Dimensions.X - 9, ListeChoix.Count + 2, ListeChoix, IntervalMAJ);

        }

        public override void Update(GameTime gameTime)
        {
            //Wait = AfficheurTexte.MessageEnCours;
            if (!Wait)
                G�rerTransitions();
            //G�rer�tats();
            Wait = AfficheurTexte.MessageEnCours;
            base.Update(gameTime);
        }
        //void G�rer�tats() //ici on ex�cute ce que l'on fait � un tel �tat
        //{
        //    switch (BattleMenuState)
        //    {
        //        case BattleMenuState.MAIN:
        //            G�rerMAIN();
        //            break;
        //        case BattleMenuState.FIGHT:
        //            G�rerFIGHT();
        //            break;
        //        case BattleMenuState.POKEMON:
        //            G�rerPOKEMON();
        //            break;
        //        case BattleMenuState.BAG:
        //            G�rerBAG();
        //            break;
        //        case BattleMenuState.RUN:
        //            G�rerRUN();
        //            break;
        //        case BattleMenuState.READY:
        //            G�rerREADY();
        //            break;
        //    }
        //}
        //private void G�rerMAIN()
        //{
        //    throw new NotImplementedException();
        //}
        //private void G�rerFIGHT()
        //{
        //    throw new NotImplementedException();
        //}
        //private void G�rerPOKEMON()
        //{
        //    throw new NotImplementedException();
        //}
        //private void G�rerBAG()
        //{
        //    throw new NotImplementedException();
        //}
        //private void G�rerRUN()
        //{
        //    throw new NotImplementedException();
        //}
        //private void G�rerREADY()
        //{
        //    throw new NotImplementedException();
        //}

        void G�rerTransitions() //ici on v�rifie la condition qui change l'�tat (et on le change au besoin) (rendre visible et enabled)
        {
            switch (BattleMenuState)
            {
                case BattleMenuState.MAIN:
                    G�rerTransitionMAIN();
                    break;
                case BattleMenuState.FIGHT:
                    G�rerTransitionFIGHT();
                    break;
                case BattleMenuState.POKEMON:
                    G�rerTransitionPOKEMON();
                    break;
                case BattleMenuState.BAG:
                    G�rerTransitionBAG();
                    break;
                case BattleMenuState.RUN:
                    G�rerTransitionRUN();
                    break;
                case BattleMenuState.READY:
                    G�rerTransitionREADY();
                    break;
            }
        }

        private void G�rerTransitionMAIN()
        {
            MainChoix.Visible = true;
            MainChoix.Enabled = true;

            if (ChoixEstEffectu�()) //On a pes� sur A
            {
                DisableComponents(); //On d�sactive tout parce que quand on arrive ailleur, on active ce dont on a besoin
                
                if (MainChoix.IndexS�lectionn� == 0)
                {
                    //MainChoix.Visible = true;? (on pourrait garder le menu principal visible si l'on peut le voir)
                    BattleMenuState = BattleMenuState.FIGHT;
                }
                if (MainChoix.IndexS�lectionn� == 1)
                {
                    //MainChoix.Visible = true;? (on pourrait garder le menu principal visible si l'on peut le voir)
                    BattleMenuState = BattleMenuState.BAG;
                }
                if (MainChoix.IndexS�lectionn� == 2)
                {
                    BattleMenuState = BattleMenuState.POKEMON;
                }
                if (MainChoix.IndexS�lectionn� == 3)
                {
                    BattleMenuState = BattleMenuState.RUN;
                }

            }
        }

        private void G�rerTransitionFIGHT()
        {
            MainChoix.Visible = true;
            FightChoix.Visible = true;
            FightChoix.Enabled = true;

            if (ChoixEstEffectu�()) //On a pes� sur A
            {
                DisableComponents();
                AttaqueUtilis�e = true;
                Num�roChoisi = FightChoix.IndexS�lectionn�;

                if (FightChoix.IndexS�lectionn� == 0)
                {
                    //AttaqueChoisie = UserPokemon[0]; (.ToInt?)
                    //AfficheurTexte message = new AfficheurTexte(Game, Position, (int)Dimensions.X, (int)Dimensions.Y, "temp: Pokemon used attack 0!", IntervalMAJ);
                    //Game.Components.Add(message); On doit pas faire �a ici. C'est au Combat de g�rer les messages et tout, le menu principal c'est juste pour choisir
                    
                    BattleMenuState = BattleMenuState.READY;
                    //BattleMenuState = BattleMenuState.MAIN;
                }
                if (FightChoix.IndexS�lectionn� == 1)
                {
                    //AttaqueChoisie = UserPokemon[1]; (.ToInt?)

                    //AfficheurTexte message = new AfficheurTexte(Game, Position, (int)Dimensions.X, (int)Dimensions.Y, "temp: Pokemon used attack 1!", IntervalMAJ);
                    //Game.Components.Add(message);
                    BattleMenuState = BattleMenuState.READY;
                    //BattleMenuState = BattleMenuState.MAIN;
                }
                if (FightChoix.IndexS�lectionn� == 2)
                {
                    //AttaqueChoisie = UserPokemon[2]; (.ToInt?)

                    //AfficheurTexte message = new AfficheurTexte(Game, Position, (int)Dimensions.X, (int)Dimensions.Y, "temp: Pokemon used attack 2!", IntervalMAJ);
                    BattleMenuState = BattleMenuState.READY;
                    //Game.Components.Add(message);
                    //BattleMenuState = BattleMenuState.MAIN;
                }
                if (FightChoix.IndexS�lectionn� == 3)
                {
                    //AttaqueChoisie = UserPokemon[3]; (.ToInt?)

                    //AfficheurTexte message = new AfficheurTexte(Game, Position, (int)Dimensions.X, (int)Dimensions.Y, "temp: Pokemon used attack 3!", IntervalMAJ);
                    //Game.Components.Add(message);
                    BattleMenuState = BattleMenuState.READY;
                    //BattleMenuState = BattleMenuState.MAIN;
                }
            }

            if (Back()) //On a pes� sur B
            {
                DisableComponents();
                BattleMenuState = BattleMenuState.MAIN;
            }
        }

        private void G�rerTransitionPOKEMON()
        {
            MainChoix.Visible = true;
            PokemonChoix.Visible = true;
            PokemonChoix.Enabled = true;
            if (ChoixEstEffectu�()) //On a pes� sur A
            {
                DisableComponents();
                Pok�monChang� = true;
                Num�roChoisi = FightChoix.IndexS�lectionn�;
                //Combat.SwithPokemon(UserTrainer, PokemonChoix.IndexS�lectionn�);

                //AfficheurTexte message = new AfficheurTexte(Game, Position, (int)Dimensions.X, (int)Dimensions.Y, "temp: User switched pokemon!", IntervalMAJ);
                //Game.Components.Add(message);
                BattleMenuState = BattleMenuState.READY;
                //BattleMenuState = BattleMenuState.MAIN;
            }
            if (Back()) //On a pes� sur B
            {
                DisableComponents();
                BattleMenuState = BattleMenuState.MAIN;
            }
        }

        private void G�rerTransitionBAG()
        {
            MainChoix.Visible = true;
            BagChoix.Visible = true;
            BagChoix.Enabled = true;
            if (ChoixEstEffectu�()) //On a pes� sur A
            {
                DisableComponents();
                ItemUtilis� = true;
                Num�roChoisi = FightChoix.IndexS�lectionn�;
                //Combat.UseItem(); Pok�mon sur lequel il y a un effet, Trainer qui a utilis� l'item, num�ro de l'item
                BattleMenuState = BattleMenuState.READY;
                //AfficheurTexte message = new AfficheurTexte(Game, Position, (int)Dimensions.X, (int)Dimensions.Y, "temp: User threw an item!", IntervalMAJ);
                //Game.Components.Add(message);
                //BattleMenuState = BattleMenuState.MAIN;
            }
            if (Back()) //On a pes� sur B
            {
                DisableComponents();
                BattleMenuState = BattleMenuState.MAIN;
            }
        }

        private void G�rerTransitionRUN()
        {
            DisableComponents();
            //if (!Combat.EstOpponentSauvage)
            //{
            //    //There's no running from a trainer battle!!
            //    AfficheurTexte message = new AfficheurTexte(Game, Position, (int)Dimensions.X, (int)Dimensions.Y, "There's no running from a trainer battle!", IntervalMAJ);
            //    BattleMenuState = BattleMenuState.MAIN;
            //}
            //else
            //{
            //    //Bool FailedRunning = true;?
            //    Message = new AfficheurTexte(Game, Position, (int)Dimensions.X, (int)Dimensions.Y, "Can't escape!", IntervalMAJ);
            //    Game.Components.Add(Message);

            //    //BattleMenuState = BattleMenuState.READY;
            //    ////BattleMenuState = BattleMenuState.MAIN;
            //    EnCombat = false;
            //    �D�truire = true;//changer pour state = end dans combat
            //    //(dans les tours, on va g�r� selon si le BattleMenu dit qu'on a soit, chang� pokemon, attaqu�, utilis� item ou run)
            //}
            TentativeFuite = true; //le Combat va choisir de l'�tat du battle menu pour revenir au main ou a end
        }

        private void G�rerTransitionREADY()
        {
        }

        bool ChoixEstEffectu�()
        {
            return GestionInput.EstNouvelleTouche(Keys.A) && (!Wait);
        }
        bool Back()
        {
            return GestionInput.EstNouvelleTouche(Keys.B) && (!Wait);
        }
        void DisableComponents()
        {
            foreach (DrawableGameComponent item in ComposantesBattleMenu)
            {
                item.Enabled = false;
                item.Visible = false;
            }
            AttaqueUtilis�e = false;
            ItemUtilis� = false;
            Pok�monChang� = false;
            TentativeFuite = false;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
