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
using AtelierXNA;

namespace AtelierXNA
{
    enum ÉtatsJeu { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float ÉCHELLE_OBJET = 0.004f;
        const int POKEDEX_MAX = 35;
        Vector2 POSITION_BOX_STANDARD = new Vector2(2, 300);
        Player LeJoueur { get; set; }
        InputManager GestionInput { get; set; }
        AccessBaseDeDonnée Database { get; set; }
        Combat LeCombat { get; set; }
        CaméraSubjective CaméraJeu { get; set; }
        TerrainAvecBase TerrainDeJeu { get; set; }
        ObjetDeBase PokemonRandom1 { get; set; }
        Pokemon PokemonRandom1String { get; set; }
        Pokemon_B test { get; set; }
        ÉtatsJeu ÉtatJeu { get; set; }
        Random générateurAléatoire { get; set; }
        List<ObjetDeBase> PokemonSurLeTerrain { get; set; }
        ObjetDeBase PokemonEnCollision { get; set; }
        int indexPokemonEnCollission;

        public Jeu(Game game, int choix)
           : base(game)
        {
            /*DataBase = new AccessBaseDeDonnée();*/
            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI / 4, 0);
            Vector3 positionCPU = new Vector3(96, 18f, -30);
            LeJoueur = new Player(Game, "El_guapo", ÉCHELLE_OBJET, rotationObjet, positionCPU, INTERVALLE_MAJ_STANDARD, 1f);
            //LeJoueur.PokemonsDansLesMains = new List<int>();

