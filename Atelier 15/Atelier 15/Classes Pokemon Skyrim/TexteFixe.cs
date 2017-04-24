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
    public class TexteFixe : Microsoft.Xna.Framework.DrawableGameComponent, IDestructible
    {
        string NomPolice { get; set; }
        protected Texture2D TexturePolice { get; private set; }
        List<Rectangle> RectangleSourcesPolice { get; set; }//faudrait vraiment mettre toute la conversion en rectangles sources dans une unique classe sérieux
        string Message { get; set; }
        List<int> MessageListInt { get; set; }
        SpriteBatch GestionSprites { get; set; }
        Vector2 Position { get; set; }
        List<Vector2> PositionsList { get; set; }
        public bool ÀDétruire { get; set; }

        public TexteFixe(Game game, Vector2 position, string message)
            : base(game)
        {
            NomPolice = "Police848x32";
            Message = message;
            Position = position;
        }

        public override void Initialize()
        {
            RectangleSourcesPolice = new List<Rectangle>();
            
            base.Initialize();
        }
        protected override void LoadContent()
        {
            base.LoadContent();
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            TexturePolice = Game.Content.Load<Texture2D>("Textures/" + NomPolice);
            CréerListeTiles();
            CréerListeMessageInt(Message);
            CréerListePositions();
        }

        void CréerListeTiles()
        {
            for (int i = 0; i < (TexturePolice.Width / Cadre.TAILLE_TILE) - 1; i++)//Ligne A
            {
                RectangleSourcesPolice.Add(new Rectangle(Cadre.TAILLE_TILE * i, 0, Cadre.TAILLE_TILE, Cadre.TAILLE_TILE)); // A = 0; i = 0; j = 0
            }
            for (int i = 0; i < (TexturePolice.Width / Cadre.TAILLE_TILE) - 1; i++)//Ligne B
            {
                RectangleSourcesPolice.Add(new Rectangle(Cadre.TAILLE_TILE * i, Cadre.TAILLE_TILE, Cadre.TAILLE_TILE, Cadre.TAILLE_TILE)); // A = 0; i = 0; j = 0
            }
        }

        void CréerListeMessageInt(string message)
        {
            MessageListInt = new List<int>();
            MessageListInt = EncodeurFont.ConvertirStringEnListeInt(message);
        }


        void CréerListePositions()
        {
            PositionsList = new List<Vector2>();
            for (int i = 1; i <= MessageListInt.Count; i++)
            {
                PositionsList.Add(new Vector2(Position.X + i * Cadre.TAILLE_TILE, Position.Y));
            }
        }
        public void RemplacerMessage(string nouveauMessage)
        {
            Message = nouveauMessage;
        }
        public override void Update(GameTime gameTime)
        {
            if (!AfficheurTexte.MessageEnCours)
                MettreÀJourMessage();
            base.Update(gameTime);
        }
        void MettreÀJourMessage()
        {
            CréerListeMessageInt(Message);
            CréerListePositions();
        }
        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            for (int i = 0; i < MessageListInt.Count; i++)//on commence à écrire juste après le cadre, l'espace et le curseur et entre l'autre cadre. On écrit pour la longueur du message
            {
                GestionSprites.Draw(TexturePolice, PositionsList[i], RectangleSourcesPolice[MessageListInt[i]], Color.White);
            }
            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}
