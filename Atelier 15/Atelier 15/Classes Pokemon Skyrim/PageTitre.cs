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
        public bool NewGameReached { get; set; }
        public bool LoadGameReached { get; set; }

        Vector2 screenSize;

        enum GameState
        {
            MainMenu,
            Options,
            Playing,
            LoadGame,
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
        Button1 btnBack;
        //Button1 tempOn;
        //Button1 tempOff;

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
            btnOptions = new Button1(Game, GestionnaireDeTextures.Find("btn_Options"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnNewGame = new Button1(Game,GestionnaireDeTextures.Find("Btn_New_Game"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnLoadGame = new Button1(Game,GestionnaireDeTextures.Find("btn_Load_Game"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnSoundOn = new Button1(Game,GestionnaireDeTextures.Find("SoundOn"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnSoundOff = new Button1(Game,GestionnaireDeTextures.Find("SoundOff"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnBack = new Button1(Game,GestionnaireDeTextures.Find("btn_back"), graphics.GraphicsDevice, INTERVALLEMAJ);
            btnNewGame.ResetPosition(new Vector2(75, 320));
            btnOptions.ResetPosition(new Vector2(275, 320));
            btnLoadGame.ResetPosition(new Vector2(175, 320));
            btnSoundOn.ResetPosition(new Vector2(425, 280));
            btnSoundOff.ResetPosition(new Vector2(425, 280));
            btnBack.ResetPosition(new Vector2(0, 320));

        }


        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    //MediaPlayer.Play(ChansonTitre);
                    if (btnOptions.isclicked) CurrentGameState = GameState.Options;
                    btnOptions.Update(mouse, gameTime);
                    if (btnNewGame.isclicked) CurrentGameState = GameState.Playing;
                    btnNewGame.Update(mouse, gameTime);
                    if (btnLoadGame.isclicked) CurrentGameState = GameState.LoadGame;
                    btnLoadGame.Update(mouse, gameTime);
                    break;
                case GameState.Playing:
                    NewStateReached(CurrentGameState, NewGameReached);
                    break;
                case GameState.LoadGame:
                    LoadStateReached(CurrentGameState, LoadGameReached);
                    break;
                case GameState.Options:
                    if (btnBack.isclicked) CurrentGameState = GameState.MainMenu;
                    if (btnSoundOn.isclicked || btnSoundOff.isclicked) GestionMusique();


                    //SoundOn.ResetPosition(new Vector2(1000, 1000));
                    //SoundOff.ResetPosition(new Vector2(380, 320));

                    //if(SoundOff.isclicked)
                    //{
                    //        GestionMusique();
                    //    SoundOn.ResetPosition(new Vector2(380, 320));
                    //    SoundOff.ResetPosition(new Vector2(1000, 1000));
                    //}
                    btnBack.Update(mouse, gameTime);
                    btnSoundOn.Update(mouse, gameTime);
                    btnSoundOff.Update(mouse, gameTime);
                    break;
            }
            base.Update(gameTime);
        }
        private bool NewStateReached(GameState CurrentGameState, bool NewGameReached)
        {
            if (CurrentGameState == GameState.Playing) NewGameReached = true;
            return NewGameReached;
        }
        private bool LoadStateReached(GameState CurrentGameState, bool LoadGameReached)
        {
            if (CurrentGameState == GameState.LoadGame) LoadGameReached = true;
            return LoadGameReached;
        }
        private void GestionMusique()
        {
            if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
            else
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Pause();
                }
                //else
                //{
                //    MediaPlayer.Play(ChansonTitre);
                //}
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
                    btnSoundOff.Draw(spriteBatch);
                    btnBack.Draw(spriteBatch);
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
