using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    enum ÉtatsDépart { PAGE_TITRE, JEU3D, FIN }
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        InputManager GestionInput { get; set; }
        ÉtatsDépart ÉtatDépart { get; set; }
        Caméra CaméraJeu { get; set; }
        public PageTitre PageTitre { get; private set; }

        RessourcesManager<Model> GestionnaireDeModèles { get; set; }


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

            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));
            //Services.AddService(typeof(RessourcesManager<SoundEffect>), new RessourcesManager<SoundEffect>(this, "Sounds"));
            Services.AddService(typeof(RessourcesManager<Song>), new RessourcesManager<Song>(this, "Songs"));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);

            Services.AddService(typeof(InputManager), GestionInput);

            Services.AddService(typeof(GraphicsDeviceManager), PériphériqueGraphique);
            Services.AddService(typeof(SpriteBatch), new SpriteBatch(GraphicsDevice));
            //Services.AddService(typeof(AccessBaseDeDonnée), new AccessBaseDeDonnée());
            PageTitre = new PageTitre(this);
            Components.Add(PageTitre);
            ÉtatDépart = ÉtatsDépart.PAGE_TITRE;

            //LoadSauvegarde();
            base.Initialize();
        }
        protected override void Update(GameTime gameTime)
        {
            GérerTransition();
            GérerÉtat();

            NettoyerListeComponents();
            GérerClavier();
            base.Update(gameTime);
        }
        private void GérerTransition()
        {
            switch (ÉtatDépart)
            {
                case ÉtatsDépart.PAGE_TITRE:
                    
                    if(PageTitre.CurrentPageTitreState == PageTitre.PageTitreState.LoadGame || PageTitre.CurrentPageTitreState == PageTitre.PageTitreState.Playing)
                    {
                        ÉtatDépart = ÉtatsDépart.JEU3D;
                        //PériphériqueGraphique.IsFullScreen = true;
                        PériphériqueGraphique.IsFullScreen = false;
                        PériphériqueGraphique.ApplyChanges();

                        CaméraJeu = new CaméraSubjective(this, new Vector3(96, 16, -96), Vector3.Zero /*new Vector3(80, 16, -96)*/, Vector3.Up, INTERVALLE_MAJ_STANDARD);

                        Components.Add(CaméraJeu);
                        Services.AddService((typeof (Caméra)),CaméraJeu);
                        Components.Add(new Afficheur3D(this));
                        Components.Add(new Jeu(this));
                        Components.Add(new AfficheurFPS(this,"Arial20",Color.Red, INTERVALLE_CALCUL_FPS));
                        
                        //LoadSauvegarde(); dans l'Initialize
                    }
                        
                    break;
                case ÉtatsDépart.JEU3D:
                    GérerTransitionJEU();
                    //InitialiserParcours();
                    //InitialiserCaméra();
                    break;
            }
        }
        private void GérerÉtat()
        {
            switch (ÉtatDépart)
            {
                case ÉtatsDépart.JEU3D:
                    //VérifierCollision();
                    break;
                default:
                    break;
            }
        }
        private void GérerTransitionJEU()
        {
            Components.Remove(PageTitre);

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
            if (GestionInput.EstEnfoncée(Keys.Escape))
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}

