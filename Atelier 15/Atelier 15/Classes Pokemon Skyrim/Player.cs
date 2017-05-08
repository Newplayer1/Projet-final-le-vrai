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
        public const int DISTANCE_MODÈLE_CAMÉRA = 1;
        const float vitesseDéplacement = 0.2f;
        const float HAUTEUR_CAMÉRA = 2f;
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
        const float DÉPLACEMEMENT_MODÈLE = /*0.05f*/1f;
        public float Hauteur { get; private set; }
        TerrainAvecBase Terrain { get; set; }
        const float VitesseRotation = 5f;
        public Vector2 Souris { get; private set; }

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float Rayon { get; set; }
        public Vector3 UpPositionTrainer { get; set; }
        float positionMilieuHauteurterrain;

        protected InputManager GestionInput { get; private set; }
        bool inventaireOuvert;

        BasicEffect EffetDeBase { get; set; }
        Vector3 Direction { get; set; }
        Vector3 OrientationVertical { get; } = Vector3.Up;
        Vector3 Latéral { get; set; }
        float AngleDirection { get; set; }
        public Vector3 DirectionCaméra { get; private set; }

        public Player(Game jeu, string nomModèle, float échelle, Vector3 rotation, Vector3 position, float intervallleMAJ, float rayon)
            : base(jeu, "PLAYER", nomModèle, échelle, rotation, position)
        {
            IntervalleMAJ = intervallleMAJ;
            Rayon = rayon;

            SphèreDeCollision = new BoundingSphere(position, Rayon);
            Hauteur = 2000 * Rayon * Échelle;
        }

        public override void Initialize()
        {
            inventaireOuvert = false;
            UpPositionTrainer = Vector3.Up + Position;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
            positionMilieuHauteurterrain = Terrain.HAUTEUR_MAXIMALE / 2;
        }

        public override void Update(GameTime gameTime)
        {
            InventairePoks();

            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMiseÀJour();
                TempsÉcouléDepuisMAJ = 0;
            }
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
            SphèreDeCollision = new BoundingSphere(Position, SphèreDeCollision.Radius);
        }

        protected void EffectuerMiseÀJour()
        {
            if (!(Combat.EnCombat || AfficheurTexte.MessageEnCours))
            {
                Bouger();
                TournerTrainer();
            }
            CaméraJeu.Position = new Vector3(Position.X + 3, Position.Y + HAUTEUR_CAMÉRA, Position.Z - 3);
            CalculerMonde();

        }

        private void InventairePoks()
        {
            if (GestionInput.EstNouvelleTouche(Keys.P) && !inventaireOuvert && !Combat.EnCombat)
            {
                foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                {
                    t.ÀDétruire = true;
                }
                string InventaireParLigne = null;
                for (int i = 0; i < GetNbPokemon; i++)
                {
                    InventaireParLigne = GetPokemon(i).Nom + " Level : " + GetPokemon(i).Level + " Type1 : " + GetPokemon(i).Type1;  
                    if(GetPokemon(i).Type2 != "NULL")
                    {
                        InventaireParLigne += " Type2 : " + GetPokemon(i).Type2;
                    }
                    InventaireParLigne += " HP : " + GetPokemon(i).HP + " Exp : " +GetPokemon(i).Exp;

                    Game.Components.Add(new TexteFixe(Game, new Vector2(1, 1 + i * 16), InventaireParLigne));
                }
            inventaireOuvert = !inventaireOuvert;
            }
            else if  (GestionInput.EstNouvelleTouche(Keys.P) && inventaireOuvert)
            {
                foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                {
                    t.ÀDétruire = true;
                }
                inventaireOuvert = !inventaireOuvert;
            }
        }

        private void TournerTrainer()
        {
            int valYaw = GestionInput.GetPositionSouris().X > Souris.X ? 1 : -1;
            int valPitch = GestionInput.GetPositionSouris().Y > Souris.Y ? 1 : -1;
             Vector3 DirectionJoueur = Monde.Forward - Monde.Backward;  

            //déplacement horizontale Angle # pas de limite
            if (GestionInput.GetPositionSouris().X != Souris.X)
            {
                float valRotationAjouter = DELTA_LACET * valYaw * VitesseRotation;
                ((CaméraJeu) as CaméraSubjective).Direction = Vector3.Normalize(Vector3.Transform(((CaméraJeu) as CaméraSubjective).Direction, Matrix.CreateFromAxisAngle(((CaméraJeu) as CaméraSubjective).OrientationVerticale, valRotationAjouter)));
                DirectionCaméra = ((CaméraJeu) as CaméraSubjective).Direction; // Pour qu'on puisse avoir accès à la direction de la caméra dans la classe pokéball 
                DirectionCaméra = Vector3.Normalize(DirectionCaméra);
                Rotation = new Vector3(0, Rotation.Y + valRotationAjouter, 0);
                //Vector3 PositionCaméra = Vector3.Transform(CaméraJeu.Position, Matrix.CreateFromAxisAngle(DirectionJoueur, valRotationAjouter));
                //CaméraJeu.CréerPointDeVue(PositionCaméra,Position,Vector3.Up);
            }
        }
        protected void BougerCaméra(float déplacementHorizontal, float déplacementProfondeur)
        {
            ((CaméraJeu) as CaméraSubjective).GérerDéplacement(déplacementProfondeur, déplacementHorizontal);
            CaméraJeu.CréerPointDeVue(CaméraJeu.Position, new Vector3(Position.X, Position.Y + 2f, Position.Z), CaméraJeu.OrientationVerticale);
        }
        protected void Bouger()
        {
            if (GestionInput.EstClavierActivé)
            {
                float déplacementHorizontal = (GérerTouche(Keys.D) - GérerTouche(Keys.A)) * vitesseDéplacement;
                float déplacementProfondeur = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * vitesseDéplacement;
                if (déplacementHorizontal != 0 || déplacementProfondeur != 0)
                {
                    BougerTrainer(déplacementHorizontal, déplacementProfondeur);
                    BougerCaméra(déplacementHorizontal, déplacementProfondeur);
                }
            }
        }
        protected void BougerTrainer(float déplacementHorizontal, float déplacementProfondeur)
        {
            Direction = ((CaméraJeu) as CaméraSubjective).Direction;
            Latéral = Vector3.Cross(Direction, OrientationVertical);

            Position += Direction * déplacementProfondeur;
            Position += Latéral * déplacementHorizontal;
            Limites();

            Vector2 vecteurPosition = new Vector2(Position.X + Terrain.NbColonnes / 2, Position.Z + Terrain.NbRangées / 2);
            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
            Position = new Vector3(Position.X, posY, Position.Z);

            SphèreDeCollision = new BoundingSphere(Position, SphèreDeCollision.Radius);

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
            Game.Window.Title = Game.Components.Count.ToString();
        }
    }
}
