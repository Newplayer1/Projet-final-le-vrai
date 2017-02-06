using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class Terrain : PrimitiveDeBaseAnimée
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

        int NbColonnes { get; set; }
        int NbRangées { get; set; }

        float ÉcartTexture { get; set; }

        Color[] TabCouleur { get; set; } //DataHeightMap (DataCarteTerrain)

        Vector3[,] TabPtsSommet { get; set; }
        VertexPositionTexture[] Sommets { get; set; }


        //List<float> ListValueCouleur { get; set; }//?


        // à compléter en ajoutant les propriétés qui vous seront nécessaires pour l'implémentation du composant


        public Terrain(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
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
            TabPtsSommet = new Vector3[NbColonnes + 1, NbRangées + 1];
            Sommets = new VertexPositionTexture[NbSommets];
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

            return new Vector2(10f, rapportHauteurÉtendue /** ÉcartTexture*/);//ça donne juste du bleu / eau
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
