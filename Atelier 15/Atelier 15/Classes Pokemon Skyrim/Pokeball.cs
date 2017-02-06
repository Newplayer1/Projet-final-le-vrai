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
    public class Pokeball : PrimitiveDeBaseAnim�e, ICollisionable
    {
        const float NB_DEG_CERCLE = 360f;
        const int NB_TRIANGLES = 2;
        const int NB_SOMMETS = 3;

        string NomTextureSph�re { get; set; }
        Texture2D TextureSph�re { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        Vector3 Origine { get; set; }
        int NbColonnes { get; set; }
        int NbRang�es { get; set; }
        protected float Rayon { get; set; }

        Vector2[] Rang�es { get; set; }
        Vector2[,] PtsTexture { get; set; }
        Vector3[,] PtsSommets { get; set; }
        VertexPositionTexture[] Sommets { get; set; }

        BasicEffect EffetDeBase { get; set; }
        public BoundingSphere Sph�reDeCollision { get; protected set; }


        public bool EstEnCollision(object autreObjet)
        {
            if (!(autreObjet is ICollisionable))
            {
                return false;
            }
            return Sph�reDeCollision.Intersects(((ICollisionable)autreObjet).Sph�reDeCollision);
        }

        public Pokeball(Game jeu, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float rayon, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Rayon = rayon;
            NomTextureSph�re = nomTexture;

            Origine = Vector3.Zero;
            NbColonnes = (int)charpente.X;
            NbRang�es = (int)charpente.Y;
            Sph�reDeCollision = new BoundingSphere(positionInitiale, rayon);
        }

        public override void Initialize()
        {
            NbTriangles = NbColonnes * NbRang�es * NB_TRIANGLES;
            NbSommets = NbTriangles * NB_SOMMETS;
            Cr�erTableaux();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            base.LoadContent();
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TextureSph�re = GestionnaireDeTextures.Find(NomTextureSph�re);

            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParam�tresEffetDeBase();
        }

        protected void InitialiserParam�tresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureSph�re;
        }

        protected void Cr�erTableaux()
        {
            Sommets = new VertexPositionTexture[NbSommets];

            Rang�es = new Vector2[NbRang�es + 1];
            PtsSommets = new Vector3[NbColonnes + 1, NbRang�es + 1];
            PtsTexture = new Vector2[NbColonnes + 1, NbRang�es + 1];

            InitialiserRang�es();
            InitialiserPtsSommets();
            InitialiserPtsTexture();
        }

        void InitialiserRang�es()
        {
            float variationEnRadians = (float)Math.PI / NbRang�es;
            for (int i = 0; i <= NbRang�es; ++i)
            {
                float positionY = (float)Math.Sin(MathHelper.PiOver2 - i * variationEnRadians) * Rayon;//La position du point d�finie par sin(tetha)

                Rang�es[i].X = (float)Math.Sqrt(Math.Pow(Rayon, 2) - Math.Pow(positionY, 2));//Le X correspondant
                Rang�es[i].Y = positionY + Origine.Y;
            }
        }

        void InitialiserPtsSommets()
        {
            float variationEnDegr�s = NB_DEG_CERCLE / NbColonnes;
            for (int i = 0; i <= NbRang�es; i++) // Perc�e des p�les
            {
                for (int j = 0; j <= NbColonnes; j++) //Retirer cartier d'orange
                {
                    float angle = MathHelper.ToRadians(j * variationEnDegr�s);
                    PtsSommets[i, j] = new Vector3(Origine.X + Rang�es[i].X * (float)Math.Sin(angle), Origine.Y + Rang�es[i].Y, Origine.Z + Rang�es[i].X * (float)Math.Cos(angle)); //Le point se trouvant sur un cercle qui est la rang�e (chaque rang�e est un cercle, cylindre)
                }
            }
        }

        void InitialiserPtsTexture()
        {
            Vector2 vectTexture = new Vector2(1f / NbColonnes, 1f / NbRang�es);

            float y = 0;
            for (int i = 0; i <= NbRang�es; ++i)
            {
                float x = 0;
                for (int j = 0; j <= NbColonnes; ++j)
                {
                    PtsTexture[i, j] = new Vector2(x, y);
                    x += vectTexture.X;
                }
                y += vectTexture.Y;
            }
        }

        protected override void InitialiserSommets()
        {
            int NoSommet = 0;
            for (int j = 0; j < NbRang�es; ++j)
            {
                for (int i = 0; i < NbColonnes; ++i)
                {
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]); //Moiti� A
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);

                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]); //Moiti� B
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j + 1], PtsTexture[i + 1, j + 1]);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NbTriangles);
            }
        }
    }
}
