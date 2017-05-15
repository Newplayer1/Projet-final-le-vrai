using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace AtelierXNA
{
    public class ObjetDeBase : Microsoft.Xna.Framework.DrawableGameComponent, ICollisionable, IDestructible
    {
        public bool ÀDétruire { get; set; }
        string NomModèle { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        public Caméra CaméraJeu { get; set; }
        public float Échelle { get; protected set; }
        public Pokemon UnPokemon { get; protected set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Position { get; set; }

        protected Model Modèle { get; private set; }
        protected Matrix[] TransformationsModèle { get; private set; }
        protected Matrix Monde { get; set; }
        public BoundingSphere SphèreDeCollision { get; protected set; }

        public const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        float TempsÉcouléDepuisMAJ { get; set; }
        bool EstPremierSaut { get; set; }
        float Tps { get; set; }
        Vector3 AnciennePositionAléatoire { get; set; }
        Vector3 PositionPokemon { get; set; }
        Vector3 PosTmpPok { get; set; }
        List<Vector3> ListePositionsRandom { get; set; }
        TerrainAvecBase TerrainDeJeu { get; set; }



        public bool EstEnCollision(object autreObjet)
        {
            if (!(autreObjet is ICollisionable))
            {
                return false;
            }
            return SphèreDeCollision.Intersects(((ICollisionable)autreObjet).SphèreDeCollision);
        }
        public ObjetDeBase(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
           : base(jeu)
        {
            NomModèle = nomModèle;
            Position = positionInitiale;
            Échelle = échelleInitiale;
            Rotation = rotationInitiale;
        }
        public ObjetDeBase(Game jeu, Pokemon unPokemon, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
           : base(jeu)
        {
            UnPokemon = unPokemon;
            NomModèle = nomModèle;
            Position = positionInitiale;
            Échelle = échelleInitiale;
            Rotation = rotationInitiale;
        }
        public override void Initialize()
        {
            EstPremierSaut = true;
            Tps = 0;
            Vector3 positionTmp = new Vector3(0, 0, 0);
            ListePositionsRandom = new List<Vector3>();
            ListePositionsRandom.Add(positionTmp);
            TerrainDeJeu = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;

            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
            base.Initialize();
            SphèreDeCollision = new BoundingSphere(Position, 1);
        }
        public void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateWorld(Position, -((CaméraJeu) as CaméraSubjective).Direction, Vector3.Up);
        }
        protected override void LoadContent()
        {
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as Caméra;
            GestionnaireDeModèles = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            Modèle = GestionnaireDeModèles.Find(NomModèle);
            TransformationsModèle = new Matrix[Modèle.Bones.Count];
            Modèle.CopyAbsoluteBoneTransformsTo(TransformationsModèle);
        }

        public void ChangerModèle(string dossier)
        {
            Modèle = GestionnaireDeModèles.Find(dossier);
            TransformationsModèle = new Matrix[Modèle.Bones.Count];
            Modèle.CopyAbsoluteBoneTransformsTo(TransformationsModèle);
        }
        public override void Update(GameTime gameTime)
        {
            SphèreDeCollision = new BoundingSphere(Position, SphèreDeCollision.Radius);

            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= INTERVALLE_MAJ_STANDARD)
            {
                EffectuerMiseÀJour();
                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }


        protected virtual void EffectuerMiseÀJour()
        {
            if(!Combat.EnCombat)
            {
                GérerDéplacementAléatoire();
                CalculerMonde();
            }
        }

        void GérerDéplacementAléatoire()
        {
            if (EstPremierSaut)
            {
                DéterminerDéplacementModèle();
                EstPremierSaut = false;
            }

            Vector3[] ptsSaut = InitialiserSaut();

            if (Tps > 60)
            {
                DéterminerDéplacementModèle();
                Tps = 0;
            }
            
            Position = CalculerBézier(Tps * (1f / 60f), ptsSaut);
            // Position = PosTmpPok;
            ++Tps;

            NettoyerListePositions();
        }

        void DéterminerDéplacementModèle()
        {
            const int DÉPLACEMENTS_POSSIBLES = 8;

            AnciennePositionAléatoire = Position;

            ListePositionsRandom.Add(AnciennePositionAléatoire);

            Random GérérateurAléatoire = new Random();
            float posX;
            float posZ;
            float posY;

            Vector2[] PositionsPossibles = new Vector2[DÉPLACEMENTS_POSSIBLES] { new Vector2(Position.X, Position.Z - 3), new Vector2(Position.X, Position.Z + 3), new Vector2(Position.X + 3, Position.Z), new Vector2(Position.X - 3, Position.Z), new Vector2(Position.X + 3, Position.Z - 3), new Vector2(Position.X + 3, Position.Z + 3), new Vector2(Position.X - 3, Position.Z - 3), new Vector2(Position.X - 3, Position.Z + 3) };
            Vector2 vecteurPosition;

            do
            {
                int valeurAléatoire = GérérateurAléatoire.Next(0, DÉPLACEMENTS_POSSIBLES);


                posX = MathHelper.Max(MathHelper.Min(PositionsPossibles[valeurAléatoire].X, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);
                posZ = MathHelper.Max(MathHelper.Min(PositionsPossibles[valeurAléatoire].Y, TerrainDeJeu.NbRangées / 2), -TerrainDeJeu.NbRangées / 2);

                vecteurPosition = new Vector2(posX + TerrainDeJeu.NbColonnes / 2, posZ + TerrainDeJeu.NbRangées / 2);


                posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;

            }
            while (posX == ListePositionsRandom.ElementAt(ListePositionsRandom.Count - 2).X && posZ == ListePositionsRandom.ElementAt(ListePositionsRandom.Count - 2).Z);

            PosTmpPok = new Vector3(posX, posY, posZ);

            /*
             
             const int DÉPLACEMENTS_POSSIBLES = 8;

            AnciennePositionAléatoire = Position;

            ListePositionsRandom.Add(AnciennePositionAléatoire);

            Random GérérateurAléatoire = new Random();
            float posX;
            float posZ;
            float posY;

            Vector2[] PositionsPossibles = new Vector2[DÉPLACEMENTS_POSSIBLES] { new Vector2(Position.X, Position.Z - 3), new Vector2(Position.X, Position.Z + 3), new Vector2(Position.X + 3, Position.Z), new Vector2(Position.X - 3, Position.Z), new Vector2(Position.X + 3, Position.Z - 3), new Vector2(Position.X + 3, Position.Z + 3), new Vector2(Position.X - 3, Position.Z - 3), new Vector2(Position.X - 3, Position.Z + 3) };
            int valeurAléatoire = GérérateurAléatoire.Next(0, DÉPLACEMENTS_POSSIBLES);
            Vector2 vecteurPosition = new Vector2(Position.X + TerrainDeJeu.NbColonnes / 2, Position.Z + TerrainDeJeu.NbRangées / 2);

            posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;

            Vector2 direction = new Vector2(Position.X,Position.Z) - new Vector2(PositionsPossibles[valeurAléatoire].X, PositionsPossibles[valeurAléatoire].Y);

            //do
            //{
            //    int valeurAléatoire = GérérateurAléatoire.Next(0, DÉPLACEMENTS_POSSIBLES);


            //    posX = MathHelper.Max(MathHelper.Min(PositionsPossibles[valeurAléatoire].X, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);
            //    posZ = MathHelper.Max(MathHelper.Min(PositionsPossibles[valeurAléatoire].Y, TerrainDeJeu.NbRangées / 2), -TerrainDeJeu.NbRangées / 2);

            //    vecteurPosition = new Vector2(posX + TerrainDeJeu.NbColonnes / 2, posZ + TerrainDeJeu.NbRangées / 2);


            //    posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;

            //}
            //while (posX == ListePositionsRandom.ElementAt(ListePositionsRandom.Count - 2).X && posZ == ListePositionsRandom.ElementAt(ListePositionsRandom.Count - 2).Z);

            //Vector3 direc = Position + new Vector3(direction.X, 0, direction.Y);

            PosTmpPok = new Vector3(direc.X, posY, direc.Z);
                       
             */
        }

        Vector3[] InitialiserSaut()
        {
            Vector3[] tabPts = new Vector3[4];

            tabPts[0] = Position;
            tabPts[3] = PosTmpPok;

            tabPts[1] = new Vector3(tabPts[0].X, tabPts[0].Y + 1, tabPts[0].Z);
            tabPts[2] = new Vector3(tabPts[3].X, tabPts[3].Y + 1, tabPts[3].Z);

            //tabPts[1] = new Vector3((tabPts[3].X - tabPts[0].X) / 3, (tabPts[3].X - tabPts[0].X) / 3, (tabPts[3].Z - tabPts[0].Z) / 3) + Position;
            //tabPts[2] = new Vector3((2 * (tabPts[3].X - tabPts[0].X)) / 3, (2 * (tabPts[3].X - tabPts[0].X)) / 3, (2 * (tabPts[3].Z - tabPts[0].Z)) / 3) + Position;

            return tabPts;
        }

        void NettoyerListePositions()
        {
            int cpt = 0;
            int grandeurListe = ListePositionsRandom.Count;
            for (int i = 0; i < grandeurListe - 1; i++)
            {
                ListePositionsRandom.RemoveAt(cpt);
            }
        }

        Vector3 CalculerBézier(float t, Vector3[] pts)
        {
            float moinsUn = (1 - t);

            return pts[0] * (float)Math.Pow(moinsUn, 3) + 3 * pts[1] * t * (float)Math.Pow(moinsUn, 2) + 3 * pts[2] * (float)Math.Pow(t, 2) * moinsUn + pts[3] * (float)Math.Pow(t, 3);
            //return pts[0] * (float)Math.Pow(moinsUn, 3) +
            //    3 * pts[1] * t * (float)Math.Pow(moinsUn, 2) +
            //    3 * pts[2] * t * t * moinsUn +
            //    pts[3] * t * t * t;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh maille in Modèle.Meshes)
            {
                Matrix mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.EnableDefaultLighting();
                    effet.Projection = CaméraJeu.Projection;
                    effet.View = CaméraJeu.Vue;
                    effet.World = mondeLocal;
                }
                maille.Draw();
            }

            Game.Window.Title = Position.ToString();
        }

        public virtual Matrix GetMonde()
        {
            return Monde;
        }
    }
}
