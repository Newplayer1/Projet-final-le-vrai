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
using AtelierXNA.Classes_Pokemon_Skyrim;

namespace AtelierXNA
{
        enum �tatsJeu { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float �CHELLE_OBJET = 0.004f;
        const int POKEDEX_MAX = 35;
        Trainer LeJoueur { get; set; }
        InputManager GestionInput { get; set; }
        AccessBaseDeDonn�e DataBase { get; set; }

        TerrainAvecBase TerrainDeJeu { get; set; }
        Pokemon PokemonRandom1 { get; set; }
        Pokemon test { get; set; }
        �tatsJeu �tatJeu { get; set; }
        Random g�n�rateurAl�atoire { get; set; }
        List<Pokemon> PokemonSurLeTerrain { get; set; }

        public Jeu(Game game, int choix)
           : base(game)
        {
            DataBase = new AccessBaseDeDonn�e();
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI / 4, 0);
            Vector3 positionCPU = new Vector3(96, 18f, -30);
            LeJoueur = new Trainer(Game, "09/09", �CHELLE_OBJET, rotationObjet, positionCPU, INTERVALLE_MAJ_STANDARD, 1f);
            LeJoueur.PokemonsDansLesMains = new List<int>();

            LeJoueur.PokemonsDansLesMains.Add(choix);
            PokemonSurLeTerrain = new List<Pokemon>();
        }
        public Jeu(Game game,List<string> Sauvegarde)
            : base(game)
        {
            DataBase = new AccessBaseDeDonn�e();
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI / 4, 0);
            LeJoueur = new Trainer(Game, "09/09", �CHELLE_OBJET, rotationObjet, new Vector3(float.Parse(DataBase.LoadSauvegarde()[0]), float.Parse(DataBase.LoadSauvegarde()[1]), float.Parse(DataBase.LoadSauvegarde()[2])), INTERVALLE_MAJ_STANDARD, 1f);
            PokemonSurLeTerrain = new List<Pokemon>();
        }
        public override void Initialize()
        {
            g�n�rateurAl�atoire = new Random();

            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            //Vector3 positionObjet = new Vector3(100, 20, -100);

            �tatJeu = �tatsJeu.JEU3D;
            //LoadSauvegarde();
            Game.Components.Add(new Arri�rePlan(Game, "BackGroundNuage"));
            Game.Components.Insert(Game.Components.Count - 1, new Afficheur3D(Game));
            TerrainDeJeu = new TerrainAvecBase(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 17, 256), "TerrainPokemon", "D�tailsTerrain", 5 ,INTERVALLE_MAJ_STANDARD);
            Game.Components.Insert(Game.Components.Count - 1, TerrainDeJeu);
            Game.Services.AddService(typeof(TerrainAvecBase), TerrainDeJeu);
            Game.Components.Add(LeJoueur);
            Game.Services.AddService(typeof(Trainer), LeJoueur);
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }
        public override void Update(GameTime gameTime)
        {
            G�rerClavier();
            //G�rerTransition();
            G�rer�tat();
            if(Game.Components.Count < 20)
            {
                if(Game.Components.Count(p=> p is Afficheur3D)==2)
                Game.Components.Insert(Game.Components.Count - 1,new Afficheur3D(Game));
            AjoutPokemonsRandom();
            }
            base.Update(gameTime);
        }

        private void G�rerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Enter) || GestionInput.EstNouvelleTouche(Keys.Escape))
            {
                UploadSauvegarde();
            }            
        }

        private void UploadSauvegarde()
        {
            List<string> Sauvegarde = new List<string>();
            Sauvegarde.Add(LeJoueur.Position.X.ToString());
            Sauvegarde.Add(LeJoueur.Position.Y.ToString());
            Sauvegarde.Add(LeJoueur.Position.Z.ToString());

            DataBase.Sauvegarder(Sauvegarde);
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

            PokemonRandom1 = new Pokemon(Game, 1, 1, TrouverAl�atoire(), �CHELLE_OBJET, new Vector3(0, 0, 0), TrouverPositionRandom());
            //Game.Services.AddService(typeof(Pokemon), PokemonRandom1);
            Game.Components.Insert(Game.Components.Count - 1, PokemonRandom1);
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
            float positionRandomX = g�n�rateurAl�atoire.Next(-TerrainDeJeu.NbColonnes / 2, TerrainDeJeu.NbColonnes/2);
            float positionRandomY = g�n�rateurAl�atoire.Next(-TerrainDeJeu.NbRang�es / 2, TerrainDeJeu.NbRang�es/ 2);

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
                    //case �tats.COMBAT:
                    //    Combat();
                    //    break;
                    //case �tats.MAISON:
                    //    G�rerCollision();
                    //    G�rerVitesseD�placement();
                    //    G�rerComputer();
                    //    break;
                    //case �tats.GYM:
                    //    G�rerVitesseD�placement();
                    //    G�rerComputer();
                    //    G�rerCombat();
                    //    break;
                    //default: //�tats.FIN:
                    //    Fin();
                    //    SauvegardeAuto();
                    //    break;
            }
        }

        private void G�rerCollision()
        {
            foreach (Pokemon p in Game.Components.Where(r => r is Pokemon))
            {
                if (LeJoueur.EstEnCollision(p))
                {
                    Game.Exit();
                }
            }
        
        }
    }
}
