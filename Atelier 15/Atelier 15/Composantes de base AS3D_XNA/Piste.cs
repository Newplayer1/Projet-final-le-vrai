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
    public class Piste : PrimitiveDeBaseAnim�e
    {
        const int NB_TRIANGLES = 2;
        
        BasicEffect EffetDeBase { get; set; }
        DepthStencilState DepthPiste { get; set; }
        VertexPositionColor[] Sommets { get; set; }

        DataPiste Donn�esPiste { get; set; }
        TerrainAvecBase Terrain { get; set; }

        List<Vector2> BordureInt�rieure { get; set; }
        List<Vector2> BordureExt�rieure { get; set; }

        public Piste(Game jeu, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
             : base(jeu, homoth�tieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            
        }

        
        public override void Initialize()
        {
            Donn�esPiste = Game.Services.GetService(typeof(DataPiste)) as DataPiste;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;

            BordureInt�rieure = Donn�esPiste.GetBordureInt�rieure();
            BordureExt�rieure = Donn�esPiste.GetBordureExt�rieure();

            NbSommets = BordureExt�rieure.Count * NB_TRIANGLES;
            Sommets = new VertexPositionColor[NbSommets];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            DepthPiste = new DepthStencilState();

            EffetDeBase.VertexColorEnabled = true;
            DepthPiste.DepthBufferEnable = false;
        }


        protected override void InitialiserSommets()
        {
            int NoSommets = 0;
            for (int i = 0; i < BordureInt�rieure.Count; ++i)
            {
                Sommets[NoSommets++] = new VertexPositionColor(Terrain.GetPointSpatial((int)Math.Round(BordureExt�rieure[i].X, 0), Terrain.NbRang�es - (int)Math.Round(BordureExt�rieure[i].Y, 0)), Color.Black);
                Sommets[NoSommets++] = new VertexPositionColor(Terrain.GetPointSpatial((int)Math.Round(BordureInt�rieure[i].X, 0), Terrain.NbRang�es - (int)Math.Round(BordureInt�rieure[i].Y, 0)), Color.Black);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            DepthStencilState depthJeu = GraphicsDevice.DepthStencilState;//Comme cullmode du drapeau, on va y remettre l'ancien apr�s avoir draw la piste, donc on garde l'ancien en m�moire
            GraphicsDevice.DepthStencilState = DepthPiste;

            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;

            foreach (EffectPass effectPass in EffetDeBase.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, Sommets, 0, Sommets.Length - 2);
            }
            GraphicsDevice.DepthStencilState = depthJeu;
        }
    }
}
