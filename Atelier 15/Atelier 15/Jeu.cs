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
        enum ÉtatsJeu { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        ÉtatsJeu ÉtatJeu { get; set; }
        public Jeu(Game game)
            : base(game)
        {

        }
        public override void Initialize()
        {
            //CaméraJeu = new CaméraSubjective(this, Vector3.Zero, Vector3.Zero, Vector3.Up, INTERVALLE_MAJ_STANDARD);
            //Game.Components.Add(CaméraJeu);
            Game.Components.Add(new ArrièrePlanSpatial(Game, "CielWindowsXp", INTERVALLE_MAJ_STANDARD));
            Game.Components.Add(new Afficheur3D(Game));
            Game.Components.Add(new Jeu(Game));
            Game.Components.Add(new AfficheurFPS(Game, "Arial20", Color.Red, INTERVALLE_CALCUL_FPS));
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            //GérerClavier();
            GérerTransition();
            GérerÉtat(); 
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
