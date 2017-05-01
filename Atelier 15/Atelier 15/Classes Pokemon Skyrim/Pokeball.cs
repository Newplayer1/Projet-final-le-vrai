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
    public class Pokeball : SphèreTexturée
    {
        const float VITESSE_BALLE = 1.5f;
        const float FORCE_G = 9.8f;
        const float FORCE_G2 = 520;

        float AngleX { get; set; }
        float AngleY { get; set; }
        float AngleZ { get; set; }

        float VitesseX { get; set; }
        float VitesseY { get; set; }
        float VitesseZ { get; set; }

        double temps { get; set; }

        Vector3 DirectionLancer { get; set; }

        Trainer Joueur { get; set; }

        public Pokeball(Game jeu, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float rayon, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, échelleInitiale, rotationInitiale, positionInitiale, rayon, charpente, nomTexture, intervalleMAJ)
        { }

        public override void Initialize()
        {
            base.Initialize();
            Joueur = Game.Services.GetService(typeof(Trainer)) as Trainer;
            DirectionLancer = (CaméraJeu as CaméraSubjective).Direction;
            AngleY = MathHelper.ToRadians(30);
            temps = 0;
        }

        public override void Update(GameTime gameTime)
        {
            TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= 1f / 65f)
            {
                GérerProjectile();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        void GérerProjectile()
        {
            //double théta = MathHelper.ToRadians(45f);

            //float résultatProduitScalaireX = DirectionLancer.X * Vector3.UnitX.X + DirectionLancer.Y * Vector3.UnitX.Y + DirectionLancer.Z * Vector3.UnitX.Z;
            //float résultatProduitScalaireZ = DirectionLancer.X * Vector3.UnitZ.X + DirectionLancer.Y * Vector3.UnitZ.Y + DirectionLancer.Z * Vector3.UnitZ.Z;

            float normeDirectionLancer = (float)Math.Sqrt(Math.Pow(DirectionLancer.X, 2) + Math.Pow(DirectionLancer.Y, 2) + Math.Pow(DirectionLancer.Z, 2));
            float normeUnit = (float)Math.Sqrt(Math.Pow(Vector3.UnitX.X, 2) + Math.Pow(Vector3.UnitX.Y, 2) + Math.Pow(Vector3.UnitX.Z, 2));

            //float AngleXRad = MathHelper.ToRadians(résultatProduitScalaireX / normeDirectionLancer * normeUnit);
            //float AngleZRad = MathHelper.ToRadians(résultatProduitScalaireZ / normeDirectionLancer * normeUnit);

            //AngleX = (float)Math.Acos(AngleXRad);
            //AngleZ = (float)Math.Acos(AngleZRad);

            VitesseX = (float)Math.Cos(AngleX) * VITESSE_BALLE;
            //VitesseY = VITESSE_BALLE * (float)Math.Cos(AngleY);

            //float positionPokéballX = Position.X + VitesseX * TempsÉcoulé;

            //float positionPokéballY = Position.Y + VitesseY * TempsÉcoulé - (FORCE_G / 2) * (float)Math.Pow(TempsÉcoulé, 2);

            double vitesseInitiale = 1.5;
            double théta = MathHelper.ToRadians(20);
            //VitesseZ = (float)Math.Cos(45) * VITESSE_BALLE;
            //float positionPokéballZ = Position.Z + VitesseZ * TempsÉcoulé;
            double vitesseX = vitesseInitiale * Math.Cos(théta);
            double vitesseY = vitesseInitiale * Math.Sin(théta);
            float positionPokéballX;
            float positionPokéballY;

            temps -= TempsÉcoulé;
            positionPokéballY = (float)(vitesseY * temps + (FORCE_G * Math.Pow(temps, 2)));
            positionPokéballX = (float)((vitesseX) * temps);

            Position -= new Vector3(positionPokéballX, positionPokéballY, 0);

            CalculerMatriceMonde();
            //AjusterCaméra();
        }

        void AjusterCaméra()
        {
            float positionX = Position.X + 20;
            float positionY = 25;
            float positionZ = Position.Z + 20;

            Vector3 positionCaméra = new Vector3(positionX, positionY, positionZ);

            CaméraJeu.Déplacer(positionCaméra, Position, Vector3.Up);
        }


        public override void Draw(GameTime gameTime)
        {
            //Game.Window.Title = Game.Components.Count.ToString()
            Position.ToString();
            base.Draw(gameTime);
        }
    }
}

