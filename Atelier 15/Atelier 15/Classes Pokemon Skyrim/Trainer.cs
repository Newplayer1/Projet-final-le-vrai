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
        public Vector3 UpPositionTrainer { get; set; }
        
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
            UpPositionTrainer = Vector3.Up + Position;
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
            BougerTrainer();
            LimitesTerrain();
            //TournerTrainer();
            Position = BougerSelonLesNormales(Position);
            Cam�raJeu.Position = new Vector3(Position.X +3, Position.Y + HAUTEUR_CAM�RA, Position.Z  +3);
            CalculerMonde();
        }
        private void LimitesTerrain()
        {

        }
        protected void BougerTrainer()
        {
            if (GestionInput.EstClavierActiv�)
            {
                float d�placementHorizontal = G�rerTouche(Keys.A) - G�rerTouche(Keys.D); // touche d = ajoute des pixels � mon image. touche a = enl�ve des pixels
                float d�placementProfondeur = G�rerTouche(Keys.S) - G�rerTouche(Keys.W);
                if (d�placementHorizontal != 0 || d�placementProfondeur != 0)
                {
                    Position += Vector3.Cross(Vector3.Right + Position, Vector3.Up) * d�placementProfondeur;
                    Position += Vector3.Right * d�placementHorizontal;
                    Sph�reDeCollisionBalle = new BoundingSphere(Position, Sph�reDeCollisionBalle.Radius); 
                }
            }
        }
        private void TournerCam�raAvecSouris()
        {
            //int valYaw = GestionInput.GetPositionSouris().X > Souris.X ? 1 : -1;
            //int valPitch = GestionInput.GetPositionSouris().Y > Souris.Y ? 1 : -1;

            ////d�placement hrizontale Angle # pas de limite
            //if (GestionInput.GetPositionSouris().X != Souris.X)
            //{

            //    bool valeur = false;
            //    for (int i = 0; i < Game.Components.Count; i++)
            //    {
            //        if (Game.Components[i] is Trainer)
            //            valeur = true;
            //    }
            //    //MARCHE PAS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //    if (valeur)
            //    {
            //        LeJoueur = Game.Services.GetService(typeof(Trainer)) as Trainer;
            //        Position = new Vector3((float)(valYaw * Math.Cos(DELTA_TANGAGE * VitesseRotation)
            //            * (LeJoueur.Position.X - Position.X)), Position.Y,
            //            (float)(valYaw * Math.Sin(DELTA_TANGAGE * VitesseRotation) *
            //            (LeJoueur.Position.Z - Position.Z)));

            //        //Position = MathHelper.
            //        //Vector3.Transform(Position,
            //        //Matrix.CreateFromAxisAngle(LeJoueur.UpPositionTrainer, DELTA_LACET * valYaw * VitesseRotation));

            //        Cr�erPointDeVue(Position, LeJoueur.Position, Vector3.Up);
            //        //LeJoueur.Position = Vector3.Transform(LeJoueur.Position, 
            //        //Matrix.CreateFromAxisAngle(OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation));

            //        //Direction = Vector3.Normalize(Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(/*LeJoueur.Position*/OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation)));
            //    }
            //    else
            //    {
            //        Direction = Vector3.Normalize(Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation)));
            //    }
            //}
            //// d�placement vertical Angle # limite = 45'
            //if (GestionInput.GetPositionSouris().Y != Souris.Y)
            //{
            //    Direction = Vector3.Normalize(Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Lat�ral, DELTA_TANGAGE * valPitch * VitesseRotation)));
            //    Vector3 ancienneDirection = Direction;
            //    float angleDirection = (float)Math.Asin(Direction.Y);
            //    //Marche pas
            //    if (angleDirection < -100/*angleDirection > ANGLE_MAX || angleDirection < -ANGLE_MAX*/)
            //    {
            //        Direction = ancienneDirection;
            //    }
            //}


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
