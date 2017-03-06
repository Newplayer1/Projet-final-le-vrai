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


namespace AtelierXNA.Classes_Pokemon_Skyrim
{
    public abstract class Vivant : ObjetDeBase, ICollisionable
    {

        //Pour la position, dans ObjetDeBase et dans les PrimitiveDeBase-Animée, la position n'est jamais updatée... Alors comment fait-on pour avoir la position après déplacement? 
        //     - Solution : ----> Dans le Update, on ajoute la quantité de déplacement (vecteur?) à Position. Si pour quelconque raison le déplacement du vivant est restreint par une collision, 
        //                        il ne faut pas ajouter la longueur totale d'un déplacement normal, mais juste celle du déplacement qui a été possible de faire. (sinon la valeur dans Position accumulera les décalages par rapport à la vraie position du vivant.)
        public bool EstEnVie { get; protected set; }
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

        public Vivant(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
        }
        
        
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
