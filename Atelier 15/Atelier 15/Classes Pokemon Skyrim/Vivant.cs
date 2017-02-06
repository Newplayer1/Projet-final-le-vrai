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
    public abstract class Vivant : ObjetDeBase, ICollisionable
    {

        //Pour la position, dans ObjetDeBase et dans les PrimitiveDeBase-Animée, la position n'est jamais updatée... Alors comment fait-on pour avoir la position après déplacement? 
        //     - Solution possible: ----> Dans le Update, on ajoute le vecteur de déplacement à Position? Ce qui simulerait le déplacement et garderait en mémoire la position... 
        //                               (ATTENTION: Si on ajoutait 20 à la composante X mais que le terrain limite le déplacement (bordures/arbre/maison), faut pas ajouter 20 dans la position, sinon on cause un décalage et on ajouterait un déplacement inexistant à la Position.)

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
