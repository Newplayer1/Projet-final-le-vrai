using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


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
            base.Update(gameTime);
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
        }

        public virtual Matrix GetMonde()
        {
            return Monde;
        }
    }
}
