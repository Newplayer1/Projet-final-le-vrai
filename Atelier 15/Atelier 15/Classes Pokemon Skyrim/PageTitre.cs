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
    public class PageTitre : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const float INTERVALLEMAJ = 1 / 60f;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Song> GestionnaireDeChansons { get; set; }

        MediaState Son = MediaState.Playing;
        Vector2 screenSize;

        enum GameState
        {
            MainMenu,
            Options,
            Playing,
        }
        GameState CurrentGameState = GameState.MainMenu;

        public PageTitre(Game game)
          : base(game)
      {
            graphics = Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager;

        }


        Button1 btnOptions;
        Button1 btnNewGame;
        Button1 btnLoadGame;
        Button1 btnSoundOn;
        Button1 btnSoundOff;

        public override void Initialize()
        {
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(Game, "Textures");
            //Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            GestionnaireDeChansons = new RessourcesManager<Song>(Game, "Songs");
            //Services.AddService(typeof(RessourcesManager<Song>), GestionnaireDeChansons);

            screenSize = new Vector2(800, 400);
            base.Initialize();

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Screen Ajustments
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            AjoutDeboutons();
        }
        private void AjoutDeboutons()
        {
            btnOptions = new Button1(GestionnaireDeTextures.Find("btn_Options"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnNewGame = new Button1(GestionnaireDeTextures.Find("Btn_New_Game"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnLoadGame = new Button1(GestionnaireDeTextures.Find("btn_Load_Game"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnSoundOn = new Button1(GestionnaireDeTextures.Find("SoundOn"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnSoundOff = new Button1(GestionnaireDeTextures.Find("SoundOff"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnNewGame.ResetPosition(new Vector2(75, 320));
            btnOptions.ResetPosition(new Vector2(275, 320));
            btnLoadGame.ResetPosition(new Vector2(175, 320));
            btnSoundOn.ResetPosition(new Vector2(425, 280));
            btnSoundOff.ResetPosition(new Vector2(425, 280));

        }


        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    if (btnOptions.isclicked) CurrentGameState = GameState.Options;
                    btnOptions.Update(mouse, gameTime);
                    if (btnNewGame.isclicked) CurrentGameState = GameState.Playing;
                    btnNewGame.Update(mouse, gameTime);
                    if (btnLoadGame.isclicked) CurrentGameState = GameState.Playing;
                    btnLoadGame.Update(mouse, gameTime);
                    break;
                case GameState.Options:
                    GérerSon(mouse, gameTime);

                    break;
            }
            base.Update(gameTime);
        }
        private void GérerSon(MouseState mouse, GameTime gameTime)
        {
            btnSoundOn.Update(mouse, gameTime);

            if (btnSoundOn.isclicked)
            {
                Son = MediaState.Paused;
                btnSoundOff.Update(mouse, gameTime);
            }

            if (btnSoundOff.isclicked)
            {
                Son = MediaState.Playing;
                btnSoundOn.Update(mouse, gameTime);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(GestionnaireDeTextures.Find("BackGround"), new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
                    btnOptions.Draw(spriteBatch);
                    btnNewGame.Draw(spriteBatch);
                    btnLoadGame.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    break;
                case GameState.Options:
                    spriteBatch.Draw(GestionnaireDeTextures.Find("BackGround"), new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
                    btnSoundOn.Draw(spriteBatch);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }











        //protected float WeaknessOrStrenght(int TypeAttack, int FoeOrFriendType1, int FoeOrFriendType2)
        //{
        //    float[,] WeaknessOrStrenghtTableau = new float[18, 18];
        //    //Remplir Tableau avec DataBase

        //    float Coefficient = WeaknessOrStrenghtTableau[TypeAttack, FoeOrFriendType1];

        //    if (FoeOrFriendType2 != 0)
        //    {
        //        Coefficient *= WeaknessOrStrenghtTableau[TypeAttack, FoeOrFriendType2];
        //    }

        //    Coefficient /= 100;
        //    return Coefficient;
        //}
    }
}
