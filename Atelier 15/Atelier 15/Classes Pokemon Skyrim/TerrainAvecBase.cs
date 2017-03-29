//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace AtelierXNA
//{
//    public class TerrainAvecBase : PrimitiveDeBaseAnimée
//    {
//        const int NB_TRIANGLES_PAR_TUILE = 2;
//        const int NB_SOMMETS_PAR_TRIANGLE = 3;
//        const float MAX_COULEUR = 255f;

//        Vector3 Étendue { get; }
//        string NomCarteTerrain { get; }
//        string[] NomTextureTerrain { get; }
//        int NbNiveauTexture { get; }
//        string NomTextureBase { get; }
//        float IntervalleMAJ { get; set; }

//        BasicEffect EffetDeBase { get; set; }
//        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

//        Texture2D CarteTerrain { get; set; }
//        Texture2D TextureTerrain { get; set; }
//        Texture2D TextureSable { get; set; }
//        Texture2D TextureHerbe { get; set; }
//        Texture2D TextureBase { get; set; }

//        Vector3 Origine { get; set; }

//        Vector2 DeltaCarte { get; set; }
//        float HauteurMinimale { get; set; }
//        float HauteurMaximale { get; set; }
//        float VariationHauteur { get; set; }

//        public int NbColonnes { get; private set; }
//        public int NbRangées { get; private set; }

//        int NbTrianglesSurface { get; set; }
//        int NbTrianglesBase { get; set; }

//        float ÉcartTexture { get; set; }
//        Vector3[,] PtsSommets { get; set; }
//        Color[] TabCouleur { get; set; }
//        Vector3[,] NormalesSommets { get; set; }
//        VertexPositionTexture[] Sommets { get; set; }

//        public TerrainAvecBase(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
//            Vector3 étendue, string nomCarteTerrain, string[] nomTextureTerrain,/* int nbNiveauxTexture,*/
//            string nomTextureBase, float intervalleMAJ)
//            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
//        {
//            Étendue = étendue;
//            NomCarteTerrain = nomCarteTerrain;
//            NomTextureTerrain = nomTextureTerrain;
//            NomTextureBase = nomTextureBase;
//        }

//        public override void Initialize()
//        {
//            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
//            HauteurMinimale = MAX_COULEUR;
//            HauteurMaximale = 0;

//            InitialiserDonnéesCarte();
//            InitialiserDonnéesTexture();

//            Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2);

//            AllouerTableaux();
//            CréerTableauPoints();

//            //IniTableauTextures();
//            CalculerNormales();
//            //CréerTextureDuTerrain();

//            base.Initialize();
//        }

//        void InitialiserDonnéesCarte()
//        {
//            CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);

//            NbColonnes = CarteTerrain.Width - 1;
//            NbRangées = CarteTerrain.Height - 1;

//            TabCouleur = new Color[CarteTerrain.Width * CarteTerrain.Height];

//            CarteTerrain.GetData(TabCouleur);

//            DeltaCarte = new Vector2(Étendue.X / NbColonnes, Étendue.Z / NbRangées);
//            NbTrianglesSurface = NbColonnes * NbRangées * NB_TRIANGLES_PAR_TUILE;
//            NbTrianglesBase = 2 * (NbColonnes + NbRangées) * NB_TRIANGLES_PAR_TUILE;

//            NbTriangles = NbTrianglesSurface + NbTrianglesBase;
//            NbSommets = NbTriangles * NB_SOMMETS_PAR_TRIANGLE;
//        }

//        void InitialiserDonnéesTexture()
//        {
//            //TextureSable = GestionnaireDeTextures.Find(NomTextureTerrain[0]);
//            //TextureHerbe = GestionnaireDeTextures.Find(NomTextureTerrain[1]);

//            TextureBase = GestionnaireDeTextures.Find(NomTextureBase);
//        }

//        void AllouerTableaux()
//        {
//            PtsSommets = new Vector3[NbColonnes + 1, NbRangées + 1];
//            Sommets = new VertexPositionTexture[NbSommets];
//        }

//        protected override void LoadContent()
//        {
//            base.LoadContent();
//            EffetDeBase = new BasicEffect(GraphicsDevice);
//            InitialiserParamètresEffetDeBase();
//        }

//        void InitialiserParamètresEffetDeBase()
//        {
//            EffetDeBase.TextureEnabled = true;
//            EffetDeBase.Texture = TextureTerrain;
//        }

//        void CréerTableauPoints()
//        {
//            for (int i = 0; i <= NbColonnes; ++i)
//                for (int j = 0; j <= NbRangées; ++j)
//                {
//                    float OrigineX = Origine.X;
//                    float OrigineZ = Origine.Z;

//                    OrigineX += i * DeltaCarte.X;
//                    OrigineZ -= j * DeltaCarte.Y;

