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
        enum �tats { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        �tats �tatJeu { get; set; }
        public Jeu(Game game)
            : base(game)
        {

        }
        public override void Initialize()
        {
            base.Initialize();
            �tatJeu = �tats.PAGE_TITRE;
        }
        public override void Update(GameTime gameTime)
        {
            G�rerClavier();
            G�rerTransition();
            G�rer�tat(); 
            base.Update(gameTime);
        }
        private void G�rerTransition()
        {
            switch (�tatJeu)
            {
                case �tats.PAGE_TITRE:
                    G�rerTransitionPageTitre();
                    break;
                case �tats.JEU3D:
                    G�rerTransitionJEU3D();
                    break;
                case �tats.COMBAT:
                    G�rerTransitionCombat();
                    break;
                case �tats.MAISON:
                    G�rerTransitionMaison();
                    break;
                case �tats.GYM:
                    G�rerTransitionGym();
                    break;
                case �tats.FIN:
                    G�rerTransitionFin();
                    break;
                default:
                    break;
            }
        }
        private void G�rer�tat()
        {
            switch (�tatJeu)
            {
                case �tats.PAGE_TITRE:
                    PageTitre();
                    break;
                case �tats.JEU3D:
                    G�rerCollision();
                    G�rerCombat();
                    G�rerComputer();
                    break;
                case �tats.COMBAT:
                    Combat();
                    break;
                case �tats.MAISON:
                    G�rerCollision();
                    G�rerVitesseD�placement();
                    G�rerComputer();
                    break;
                case �tats.GYM:
                    G�rerVitesseD�placement();
                    G�rerComputer();
                    G�rerCombat();
                    break;
                default: //�tats.FIN:
                    Fin();
                    SauvegardeAuto();
                    break;
            }
        }
    }
}
