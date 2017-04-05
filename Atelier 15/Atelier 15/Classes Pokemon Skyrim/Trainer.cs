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

        Vector3 AnciennePositionCaméra { get; set; }
        Vector3 PositionAncienne { get; set; }
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
                GérerClavier();
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
            PositionAncienne = Position;
            CaméraJeu.Position = BougerSelonLesNormales((CaméraJeu as CaméraSubjective).nouvellePosition);
            Position = new Vector3(CaméraJeu.Position.X - 3, CaméraJeu.Position.Y - HAUTEUR_CAMÉRA, CaméraJeu.Position.Z + 3);
            CalculerMonde();
        }

        protected void GérerClavier()
        {
            if (GestionInput.EstClavierActivé)
            {
                float déplacementHorizontal = GérerTouche(Keys.D) - GérerTouche(Keys.A); // touche d = ajoute des pixels à mon image. touche a = enlève des pixels
                float déplacementProfondeur = GérerTouche(Keys.S) - GérerTouche(Keys.W);
                if (déplacementHorizontal != 0 || déplacementProfondeur != 0)
                {
                    SphèreDeCollisionBalle = new BoundingSphere(Position, SphèreDeCollisionBalle.Radius); 
                }
            }
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
