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
        const int DISTANCE_MODÈLE_CAMÉRA = 1;
        const float HAUTEUR_CAMÉRA = 0.25f;

        const float DÉPLACEMEMENT_MODÈLE = /*0.05f*/1f;
        public  float Hauteur {get; private set;}
        TerrainAvecBase Terrain { get; set; }
        protected Point PositionSouris { get; set; }
        private Point AnciennePositionSouris { get; set; }

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
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            // AnciennePositionCaméra = CaméraJeu.Position; // --> à continuer

        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            PositionSouris = GestionInput.GetPositionSouris();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerClavier();
                EffectuerMiseÀJour();
                TempsÉcouléDepuisMAJ = 0;
            }
        }

        protected void EffectuerMiseÀJour()
        {
            PositionAncienne = Position;
            CalculerMonde();
        }

        protected void GérerClavier()
        {
            if (GestionInput.EstClavierActivé)
            {
                float déplacementHorizontal = GérerTouche(Keys.S) - GérerTouche(Keys.W); // touche d = ajoute des pixels à mon image. touche a = enlève des pixels
                float déplacementProfondeur = GérerTouche(Keys.A) - GérerTouche(Keys.D);
                if (déplacementHorizontal != 0 || déplacementProfondeur != 0)
                {
                    CalculerPosition(déplacementHorizontal, déplacementProfondeur);
                }
            }

             AjusterPositionCaméra();
        }

        void CalculerPosition(float déplacementHorizontal, float déplacementProfondeur)
        {
            Vector3 vecteurRayon = new Vector3(0, 0, 0); // Test................
            // Position = new Vector3(Position.X, (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y, Position.Z);


            SphèreDeCollisionBalle = new BoundingSphere(Position, SphèreDeCollisionBalle.Radius);  //--> À utiliser pour que lorsque le modèle 3D entre en collision avec une porte.

            float posX = CalculerPosition2(déplacementHorizontal, Position.X);
            float posZ = CalculerPosition2(déplacementProfondeur, Position.Z);

            Vector2 vecteurPosition = new Vector2(posX + Terrain.NbColonnes / 2, posZ + Terrain.NbRangées / 2);

            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y;

            // float posY = (Terrain.GetPointSpatial(Math.Abs((int)Math.Round(posX, 0)), Terrain.NbRangées - Math.Abs((int)Math.Round(posZ, 0)))).Y;


            Position = new Vector3(posX, posY, posZ);

            SphèreDeCollisionBalle = new BoundingSphere(Position, SphèreDeCollisionBalle.Radius);
        }

        float CalculerPosition2(float déplacement, float posActuelle)
        {
            float position = posActuelle + déplacement;

            position = MathHelper.Max(MathHelper.Min(position, Terrain.NbColonnes / 2), -Terrain.NbColonnes / 2);
            position = MathHelper.Max(MathHelper.Min(position, Terrain.NbRangées / 2), -Terrain.NbRangées / 2);

            return position;
        }

        float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? DÉPLACEMEMENT_MODÈLE : 0;
        }


        void AjusterPositionCaméra()
        {
            
            //Bouger
            float positionX = Position.X;
            float positionY = Position.Y + Hauteur;
            float positionZ = Position.Z + (DISTANCE_MODÈLE_CAMÉRA);
            Vector3 positionCaméra = new Vector3(positionX, positionY, positionZ);
            CaméraJeu.Déplacer(positionCaméra, new Vector3(Position.X,Position.Y + 1.1f, Position.Z), Vector3.Up);
           


            //Tourner
            PositionSouris = GestionInput.GetPositionSouris();
            CaméraJeu.Cible = new Vector3(PositionSouris.X - Game.Window.ClientBounds.Width / 2, 0, 0);
            (CaméraJeu as CaméraSubjective).Direction = CaméraJeu.Cible - Position;
            CaméraJeu.Déplacer(positionCaméra, (CaméraJeu as CaméraSubjective).Direction, Vector3.Up);
            if(PositionSouris.X == Game.Window.ClientBounds.Width)
            {
                AnciennePositionSouris = PositionSouris;
                PositionSouris = new Point(Game.Window.ClientBounds.Width / 2, PositionSouris.Y);


                CaméraJeu.Cible = new Vector3(PositionSouris.X - Game.Window.ClientBounds.Width / 2, 0, 0);
                (CaméraJeu as CaméraSubjective).Direction = CaméraJeu.Cible - Position;
                CaméraJeu.Déplacer(positionCaméra, (CaméraJeu as CaméraSubjective).Direction, Vector3.Up);
                PositionSouris = AnciennePositionSouris;
            }
            if (PositionSouris.X == 0)
            {
                AnciennePositionSouris = PositionSouris;
                PositionSouris = new Point(Game.Window.ClientBounds.Width / 2, PositionSouris.Y);

                CaméraJeu.Cible = new Vector3(PositionSouris.X - Game.Window.ClientBounds.Width / 2, 0, 0);
                (CaméraJeu as CaméraSubjective).Direction = CaméraJeu.Cible - Position;
                CaméraJeu.Déplacer(positionCaméra, (CaméraJeu as CaméraSubjective).Direction, Vector3.Up);
                PositionSouris = AnciennePositionSouris;
            }

            AnciennePositionCaméra = positionCaméra;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Game.Window.Title = "........" + CaméraJeu.Position.ToString() + "........" + Position.ToString();
        }
    }
}