            //LeJoueur.PokemonsDansLesMains.Add(choix);
            LeJoueur.AddPokemon(choix); //tout les trainers ont une liste de pokémon, la liste va dans trainer (et faut jamais mettre une liste publique)
            PokemonSurLeTerrain = new List<ObjetDeBase>();
            UploadSauvegarde();
        }
        public Jeu(Game game, List<string> Sauvegarde)
            : base(game)
        {
            /*DataBase = new AccessBaseDeDonnée();*/
            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI / 4, 0);
            LeJoueur = new Player(Game, "El_guapo", ÉCHELLE_OBJET, rotationObjet, new Vector3(float.Parse(Database.LoadSauvegarde()[0]), float.Parse(Database.LoadSauvegarde()[1]), float.Parse(Database.LoadSauvegarde()[2])), INTERVALLE_MAJ_STANDARD, 1f);
            PokemonSurLeTerrain = new List<ObjetDeBase>();
        }
        public override void Initialize()
        {
            générateurAléatoire = new Random();

            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            //Vector3 positionObjet = new Vector3(100, 20, -100);

            ÉtatJeu = ÉtatsJeu.JEU3D;
            //LoadSauvegarde();
            Game.Components.Add(new ArrièrePlan(Game, "BackGroundNuage"));
            Game.Components.Add(new Afficheur3D(Game));
            TerrainDeJeu = new TerrainAvecBase(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 17, 256), "TerrainPokemon", "DétailsTerrain", 5, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(TerrainDeJeu);
            Game.Services.AddService(typeof(TerrainAvecBase), TerrainDeJeu);
            Game.Components.Add(LeJoueur);
            Game.Services.AddService(typeof(Player), LeJoueur);
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as CaméraSubjective;
        }
        public override void Update(GameTime gameTime)
        {
            GérerClavier();
            //GérerTransition();
            GérerÉtat();
            if (Game.Components.Count < 20)
            {
                if (Game.Components.Count(p => p is Afficheur3D) == 2)
                    Game.Components.Add(new Afficheur3D(Game));
                AjoutPokemonsRandom();
            }
            base.Update(gameTime);
        }

        private void GérerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Enter))
            {
                UploadSauvegarde();
            }

        }

        public void UploadSauvegarde()
        {
            List<string> Sauvegarde = new List<string>();
            Sauvegarde.Add(LeJoueur.Position.X.ToString());
            Sauvegarde.Add(LeJoueur.Position.Y.ToString());
            Sauvegarde.Add(LeJoueur.Position.Z.ToString());

            Database.Sauvegarder(Sauvegarde);
        }

        private void GérerTransition()
        {
            switch (ÉtatJeu)
            {
                case ÉtatsJeu.JEU3D:
                    break;
                    //case États.COMBAT:
                    //    GérerTransitionCombat();
                    //    break;
                    //case États.MAISON:
                    //    GérerTransitionMaison();
                    //    break;
                    //case États.GYM:
                    //    GérerTransitionGym();
                    //    break;
                    //case États.FIN:
                    //    GérerTransitionFin();
                    //    break;
                    //default:
                    //    break;
            }
        }
        private void AjoutPokemonsRandom()
        {
            PokemonRandom1String = new Pokemon(Game, générateurAléatoire.Next(1, POKEDEX_MAX));
            PokemonRandom1 = new ObjetDeBase(Game, PokemonRandom1String, TrouverAléatoire(), ÉCHELLE_OBJET, Vector3.Zero, TrouverPositionRandom());
            //PokemonRandom1 = new ObjetDeBase(Game, 1, 1, TrouverAléatoire(), ÉCHELLE_OBJET, new Vector3(0, 0, 0), TrouverPositionRandom());
            //Game.Services.AddService(typeof(Pokemon), PokemonRandom1);
            Game.Components.Add(PokemonRandom1);
            PokemonSurLeTerrain.Add(PokemonRandom1);
        }
        
        private string TrouverAléatoire()
        {
            int unNombre = générateurAléatoire.Next(1, POKEDEX_MAX);
            string local = unNombre.ToString();
            if (local.Count() == 1)
            {
                local = "0" + local;
            }
            string nomMod = local + '/' + local;
            return nomMod;

            //float échelleObjTest = 0.01f;

            //if (local1 == 14.ToString())
            //{
            //    échelleObjTest = ÉCHELLE_OBJET * 10;
            //}
        }


        private Vector3 TrouverPositionRandom()
        {
            float positionRandomX = générateurAléatoire.Next(-TerrainDeJeu.NbColonnes / 2, TerrainDeJeu.NbColonnes / 2);
            float positionRandomY = générateurAléatoire.Next(-TerrainDeJeu.NbRangées / 2, TerrainDeJeu.NbRangées / 2);

            //positionRandomX = MathHelper.Max(MathHelper.Min(positionRandomX, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);
            //positionRandomY = MathHelper.Max(MathHelper.Min(positionRandomY, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);

            Vector2 position = new Vector2(positionRandomX + TerrainDeJeu.NbColonnes / 2, positionRandomY + TerrainDeJeu.NbRangées / 2);

            float hauteur = (TerrainDeJeu.GetPointSpatial((int)Math.Round(position.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(position.Y, 0)) + Vector3.Zero).Y;

            return new Vector3(positionRandomX, hauteur, positionRandomY);

        }
        private void GérerÉtat()
        {
            switch (ÉtatJeu)
            {
                case ÉtatsJeu.JEU3D:
                    GérerCollision();
                    break;
                case ÉtatsJeu.COMBAT:
                    if (!(Game.Components.Contains(Game.Components.Where(c => c is Combat) as Combat)) && !Flags.Combat)
                    {
                        LeJoueur.Visible = false;
                        ObjetDeBase PokemonLancer = new ObjetDeBase(Game,"09/09",ÉCHELLE_OBJET,new Vector3(0,(float)(16*Math.PI/10),0),new Vector3(LeJoueur.Position.X + 1,LeJoueur.Position.Y, LeJoueur.Position.Z +1 ));
                        Game.Components.Add(new Afficheur3D(Game));
                        Game.Components.Add(PokemonLancer);
                        PokemonEnCollision.Position = new Vector3(PokemonLancer.Position.X -1 , PokemonLancer.Position.Y, PokemonLancer.Position.Z- 1);
                        PokemonSurLeTerrain[indexPokemonEnCollission - 1] = PokemonEnCollision;// CAUSE UNE EXCEPTION (quand l'index = 0, ça crash)
                        Flags.Combat = true;
                        LeCombat = new Combat(Game, POSITION_BOX_STANDARD, LeJoueur, new Pokemon(Game, 5), INTERVALLE_MAJ_STANDARD);
                        Game.Components.Add(LeCombat);
                        //CaméraJeu.Cible = new Vector3(LeJoueur.Position.X + 3, LeJoueur.Position.Y + 3, LeJoueur.Position.Z);
                        //CaméraJeu.CréerPointDeVue(CaméraJeu.Position, CaméraJeu.Cible, CaméraJeu.OrientationVerticale);
                    }
                    break;
                    //case États.GYM:
                    //    GérerVitesseDéplacement();
                    //    GérerComputer();
                    //    GérerCombat();
                    //    break;
                    //default: //États.FIN:
                    //    Fin();
                    //    SauvegardeAuto();
                    //    break;
                    //
            }
        }

        private void GérerCollision()
        {
            foreach (ObjetDeBase p in Game.Components.Where(r => r is ObjetDeBase))
            {
                if (!(p is Player))
                {
                    if (LeJoueur.EstEnCollision(p))
                    {
                        indexPokemonEnCollission = PokemonSurLeTerrain.IndexOf(p);
                        PokemonEnCollision = PokemonSurLeTerrain[indexPokemonEnCollission];
                        ÉtatJeu = ÉtatsJeu.COMBAT;
                    }
                }
            }

        }
    }
}
