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
        public bool �D�truire { get; set; }
        string NomMod�le { get; set; }
        RessourcesManager<Model> GestionnaireDeMod�les { get; set; }
        public Cam�ra Cam�raJeu { get; set; }
        public float �chelle { get; protected set; }
        public Pokemon UnPokemon { get; protected set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Position { get; set; }

        protected Model Mod�le { get; private set; }
        protected Matrix[] TransformationsMod�le { get; private set; }
        protected Matrix Monde { get; set; }
        public BoundingSphere Sph�reDeCollision { get; protected set; }

        public const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        float Temps�coul�DepuisMAJ { get; set; }
        bool EstPremierSaut { get; set; }
        float Tps { get; set; }
        Vector3 AnciennePositionAl�atoire { get; set; }
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
            return Sph�reDeCollision.Intersects(((ICollisionable)autreObjet).Sph�reDeCollision);
        }
        public ObjetDeBase(Game jeu, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
           : base(jeu)
        {
            NomMod�le = nomMod�le;
            Position = positionInitiale;
            �chelle = �chelleInitiale;
            Rotation = rotationInitiale;
        }
        public ObjetDeBase(Game jeu, Pokemon unPokemon, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
           : base(jeu)
        {
            UnPokemon = unPokemon;
            NomMod�le = nomMod�le;
            Position = positionInitiale;
            �chelle = �chelleInitiale;
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
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
            base.Initialize();
            Sph�reDeCollision = new BoundingSphere(Position, 1);
        }
        public void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateWorld(Position, -((Cam�raJeu) as Cam�raSubjective).Direction, Vector3.Up);
        }
        protected override void LoadContent()
        {
            Cam�raJeu = Game.Services.GetService(typeof(Cam�ra)) as Cam�ra;
            GestionnaireDeMod�les = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            Mod�le = GestionnaireDeMod�les.Find(NomMod�le);
            TransformationsMod�le = new Matrix[Mod�le.Bones.Count];
            Mod�le.CopyAbsoluteBoneTransformsTo(TransformationsMod�le);
        }

        public void ChangerMod�le(string dossier)
        {
            Mod�le = GestionnaireDeMod�les.Find(dossier);
            TransformationsMod�le = new Matrix[Mod�le.Bones.Count];
            Mod�le.CopyAbsoluteBoneTransformsTo(TransformationsMod�le);
        }
        public override void Update(GameTime gameTime)
        {
            Sph�reDeCollision = new BoundingSphere(Position, Sph�reDeCollision.Radius);

            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;

            if (Temps�coul�DepuisMAJ >= INTERVALLE_MAJ_STANDARD)
            {
                EffectuerMise�Jour();
                Temps�coul�DepuisMAJ = 0;
            }

            base.Update(gameTime);
        }


        protected virtual void EffectuerMise�Jour()
        {
            if(!Combat.EnCombat)
            {
                G�rerD�placementAl�atoire();
                CalculerMonde();
            }
        }

        void G�rerD�placementAl�atoire()
        {
            if (EstPremierSaut)
            {
                D�terminerD�placementMod�le();
                EstPremierSaut = false;
            }

            Vector3[] ptsSaut = InitialiserSaut();

            if (Tps > 60)
            {
                D�terminerD�placementMod�le();
                Tps = 0;
            }
            
            Position = CalculerB�zier(Tps * (1f / 60f), ptsSaut);
            // Position = PosTmpPok;
            ++Tps;

            NettoyerListePositions();
        }

        void D�terminerD�placementMod�le()
        {
            const int D�PLACEMENTS_POSSIBLES = 8;

            AnciennePositionAl�atoire = Position;

            ListePositionsRandom.Add(AnciennePositionAl�atoire);

            Random G�r�rateurAl�atoire = new Random();
            float posX;
            float posZ;
            float posY;

            Vector2[] PositionsPossibles = new Vector2[D�PLACEMENTS_POSSIBLES] { new Vector2(Position.X, Position.Z - 3), new Vector2(Position.X, Position.Z + 3), new Vector2(Position.X + 3, Position.Z), new Vector2(Position.X - 3, Position.Z), new Vector2(Position.X + 3, Position.Z - 3), new Vector2(Position.X + 3, Position.Z + 3), new Vector2(Position.X - 3, Position.Z - 3), new Vector2(Position.X - 3, Position.Z + 3) };
            Vector2 vecteurPosition;

            do
            {
                int valeurAl�atoire = G�r�rateurAl�atoire.Next(0, D�PLACEMENTS_POSSIBLES);


                posX = MathHelper.Max(MathHelper.Min(PositionsPossibles[valeurAl�atoire].X, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);
                posZ = MathHelper.Max(MathHelper.Min(PositionsPossibles[valeurAl�atoire].Y, TerrainDeJeu.NbRang�es / 2), -TerrainDeJeu.NbRang�es / 2);

                vecteurPosition = new Vector2(posX + TerrainDeJeu.NbColonnes / 2, posZ + TerrainDeJeu.NbRang�es / 2);


                posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;

            }
            while (posX == ListePositionsRandom.ElementAt(ListePositionsRandom.Count - 2).X && posZ == ListePositionsRandom.ElementAt(ListePositionsRandom.Count - 2).Z);

            PosTmpPok = new Vector3(posX, posY, posZ);

            /*
             
             const int D�PLACEMENTS_POSSIBLES = 8;

            AnciennePositionAl�atoire = Position;

            ListePositionsRandom.Add(AnciennePositionAl�atoire);

            Random G�r�rateurAl�atoire = new Random();
            float posX;
            float posZ;
            float posY;

            Vector2[] PositionsPossibles = new Vector2[D�PLACEMENTS_POSSIBLES] { new Vector2(Position.X, Position.Z - 3), new Vector2(Position.X, Position.Z + 3), new Vector2(Position.X + 3, Position.Z), new Vector2(Position.X - 3, Position.Z), new Vector2(Position.X + 3, Position.Z - 3), new Vector2(Position.X + 3, Position.Z + 3), new Vector2(Position.X - 3, Position.Z - 3), new Vector2(Position.X - 3, Position.Z + 3) };
            int valeurAl�atoire = G�r�rateurAl�atoire.Next(0, D�PLACEMENTS_POSSIBLES);
            Vector2 vecteurPosition = new Vector2(Position.X + TerrainDeJeu.NbColonnes / 2, Position.Z + TerrainDeJeu.NbRang�es / 2);

            posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;

            Vector2 direction = new Vector2(Position.X,Position.Z) - new Vector2(PositionsPossibles[valeurAl�atoire].X, PositionsPossibles[valeurAl�atoire].Y);

            //do
            //{
            //    int valeurAl�atoire = G�r�rateurAl�atoire.Next(0, D�PLACEMENTS_POSSIBLES);


            //    posX = MathHelper.Max(MathHelper.Min(PositionsPossibles[valeurAl�atoire].X, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);
            //    posZ = MathHelper.Max(MathHelper.Min(PositionsPossibles[valeurAl�atoire].Y, TerrainDeJeu.NbRang�es / 2), -TerrainDeJeu.NbRang�es / 2);

            //    vecteurPosition = new Vector2(posX + TerrainDeJeu.NbColonnes / 2, posZ + TerrainDeJeu.NbRang�es / 2);


            //    posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;

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

        Vector3 CalculerB�zier(float t, Vector3[] pts)
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
            foreach (ModelMesh maille in Mod�le.Meshes)
            {
                Matrix mondeLocal = TransformationsMod�le[maille.ParentBone.Index] * GetMonde();
                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.EnableDefaultLighting();
                    effet.Projection = Cam�raJeu.Projection;
                    effet.View = Cam�raJeu.Vue;
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
