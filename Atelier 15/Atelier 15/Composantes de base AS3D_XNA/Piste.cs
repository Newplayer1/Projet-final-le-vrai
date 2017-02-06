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
    public class Piste : PrimitiveDeBaseAnimée
    {
        const int NB_TRIANGLES = 2;
        
        BasicEffect EffetDeBase { get; set; }
        DepthStencilState DepthPiste { get; set; }
        VertexPositionColor[] Sommets { get; set; }

        DataPiste DonnéesPiste { get; set; }
        TerrainAvecBase Terrain { get; set; }

        List<Vector2> BordureIntérieure { get; set; }
        List<Vector2> BordureExtérieure { get; set; }

        public Piste(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
             : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            
        }

        
        public override void Initialize()
        {
            DonnéesPiste = Game.Services.GetService(typeof(DataPiste)) as DataPiste;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;

            BordureIntérieure = DonnéesPiste.GetBordureIntérieure();
            BordureExtérieure = DonnéesPiste.GetBordureExtérieure();

            NbSommets = BordureExtérieure.Count * NB_TRIANGLES;
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
            for (int i = 0; i < BordureIntérieure.Count; ++i)
            {
                Sommets[NoSommets++] = new VertexPositionColor(Terrain.GetPointSpatial((int)Math.Round(BordureExtérieure[i].X, 0), Terrain.NbRangées - (int)Math.Round(BordureExtérieure[i].Y, 0)), Color.Black);
                Sommets[NoSommets++] = new VertexPositionColor(Terrain.GetPointSpatial((int)Math.Round(BordureIntérieure[i].X, 0), Terrain.NbRangées - (int)Math.Round(BordureIntérieure[i].Y, 0)), Color.Black);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            DepthStencilState depthJeu = GraphicsDevice.DepthStencilState;//Comme cullmode du drapeau, on va y remettre l'ancien après avoir draw la piste, donc on garde l'ancien en mémoire
            GraphicsDevice.DepthStencilState = DepthPiste;

            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;

            foreach (EffectPass effectPass in EffetDeBase.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, Sommets, 0, Sommets.Length - 2);
            }
            GraphicsDevice.DepthStencilState = depthJeu;
        }
    }
}
