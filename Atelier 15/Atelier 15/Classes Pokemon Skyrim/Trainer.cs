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
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degr� � la fois
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degr� � la fois
        const float D�PLACEMEMENT_MOD�LE = /*0.05f*/1f;
        public float Hauteur { get; private set; }
        TerrainAvecBase Terrain { get; set; }
        const float VitesseRotation = 1.5f;
        public Vector2 Souris { get; private set; }

        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        float Rayon { get; set; }
        public Vector3 UpPositionTrainer { get; set; }

        protected InputManager GestionInput { get; private set; }

        BasicEffect EffetDeBase { get; set; }
        public BoundingSphere Sph�reDeCollisionBalle { get; protected set; }
        Vector3 Direction { get; set; }
        Vector3 OrientationVertical { get; } = Vector3.Up;
        Vector3 Lat�ral { get; set; }


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
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
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
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
        }

        protected void EffectuerMise�Jour()
        {
            BougerTrainer();
            TournerTrainer();
            Cam�raJeu.Position = new Vector3(Position.X + 3, Position.Y + HAUTEUR_CAM�RA, Position.Z - 3);
            CalculerMonde();

        }

        private void TournerTrainer()
        {
            int valYaw = GestionInput.GetPositionSouris().X > Souris.X ? 1 : -1;
            int valPitch = GestionInput.GetPositionSouris().Y > Souris.Y ? 1 : -1;

            //d�placement hrizontale Angle # pas de limite
            if (GestionInput.GetPositionSouris().X != Souris.X)
            {
                //MARCHE PAS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                float rotation = DELTA_TANGAGE* VitesseRotation;


                //((Cam�raJeu) as Cam�raSubjective).Position = new Vector3((float)(valYaw * Math.Cos(DELTA_TANGAGE * VitesseRotation)
                //    * (Position.X - ((Cam�raJeu) as Cam�raSubjective).Position.X)), ((Cam�raJeu) as Cam�raSubjective).Position.Y,
                //    (float)(valYaw * Math.Sin(DELTA_TANGAGE * VitesseRotation) *
                //    (Position.Z - ((Cam�raJeu) as Cam�raSubjective).Position.Z)));

                ((Cam�raJeu) as Cam�raSubjective).Cr�erPointDeVue(((Cam�raJeu) as Cam�raSubjective).Position, Position, Vector3.Up);
                //LeJoueur.Position = Vector3.Transform(LeJoueur.Position, 
                //Matrix.CreateFromAxisAngle(OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation));

                //Direction = Vector3.Normalize(Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(/*LeJoueur.Position*/OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation)));
            }
            //    ((Cam�raJeu) as Cam�raSubjective).Direction = Vector3.Normalize(Vector3.Transform(((Cam�raJeu) as Cam�raSubjective).Direction, Matrix.CreateFromAxisAngle(((Cam�raJeu) as Cam�raSubjective).OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation)));

            // d�placement vertical Angle # limite = 45'
            if (GestionInput.GetPositionSouris().Y != Souris.Y)
            {
                ((Cam�raJeu) as Cam�raSubjective).Direction = Vector3.Normalize(Vector3.Transform(((Cam�raJeu) as Cam�raSubjective).Direction, Matrix.CreateFromAxisAngle(((Cam�raJeu) as Cam�raSubjective).Lat�ral, DELTA_TANGAGE * valPitch * VitesseRotation)));
                Vector3 ancienneDirection = ((Cam�raJeu) as Cam�raSubjective).Direction;
                float angleDirection = (float)Math.Asin(((Cam�raJeu) as Cam�raSubjective).Direction.Y);
                //Marche pas
                if (angleDirection < -100/*angleDirection > ANGLE_MAX || angleDirection < -ANGLE_MAX*/)
                {
                    ((Cam�raJeu) as Cam�raSubjective).Direction = ancienneDirection;
                }
            }
        }

        protected void BougerTrainer()
        {
            if (GestionInput.EstClavierActiv�)
            {
                float d�placementHorizontal = G�rerTouche(Keys.D) - G�rerTouche(Keys.A); // touche d = ajoute des pixels � mon image. touche a = enl�ve des pixels
                float d�placementProfondeur = G�rerTouche(Keys.W) - G�rerTouche(Keys.S);
                if (d�placementHorizontal != 0 || d�placementProfondeur != 0)
                {
                    CalculerPosition(d�placementHorizontal, d�placementProfondeur);
                }
            }
        }

        private void CalculerPosition(float d�placementHorizontal, float d�placementProfondeur)
        {

            Direction = ((Cam�raJeu) as Cam�raSubjective).Direction;
            Lat�ral = Vector3.Cross(Direction, OrientationVertical);

            Position += Direction * d�placementProfondeur;
            Position += Lat�ral * d�placementHorizontal;
            Limites();

            Vector2 vecteurPosition = new Vector2(Position.X + Terrain.NbColonnes / 2, Position.Z + Terrain.NbRang�es / 2);
            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
            Position = new Vector3(Position.X, posY, Position.Z);
            (Cam�raJeu as Cam�raSubjective).Position = new Vector3(Position.X + 3, posY + HAUTEUR_CAM�RA, Position.Z + 3);

            Sph�reDeCollisionBalle = new BoundingSphere(Position, Sph�reDeCollisionBalle.Radius);

        }
        private void Limites()
        {
            Position = new Vector3(MathHelper.Max(MathHelper.Min(Position.X, Terrain.NbColonnes / 2), -Terrain.NbColonnes / 2), Position.Y,
             MathHelper.Max(MathHelper.Min(Position.Z, Terrain.NbRang�es / 2), -Terrain.NbRang�es / 2));
        }

        float G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? 1 : 0;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Game.Window.Title = (Cam�raJeu as Cam�raSubjective).Souris.ToString();
        }
    }
}
