using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class CubeTexturéFlottant : CubeTexturé
    {
        float IntervalleMAJOscillation { get; }
        float TempsÉcouléDepuisMAJOscillation { get; set; }
        float Angle { get; set; }

        public CubeTexturéFlottant(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, string nomTextureCube, Vector3 dimension, float intervalleMAJ)
            : base(game, homothétieInitiale, rotationInitiale, positionInitiale, nomTextureCube, dimension, intervalleMAJ)
        {
            IntervalleMAJOscillation = intervalleMAJ;
        }

        public override void Initialize()
        {
            base.Initialize();

            Angle = 0;
            TempsÉcouléDepuisMAJOscillation = 0;
            Lacet = true;
        }

        public override void Update(GameTime gameTime)
        {
            TempsÉcouléDepuisMAJOscillation += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsÉcouléDepuisMAJOscillation >= IntervalleMAJOscillation)
            {
                Angle += IntervalleMAJOscillation;
                Position = PositionInitiale + (Vector3.UnitY / 2) * (float)Math.Sin(Angle); //L'amplitude c'est 1/2 en Y, car c'est le vecteur unitaire de Y / 2, avec une position initiale. A * sin(angle) => Oscillateur harmonique

                SphèreDeCollision = new BoundingSphere(Position, SphèreDeCollision.Radius);//on fait une sphère de collision avec le même rayon que l'ancienne
                MondeÀRecalculer = true; //Parce qu'on a changé les données et qu'on veut qu'y change ce qui est affiché pour avoir un mouvement

                TempsÉcouléDepuisMAJOscillation = 0;
            }
            base.Update(gameTime);
        }
    }
}