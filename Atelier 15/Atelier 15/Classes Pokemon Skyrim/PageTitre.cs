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
        public PageTitreState CurrentPageTitreState { get; private set; }        
        public bool Yachoisi { get; set; }
        public bool IsFullScreen { get; set; }
        public int choix { get; set; }
        bool IsAudioPlaying { get; set; }
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
        Button1 btnfullscreen { get; set; }
        Button1 btnnotfullscreen { get; set; }

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
          : base(game)   {     }
            
        public override void Initialize()
        {
            CurrentPageTitreState = PageTitreState.MainMenu;
            graphics = Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionnaireDeChansons = new RessourcesManager<Song>(Game, "Songs");
            ArialFont = new RessourcesManager<SpriteFont>(Game, "Fonts");

            screenSize = new Vector2(800, 400);
            base.Initialize();

        }

        protected override void LoadContent()
        {
            // Aller chercher les ressources nécessaires
            spriteBatch = new SpriteBatch(GraphicsDevice);
            IsFullScreen = false;
            IsAudioPlaying = true;
            Yachoisi = false;
            Background = GestionnaireDeTextures.Find("BackGround");
            Controls = GestionnaireDeTextures.Find("controls");
            Chanson = GestionnaireDeChansons.Find("Pokemon");
            Arial = ArialFont.Find("Arial20");
            
            RectangleAffichage = new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y);

            //Ajustements Audio
            MediaPlayer.Play(Chanson);
            MediaPlayer.IsRepeating = true;

            //Ajustements d'écran
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.ApplyChanges();

            AjoutDeboutons();
        }
        private void AjoutDeboutons()
        {
            btnOptions = NewBouton("btn_Options");
            btnNewGame = NewBouton("Btn_New_Game");
            btnLoadGame = NewBouton("btn_Load_Game");
            btnSoundOn = NewBouton("SoundOn");
            btnSoundOff = NewBouton("SoundOff");
            btnBack = NewBouton("btn_back");
            controls = NewBouton("btn_Controles");
            squirtle = NewBouton("squirtle");
            bulbusaur = NewBouton("bulbasaur");
            charmander = NewBouton("Charmander");
            btnfullscreen = NewBouton("btnpleinécran");
            btnnotfullscreen = NewBouton("btnminiscreen");

            SetPosition(squirtle, 600, 200);
            SetPosition(bulbusaur, 400, 200);
            SetPosition(charmander, 200, 200);
            SetPosition(btnNewGame, 75, 315);
            SetPosition(btnLoadGame, 175, 320);
            SetPosition(btnOptions, 275, 320);
            SetPosition(btnSoundOn, 375, 325);
            SetPosition(btnSoundOff, 475, 325);
            SetPosition(btnBack, 0, 320);
            SetPosition(controls, 375, 320);
            SetPosition(btnfullscreen, 390, 230);
            SetPosition(btnnotfullscreen, 475, 230);
        }
        private void SetPosition(Button1 button, int X, int Y) 
        {
            button.ResetPosition(new Vector2(X, Y));
        }

        private Button1 NewBouton(string nomBouton) 
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
                    if (btnSoundOn.isclicked) { MediaPlayer.Resume(); IsAudioPlaying = true; }
                    if (btnSoundOff.isclicked) { MediaPlayer.Pause(); IsAudioPlaying = false; }
                    if (btnfullscreen.isclicked) IsFullScreen = true;
                    if (btnnotfullscreen.isclicked) IsFullScreen = false;

                    btnnotfullscreen.Update(mouse, gameTime);
                    btnBack.Update(mouse, gameTime);
                    btnSoundOn.Update(mouse, gameTime);
                    btnSoundOff.Update(mouse, gameTime);
                    btnfullscreen.Update(mouse, gameTime);
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
                    //On associe le numéro de pokedex du Pokemon que le user choisira
                    // afin de pouvoir l'ajouter au party lors du commencement du jeu
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
                    spriteBatch.DrawString(Arial, "Il y a aussi la possibililité de jouer avec un manette de Xbox 360!", new Vector2(70, 340), Color.Black, 0,
                                                           Vector2.Zero, 1f, SpriteEffects.None, 0);
                    btnBack.Draw(spriteBatch);
                    break;
                case PageTitreState.Options:
                    spriteBatch.Draw(Background, RectangleAffichage, Color.White);
                    btnSoundOn.Draw(spriteBatch);
                    btnSoundOff.Draw(spriteBatch);
                    btnfullscreen.Draw(spriteBatch);
                    btnnotfullscreen.Draw(spriteBatch);
                    btnBack.Draw(spriteBatch);

                    if (IsFullScreen) 
                    {
                     spriteBatch.DrawString(Arial, "Full Screen Activé", new Vector2(400, 210), Color.Black, 0,
                                                            Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.DrawString(Arial, "Full Screen Désactivé", new Vector2(400, 210), Color.Black, 0,
                                                            Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    if (IsAudioPlaying)
                    {
                        spriteBatch.DrawString(Arial, "Audio Activé", new Vector2(385, 303), Color.Black, 0,
                                                               Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.DrawString(Arial, "Audio Désactivé", new Vector2(385, 303), Color.Black, 0,
                                                            Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
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
