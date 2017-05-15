using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class CubeTexturé : PrimitiveDeBaseAnimée, ICollisionable, IDestructible
    {
        const int NB_POINTS = 8;
        const int NB_SOMMETS = 16;
        const int NB_TRIANGLES = 6;
        
        string NomTextureCube { get; set; }
        Texture2D TextureCube { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        Vector3 Origine { get; set; }
        float DeltaX { get; set; }
        float DeltaY { get; set; }
        float DeltaZ { get; set; }

        Vector3[] Points { get; set; }
        VertexPositionTexture[] Sommets { get; set; }

        BasicEffect EffetDeBase { get; set; }
        public BoundingSphere SphèreDeCollision { get; protected set; }
        public bool ÀDétruire { get; set; }

        public bool EstEnCollision(object autreObjet)
        {
            if (!(autreObjet is ICollisionable))
            {
                return false;
            }
            return SphèreDeCollision.Intersects(((ICollisionable)autreObjet).SphèreDeCollision);
        }

        public CubeTexturé(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, string nomTextureCube, Vector3 dimension, float intervalleMAJ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NomTextureCube = nomTextureCube;
            DeltaX = dimension.X;
            DeltaY = dimension.Y;
            DeltaZ = dimension.Z;
            Origine = new Vector3(-DeltaX / 2, -DeltaY / 2, DeltaZ / 2);

            SphèreDeCollision = new BoundingSphere(positionInitiale, MathHelper.Min(MathHelper.Min(DeltaX, DeltaY), DeltaZ));//Parcequ'on s'attend à foncer dedans
        }

        public override void Initialize()
        {
            Sommets = new VertexPositionTexture[NB_SOMMETS];
            Points = new Vector3[NB_POINTS];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TextureCube = GestionnaireDeTextures.Find(NomTextureCube);

            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();
            base.LoadContent();
        }
        protected void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureCube;
            //GestionAlpha = BlendState.AlphaBlend; // Si jamais on voulait faire le bloc transparent et avoir un point d'interrogation au milieu (comme Mario Kart)
        }

        protected override void InitialiserSommets()
        {
            Points[0] = Origine;
            Points[1] = new Vector3(Origine.X, Origine.Y + DeltaY, Origine.Z);
            Points[2] = new Vector3(Origine.X + DeltaX, Origine.Y, Origine.Z);
            Points[3] = new Vector3(Origine.X + DeltaX, Origine.Y + DeltaY, Origine.Z);
            Points[4] = new Vector3(Origine.X + DeltaX, Origine.Y, Origine.Z - DeltaZ);
            Points[5] = new Vector3(Origine.X + DeltaX, Origine.Y + DeltaY, Origine.Z - DeltaZ);
            Points[6] = new Vector3(Origine.X, Origine.Y, Origine.Z - DeltaZ);
            Points[7] = new Vector3(Origine.X, Origine.Y + DeltaY, Origine.Z - DeltaZ);

            //Tout est identique au cube de l'atelier 12. Cependant, il faut repérer les points/arrêtes du cube pour faire correspondre à la texture
            Sommets[0] = new VertexPositionTexture(Points[0], new Vector2(0, 1)); //Arrête 1
            Sommets[1] = new VertexPositionTexture(Points[1], new Vector2(0, 0));
            Sommets[2] = new VertexPositionTexture(Points[2], new Vector2(1/3f, 1));//Arrête 2
            Sommets[3] = new VertexPositionTexture(Points[3], new Vector2(1/3f, 0));

            Sommets[4] = new VertexPositionTexture(Points[4], new Vector2(2/3f, 1));//Arrête 3
            Sommets[5] = new VertexPositionTexture(Points[5], new Vector2(2/3f, 0));
            Sommets[6] = new VertexPositionTexture(Points[6], new Vector2(1, 1));//Arrête 4
            Sommets[7] = new VertexPositionTexture(Points[7], new Vector2(1, 0));

            Sommets[8] = new VertexPositionTexture(Points[2], new Vector2(0, 1));//Arrête 5
            Sommets[9] = new VertexPositionTexture(Points[4], new Vector2(0, 0));
            Sommets[10] = new VertexPositionTexture(Points[0], new Vector2(1/3f, 1));//Arrête 6
            Sommets[11] = new VertexPositionTexture(Points[6], new Vector2(1/3f, 0));

            Sommets[12] = new VertexPositionTexture(Points[1], new Vector2(2/3f, 1));//Arrête 7
            Sommets[13] = new VertexPositionTexture(Points[7], new Vector2(2/3f, 0));
            Sommets[14] = new VertexPositionTexture(Points[3], new Vector2(1, 1));//Arrête 8
            Sommets[15] = new VertexPositionTexture(Points[5], new Vector2(1, 0));
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, 0, NB_TRIANGLES);
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, NB_POINTS, NB_TRIANGLES);
            }
        }
    }
}