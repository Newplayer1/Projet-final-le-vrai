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
        Texture2D Background { get; set; }
        Rectangle RectangleAffichage { get; set; }
        public PageTitreState CurrentPageTitreState { get; private set; }
        Vector2 screenSize;

        public enum PageTitreState
        {
            MainMenu,
            Options,
            Playing,
            LoadGame
        }
        

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
            CurrentPageTitreState = PageTitreState.MainMenu;
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
            Background = GestionnaireDeTextures.Find("BackGround");
            RectangleAffichage = new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y);
            
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

            switch (CurrentPageTitreState)
            {
                case PageTitreState.MainMenu:
                    //MediaPlayer.Play(ChansonTitre);
                    if (btnOptions.isclicked) CurrentPageTitreState = PageTitreState.Options;
                    btnOptions.Update(mouse, gameTime);
                    if (btnNewGame.isclicked)
                        CurrentPageTitreState = PageTitreState.Playing;
                    btnNewGame.Update(mouse, gameTime);
                    if (btnLoadGame.isclicked) CurrentPageTitreState = PageTitreState.LoadGame;
                    btnLoadGame.Update(mouse, gameTime);
                    break;
                case PageTitreState.Playing:
                    break;
                case PageTitreState.LoadGame:
                    break;
                case PageTitreState.Options:
                    if (btnBack.isclicked) CurrentPageTitreState = PageTitreState.MainMenu;
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
        //private void NewStateReached(PageTitreState CurrentPageTitreState, bool NewGameReached)
        //{
        //    if (CurrentPageTitreState == PageTitreState.Playing)
        //        NewGameReached = true;
        //}
        //private void LoadStateReached(PageTitreState CurrentPageTitreState, bool LoadGameReached)
        //{
        //    if (CurrentPageTitreState == PageTitreState.LoadGame)
        //        LoadGameReached = true;
        //}
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
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            switch (CurrentPageTitreState)
            {
                case PageTitreState.MainMenu:
                    spriteBatch.Draw(Background, RectangleAffichage, Color.White);
                    btnOptions.Draw(spriteBatch);
                    btnNewGame.Draw(spriteBatch);
                    btnLoadGame.Draw(spriteBatch);
                    break;
                case PageTitreState.Playing:
                    break;
                case PageTitreState.Options:
                    spriteBatch.Draw(Background, RectangleAffichage, Color.White);
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
