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

    public class Trainer : ObjetDeBase
    {
         public const int DISTANCE_MOD�LE_CAM�RA = 1;
        const float HAUTEUR_CAM�RA = 2f;

        const float D�PLACEMEMENT_MOD�LE = /*0.05f*/1f;
        public float Hauteur { get; private set; }
        TerrainAvecBase Terrain { get; set; }

        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        float Rayon { get; set; }

        Vector3 AnciennePositionCam�ra { get; set; }
        Vector3 PositionAncienne { get; set; }
        protected InputManager GestionInput { get; private set; }

        BasicEffect EffetDeBase { get; set; }
        public BoundingSphere Sph�reDeCollisionBalle { get; protected set; }


        //public bool EstEnCollision(object autreObjet)
        //{
        //    if (!(autreObjet is ICollisionable))
        //    {
        //        return false;
        //    }
        //    return Sph�reDeCollisionBalle.Intersects(((ICollisionable)autreObjet).Sph�reDeCollisionBalle);
        //}

        public Trainer(Game jeu, string nomMod�le, float �chelle, Vector3 rotation, Vector3 position, float intervallleMAJ, float rayon)
            : base(jeu, nomMod�le, �chelle, rotation, position)
        {
            IntervalleMAJ = intervallleMAJ;
            Rayon = rayon;

            Sph�reDeCollisionBalle = new BoundingSphere(position, Rayon);
            Hauteur = 2000 * Rayon * �chelle;
        }

        public override void Initialize()
        {
            Temps�coul�DepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                G�rerClavier();
                EffectuerMise�Jour();
                Temps�coul�DepuisMAJ = 0;
            }

        }

        private Vector3 BougerSelonLesNormales(Vector3 nouvellePosition)
        {
            Vector2 vecteurPosition = new Vector2(nouvellePosition.X + Terrain.NbColonnes / 2, nouvellePosition.Z + Terrain.NbRang�es / 2);
            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
            nouvellePosition = new Vector3(nouvellePosition.X, posY + HAUTEUR_CAM�RA, nouvellePosition.Z);

            return nouvellePosition;
        }

        protected void EffectuerMise�Jour()
        {
            PositionAncienne = Position;
            Cam�raJeu.Position = BougerSelonLesNormales((Cam�raJeu as Cam�raSubjective).nouvellePosition);
            Position = new Vector3(Cam�raJeu.Position.X - 3, Cam�raJeu.Position.Y - HAUTEUR_CAM�RA, Cam�raJeu.Position.Z + 3);
            CalculerMonde();
        }

        protected void G�rerClavier()
        {
            if (GestionInput.EstClavierActiv�)
            {
                float d�placementHorizontal = G�rerTouche(Keys.D) - G�rerTouche(Keys.A); // touche d = ajoute des pixels � mon image. touche a = enl�ve des pixels
                float d�placementProfondeur = G�rerTouche(Keys.S) - G�rerTouche(Keys.W);
                if (d�placementHorizontal != 0 || d�placementProfondeur != 0)
                {
                    Sph�reDeCollisionBalle = new BoundingSphere(Position, Sph�reDeCollisionBalle.Radius); 
                }
            }
        }
        float G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? D�PLACEMEMENT_MOD�LE : 0;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Game.Window.Title = "........" + Cam�raJeu.Position.ToString() + "........" + Position.ToString() + "..........." + (Cam�raJeu as Cam�raSubjective).Souris.ToString();
        }
    }
}
