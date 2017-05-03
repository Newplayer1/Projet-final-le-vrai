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

    public class Player : Trainer
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
        bool inventaireOuvert;

        BasicEffect EffetDeBase { get; set; }
        Vector3 Direction { get; set; }
        Vector3 OrientationVertical { get; } = Vector3.Up;
        Vector3 Lat�ral { get; set; }
        float AngleDirection { get; set; }
        public Vector3 DirectionCam�ra { get; private set; }

        public Player(Game jeu, string nomMod�le, float �chelle, Vector3 rotation, Vector3 position, float intervallleMAJ, float rayon)
            : base(jeu, "PLAYER", nomMod�le, �chelle, rotation, position)
        {
            IntervalleMAJ = intervallleMAJ;
            Rayon = rayon;

            Sph�reDeCollision = new BoundingSphere(position, Rayon);
            Hauteur = 2000 * Rayon * �chelle;
        }

        public override void Initialize()
        {
            inventaireOuvert = false;
            UpPositionTrainer = Vector3.Up + Position;
            Temps�coul�DepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
        }

        public override void Update(GameTime gameTime)
        {
            InventairePoks();

            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMise�Jour();
                Temps�coul�DepuisMAJ = 0;
            }
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
            Sph�reDeCollision = new BoundingSphere(Position, Sph�reDeCollision.Radius);
        }

        protected void EffectuerMise�Jour()
        {
            if (!(Combat.EnCombat || AfficheurTexte.MessageEnCours))
            {
                BougerTrainer();
                TournerTrainer();
            }
            Cam�raJeu.Position = new Vector3(Position.X + 3, Position.Y + HAUTEUR_CAM�RA, Position.Z - 3);
            CalculerMonde();

        }

        private void InventairePoks()
        {
            if (GestionInput.EstNouvelleTouche(Keys.P) && !inventaireOuvert)
            {
                foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                {
                    t.�D�truire = true;
                }
                string InventaireParLigne = null;
                for (int i = 0; i < GetNbPokemon; i++)
                {
                    InventaireParLigne = GetNomPokemon()[i] + " Level : " + GetLVLPokemon()[i] + " Type1 : " + GetType1Pokemon()[i];  
                    if(GetType2Pokemon()[i] != "NULL")
                    {
                        InventaireParLigne += " Type2 : " + GetType2Pokemon()[i];
                    }
                    InventaireParLigne += " HP : " + GetHPPokemon()[i];

                    Game.Components.Add(new TexteFixe(Game, new Vector2(1, 1 + i * 16), InventaireParLigne));
                }
            inventaireOuvert = !inventaireOuvert;
            }
            else if  (GestionInput.EstNouvelleTouche(Keys.P) && inventaireOuvert)
            {
                foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                {
                    t.�D�truire = true;
                }
                inventaireOuvert = !inventaireOuvert;
            }
        }

        private void TournerTrainer()
        {
            int valYaw = GestionInput.GetPositionSouris().X > Souris.X ? 1 : -1;
            int valPitch = GestionInput.GetPositionSouris().Y > Souris.Y ? 1 : -1;

            //d�placement horizontale Angle # pas de limite
            if (GestionInput.GetPositionSouris().X != Souris.X)
            {
                float valRotationAjouter = DELTA_LACET * valYaw * VitesseRotation;
                ((Cam�raJeu) as Cam�raSubjective).Direction = Vector3.Normalize(Vector3.Transform(((Cam�raJeu) as Cam�raSubjective).Direction, Matrix.CreateFromAxisAngle(((Cam�raJeu) as Cam�raSubjective).OrientationVerticale, valRotationAjouter)));
                DirectionCam�ra = ((Cam�raJeu) as Cam�raSubjective).Direction; // Pour qu'on puisse avoir acc�s � la direction de la cam�ra dans la classe pok�ball 
                DirectionCam�ra = Vector3.Normalize(DirectionCam�ra);
                Rotation = new Vector3(0, Rotation.Y + valRotationAjouter, 0);
                //Vector3 PositionCam�ra = Vector3.Transform(Cam�raJeu.Position, Matrix.CreateFromAxisAngle(Position, valRotationAjouter));
                //Cam�raJeu.Cr�erPointDeVue(PositionCam�ra,Position,Vector3.Up);
            }
            // d�placement vertical Angle # limite = 45'
            if (GestionInput.GetPositionSouris().Y != Souris.Y)
            {
                ((Cam�raJeu) as Cam�raSubjective).Direction = Vector3.Normalize(Vector3.Transform(((Cam�raJeu) as Cam�raSubjective).Direction, Matrix.CreateFromAxisAngle(((Cam�raJeu) as Cam�raSubjective).Lat�ral, DELTA_TANGAGE * valPitch * VitesseRotation)));
                Vector3 ancienneDirection = ((Cam�raJeu) as Cam�raSubjective).Direction;
                AngleDirection = (float)Math.Asin(((Cam�raJeu) as Cam�raSubjective).Direction.Y);

                if (AngleDirection > 45 || AngleDirection < -45)
                {
                    ((Cam�raJeu) as Cam�raSubjective).Direction = ancienneDirection;
                }
            }
        }

        protected void BougerTrainer()
        {
            if (GestionInput.EstClavierActiv�)
            {
                float d�placementHorizontal = G�rerTouche(Keys.D) - G�rerTouche(Keys.A);
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

            Sph�reDeCollision = new BoundingSphere(Position, Sph�reDeCollision.Radius);

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
            Game.Window.Title = Game.Components.Count.ToString();
        }
    }
}
