using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA
{
    public class Button1 : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int DIMENSIONS_BTN = 80;
        const int COULEUR_MAX = 255;

        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        InputManager GestionsTouches { get; set; }
        MouseState AncienÉtatSouris { get; set; }
        MouseState NouvelÉtatSouris { get; set; }
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        bool down;
        public bool isclicked { get; set; }

        Color colour = new Color(COULEUR_MAX, COULEUR_MAX, COULEUR_MAX, COULEUR_MAX);

        float IntervalleMAJ { get; set; }
        public Vector2 size { get; set; }
        float tempsÉcouléDepuisMAJ { get; set; }
        public Button1(Game game, Texture2D nomTexture, GraphicsDevice graphics, float intervalleMAJ)
        : base(game)
        {
            texture = nomTexture;
            //Image 960 par 418
            // 1600*900
            size = new Vector2(DIMENSIONS_BTN, DIMENSIONS_BTN);
            GestionsTouches = Game.Services.GetService(typeof(InputManager)) as InputManager;
            IntervalleMAJ = intervalleMAJ;

        }
        public void Update(MouseState mouse, GameTime gametime)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            Rectangle mouserectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);
            GérerSouris(mouse, mouserectangle, rectangle);

            float tempsÉcoulé = (float)gametime.ElapsedGameTime.TotalSeconds;
            tempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (tempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {

                if (mouserectangle.Intersects(rectangle))
                {

                    if (colour.A == COULEUR_MAX) down = false;
                    if (colour.A == 0) down = true;
                    if (down == true) colour.A += 3; else colour.A -= 3; // sujet porté à modification!!!
                }

            else if (colour.A < COULEUR_MAX)
                    {
                        colour.A += 3;
                        isclicked = false;
                    }


                ActualiserÉtatSouris();

            }
        }

        private void GérerSouris(MouseState mouse, Rectangle mouserectangle, Rectangle rectangle)
        {

            if (mouserectangle.Intersects(rectangle))
            {
                if (EstNouveauClicGauche())
                {
                    isclicked = true;
                    if (EstAncienClicGauche())
                    {
                        isclicked = false;
                    }
                }
            }

        }
        void ActualiserÉtatSouris()
        {
            AncienÉtatSouris = NouvelÉtatSouris;
            NouvelÉtatSouris = Mouse.GetState();
            tempsÉcouléDepuisMAJ = 0;
        }
        private bool EstNouveauClicGauche()
        {
            return NouvelÉtatSouris.LeftButton == ButtonState.Pressed &&
                   AncienÉtatSouris.LeftButton == ButtonState.Released;
        }
        public bool EstAncienClicGauche()
        {
            return NouvelÉtatSouris.LeftButton == ButtonState.Pressed &&
            AncienÉtatSouris.LeftButton == ButtonState.Pressed;
        }
        public void ResetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }
        public void Draw(SpriteBatch GestionSprites)
        {
            GestionSprites.Draw(texture, rectangle, colour);
        }
    }
}