//                    PtsSommets[i, j] = new Vector3(OrigineX, CalculerHauteur(i, NbRangées - j), OrigineZ);

//                    if (PtsSommets[i, j].Y > HauteurMaximale)
//                        HauteurMaximale = PtsSommets[i, j].Y;

//                    if (PtsSommets[i, j].Y < HauteurMinimale)
//                        HauteurMinimale = PtsSommets[i, j].Y;
//                }
//            VariationHauteur = HauteurMaximale - HauteurMinimale;
//        }

//        float CalculerHauteur(int i, int j)
//        {
//            return TabCouleur[i + (NbRangées + 1) * j].B / MAX_COULEUR * Étendue.Y;
//        }
//        void CalculerNormales()
//        {
//            NormalesSommets = new Vector3[NbColonnes + 1, NbRangées + 1];

//            for (int i = 0; i < NbColonnes; ++i)
//                for (int j = 0; j < NbRangées; ++j)
//                {
//                    Vector3 normale = Vector3.Normalize(Vector3.Cross(PtsSommets[i + 1, j] - PtsSommets[i, j], PtsSommets[i, j + 1] - PtsSommets[i, j]));

//                    NormalesSommets[i, j] = normale;
//                    NormalesSommets[i + 1, j] = normale;
//                    NormalesSommets[i, j + 1] = normale;
//                }
//        }

//        //void CréerTextureDuTerrain()
//        //{
//        //    TextureTerrain = new Texture2D(CarteTerrain.GraphicsDevice, CarteTerrain.Width, CarteTerrain.Height);

//        //    Color[] nouvellesCouleurs = new Color[CarteTerrain.Width * CarteTerrain.Height];

//        //    Color[] couleurSable = new Color[CarteTerrain.Width * CarteTerrain.Height];
//        //    Color[] couleurHerbe = new Color[CarteTerrain.Width * CarteTerrain.Height];

//        //    TextureSable.GetData(couleurSable);
//        //    TextureHerbe.GetData(couleurHerbe);

//        //    for (int i = 0; i <= NbColonnes; ++i)
//        //        for (int j = 0; j <= NbRangées; ++j)
//        //        {
//        //            float rapport = ((PtsSommets[i, j].Y - HauteurMinimale) / (VariationHauteur));
//        //            int k = i * CarteTerrain.Width + j;

//        //            byte canalR = (byte)(couleurSable[k].R * (1 - rapport) + couleurHerbe[k].R * rapport);
//        //            byte canalG = (byte)(couleurSable[k].G * (1 - rapport) + couleurHerbe[k].G * rapport);
//        //            byte canalB = (byte)(couleurSable[k].B * (1 - rapport) + couleurHerbe[k].B * rapport);

//        //            nouvellesCouleurs[k] = new Color(canalR, canalG, canalB);
//        //        }
//        //    TextureTerrain.SetData(nouvellesCouleurs);
//        //}


//        protected override void InitialiserSommets()
//        {
//            int NoSommet = 0;
//            for (int j = 0; j < NbRangées; ++j)
//                for (int i = 0; i < NbColonnes - 1; ++i)
//                {
//                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j], DéterminerTexture(PtsSommets[i, j].Y)); //Moitié A
//                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j + 1], DéterminerTexture(PtsSommets[i, j + 1].Y));
//                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j], DéterminerTexture(PtsSommets[i + 1, j].Y));

//                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j], DéterminerTexture(PtsSommets[i + 1, j].Y)); //Moitié B
//                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i, j + 1], DéterminerTexture(PtsSommets[i, j + 1].Y));
//                    Sommets[NoSommet++] = new VertexPositionTexture(PtsSommets[i + 1, j + 1], DéterminerTexture(PtsSommets[i + 1, j + 1].Y));
//                }

//        }

//        Vector2 DéterminerTexture(float hauteur)//C'est ici qu'on va codifier le "20%" et le choix de la texture appropriée
//        { //Diviser la hauteur par l'étendue pour donner le pourcntage de la hauteur?

//            //"il faudra associer au sommet de notre tuile, les coordonnées de texture appropriées en fonction de la hauteur moyenne des sommets de la tuile." - document Atelier 15
//            float rapportHauteurÉtendue = hauteur / Étendue.Y;//?

//            return new Vector2(10f, rapportHauteurÉtendue);
//        }
//        public Vector3 GetPointSpatial(int x, int y)
//        {
//            return new Vector3(PtsSommets[x, y].X, PtsSommets[x, y].Y, PtsSommets[x, y].Z);
//        }

//        public Vector3 GetNormale(int x, int y)
//        {
//            return new Vector3(NormalesSommets[x, y].X, NormalesSommets[x, y].Y, NormalesSommets[x, y].Z);

//            //return new Vector3(NormalesSommets[x - 256, y].X, NormalesSommets[x, y].Y, NormalesSommets[x, y].Z);

