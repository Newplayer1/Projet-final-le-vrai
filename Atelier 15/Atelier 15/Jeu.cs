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
        enum ÉtatsJeu { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float ÉCHELLE_OBJET = 0.04f;

        TerrainAvecBase TerrainDeJeu { get; set; }
        Pokemon PokemonRandom1 { get; set; }
        ÉtatsJeu ÉtatJeu { get; set; }
        Random générateurAléatoire { get; set; }
        //String[] TexturesTerrain = new string[] { "Sable", "HerbeB" };
        public Jeu(Game game)
            : base(game)
        {

        }
        public override void Initialize()
        {
            const float ÉCHELLE_OBJET = 0.004f;
            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            Vector3 positionCPU = new Vector3(96, 18f, -30);
            //Vector3 positionObjet = new Vector3(100, 20, -100);
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI/4, 0);
            


            //LoadSauvegarde();
            Game.Components.Add(new ArrièrePlan(Game, "BackGroundNuage"));
            Game.Components.Insert(Game.Components.Count - 1, new Afficheur3D(Game));
            TerrainDeJeu = new TerrainAvecBase(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 17, 256), "TerrainPokemon", "DétailsTerrain", 5 ,INTERVALLE_MAJ_STANDARD);
            Game.Components.Insert(Game.Components.Count - 1, TerrainDeJeu);
            Game.Services.AddService(typeof(TerrainAvecBase), TerrainDeJeu);
            Game.Components.Add(new Trainer(Game, "03/03", ÉCHELLE_OBJET, rotationObjet, positionCPU, INTERVALLE_MAJ_STANDARD, 1f));
            Game.Services.AddService(typeof(Trainer), new Trainer(Game, "03/03", ÉCHELLE_OBJET, rotationObjet, positionCPU, INTERVALLE_MAJ_STANDARD, 1f));
        }
        public override void Update(GameTime gameTime)
        {
            //GérerClavier();
            GérerTransition();
            GérerÉtat();
            if(Game.Components.Count < 20)
            {
                if(Game.Components.Count(p=> p is Afficheur3D)==2)
                Game.Components.Insert(Game.Components.Count - 1,new Afficheur3D(Game));
            AjoutPokemonsRandom();
            }
            base.Update(gameTime);
        }
        private void GérerTransition()
        {
            switch (ÉtatJeu)
            {
                case ÉtatsJeu.PAGE_TITRE:
                    //GérerTransitionPageTitre();
                    break;
                //case États.JEU3D:
                //    GérerTransitionJEU3D();
                //    break;
                //case États.COMBAT:
                //    GérerTransitionCombat();
                //    break;
                //case États.MAISON:
                //    GérerTransitionMaison();
                //    break;
                //case États.GYM:
                //    GérerTransitionGym();
                //    break;
                //case États.FIN:
                //    GérerTransitionFin();
                //    break;
                //default:
                //    break;
            }
        }
        private void AjoutPokemonsRandom()
        {
            générateurAléatoire = new Random();

            PokemonRandom1 = new Pokemon(Game, 1, 1, TrouverAléatoire(), ÉCHELLE_OBJET, new Vector3(0, 0, 0), TrouverPositionRandom());
            //Game.Services.AddService(typeof(Pokemon), PokemonRandom1);
            Game.Components.Insert(Game.Components.Count - 1, PokemonRandom1);
        }
        private string TrouverAléatoire()
        {
            int unNombre = générateurAléatoire.Next(1, 35);
            string local = unNombre.ToString();
            if (local.Count() == 1)
            {
                local = "0" + local;
            }
            string nomMod = local + '/' + local;
            return nomMod;

            //float échelleObjTest = 0.01f;

            //if (local1 == 14.ToString())
            //{
            //    échelleObjTest = ÉCHELLE_OBJET * 10;
            //}
        }
        private Vector3 TrouverPositionRandom()
        {
            float positionRandomX = générateurAléatoire.Next(1, TerrainDeJeu.NbColonnes);
            float positionRandomY = générateurAléatoire.Next(1, TerrainDeJeu.NbRangées);

            positionRandomX = MathHelper.Max(MathHelper.Min(positionRandomX, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);
            positionRandomY = MathHelper.Max(MathHelper.Min(positionRandomY, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);

            Vector2 position = new Vector2(positionRandomX + TerrainDeJeu.NbColonnes / 2, positionRandomY + TerrainDeJeu.NbRangées / 2);

            float hauteur = (TerrainDeJeu.GetPointSpatial((int)Math.Round(position.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(position.Y, 0)) + Vector3.Zero).Y;

            return new Vector3(positionRandomX, hauteur, positionRandomY);

        }
        private void GérerÉtat()
        {
            switch (ÉtatJeu)
            {
                case ÉtatsJeu.PAGE_TITRE:
                    Game.Components.Add(new PageTitre(Game));
                    break;
                //case États.JEU3D:
                //    GérerCollision();
                //    GérerCombat();
                //    GérerComputer();
                //    break;
                //case États.COMBAT:
                //    Combat();
                //    break;
                //case États.MAISON:
                //    GérerCollision();
                //    GérerVitesseDéplacement();
                //    GérerComputer();
                //    break;
                //case États.GYM:
                //    GérerVitesseDéplacement();
                //    GérerComputer();
                //    GérerCombat();
                //    break;
                //default: //États.FIN:
                //    Fin();
                //    SauvegardeAuto();
                //    break;
            }
        }
    }
}
