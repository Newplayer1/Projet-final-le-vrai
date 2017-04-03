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
        Texture2D Controls { get; set; }
        Rectangle RectangleAffichage { get; set; }
        Song Chanson { get; set; }
        public PageTitreState CurrentPageTitreState { get; private set; }
        Vector2 screenSize;

        public enum PageTitreState
        {
            MainMenu,
            Options,
            Playing,
            Controls,
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
        Button1 controls;

        public override void Initialize()
        {
            CurrentPageTitreState = PageTitreState.MainMenu;
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(Game, "Textures");
            GestionnaireDeChansons = new RessourcesManager<Song>(Game, "Songs");
            

            screenSize = new Vector2(800, 400);
            base.Initialize();

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Background = GestionnaireDeTextures.Find("BackGround");
            Controls = GestionnaireDeTextures.Find("controls");
            Chanson = GestionnaireDeChansons.Find("hey");
            RectangleAffichage = new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y);

            //Audio Ajustments
            MediaPlayer.Play(Chanson);

            //Screen Ajustments
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
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
            controls = new Button1(Game, GestionnaireDeTextures.Find("btn_Controles"), graphics.GraphicsDevice, INTERVALLEMAJ);

            btnNewGame.ResetPosition(new Vector2(75, 315));
            btnOptions.ResetPosition(new Vector2(275, 320));
            btnLoadGame.ResetPosition(new Vector2(175, 320));
            btnSoundOn.ResetPosition(new Vector2(375, 325));
            btnSoundOff.ResetPosition(new Vector2(475, 325));
            btnBack.ResetPosition(new Vector2(0, 320));
            controls.ResetPosition(new Vector2(375, 320));

        }


        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            switch (CurrentPageTitreState)
            {
                case PageTitreState.MainMenu:
                    //MediaPlayer.Play(ChansonTitre);
                    if (btnOptions.isclicked) CurrentPageTitreState = PageTitreState.Options;
                    if (btnNewGame.isclicked) CurrentPageTitreState = PageTitreState.Playing;
                    if (btnLoadGame.isclicked) CurrentPageTitreState = PageTitreState.LoadGame;
                    if (controls.isclicked) CurrentPageTitreState = PageTitreState.Controls;

                    btnLoadGame.Update(mouse, gameTime);
                    btnNewGame.Update(mouse, gameTime);
                    btnOptions.Update(mouse, gameTime);
                    controls.Update(mouse, gameTime);
                    break;
                case PageTitreState.Options:
                    if (btnBack.isclicked) CurrentPageTitreState = PageTitreState.MainMenu;
                    if (btnSoundOn.isclicked) MediaPlayer.Resume();
                    if (btnSoundOff.isclicked) MediaPlayer.Pause();

                    btnBack.Update(mouse, gameTime);
                    btnSoundOn.Update(mouse, gameTime);
                    btnSoundOff.Update(mouse, gameTime);
                    break;
                case PageTitreState.Controls:
                    if (btnBack.isclicked) CurrentPageTitreState = PageTitreState.MainMenu;
                    btnBack.Update(mouse, gameTime);
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
        //private void GestionMusique()
        //{
        //    if (MediaPlayer.State == MediaState.Paused)
        //    {
        //        MediaPlayer.Resume();
        //    }
        //    else
        //    {
        //        if (MediaPlayer.State == MediaState.Playing)
        //        {
        //            MediaPlayer.Pause();
        //        }
        //        //else
        //        //{
        //        //    MediaPlayer.Play(ChansonTitre);
        //        //}
        //    }
        //}
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
                    controls.Draw(spriteBatch);
                    break;
                case PageTitreState.Controls:
                    spriteBatch.Draw(Controls, RectangleAffichage, Color.White);
                    btnBack.Draw(spriteBatch);
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
