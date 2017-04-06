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
        enum �tatsJeu { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        const float INTERVALLE_CALCUL_FPS = 1f;

        TerrainAvecBase TerrainDeJeu { get; set; }

        �tatsJeu �tatJeu { get; set; }
        //String[] TexturesTerrain = new string[] { "Sable", "HerbeB" };
        public Jeu(Game game)
            : base(game)
        {

        }
        public override void Initialize()
        {
            const float �CHELLE_OBJET = 0.001f;
            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            Vector3 positionCPU = new Vector3(96, 18f, -30);
            //Vector3 positionObjet = new Vector3(100, 20, -100);
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI/4, 0);


            //LoadSauvegarde();
            Game.Components.Add(new Arri�rePlan(Game, "BackGroundNuage"));
            Game.Components.Insert(Game.Components.Count - 1, new Afficheur3D(Game));
            TerrainDeJeu = new TerrainAvecBase(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 25, 256), "TerrainPokemon", "D�tailsTerrain", 5 ,INTERVALLE_MAJ_STANDARD);

            Game.Components.Insert(Game.Components.Count - 1, TerrainDeJeu);
            Game.Services.AddService(typeof(TerrainAvecBase), TerrainDeJeu);
            Game.Components.Insert(Game.Components.Count - 1, new Trainer(Game, "1Bulbasaur/Bulbasaur", �CHELLE_OBJET, rotationObjet, positionCPU, INTERVALLE_MAJ_STANDARD, 1f));
            //Game.Components.Insert(Game.Components.Count - 1, new ObjetDeBase(Game, "Maison", �CHELLE_OBJET * 100, rotationObjet, positionCPU));
            //Game.Components.Insert(Game.Components.Count - 1, new ObjetDeBase(Game, "Professor", �CHELLE_OBJET * 1000, rotationObjet, positionCPU));
            Game.Services.AddService(typeof(Trainer), new Trainer(Game, "1Bulbasaur/Bulbasaur", �CHELLE_OBJET, rotationObjet, positionCPU, INTERVALLE_MAJ_STANDARD, 1f));
        }
        public override void Update(GameTime gameTime)
        {
            //G�rerClavier();
            G�rerTransition();
            G�rer�tat(); 
            base.Update(gameTime);
        }
        private void G�rerTransition()
        {
            switch (�tatJeu)
            {
                case �tatsJeu.PAGE_TITRE:
                    //G�rerTransitionPageTitre();
                    break;
                //case �tats.JEU3D:
                //    G�rerTransitionJEU3D();
                //    break;
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
        private void G�rer�tat()
        {
            switch (�tatJeu)
            {
                case �tatsJeu.PAGE_TITRE:
                    Game.Components.Add(new PageTitre(Game));
                    break;
                //case �tats.JEU3D:
                //    G�rerCollision();
                //    G�rerCombat();
                //    G�rerComputer();
                //    break;
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
    }
}
