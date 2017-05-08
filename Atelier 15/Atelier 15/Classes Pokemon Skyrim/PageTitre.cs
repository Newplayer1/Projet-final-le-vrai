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
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;
        RessourcesManager<SpriteFont> ArialFont { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Song> GestionnaireDeChansons { get; set; }
        Texture2D Background { get; set; }
        Texture2D Controls { get; set; }
        SpriteFont Arial { get; set; }
        Rectangle RectangleAffichage { get; set; }
        Song Chanson { get; set; }
        public PageTitreState CurrentPageTitreState { get; private set; }
        Vector2 screenSize;
        
        Button1 btnOptions { get; set; }
        Button1 btnNewGame { get; set; }
        Button1 btnLoadGame { get; set; }
        Button1 btnSoundOn { get; set; }
        Button1 btnSoundOff { get; set; }
        Button1 btnBack { get; set; }
        Button1 controls { get; set; }
        Button1 charmander { get; set; }
        Button1 bulbusaur { get; set; }
        Button1 squirtle { get; set; }

        public bool Yachoisi { get; set; }
        public int choix { get; set; }


        public enum PageTitreState
        {
            MainMenu,
            Options,
            Playing,
            Controls,
            LoadGame,
            PokemonDébut
        }
        

        public PageTitre(Game game)
          : base(game)
        {
            graphics = Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager;
        }

        public override void Initialize()
        {
            CurrentPageTitreState = PageTitreState.MainMenu;
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(Game, "Textures");
            GestionnaireDeChansons = new RessourcesManager<Song>(Game, "Songs");
            ArialFont = new RessourcesManager<SpriteFont>(Game, "Fonts");

            screenSize = new Vector2(800, 400);
            base.Initialize();

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Background = GestionnaireDeTextures.Find("BackGround");
            Controls = GestionnaireDeTextures.Find("controls");
            Chanson = GestionnaireDeChansons.Find("Pokemon");
            Arial = ArialFont.Find("Arial20"); 
            RectangleAffichage = new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y);

            //Audio Ajustments
            MediaPlayer.Play(Chanson);
            MediaPlayer.IsRepeating = true;

            //Screen Ajustments
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.ApplyChanges();

            AjoutDeboutons();
        }
        private void AjoutDeboutons()
        {
            btnOptions = ChaqueBouton("btn_Options");
            btnNewGame = ChaqueBouton("Btn_New_Game");
            btnLoadGame =  ChaqueBouton("btn_Load_Game");
            btnSoundOn = ChaqueBouton("SoundOn");
            btnSoundOff =  ChaqueBouton("SoundOff");
            btnBack = ChaqueBouton("btn_back");
            controls = ChaqueBouton("btn_Controles");
            squirtle = ChaqueBouton("squirtle");
            bulbusaur = ChaqueBouton("bulbasaur");
            charmander = ChaqueBouton("Charmander");

            squirtle.ResetPosition(new Vector2(600, 200));
            bulbusaur.ResetPosition(new Vector2(400, 200));
            charmander.ResetPosition(new Vector2(200, 200));
            btnNewGame.ResetPosition(new Vector2(75, 315));
            btnOptions.ResetPosition(new Vector2(275, 320));
            btnLoadGame.ResetPosition(new Vector2(175, 320));
            btnSoundOn.ResetPosition(new Vector2(375, 325));
            btnSoundOff.ResetPosition(new Vector2(475, 325));
            btnBack.ResetPosition(new Vector2(0, 320));
            controls.ResetPosition(new Vector2(375, 320));

        }
        private Button1 ChaqueBouton(string nomBouton) 
        {
            return new Button1(Game, GestionnaireDeTextures.Find(nomBouton), graphics.GraphicsDevice, INTERVALLEMAJ);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            switch (CurrentPageTitreState)
            {
                case PageTitreState.MainMenu:
                    if (btnOptions.isclicked) CurrentPageTitreState = PageTitreState.Options;
                    if (btnNewGame.isclicked) CurrentPageTitreState = PageTitreState.PokemonDébut;
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
                case PageTitreState.PokemonDébut:
                    if (btnBack.isclicked) CurrentPageTitreState = PageTitreState.MainMenu;
                    btnBack.Update(mouse, gameTime);
                    bulbusaur.Update(mouse, gameTime);
                    squirtle.Update(mouse, gameTime);
                    charmander.Update(mouse, gameTime);
                    Yachoisi = false;
                    if (squirtle.isclicked)
                    {
                        Yachoisi = true;
                        choix = 7;
                    }
                    if (charmander.isclicked)
                    {
                        Yachoisi = true;
                        choix = 4;
                    }
                    if (bulbusaur.isclicked)
                    {
                        Yachoisi = true;
                        choix = 1;
                    }

                    break;
            }
            base.Update(gameTime);
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
                case PageTitreState.PokemonDébut:
                    spriteBatch.DrawString(Arial, "Choose your Starter Pokemon : ", new Vector2(260,100), Color.WhiteSmoke, 0,
                                   Vector2.Zero, 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Arial, "Please don't.", new Vector2(400, 280), Color.WhiteSmoke, 0,
                                   Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                    charmander.Draw(spriteBatch);
                    squirtle.Draw(spriteBatch);
                    bulbusaur.Draw(spriteBatch);
                    btnBack.Draw(spriteBatch);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
