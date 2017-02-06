using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class TerrainAvecBase : PrimitiveDeBaseAnimée
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;
        const float MAX_COULEUR = 255f;

        Vector3 Étendue { get; }
        string NomCarteTerrain { get; }
        string[] NomTextureTerrain { get; }
        int NbNiveauTexture { get; }
        string NomTextureBase { get; }
        float IntervalleMAJ { get; set; }

        BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        Texture2D CarteTerrain { get; set; }
        Texture2D TextureTerrain { get; set; }
        Texture2D TextureSable { get; set; }
        Texture2D TextureHerbe { get; set; }
        Texture2D TextureBase { get; set; }

        Vector3 Origine { get; set; }

        Vector2 DeltaCarte { get; set; }
        float HauteurMinimale { get; set; }
        float HauteurMaximale { get; set; }
        float VariationHauteur { get; set; }

        public int NbColonnes { get; private set; }
        public int NbRangées { get; private set; }

        int NbTrianglesSurface { get; set; }
        int NbTrianglesBase { get; set; }

        float ÉcartTexture { get; set; }
        Vector3[,] PtsSommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        Color[] TabCouleur { get; set; }
        Vector3[,] NormalesSommets { get; set; }
        VertexPositionTexture[] Sommets { get; set; }

        public TerrainAvecBase(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
            Vector3 étendue, string nomCarteTerrain, string[] nomTextureTerrain,/* int nbNiveauxTexture,*/
            string nomTextureBase, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Étendue = étendue;
            NomCarteTerrain = nomCarteTerrain;
            NomTextureTerrain = nomTextureTerrain;
            NomTextureBase = nomTextureBase;
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            HauteurMinimale = MAX_COULEUR;
            HauteurMaximale = 0;

            InitialiserDonnéesCarte();
            InitialiserDonnéesTexture();

            Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2);

            AllouerTableaux();
            CréerTableauPoints();

            IniTableauTextures();
            CalculerNormales();
            CréerTextureDuTerrain();

            base.Initialize();
        }

        void InitialiserDonnéesCarte()
        {
            CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);

            NbColonnes = CarteTerrain.Width - 1;
            NbRangées = CarteTerrain.Height - 1;

            TabCouleur = new Color[CarteTerrain.Width * CarteTerrain.Height];

            CarteTerrain.GetData(TabCouleur);

            DeltaCarte = new Vector2(Étendue.X / NbColonnes, Étendue.Z / NbRangées);
            NbTrianglesSurface = NbColonnes * NbRangées * NB_TRIANGLES_PAR_TUILE;
            NbTrianglesBase = 2 * (NbColonnes + NbRangées) * NB_TRIANGLES_PAR_TUILE;

            NbTriangles = NbTrianglesSurface + NbTrianglesBase;
            NbSommets = NbTriangles * NB_SOMMETS_PAR_TRIANGLE;
        }

        void InitialiserDonnéesTexture()
        {
            TextureSable = GestionnaireDeTextures.Find(NomTextureTerrain[0]);
            TextureHerbe = GestionnaireDeTextures.Find(NomTextureTerrain[1]);

            TextureBase = GestionnaireDeTextures.Find(NomTextureBase);
        }

        void AllouerTableaux()
        {
            PtsSommets = new Vector3[NbColonnes + 1, NbRangées + 1];
            Sommets = new VertexPositionTexture[NbSommets];

            PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();
        }

        void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureTerrain;
        }

        void CréerTableauPoints()
        {
            for (int i = 0; i <= NbColonnes; ++i)
                for (int j = 0; j <= NbRangées; ++j)
                {
                    float OrigineX = Origine.X;
                    float OrigineZ = Origine.Z;

                    OrigineX += i * DeltaCarte.X;
                    OrigineZ -= j * DeltaCarte.Y;

                    PtsSommets[i, j] = new Vector3(OrigineX, CalculerHauteur(i, NbRangées - j), OrigineZ);

                    if (PtsSommets[i, j].Y > HauteurMaximale)
                        HauteurMaximale = PtsSommets[i, j].Y;

                    if (PtsSommets[i, j].Y < HauteurMinimale)
                        HauteurMinimale = PtsSommets[i, j].Y;
                }
            VariationHauteur = HauteurMaximale - HauteurMinimale;
        }

        float CalculerHauteur(int i, int j)
        {
            return TabCouleur[i + (NbRangées + 1) * j].B / MAX_COULEUR * Étendue.Y;
        }
        void IniTableauTextures()
        {
            float num1 = 1f / NbColonnes;
            float num2 = 1f / NbRangées;

            float x = 0;
            for (int i = 0; i <= NbColonnes; ++i)
            {
                float y = 1;
                for (int j = 0; j <= NbRangées; ++j)
                {
                    PtsTexture[i, j] = new Vector2(x, y);
                    y -= num2;
                }
                x += num1;
            }
        }
        void CalculerNormales()
        {
            NormalesSommets = new Vector3[NbColonnes + 1, NbRangées + 1];

            for (int i = 0; i < NbColonnes; ++i)
                for (int j = 0; j < NbRangées; ++j)
                {
                    Vector3 normale = Vector3.Normalize(Vector3.Cross(PtsSommets[i + 1, j] - PtsSommets[i, j], PtsSommets[i, j + 1] - PtsSommets[i, j]));

                    NormalesSommets[i, j] = normale;
                    NormalesSommets[i + 1, j] = normale;
                    NormalesSommets[i, j + 1] = normale;
                }
        }

        void CréerTextureDuTerrain()
        {
            TextureTerrain = new Texture2D(CarteTerrain.GraphicsDevice, CarteTerrain.Width, CarteTerrain.Height);

            Color[] nouvellesCouleurs = new Color[CarteTerrain.Width * CarteTerrain.Height];

            Color[] couleurSable = new Color[CarteTerrain.Width * CarteTerrain.Height];
            Color[] couleurHerbe = new Color[CarteTerrain.Width * CarteTerrain.Height];

            TextureSable.GetData(couleurSable);
            TextureHerbe.GetData(couleurHerbe);

            for (int i = 0; i <= NbColonnes; ++i)
                for (int j = 0; j <= NbRangées; ++j)
                {
                    float rapport = ((PtsSommets[i, j].Y - HauteurMinimale) / (VariationHauteur));
                    int k = i * CarteTerrain.Width + j;

                    byte canalR = (byte)(couleurSable[k].R * (1 - rapport) + couleurHerbe[k].R * rapport);
                    byte canalG = (byte)(couleurSable[k].G * (1 - rapport) + couleurHerbe[k].G * rapport);
                    byte canalB = (byte)(couleurSable[k].B * (1 - rapport) + couleurHerbe[k].B * rapport);

                    nouvellesCouleurs[k] = new Color(canalR, canalG, canalB);
                }
            TextureTerrain.SetData(nouvellesCouleurs);
        }


        protected override void InitialiserSommets()
        {
            int NoSommet = 0;
            for (int j = 0; j < NbRangées; ++j)
                for (int i = 0; i < NbColonnes - 1; ++i)
                {
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]); //Moitié A
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);

                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]); //Moitié B
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j + 1], PtsTexture[i + 1, j + 1]);
                }
            
        }

        public Vector3 GetPointSpatial(int x, int y)
        {
            return new Vector3(PtsSommets[x, y].X, PtsSommets[x, y].Y, PtsSommets[x, y].Z);
        }

        public Vector3 GetNormale(int x, int y)
        {
            return new Vector3(NormalesSommets[x, y].X, NormalesSommets[x, y].Y, NormalesSommets[x, y].Z);
        }
        

        void InitialiserSommetsCôtéA(int A_0)
        {
            for (int index = 0; index < NbColonnes; ++index)
            {
                
            }
        }
        void InitialiserSommetsCôtéB(int A_0)
        {
            for (int index = 0; index < NbRangées; ++index)
            {
                
            }
        }

        void InitialiserSommetsCôtéC(int A_0)
        {
            for (int nbColonnes = NbColonnes; nbColonnes > 0; --nbColonnes)
            {
                
            }
        }

        void InitialiserSommetsCôtéD(int A_0)
        {
            for (int nbRangées = NbRangées; nbRangées > 0; --nbRangées)
            {
                
            }
        }
        

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            EffetDeBase.Texture = TextureTerrain;

            Draw(PrimitiveType.TriangleList, 0, NbTrianglesSurface, TextureTerrain);
            Draw(PrimitiveType.TriangleStrip, NbTrianglesSurface * 3, NbTrianglesBase, TextureBase);
        }

        private void Draw(PrimitiveType type, int pointDépart, int nbTriangles, Texture2D texture)
        {
            EffetDeBase.Texture = texture;
            foreach (EffectPass effectPass in EffetDeBase.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(type, Sommets, pointDépart, nbTriangles);
            }
        }
    }
}