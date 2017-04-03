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
        const int DISTANCE_MOD�LE_CAM�RA = 1;
        const float HAUTEUR_CAM�RA = 0.25f;

        const float D�PLACEMEMENT_MOD�LE = /*0.05f*/1f;
        public  float Hauteur {get; private set;}
        TerrainAvecBase Terrain { get; set; }
        protected Point PositionSouris { get; set; }
        private Point AnciennePositionSouris { get; set; }

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
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            // AnciennePositionCam�ra = Cam�raJeu.Position; // --> � continuer

        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            PositionSouris = GestionInput.GetPositionSouris();
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
                float d�placementHorizontal = G�rerTouche(Keys.S) - G�rerTouche(Keys.W); // touche d = ajoute des pixels � mon image. touche a = enl�ve des pixels
                float d�placementProfondeur = G�rerTouche(Keys.A) - G�rerTouche(Keys.D);
                if (d�placementHorizontal != 0 || d�placementProfondeur != 0)
                {
                    CalculerPosition(d�placementHorizontal, d�placementProfondeur);
                }
            }

             AjusterPositionCam�ra();
        }

        void CalculerPosition(float d�placementHorizontal, float d�placementProfondeur)
        {
            Vector3 vecteurRayon = new Vector3(0, 0, 0); // Test................
            // Position = new Vector3(Position.X, (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y, Position.Z);


            Sph�reDeCollisionBalle = new BoundingSphere(Position, Sph�reDeCollisionBalle.Radius);  //--> � utiliser pour que lorsque le mod�le 3D entre en collision avec une porte.

            float posX = CalculerPosition2(d�placementHorizontal, Position.X);
            float posZ = CalculerPosition2(d�placementProfondeur, Position.Z);

            Vector2 vecteurPosition = new Vector2(posX + Terrain.NbColonnes / 2, posZ + Terrain.NbRang�es / 2);

            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y;

            // float posY = (Terrain.GetPointSpatial(Math.Abs((int)Math.Round(posX, 0)), Terrain.NbRang�es - Math.Abs((int)Math.Round(posZ, 0)))).Y;


            Position = new Vector3(posX, posY, posZ);

            Sph�reDeCollisionBalle = new BoundingSphere(Position, Sph�reDeCollisionBalle.Radius);
        }

        float CalculerPosition2(float d�placement, float posActuelle)
        {
            float position = posActuelle + d�placement;

            position = MathHelper.Max(MathHelper.Min(position, Terrain.NbColonnes / 2), -Terrain.NbColonnes / 2);
            position = MathHelper.Max(MathHelper.Min(position, Terrain.NbRang�es / 2), -Terrain.NbRang�es / 2);

            return position;
        }

        float G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? D�PLACEMEMENT_MOD�LE : 0;
        }


        void AjusterPositionCam�ra()
        {
            
            //Bouger
            float positionX = Position.X;
            float positionY = Position.Y + Hauteur;
            float positionZ = Position.Z + (DISTANCE_MOD�LE_CAM�RA);
            Vector3 positionCam�ra = new Vector3(positionX, positionY, positionZ);
            Cam�raJeu.D�placer(positionCam�ra, new Vector3(Position.X,Position.Y + 1.1f, Position.Z), Vector3.Up);
           


            //Tourner
            PositionSouris = GestionInput.GetPositionSouris();
            Cam�raJeu.Cible = new Vector3(PositionSouris.X - Game.Window.ClientBounds.Width / 2, 0, 0);
            (Cam�raJeu as Cam�raSubjective).Direction = Cam�raJeu.Cible - Position;
            Cam�raJeu.D�placer(positionCam�ra, (Cam�raJeu as Cam�raSubjective).Direction, Vector3.Up);
            if(PositionSouris.X == Game.Window.ClientBounds.Width)
            {
                AnciennePositionSouris = PositionSouris;
                PositionSouris = new Point(Game.Window.ClientBounds.Width / 2, PositionSouris.Y);


                Cam�raJeu.Cible = new Vector3(PositionSouris.X - Game.Window.ClientBounds.Width / 2, 0, 0);
                (Cam�raJeu as Cam�raSubjective).Direction = Cam�raJeu.Cible - Position;
                Cam�raJeu.D�placer(positionCam�ra, (Cam�raJeu as Cam�raSubjective).Direction, Vector3.Up);
                PositionSouris = AnciennePositionSouris;
            }
            if (PositionSouris.X == 0)
            {
                AnciennePositionSouris = PositionSouris;
                PositionSouris = new Point(Game.Window.ClientBounds.Width / 2, PositionSouris.Y);

                Cam�raJeu.Cible = new Vector3(PositionSouris.X - Game.Window.ClientBounds.Width / 2, 0, 0);
                (Cam�raJeu as Cam�raSubjective).Direction = Cam�raJeu.Cible - Position;
                Cam�raJeu.D�placer(positionCam�ra, (Cam�raJeu as Cam�raSubjective).Direction, Vector3.Up);
                PositionSouris = AnciennePositionSouris;
            }

            AnciennePositionCam�ra = positionCam�ra;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Game.Window.Title = "........" + Cam�raJeu.Position.ToString() + "........" + Position.ToString();
        }
    }
}
