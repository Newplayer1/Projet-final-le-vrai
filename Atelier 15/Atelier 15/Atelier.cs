using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AtelierXNA
{
    enum ÉtatsDépart { PAGE_TITRE, JEU3D }
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_CALCUL_FPS = 1f;
        public const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const int LEVEL_DÉPART = 5;

        Button1 bulbusaur { get; set; }
        Button1 charmander { get; set; }
        Button1 squirtle { get; set; }
        Jeu LeJeu { get; set; }
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        InputManager GestionInput { get; set; }
        ÉtatsDépart ÉtatDépart { get; set; }
        Caméra CaméraJeu { get; set; }
        public PageTitre PageTitre { get; private set; }
        public List<string> Sauvegarde { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        AccessBaseDeDonnée DataBase { get; set; }


        public Atelier()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            PériphériqueGraphique.PreferredBackBufferWidth = 800;
            PériphériqueGraphique.PreferredBackBufferHeight = 480;
            PériphériqueGraphique.IsFullScreen = false;
        }

        protected override void Initialize()
        {

            GestionInput = new InputManager(this);
           
            Components.Add(GestionInput);
            //Components.Add(new ArrièrePlan(this, "CielWindowsXp"));

            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            DataBase = new AccessBaseDeDonnée();

            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));
            //Services.AddService(typeof(RessourcesManager<SoundEffect>), new RessourcesManager<SoundEffect>(this, "Sounds"));
            Services.AddService(typeof(RessourcesManager<Song>), new RessourcesManager<Song>(this, "Songs"));
            Services.AddService(typeof(RessourcesManager<Texture2D>),GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);

            Services.AddService(typeof(AccessBaseDeDonnée), DataBase);

            Services.AddService(typeof(InputManager), GestionInput);

            Services.AddService(typeof(GraphicsDeviceManager), PériphériqueGraphique);
            Services.AddService(typeof(SpriteBatch), new SpriteBatch(GraphicsDevice));
            //Services.AddService(typeof(AccessBaseDeDonnée), new AccessBaseDeDonnée());

            PageTitre = new PageTitre(this);
            Components.Add(PageTitre);

            ÉtatDépart = ÉtatsDépart.PAGE_TITRE;
            base.Initialize();
        }
        protected override void Update(GameTime gameTime)
        {
            GérerTransition();
            NettoyerListeComponents();
            GérerClavier();
            //UpdateBoutons(gameTime);

            base.Update(gameTime);
            //Window.Title = this.
        }
        private void GérerTransition()
        {
            switch (ÉtatDépart)
            {
                case ÉtatsDépart.PAGE_TITRE:
                    
                    if(PageTitre.CurrentPageTitreState == PageTitre.PageTitreState.LoadGame)
                    {
                        InitializePlaying();
                        Components.Add(new Jeu(this, Sauvegarde));
                        
                        Components.Add(new AfficheurFPS(this,"Arial20",Color.Green, INTERVALLE_CALCUL_FPS));
                    }
                    if (PageTitre.CurrentPageTitreState == PageTitre.PageTitreState.PokemonDébut)
                    {
                        if(PageTitre.Yachoisi)
                        {
                            
                            LeJeu = new Jeu(this, PageTitre.choix);
                            InitializePlaying();
                            Components.Add(LeJeu);
                            Components.Add(new AfficheurFPS(this, "Arial20", Color.CornflowerBlue, INTERVALLE_CALCUL_FPS));
                        }
                        
                        
                    }
                    break;
                case ÉtatsDépart.JEU3D:
                    Components.Remove(PageTitre);
                    break;
            }
        }
        private void InitializePlaying()
        {
            ÉtatDépart = ÉtatsDépart.JEU3D;
            PériphériqueGraphique.IsFullScreen = true;
            PériphériqueGraphique.ApplyChanges();
            CaméraJeu = new CaméraSubjective(this, new Vector3(96, 16, -96), Vector3.Zero, Vector3.Up, INTERVALLE_MAJ_STANDARD);

            Components.Add(CaméraJeu);
            Services.AddService((typeof(Caméra)), CaméraJeu);

            Components.Add(new Afficheur3D(this));
        }
        void NettoyerListeComponents()
        {
            for (int i = Components.Count - 1; i >= 0; --i)
            {
                if (Components[i] is IDestructible && ((IDestructible)Components[i]).ÀDétruire)
                {
                    Components.RemoveAt(i);
                }
            }
        }

        private void GérerClavier()
        {
            if (GestionInput.EstEnfoncée(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
            {
                Exit();
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            //int noDrawOrder = 0;
            //for (int i = 0; i < Components.Count; ++i)
            //{
            //    if (Components[i] is DrawableGameComponent)
            //    {
            //        ((DrawableGameComponent)Components[i]).DrawOrder = noDrawOrder++;
            //    }
            //}
            //GraphicsDevice.Clear(Color.Black);
            Window.Title = Components.Count.ToString();

            base.Draw(gameTime);
        }
    }
}

