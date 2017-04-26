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
using AtelierXNA;

namespace AtelierXNA
{
    enum �tatsJeu { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float �CHELLE_OBJET = 0.004f;
        const int POKEDEX_MAX = 35;
        Vector2 POSITION_BOX_STANDARD = new Vector2(2, 300);
        Player LeJoueur { get; set; }
        InputManager GestionInput { get; set; }
        AccessBaseDeDonn�e Database { get; set; }
        Combat LeCombat { get; set; }
        Cam�raSubjective Cam�raJeu { get; set; }
        TerrainAvecBase TerrainDeJeu { get; set; }
        ObjetDeBase PokemonRandom1 { get; set; }
        Pokemon PokemonRandom1String { get; set; }
        Pokemon_B test { get; set; }
        �tatsJeu �tatJeu { get; set; }
        Random g�n�rateurAl�atoire { get; set; }
        List<ObjetDeBase> PokemonSurLeTerrain { get; set; }
        ObjetDeBase PokemonEnCollision { get; set; }
        int indexPokemonEnCollission;

        public Jeu(Game game, int choix)
           : base(game)
        {
            /*DataBase = new AccessBaseDeDonn�e();*/
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI / 4, 0);
            Vector3 positionCPU = new Vector3(96, 18f, -30);
            LeJoueur = new Player(Game, "El_guapo", �CHELLE_OBJET, rotationObjet, positionCPU, INTERVALLE_MAJ_STANDARD, 1f);
            //LeJoueur.PokemonsDansLesMains = new List<int>();

