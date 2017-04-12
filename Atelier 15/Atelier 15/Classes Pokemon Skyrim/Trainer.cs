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
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
        const float DÉPLACEMEMENT_MODÈLE = /*0.05f*/1f;
        public float Hauteur { get; private set; }
        TerrainAvecBase Terrain { get; set; }
        const float VitesseRotation = 1.5f;
        public Vector2 Souris { get; private set; }

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float Rayon { get; set; }
        public Vector3 UpPositionTrainer { get; set; }

        protected InputManager GestionInput { get; private set; }

        BasicEffect EffetDeBase { get; set; }
        public BoundingSphere SphèreDeCollisionBalle { get; protected set; }
        Vector3 Direction { get; set; }
        Vector3 OrientationVertical { get; } = Vector3.Up;
        Vector3 Latéral { get; set; }


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
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
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
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
        }

        protected void EffectuerMiseÀJour()
        {
            BougerTrainer();
            TournerTrainer();
            CaméraJeu.Position = new Vector3(Position.X + 3, Position.Y + HAUTEUR_CAMÉRA, Position.Z - 3);
            CalculerMonde();

        }

        private void TournerTrainer()
        {
            int valYaw = GestionInput.GetPositionSouris().X > Souris.X ? 1 : -1;
            int valPitch = GestionInput.GetPositionSouris().Y > Souris.Y ? 1 : -1;

            //déplacement hrizontale Angle # pas de limite
            if (GestionInput.GetPositionSouris().X != Souris.X)
            {
                //MARCHE PAS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                float rotation = DELTA_TANGAGE* VitesseRotation;


                //((CaméraJeu) as CaméraSubjective).Position = new Vector3((float)(valYaw * Math.Cos(DELTA_TANGAGE * VitesseRotation)
                //    * (Position.X - ((CaméraJeu) as CaméraSubjective).Position.X)), ((CaméraJeu) as CaméraSubjective).Position.Y,
                //    (float)(valYaw * Math.Sin(DELTA_TANGAGE * VitesseRotation) *
                //    (Position.Z - ((CaméraJeu) as CaméraSubjective).Position.Z)));

                ((CaméraJeu) as CaméraSubjective).CréerPointDeVue(((CaméraJeu) as CaméraSubjective).Position, Position, Vector3.Up);
                //LeJoueur.Position = Vector3.Transform(LeJoueur.Position, 
                //Matrix.CreateFromAxisAngle(OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation));

                //Direction = Vector3.Normalize(Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(/*LeJoueur.Position*/OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation)));
            }
            //    ((CaméraJeu) as CaméraSubjective).Direction = Vector3.Normalize(Vector3.Transform(((CaméraJeu) as CaméraSubjective).Direction, Matrix.CreateFromAxisAngle(((CaméraJeu) as CaméraSubjective).OrientationVerticale, DELTA_LACET * valYaw * VitesseRotation)));

            // déplacement vertical Angle # limite = 45'
            if (GestionInput.GetPositionSouris().Y != Souris.Y)
            {
                ((CaméraJeu) as CaméraSubjective).Direction = Vector3.Normalize(Vector3.Transform(((CaméraJeu) as CaméraSubjective).Direction, Matrix.CreateFromAxisAngle(((CaméraJeu) as CaméraSubjective).Latéral, DELTA_TANGAGE * valPitch * VitesseRotation)));
                Vector3 ancienneDirection = ((CaméraJeu) as CaméraSubjective).Direction;
                float angleDirection = (float)Math.Asin(((CaméraJeu) as CaméraSubjective).Direction.Y);
                //Marche pas
                if (angleDirection < -100/*angleDirection > ANGLE_MAX || angleDirection < -ANGLE_MAX*/)
                {
                    ((CaméraJeu) as CaméraSubjective).Direction = ancienneDirection;
                }
            }
        }

        protected void BougerTrainer()
        {
            if (GestionInput.EstClavierActivé)
            {
                float déplacementHorizontal = GérerTouche(Keys.D) - GérerTouche(Keys.A); // touche d = ajoute des pixels à mon image. touche a = enlève des pixels
                float déplacementProfondeur = GérerTouche(Keys.W) - GérerTouche(Keys.S);
                if (déplacementHorizontal != 0 || déplacementProfondeur != 0)
                {
                    CalculerPosition(déplacementHorizontal, déplacementProfondeur);
                }
            }
        }

        private void CalculerPosition(float déplacementHorizontal, float déplacementProfondeur)
        {

            Direction = ((CaméraJeu) as CaméraSubjective).Direction;
            Latéral = Vector3.Cross(Direction, OrientationVertical);

            Position += Direction * déplacementProfondeur;
            Position += Latéral * déplacementHorizontal;
            Limites();

            Vector2 vecteurPosition = new Vector2(Position.X + Terrain.NbColonnes / 2, Position.Z + Terrain.NbRangées / 2);
            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
            Position = new Vector3(Position.X, posY, Position.Z);
            (CaméraJeu as CaméraSubjective).Position = new Vector3(Position.X + 3, posY + HAUTEUR_CAMÉRA, Position.Z + 3);

            SphèreDeCollisionBalle = new BoundingSphere(Position, SphèreDeCollisionBalle.Radius);

        }
        private void Limites()
        {
            Position = new Vector3(MathHelper.Max(MathHelper.Min(Position.X, Terrain.NbColonnes / 2), -Terrain.NbColonnes / 2), Position.Y,
             MathHelper.Max(MathHelper.Min(Position.Z, Terrain.NbRangées / 2), -Terrain.NbRangées / 2));
        }

        float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? 1 : 0;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Game.Window.Title = (CaméraJeu as CaméraSubjective).Souris.ToString();
        }
    }
}