//        }


//        void InitialiserSommetsCôtéA(int A_0)
//        {
//            for (int index = 0; index < NbColonnes; ++index)
//            {

//            }
//        }
//        void InitialiserSommetsCôtéB(int A_0)
//        {
//            for (int index = 0; index < NbRangées; ++index)
//            {

//            }
//        }

//        void InitialiserSommetsCôtéC(int A_0)
//        {
//            for (int nbColonnes = NbColonnes; nbColonnes > 0; --nbColonnes)
//            {

//            }
//        }

//        void InitialiserSommetsCôtéD(int A_0)
//        {
//            for (int nbRangées = NbRangées; nbRangées > 0; --nbRangées)
//            {

//            }
//        }


//        public override void Draw(GameTime gameTime)
//        {
//            EffetDeBase.World = GetMonde();
//            EffetDeBase.View = CaméraJeu.Vue;
//            EffetDeBase.Projection = CaméraJeu.Projection;
//            EffetDeBase.Texture = TextureTerrain;
//            // Game.Window.Title = CaméraJeu.Position.ToString() + " ----- " + Origine.ToString();
//            Draw(PrimitiveType.TriangleList, 0, NbTrianglesSurface, TextureTerrain);
//            Draw(PrimitiveType.TriangleStrip, NbTrianglesSurface * 3, NbTrianglesBase, TextureBase);
//        }