            //LeJoueur.PokemonsDansLesMains.Add(choix);
            LeJoueur.AddPokemon(choix); //tout les trainers ont une liste de pok�mon, la liste va dans trainer (et faut jamais mettre une liste publique)
            PokemonSurLeTerrain = new List<ObjetDeBase>();
            UploadSauvegarde();
        }
        public Jeu(Game game, List<string> Sauvegarde)
            : base(game)
        {
            /*DataBase = new AccessBaseDeDonn�e();*/
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI / 4, 0);
            LeJoueur = new Player(Game, "El_guapo", �CHELLE_OBJET, rotationObjet, new Vector3(float.Parse(Database.LoadSauvegarde()[0]), float.Parse(Database.LoadSauvegarde()[1]), float.Parse(Database.LoadSauvegarde()[2])), INTERVALLE_MAJ_STANDARD, 1f);
            PokemonSurLeTerrain = new List<ObjetDeBase>();
        }
        public override void Initialize()
        {
            g�n�rateurAl�atoire = new Random();

            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            //Vector3 positionObjet = new Vector3(100, 20, -100);

            �tatJeu = �tatsJeu.JEU3D;
            //LoadSauvegarde();
            Game.Components.Add(new Arri�rePlan(Game, "BackGroundNuage"));
            Game.Components.Add(new Afficheur3D(Game));
            TerrainDeJeu = new TerrainAvecBase(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 17, 256), "TerrainPokemon", "D�tailsTerrain", 5, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(TerrainDeJeu);
            Game.Services.AddService(typeof(TerrainAvecBase), TerrainDeJeu);
            Game.Components.Add(LeJoueur);
            Game.Services.AddService(typeof(Player), LeJoueur);
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Cam�raJeu = Game.Services.GetService(typeof(Cam�ra)) as Cam�raSubjective;
        }
        public override void Update(GameTime gameTime)
        {
            G�rerClavier();
            //G�rerTransition();
            G�rer�tat();
            if (Game.Components.Count < 20)
            {
                if (Game.Components.Count(p => p is Afficheur3D) == 2)
                    Game.Components.Add(new Afficheur3D(Game));
                AjoutPokemonsRandom();
            }
            base.Update(gameTime);
        }

        private void G�rerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Enter))
            {
                UploadSauvegarde();
            }

        }

        public void UploadSauvegarde()
        {
            List<string> Sauvegarde = new List<string>();
            Sauvegarde.Add(LeJoueur.Position.X.ToString());
            Sauvegarde.Add(LeJoueur.Position.Y.ToString());
            Sauvegarde.Add(LeJoueur.Position.Z.ToString());

            Database.Sauvegarder(Sauvegarde);
        }

        private void G�rerTransition()
        {
            switch (�tatJeu)
            {
                case �tatsJeu.JEU3D:
                    break;
                    //case �tats.COMBAT:
                    //    G�rerTransitionCombat();
                    //    break;
                    //case �tats.MAISON:
                    //    G�rerTransitionMaison();
                    //    break;
                    //case �tats.GYM:
                    //    G�rerTransitionGym();
                    //    break;
                    //case �tats.FIN:
                    //    G�rerTransitionFin();
                    //    break;
                    //default:
                    //    break;
            }
        }
        private void AjoutPokemonsRandom()
        {
            PokemonRandom1String = new Pokemon(Game, g�n�rateurAl�atoire.Next(1, POKEDEX_MAX));
            PokemonRandom1 = new ObjetDeBase(Game, PokemonRandom1String, TrouverAl�atoire(), �CHELLE_OBJET, Vector3.Zero, TrouverPositionRandom());
            //PokemonRandom1 = new ObjetDeBase(Game, 1, 1, TrouverAl�atoire(), �CHELLE_OBJET, new Vector3(0, 0, 0), TrouverPositionRandom());
            //Game.Services.AddService(typeof(Pokemon), PokemonRandom1);
            Game.Components.Add(PokemonRandom1);
            PokemonSurLeTerrain.Add(PokemonRandom1);
        }
        
        private string TrouverAl�atoire()
        {
            int unNombre = g�n�rateurAl�atoire.Next(1, POKEDEX_MAX);
            string local = unNombre.ToString();
            if (local.Count() == 1)
            {
                local = "0" + local;
            }
            string nomMod = local + '/' + local;
            return nomMod;

            //float �chelleObjTest = 0.01f;

            //if (local1 == 14.ToString())
            //{
            //    �chelleObjTest = �CHELLE_OBJET * 10;
            //}
        }


        private Vector3 TrouverPositionRandom()
        {
            float positionRandomX = g�n�rateurAl�atoire.Next(-TerrainDeJeu.NbColonnes / 2, TerrainDeJeu.NbColonnes / 2);
            float positionRandomY = g�n�rateurAl�atoire.Next(-TerrainDeJeu.NbRang�es / 2, TerrainDeJeu.NbRang�es / 2);

            //positionRandomX = MathHelper.Max(MathHelper.Min(positionRandomX, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);
            //positionRandomY = MathHelper.Max(MathHelper.Min(positionRandomY, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);

            Vector2 position = new Vector2(positionRandomX + TerrainDeJeu.NbColonnes / 2, positionRandomY + TerrainDeJeu.NbRang�es / 2);

            float hauteur = (TerrainDeJeu.GetPointSpatial((int)Math.Round(position.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(position.Y, 0)) + Vector3.Zero).Y;

            return new Vector3(positionRandomX, hauteur, positionRandomY);

        }
        private void G�rer�tat()
        {
            switch (�tatJeu)
            {
                case �tatsJeu.JEU3D:
                    G�rerCollision();
                    break;
                case �tatsJeu.COMBAT:
                    if (!(Game.Components.Contains(Game.Components.Where(c => c is Combat) as Combat)) && !Flags.Combat)
                    {
                        LeJoueur.Visible = false;
                        ObjetDeBase PokemonLancer = new ObjetDeBase(Game,"09/09",�CHELLE_OBJET,new Vector3(0,(float)(16*Math.PI/10),0),new Vector3(LeJoueur.Position.X + 1,LeJoueur.Position.Y, LeJoueur.Position.Z +1 ));
                        Game.Components.Add(new Afficheur3D(Game));
                        Game.Components.Add(PokemonLancer);
                        PokemonEnCollision.Position = new Vector3(PokemonLancer.Position.X -1 , PokemonLancer.Position.Y, PokemonLancer.Position.Z- 1);
                        PokemonSurLeTerrain[indexPokemonEnCollission - 1] = PokemonEnCollision;// CAUSE UNE EXCEPTION (quand l'index = 0, �a crash)
                        Flags.Combat = true;
                        LeCombat = new Combat(Game, POSITION_BOX_STANDARD, LeJoueur, new Pokemon(Game, 5), INTERVALLE_MAJ_STANDARD);
                        Game.Components.Add(LeCombat);
                        //Cam�raJeu.Cible = new Vector3(LeJoueur.Position.X + 3, LeJoueur.Position.Y + 3, LeJoueur.Position.Z);
                        //Cam�raJeu.Cr�erPointDeVue(Cam�raJeu.Position, Cam�raJeu.Cible, Cam�raJeu.OrientationVerticale);
                    }
                    break;
                    //case �tats.GYM:
                    //    G�rerVitesseD�placement();
                    //    G�rerComputer();
                    //    G�rerCombat();
                    //    break;
                    //default: //�tats.FIN:
                    //    Fin();
                    //    SauvegardeAuto();
                    //    break;
                    //
            }
        }

        private void G�rerCollision()
        {
            foreach (ObjetDeBase p in Game.Components.Where(r => r is ObjetDeBase))
            {
                if (!(p is Player))
                {
                    if (LeJoueur.EstEnCollision(p))
                    {
                        indexPokemonEnCollission = PokemonSurLeTerrain.IndexOf(p);
                        PokemonEnCollision = PokemonSurLeTerrain[indexPokemonEnCollission];
                        �tatJeu = �tatsJeu.COMBAT;
                    }
                }
            }

        }
    }
}
