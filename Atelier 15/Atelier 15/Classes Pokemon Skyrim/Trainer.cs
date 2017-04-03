//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

namespace AtelierXNA
{

    public class Trainer : ObjetDeBase
    {
        const int DISTANCE_MODÈLE_CAMÉRA = 20;
        const float HAUTEUR_CAMÉRA = 0.25f;

        const int NB_PIXELS_DE_DÉPLACEMENT = 1;

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
                int déplacementHorizontal = GérerTouche(Keys.U) - GérerTouche(Keys.J); // touche d = ajoute des pixels à mon image. touche a = enlève des pixels
                int déplacementProfondeur = GérerTouche(Keys.K) - GérerTouche(Keys.H);
                if (déplacementHorizontal != 0 || déplacementProfondeur != 0)
                {
                    CalculerPosition(déplacementHorizontal, déplacementProfondeur);
                }
            }

            // AjusterPositionCaméra();
        }

        void CalculerPosition(int déplacementHorizontal, int déplacementProfondeur)
        {
            Vector3 vecteurRayon = new Vector3(0, 0, 0); // Test................
            // Position = new Vector3(Position.X, (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y, Position.Z);


            SphèreDeCollisionBalle = new BoundingSphere(Position, SphèreDeCollisionBalle.Radius);  //--> À utiliser pour que lorsque le modèle 3D entre en collision avec une porte.

            float posX = CalculerPosition(déplacementHorizontal, Position.X);
            float posZ = CalculerPosition(déplacementProfondeur, Position.Z);

            Vector2 vecteurPosition = new Vector2(posX + Terrain.NbColonnes / 2, posZ + Terrain.NbRangées / 2);

            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y;

            // float posY = (Terrain.GetPointSpatial(Math.Abs((int)Math.Round(posX, 0)), Terrain.NbRangées - Math.Abs((int)Math.Round(posZ, 0)))).Y;


            Position = new Vector3(posX, posY, posZ);

            SphèreDeCollisionBalle = new BoundingSphere(Position, SphèreDeCollisionBalle.Radius);
        }

        float CalculerPosition(int déplacement, float posActuelle)
        {
            float position = posActuelle + déplacement;

            position = MathHelper.Max(MathHelper.Min(position, Terrain.NbColonnes / 2), -Terrain.NbColonnes / 2);
            position = MathHelper.Max(MathHelper.Min(position, Terrain.NbRangées / 2), -Terrain.NbRangées / 2);

            return position;
        }

        int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? NB_PIXELS_DE_DÉPLACEMENT : 0;
        }


        void AjusterPositionCaméra()
        {
            //Vector3 vecteurNormalisé = Position - AnciennePositionCaméra;

            // Vector3 position = Position - DISTANCE_MODÈLE_CAMÉRA * Vector3.Normalize(vecteurNormalisé);

            //position.Y = 21;

            //CaméraJeu.Déplacer(position, Position, Vector3.Up);

            //AnciennePositionCaméra = position;



            float positionX = Position.X - DISTANCE_MODÈLE_CAMÉRA/*- DISTANCE_MODÈLE_CAMÉRA * Vector3.Normalize(Position - AnciennePositionCaméra) + HAUTEUR_CAMÉRA * Vector3.Up*/;
            float positionY = 25;
            float positionZ = Position.Z - DISTANCE_MODÈLE_CAMÉRA;

            Vector3 positionCaméra = new Vector3(positionX, positionY, positionZ);

            //position.Y = 25;

            CaméraJeu.Déplacer(positionCaméra, Position, Vector3.Up);

            AnciennePositionCaméra = positionCaméra;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Game.Window.Title = "........" + Position.ToString();
        }
    }
}