//        private void Draw(PrimitiveType type, int pointDépart, int nbTriangles, Texture2D texture)
//        {
//            EffetDeBase.Texture = texture;
//            foreach (EffectPass effectPass in EffetDeBase.CurrentTechnique.Passes)
//            {
//                effectPass.Apply();
//                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(type, Sommets, pointDépart, nbTriangles);
//            }
//        }
//    }
//}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class TerrainAvecBase : PrimitiveDeBaseAnimée
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;
        const float MAX_COULEUR = 255f;

        Vector3 Étendue { get; set; }
        string NomCarteTerrain { get; set; }
        string NomTextureTerrain { get; set; }
        int NbNiveauTexture { get; set; }

        BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D CarteTerrain { get; set; }
        Texture2D TextureTerrain { get; set; }
        Vector3 Origine { get; set; }

        Vector2 DeltaCarte { get; set; }

        public int NbColonnes { get; private set; }
        public int NbRangées { get; private set; }

        float ÉcartTexture { get; set; }

        Color[] TabCouleur { get; set; } //DataHeightMap (DataCarteTerrain)

        Vector3[,] TabPtsSommet { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        Vector2[,] PtsTexture { get; set; }


        //List<float> ListValueCouleur { get; set; }//?


        // à compléter en ajoutant les propriétés qui vous seront nécessaires pour l'implémentation du composant


        public TerrainAvecBase(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                       Vector3 étendue, string nomCarteTerrain, string nomTextureTerrain, int nbNiveauxTexture, float intervalleMAJ)
           : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Étendue = étendue;
            NomCarteTerrain = nomCarteTerrain;
            NomTextureTerrain = nomTextureTerrain;
            NbNiveauTexture = nbNiveauxTexture;


        }

        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            //CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);
            //ListValueCouleur = new List<float>();
            InitialiserDonnéesCarte();
            InitialiserDonnéesTexture();
            Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2); // pour centrer la primitive au point (0,0,0)
                                                                     // il a utilisé la règle de la main droite, donc 
                                                                     // l'axe des z pointe vers nous.
            AllouerTableaux();
            CréerTableauPoints();
            base.Initialize();
        }

        //
        // à partir de la texture servant de carte de hauteur (HeightMap), on initialise les données
        // relatives à la structure de la carte
        //
        void InitialiserDonnéesCarte()
        {
            CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);

            //NbColonnes = CarteTerrain.Width;
            //NbRangées = CarteTerrain.Height;
            //TabCouleur = new Color[NbColonnes * NbRangées];
            NbColonnes = CarteTerrain.Width - 1;
            NbRangées = CarteTerrain.Height - 1;
            TabCouleur = new Color[CarteTerrain.Width * CarteTerrain.Height];

            CarteTerrain.GetData<Color>(TabCouleur);

            //DeltaCarte = new Vector2(Étendue.X / (NbColonnes - 1), Étendue.Z / (NbRangées - 1));
            //NbTriangles = (NbColonnes - 1) * (NbRangées - 1) * NB_TRIANGLES_PAR_TUILE;
            //NbSommets = NbTriangles * NB_SOMMETS_PAR_TRIANGLE;
            DeltaCarte = new Vector2(Étendue.X / NbColonnes, Étendue.Z / NbRangées);
            NbTriangles = NbColonnes * NbRangées * NB_TRIANGLES_PAR_TUILE;
            NbSommets = NbTriangles * NB_SOMMETS_PAR_TRIANGLE;

        }
        public Vector3 GetPointSpatial(int x, int y)
        {
            return new Vector3(TabPtsSommet[x, y].X, TabPtsSommet[x, y].Y, TabPtsSommet[x, y].Z);
        }

        //
        // à partir de la texture contenant les textures carte de hauteur (HeightMap), on initialise les données
        // relatives à l'application des textures de la carte
        //
        void InitialiserDonnéesTexture()
        {
            //foreach (Color c in TabCouleur) On a déjà un tableau des couleurs, pourquoi en faire une liste? On a juste à utiliser le tab quand vient le temps de faire le point d'hauteur
            //{
            //    float canalB = c.B;
            //    canalB = canalB * 25 / 255;

            //    ListValueCouleur.Add(canalB);
            //}
            TextureTerrain = GestionnaireDeTextures.Find(NomTextureTerrain);
            ÉcartTexture = 1f / NbNiveauTexture;   //attention division par entiers
        }

        //
        // Allocation des deux tableaux
        //    1) celui contenant les points de sommet (les points uniques), 
        //    2) celui contenant les sommets servant à dessiner les triangles
        void AllouerTableaux()
        {
            //TabPtsSommet = new Vector3[NbColonnes, NbRangées];
            //Sommets = new VertexPositionTexture[NbSommets];
            PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
            TabPtsSommet = new Vector3[NbColonnes + 1, NbRangées + 1];
            Sommets = new VertexPositionTexture[NbSommets];
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

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();

            //CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);
            //TextureTerrain = GestionnaireDeTextures.Find(NomTextureTerrain);
        }

        void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureTerrain;
        }

        //
        // Création du tableau des points de sommets (on crée les points)
        // Ce processus implique la transformation des points 2D de la texture en coordonnées 3D du terrain
        //
        private void CréerTableauPoints()
        {
            for (int i = 0; i <= NbColonnes; ++i)
                for (int j = 0; j <= NbRangées; ++j)
                {
                    float OrigineX = Origine.X;
                    float OrigineZ = Origine.Z;

                    OrigineX += i * DeltaCarte.X;
                    OrigineZ -= j * DeltaCarte.Y;

                    TabPtsSommet[i, j] = new Vector3(OrigineX, CalculerHauteur(i, NbRangées - j), OrigineZ);
                }
        }
        private float CalculerHauteur(int i, int j)//ici qu'un utiliserait la liste, mais c'est plus court juste le tab messemble
        {
            return TabCouleur[i + (NbRangées + 1) * j].B / MAX_COULEUR * Étendue.Y;
        }

        //
        // Création des sommets.
        // N'oubliez pas qu'il s'agit d'un TriangleList... Goddamit 
        //
        protected override void InitialiserSommets() // copy / paste du labo 13 //gab/ J'pense pas parce que la on doit faire du TriangleList, tandis que dans le 13 on fonctionnait en Strip...
        {
            int NoSommet = 0;
            for (int j = 0; j < NbRangées; ++j)
                for (int i = 0; i < NbColonnes; ++i)
                {
                    Sommets[NoSommet++] = new VertexPositionTexture(TabPtsSommet[i, j], DéterminerTexture(TabPtsSommet[i, j].Y)); //Moitié A
                    Sommets[NoSommet++] = new VertexPositionTexture(TabPtsSommet[i, j + 1], DéterminerTexture(TabPtsSommet[i, j + 1].Y));
                    Sommets[NoSommet++] = new VertexPositionTexture(TabPtsSommet[i + 1, j], DéterminerTexture(TabPtsSommet[i + 1, j].Y));

                    Sommets[NoSommet++] = new VertexPositionTexture(TabPtsSommet[i + 1, j], DéterminerTexture(TabPtsSommet[i + 1, j].Y)); //Moitié B
                    Sommets[NoSommet++] = new VertexPositionTexture(TabPtsSommet[i, j + 1], DéterminerTexture(TabPtsSommet[i, j + 1].Y));
                    Sommets[NoSommet++] = new VertexPositionTexture(TabPtsSommet[i + 1, j + 1], DéterminerTexture(TabPtsSommet[i + 1, j + 1].Y));
                }
        }
        Vector2 DéterminerTexture(float hauteur)//C'est ici qu'on va codifier le "20%" et le choix de la texture appropriée
        { //Diviser la hauteur par l'étendue pour donner le pourcntage de la hauteur?

            //"il faudra associer au sommet de notre tuile, les coordonnées de texture appropriées en fonction de la hauteur moyenne des sommets de la tuile." - document Atelier 15
            float rapportHauteurÉtendue = hauteur / Étendue.Y;//?

            return new Vector2(10f, rapportHauteurÉtendue);
        }

        //
        // Deviner ce que fait cette méthode...
        //
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

