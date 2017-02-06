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

        //Pour la position, dans ObjetDeBase et dans les PrimitiveDeBase-Anim�e, la position n'est jamais updat�e... Alors comment fait-on pour avoir la position apr�s d�placement? 
        //     - Solution possible: ----> Dans le Update, on ajoute le vecteur de d�placement � Position? Ce qui simulerait le d�placement et garderait en m�moire la position... 
        //                               (ATTENTION: Si on ajoutait 20 � la composante X mais que le terrain limite le d�placement (bordures/arbre/maison), faut pas ajouter 20 dans la position, sinon on cause un d�calage et on ajouterait un d�placement inexistant � la Position.)

        string NomTexture { get; set; }
        Texture2D Texture { get; set; }
        public BoundingSphere Sph�reDeCollision { get; protected set; }

        public bool EstEnCollision(object autreObjet)
        {
            if (!(autreObjet is ICollisionable))
            {
                return false;
            }
            return Sph�reDeCollision.Intersects(((ICollisionable)autreObjet).Sph�reDeCollision);
        }

        public Vivant(Game jeu, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
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
