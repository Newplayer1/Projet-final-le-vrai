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
        enum …tats { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        …tats …tatJeu { get; set; }
        public Jeu(Game game)
            : base(game)
        {

        }
        public override void Initialize()
        {
            base.Initialize();
            …tatJeu = …tats.PAGE_TITRE;
        }
        public override void Update(GameTime gameTime)
        {
            GÈrerClavier();
            GÈrerTransition();
            GÈrer…tat(); 
            base.Update(gameTime);
        }
        private void GÈrerTransition()
        {
            switch (…tatJeu)
            {
                case …tats.PAGE_TITRE:
                    GÈrerTransitionPageTitre();
                    break;
                case …tats.JEU3D:
                    GÈrerTransitionJEU3D();
                    break;
                case …tats.COMBAT:
                    GÈrerTransitionCombat();
                    break;
                case …tats.MAISON:
                    GÈrerTransitionMaison();
                    break;
                case …tats.GYM:
                    GÈrerTransitionGym();
                    break;
                case …tats.FIN:
                    GÈrerTransitionFin();
                    break;
                default:
                    break;
            }
        }
        private void GÈrer…tat()
        {
            switch (…tatJeu)
            {
                case …tats.PAGE_TITRE:
                    PageTitre();
                    break;
                case …tats.JEU3D:
                    GÈrerCollision();
                    GÈrerCombat();
                    GÈrerComputer();
                    break;
                case …tats.COMBAT:
                    Combat();
                    break;
                case …tats.MAISON:
                    GÈrerCollision();
                    GÈrerVitesseDÈplacement();
                    GÈrerComputer();
                    break;
                case …tats.GYM:
                    GÈrerVitesseDÈplacement();
                    GÈrerComputer();
                    GÈrerCombat();
                    break;
                default: //…tats.FIN:
                    Fin();
                    SauvegardeAuto();
                    break;
            }
        }
    }
}
