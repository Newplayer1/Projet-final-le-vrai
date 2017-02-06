using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class SphèreTexturée : PrimitiveDeBaseAnimée, ICollisionable
    {
        const float NB_DEG_CERCLE = 360f;
        const int NB_TRIANGLES = 2;
        const int NB_SOMMETS = 3;

        string NomTextureSphère { get; set; }
        Texture2D TextureSphère { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        Vector3 Origine { get; set; }
        int NbColonnes { get; set; }
        int NbRangées { get; set; }
        protected float Rayon { get; set; }

        Vector2[] Rangées { get; set; }
        Vector2[,] PtsTexture { get; set; }
        Vector3[,] PtsSommets { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        
        BasicEffect EffetDeBase { get; set; }
        public BoundingSphere SphèreDeCollision { get; protected set; }
        
        
        public bool EstEnCollision(object autreObjet)
        {
            if (!(autreObjet is ICollisionable))
            {
                return false;
            }
            return SphèreDeCollision.Intersects(((ICollisionable)autreObjet).SphèreDeCollision);
        }


        public SphèreTexturée(Game jeu, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float rayon, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Rayon = rayon;
            NomTextureSphère = nomTexture;

            Origine = Vector3.Zero;
            NbColonnes = (int)charpente.X;
            NbRangées = (int)charpente.Y;
            SphèreDeCollision = new BoundingSphere(positionInitiale, rayon);
        }

        public override void Initialize()
        {
            NbTriangles = NbColonnes * NbRangées * NB_TRIANGLES;
            NbSommets = NbTriangles * NB_SOMMETS;
            CréerTableaux();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TextureSphère = GestionnaireDeTextures.Find(NomTextureSphère);
            
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();
        }
        protected void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureSphère;
        }

        protected void CréerTableaux()
        {
            Sommets = new VertexPositionTexture[NbSommets];

            Rangées = new Vector2[NbRangées + 1];
            PtsSommets = new Vector3[NbColonnes + 1, NbRangées + 1];
            PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];

            InitialiserRangées();
            InitialiserPtsSommets();
            InitialiserPtsTexture();
        }

        void InitialiserRangées()
        {
            float variationEnRadians = (float)Math.PI / NbRangées;
            for (int i = 0; i <= NbRangées; ++i)
            {
                float positionY = (float)Math.Sin(MathHelper.PiOver2 - i * variationEnRadians) * Rayon;//La position du point définie par sin(tetha)

                Rangées[i].X = (float)Math.Sqrt(Math.Pow(Rayon, 2) - Math.Pow(positionY, 2));//Le X correspondant
                Rangées[i].Y = positionY + Origine.Y;
            }
        }

        void InitialiserPtsSommets()
        {
            float variationEnDegrés = NB_DEG_CERCLE / NbColonnes;
            for (int i = 0; i <= NbRangées; i++) // Percée des pôles
            {
                for (int j = 0; j <= NbColonnes; j++) //Retirer cartier d'orange
                {
                    float angle = MathHelper.ToRadians(j * variationEnDegrés);
                    PtsSommets[i,j] = new Vector3(Origine.X + Rangées[i].X * (float)Math.Sin(angle), Origine.Y + Rangées[i].Y, Origine.Z + Rangées[i].X * (float)Math.Cos(angle)); //Le point se trouvant sur un cercle qui est la rangée (chaque rangée est un cercle, cylindre)
                }
            }
        }
        void InitialiserPtsTexture()
        {
            Vector2 vectTexture = new Vector2(1f/NbColonnes, 1f/NbRangées);

            float y = 0;
            for (int i = 0; i <= NbRangées; ++i)
            {
                float x = 0;
                for (int j = 0; j <= NbColonnes; ++j)
                {
                    PtsTexture[i,j] = new Vector2(x, y);
                    x += vectTexture.X;
                }
                y += vectTexture.Y;
            }
        }

        protected override void InitialiserSommets()
        {
            int NoSommet = 0;
            for (int j = 0; j < NbRangées; ++j)
            {
                for (int i = 0; i < NbColonnes; ++i)
                {
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]); //Moitié A
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);

                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]); //Moitié B
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j + 1], PtsTexture[i + 1, j + 1]);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NbTriangles);
            }
        }
    }
}
