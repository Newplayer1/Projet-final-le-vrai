using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA
{
    class Button1
    {
        const int DIMENSIONS_BTN = 80;
        const int COULEUR_MAX = 255;

        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        bool down;
        public bool isclicked;

        Color colour = new Color(COULEUR_MAX, COULEUR_MAX, COULEUR_MAX, COULEUR_MAX);

        public Vector2 size { get; set; }
        float tempsécoulédepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }

        public Button1(Texture2D nomTexture, GraphicsDevice graphics, float intervalleMAJ)
        {
            texture = nomTexture;
            //Image 960 par 418
            // 1600*900
            size = new Vector2(DIMENSIONS_BTN, DIMENSIONS_BTN);
            IntervalleMAJ  = intervalleMAJ;
            
        }
        public void Update(MouseState mouse, GameTime gametime)
        {
            float tempsécoulé = (float)gametime.ElapsedGameTime.TotalSeconds;
            tempsécoulédepuisMAJ += tempsécoulé;
            if(tempsécoulédepuisMAJ >= IntervalleMAJ)
            {
                GérerSouris(mouse);
                tempsécoulédepuisMAJ = 0;
            }
        }
        private void GérerSouris(MouseState mouse)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            Rectangle mouserectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouserectangle.Intersects(rectangle))
            {
                if (colour.A == COULEUR_MAX) down = false;
                if (colour.A == 0) down = true;
                if (down) colour.A += 3; else colour.A -= 3; // sujet porté à modification!!!
                if (mouse.LeftButton == ButtonState.Pressed) isclicked = true;
            }
            else if (colour.A < COULEUR_MAX)
            {
                colour.A += 3;
                isclicked = false;
            }
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