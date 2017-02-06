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
    public abstract class Vivant : PrimitiveDeBaseAnimée, ICollisionable
    {
        
        string NomTexture { get; set; }
        Texture2D Texture { get; set; }
        public BoundingSphere SphèreDeCollision { get; protected set; }

        public bool EstEnCollision(object autreObjet)
        {
            if (!(autreObjet is ICollisionable))
            {
                return false;
            }
            return SphèreDeCollision.Intersects(((ICollisionable)autreObjet).SphèreDeCollision);
        }

        public Vivant(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, string nomTexture, Vector3 dimension, float intervalleMAJ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NomTexture = nomTexture;
        }
        public abstract bool EstEnVie();
        
        public override void Initialize()
        {
            base.Initialize();
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
