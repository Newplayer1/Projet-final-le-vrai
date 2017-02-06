using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    class SphèreTexturéeAutomate : SphèreTexturée
    {
        const float ÉCART_DÉPLACEMENT = 0.05f;
        const float PI_DIVISÉ_PAR_10 = MathHelper.Pi / 10;

        const int DISTANCE_BALLE_CAMÉRA = 20;
        const float HAUTEUR_CAMÉRA = 0.25f;

        Vector3 Direction { get; set; }
        Vector3 NouvelleDirection { get; set; }
        Vector3 DeltaDéplacement { get; set; }

        Vector3 Latéral { get; set; }

        Vector3 AnciennePositionCaméra { get; set; }

        float VitesseTranslation { get; set; }
        float VitesseRotation { get; set; }
        float AngleRotation { get; set; }

        bool Pause { get; set; }
        bool NouvelleCible { get; set; }
        int NoCible { get; set; }

        List<Vector3> ListeCible { get; set; }

        TerrainAvecBase Terrain { get; set; }
        DataPiste DonnéesPiste { get; set; }

        public SphèreTexturéeAutomate(Game jeu, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float rayon, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, échelleInitiale, rotationInitiale, positionInitiale, rayon, charpente, nomTexture, intervalleMAJ)
        {
            NouvelleCible = true;
            Pause = true;
            NoCible = 1;
        }

        public override void Initialize()
        {
            base.Initialize();
            AngleRotation = 0;
            DonnéesPiste = Game.Services.GetService(typeof(DataPiste)) as DataPiste;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;

            List<Vector2> pointsPatrouille = DonnéesPiste.GetPointsDePatrouille();

            ListeCible = new List<Vector3>(pointsPatrouille.Count - 1);

            foreach (Vector2 vecteur in pointsPatrouille)
                ListeCible.Add(Terrain.GetPointSpatial((int)Math.Round(vecteur.X, 0), Terrain.NbRangées - (int)Math.Round(vecteur.Y, 0)));
            
            AnciennePositionCaméra = CaméraJeu.Position;
        }
        protected override void GérerClavier()
        {
            if (!GestionInput.EstEnfoncée(Keys.LeftControl) && !GestionInput.EstEnfoncée(Keys.RightControl) && GestionInput.EstNouvelleTouche(Keys.Space))
                Pause = !Pause;

            base.GérerClavier();
        }

        protected override void EffectuerMiseÀJour()
        {
            if (!Pause)
            {
                if (NouvelleCible)
                    CalculerDirection();

                Position = Position + DeltaDéplacement;
                CalculerPosition();
                AjusterLatéral();
                AjusterPositionCaméra();

                if (CibleEstAtteinte())
                {
                    if (NoCible < ListeCible.Count - 1)
                        NoCible++;
                    else
                        NoCible = 0;
                    NouvelleCible = true;
                }
                MondeÀRecalculer = true;
            }
            base.EffectuerMiseÀJour();
        }
        void CalculerDirection()
        {
            NouvelleDirection = ListeCible[NoCible] - Position;
            DeltaDéplacement = NouvelleDirection * ÉCART_DÉPLACEMENT;
            Direction = NouvelleDirection;
            NouvelleCible = false;
        }
        void CalculerPosition()
        {
            Vector3 vecteurRayon = new Vector3(0, Rayon, 0);
            Vector2 vecteurPosition = new Vector2(Position.X + Terrain.NbColonnes / 2, Position.Z + Terrain.NbRangées / 2);
            Position = new Vector3(Position.X, (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + vecteurRayon).Y, Position.Z);


            SphèreDeCollision = new BoundingSphere(Position, SphèreDeCollision.Radius);
        }
        void AjusterLatéral()
        {
            Vector2 vecteurPosition = new Vector2(Position.X + Terrain.NbColonnes / 2, Position.Z + Terrain.NbRangées / 2);
            Latéral = Vector3.Normalize(Vector3.Cross(Direction, Terrain.GetNormale((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRangées - (int)Math.Round(vecteurPosition.Y, 0))));

            AngleRotation -= PI_DIVISÉ_PAR_10;
        }
        void AjusterPositionCaméra()
        {
            Vector3 position = Position - DISTANCE_BALLE_CAMÉRA * Vector3.Normalize(Position - AnciennePositionCaméra) + HAUTEUR_CAMÉRA * Vector3.Up;
            CaméraJeu.Déplacer(position, Position, Vector3.Up);

            AnciennePositionCaméra = position;
        }
        bool CibleEstAtteinte()
        {
            return Math.Abs(Vector2.Distance(new Vector2(ListeCible[NoCible].X, ListeCible[NoCible].Z), new Vector2(Position.X, Position.Z))) < ÉCART_DÉPLACEMENT;
        }

        protected override void CalculerMatriceMonde()
        {
            Monde = Matrix.Identity * Matrix.CreateScale(Homothétie) * Matrix.CreateFromAxisAngle(Latéral, AngleRotation) * Matrix.CreateTranslation(Position);
        }
    }
}
