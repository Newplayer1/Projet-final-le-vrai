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
    public abstract class Vivant : ObjetDeBase
    {

        //Pour la position, dans ObjetDeBase et dans les PrimitiveDeBase-Anim�e, la position n'est jamais updat�e... Alors comment fait-on pour avoir la position apr�s d�placement? 
        //     - Solution : ----> Dans le Update, on ajoute la quantit� de d�placement (vecteur?) � Position. Si pour quelconque raison le d�placement du vivant est restreint par une collision, 
        //                        il ne faut pas ajouter la longueur totale d'un d�placement normal, mais juste celle du d�placement qui a �t� possible de faire. (sinon la valeur dans Position accumulera les d�calages par rapport � la vraie position du vivant.)
        public bool EstEnVie { get; protected set; }
        string NomTexture { get; set; }
        Texture2D Texture { get; set; }
        

        public Vivant(Game jeu, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
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
