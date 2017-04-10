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
         public const int DISTANCE_MODÈLE_CAMÉRA = 1;
        const float HAUTEUR_CAMÉRA = 2f;

        const float DÉPLACEMEMENT_MODÈLE = /*0.05f*/1f;
        public float Hauteur { get; private set; }
        TerrainAvecBase Terrain { get; set; }

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float Rayon { get; set; }
        public Vector3 UpPositionTrainer { get; set; }
        
        protected InputManager GestionInput { get; private set; }

        BasicEffect EffetDeBase { get; set; }
        public BoundingSphere SphèreDeCollisionBalle { get; protected set; }


        //public bool EstEnCollision(object autreObjet)
        //{
        //    if (!(autreObjet is ICollisionable))
        //    {
        //        return false;
        //    }
        //    return SphèreDeCollisionBalle.Intersects(((ICollisionable)autreObjet).SphèreDeCollisionBalle);
        //}

        public Trainer(Game jeu, string nomModèle, float échelle, Vector3 rotation, Vector3 position, float intervallleMAJ, float rayon)
            : base(jeu, nomModèle, échelle, rotation, position)
        {
            IntervalleMAJ = intervallleMAJ;
            Rayon = rayon;

            SphèreDeCollisionBalle = new BoundingSphere(position, Rayon);
            Hauteur = 2000 * Rayon * Échelle;
        }

        public override void Initialize()
        {
            UpPositionTrainer = Vector3.Up + Position;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMiseÀJour();
                TempsÉcouléDepuisMAJ = 0;
            }

        }

        private Vector3 BougerSelonLesNormales(Vector3 nouvellePosition)
        {
            Vector2 vecteurPosition = new Vector2(nouvellePosition.X + Terrain.NbColonnes / 2, nouvellePosition.Z + Terrain.NbRangées / 2);
            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
            nouvellePosition = new Vector3(nouvellePosition.X, posY + HAUTEUR_CAMÉRA, nouvellePosition.Z);

            return nouvellePosition;
        }

        protected void EffectuerMiseÀJour()
        {
            BougerTrainer();
            LimitesTerrain();
            //TournerTrainer();
            Position = BougerSelonLesNormales(Position);
            CaméraJeu.Position = new Vector3(Position.X +3, Position.Y + HAUTEUR_CAMÉRA, Position.Z  +3);
            CalculerMonde();
        }
        private void LimitesTerrain()
        {

        }
        protected void BougerTrainer()
        {
            if (GestionInput.EstClavierActivé)
            {
                float déplacementHorizontal = GérerTouche(Keys.A) - GérerTouche(Keys.D); // touche d = ajoute des pixels à mon image. touche a = enlève des pixels
                float déplacementProfondeur = GérerTouche(Keys.S) - GérerTouche(Keys.W);
                if (déplacementHorizontal != 0 || déplacementProfondeur != 0)
                {
                    Position += Vector3.Cross(Vector3.Right + Position, Vector3.Up) * déplacementProfondeur;
                    Position += Vector3.Right * déplacementHorizontal;
                    SphèreDeCollisionBalle = new BoundingSphere(Position, SphèreDeCollisionBalle.Radius); 
                }
            }
        }
        private void TournerCaméraAvecSouris()
        {
            //int valYaw = GestionInput.GetPositionSouris().X > Souris.X ? 1 : -1;
            //int valPitch = GestionInput.GetPositionSouris().Y > Souris.Y ? 1 : -1;

            ////déplacement hrizontale Angle # pas de limite
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

            //        CréerPointDeVue(Position, LeJoueur.Position, Vector3.Up);
            //        //LeJoueur.Position = Vector3.Transform(LeJoueur.Position, 
            //        //Matrix.CreateFromAxisAngle(OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation));

            //        //Direction = Vector3.Normalize(Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(/*LeJoueur.Position*/OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation)));
            //    }
            //    else
            //    {
            //        Direction = Vector3.Normalize(Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation)));
            //    }
            //}
            //// déplacement vertical Angle # limite = 45'
            //if (GestionInput.GetPositionSouris().Y != Souris.Y)
            //{
            //    Direction = Vector3.Normalize(Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Latéral, DELTA_TANGAGE * valPitch * VitesseRotation)));
            //    Vector3 ancienneDirection = Direction;
            //    float angleDirection = (float)Math.Asin(Direction.Y);
            //    //Marche pas
            //    if (angleDirection < -100/*angleDirection > ANGLE_MAX || angleDirection < -ANGLE_MAX*/)
            //    {
            //        Direction = ancienneDirection;
            //    }
            //}


        }
        float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? DÉPLACEMEMENT_MODÈLE : 0;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Game.Window.Title = "........" + CaméraJeu.Position.ToString() + "........" + Position.ToString() + "..........." + (CaméraJeu as CaméraSubjective).Souris.ToString();
        }
    }
}
