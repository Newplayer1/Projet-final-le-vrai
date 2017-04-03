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
        const int DISTANCE_MOD�LE_CAM�RA = 20;
        const float HAUTEUR_CAM�RA = 0.25f;

        const int NB_PIXELS_DE_D�PLACEMENT = 1;

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
        }

        public override void Initialize()
        {
            Temps�coul�DepuisMAJ = 0;
            base.Initialize();

            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;

            // AnciennePositionCam�ra = Cam�raJeu.Position; // --> � continuer

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

        protected void EffectuerMise�Jour()
        {
            PositionAncienne = Position;
            CalculerMonde();
        }

        protected void G�rerClavier()
        {
            if (GestionInput.EstClavierActiv�)
            {
                int d�placementHorizontal = G�rerTouche(Keys.U) - G�rerTouche(Keys.J); // touche d = ajoute des pixels � mon image. touche a = enl�ve des pixels
                int d�placementProfondeur = G�rerTouche(Keys.K) - G�rerTouche(Keys.H);
                if (d�placementHorizontal != 0 || d�placementProfondeur != 0)
                {
                    CalculerPosition(d�placementHorizontal, d�placementProfondeur);
                }
            }

            // AjusterPositionCam�ra();
        }

        void CalculerPosition(int d�placementHorizontal, int d�placementProfondeur)
        {
            Vector3 vecteurRayon = new Vector3(0, 0, 0); // Test................
            // Position = new Vector3(Position.X, (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y, Position.Z);


            Sph�reDeCollisionBalle = new BoundingSphere(Position, Sph�reDeCollisionBalle.Radius);  //--> � utiliser pour que lorsque le mod�le 3D entre en collision avec une porte.

            float posX = CalculerPosition(d�placementHorizontal, Position.X);
            float posZ = CalculerPosition(d�placementProfondeur, Position.Z);

            Vector2 vecteurPosition = new Vector2(posX + Terrain.NbColonnes / 2, posZ + Terrain.NbRang�es / 2);

            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y;

            // float posY = (Terrain.GetPointSpatial(Math.Abs((int)Math.Round(posX, 0)), Terrain.NbRang�es - Math.Abs((int)Math.Round(posZ, 0)))).Y;


            Position = new Vector3(posX, posY, posZ);

            Sph�reDeCollisionBalle = new BoundingSphere(Position, Sph�reDeCollisionBalle.Radius);
        }

        float CalculerPosition(int d�placement, float posActuelle)
        {
            float position = posActuelle + d�placement;

            position = MathHelper.Max(MathHelper.Min(position, Terrain.NbColonnes / 2), -Terrain.NbColonnes / 2);
            position = MathHelper.Max(MathHelper.Min(position, Terrain.NbRang�es / 2), -Terrain.NbRang�es / 2);

            return position;
        }

        int G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? NB_PIXELS_DE_D�PLACEMENT : 0;
        }


        void AjusterPositionCam�ra()
        {
            //Vector3 vecteurNormalis� = Position - AnciennePositionCam�ra;

            // Vector3 position = Position - DISTANCE_MOD�LE_CAM�RA * Vector3.Normalize(vecteurNormalis�);

            //position.Y = 21;

            //Cam�raJeu.D�placer(position, Position, Vector3.Up);

            //AnciennePositionCam�ra = position;



            float positionX = Position.X - DISTANCE_MOD�LE_CAM�RA/*- DISTANCE_MOD�LE_CAM�RA * Vector3.Normalize(Position - AnciennePositionCam�ra) + HAUTEUR_CAM�RA * Vector3.Up*/;
            float positionY = 25;
            float positionZ = Position.Z - DISTANCE_MOD�LE_CAM�RA;

            Vector3 positionCam�ra = new Vector3(positionX, positionY, positionZ);

            //position.Y = 25;

            Cam�raJeu.D�placer(positionCam�ra, Position, Vector3.Up);

            AnciennePositionCam�ra = positionCam�ra;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Game.Window.Title = "........" + Position.ToString();
        }
    }
}
