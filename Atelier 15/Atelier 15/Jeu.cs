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
        const int POKEMON_SUR_LE_TERRAIN = 25;
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float ÉCHELLE_OBJET = 0.004f;
        const int POKEDEX_MAX = 152;
        const int RAYON_POKÉBALL = 1;
        Vector2 PositionBoxStandard { get; set; }
        ObjetDeBase PokemonJoueur { get; set; }
        Player LeJoueur { get; set; }
        Pokeball Projectile { get; set; }
        InputManager GestionInput { get; set; }
        AccessBaseDeDonnée Database { get; set; }
        Combat LeCombat { get; set; }
        CaméraSubjective CaméraJeu { get; set; }
        TerrainAvecBase TerrainDeJeu { get; set; }
        ObjetDeBase PokemonRandom1 { get; set; }
        Pokemon PokemonRandom1Infos { get; set; }
        Pokemon_B test { get; set; }
        ÉtatsJeu ÉtatJeu { get; set; }
        TexteFixe ÉtatJeuTexte { get; set; }
        Random générateurAléatoire { get; set; }
        List<ObjetDeBase> PokemonSurLeTerrain { get; set; }
        ObjetDeBase PokemonEnCollision { get; set; }
        int indexPokemonEnCollision;
        
        AfficheurTexte DebugAfficheurTexteA { get; set; }
        AfficheurTexte DebugAfficheurTexteB { get; set; }
        AfficheurTexte DebugAfficheurTexteC { get; set; }
        AfficheurTexte DebugAfficheurTexteD { get; set; }
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
            //LeJoueur.AddPokemon(int.Parse(Database.LoadSauvegarde()[3]), int.Parse(Database.LoadSauvegarde()[4]));
            PokemonSurLeTerrain = new List<ObjetDeBase>();
        }
        public override void Initialize()
        {
            générateurAléatoire = new Random();
            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            //Vector3 positionObjet = new Vector3(100, 20, -100);
            DebugAfficheurTexteA = new AfficheurTexte(Game, new Vector2(2, 2), 32, 6, "This is the first box. The pokemon used Tackle!", INTERVALLE_MAJ_STANDARD);

            DebugAfficheurTexteB = new AfficheurTexte(Game, new Vector2(2, 2), 32, 6, "It's not very effective.", INTERVALLE_MAJ_STANDARD);

            DebugAfficheurTexteC = new AfficheurTexte(Game, new Vector2(2, 2), 32, 6, "This is the third box. The pokemon used Tackle!", INTERVALLE_MAJ_STANDARD);

            DebugAfficheurTexteD = new AfficheurTexte(Game, new Vector2(2, 2), 32, 6, "OH MY GAWD IT'S SUPER EFFECTIVE!", INTERVALLE_MAJ_STANDARD);

            ÉtatJeu = ÉtatsJeu.JEU3D;
            ÉtatJeuTexte = new TexteFixe(Game, new Vector2(5, 5), "GameState : " + ÉtatJeu.ToString());
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
            PositionBoxStandard = new Vector2(0, Game.Window.ClientBounds.Height - Cadre.TAILLE_TILE * 6);

            LeJoueur.AddPokemon(02, 50);
            Game.Components.Add(ÉtatJeuTexte);
            Game.Components.Add(DebugAfficheurTexteA);
            Game.Components.Add(DebugAfficheurTexteB);
            Game.Components.Add(DebugAfficheurTexteC); Game.Components.Add(DebugAfficheurTexteD);
        }
        public override void Update(GameTime gameTime)
        {
            ÉtatJeuTexte.RemplacerMessage("GameState : " + ÉtatJeu.ToString());

            GérerClavier();
            //GérerTransition();
            GérerÉtat();
            if (Game.Components.Count < POKEMON_SUR_LE_TERRAIN)
            {
                if (Game.Components.Count(p => p is Afficheur3D) == 2)
                    Game.Components.Add(new Afficheur3D(Game));
                AjoutPokemonsRandom();
            }
            if (GestionInput.EstNouvelleTouche(Keys.N))
            {
                AjoutPokemonsRandom();
            }
            Vector3 positionPokéball = /*new Vector3(Joueur.Position.X, Joueur.Position.Y + 5, Joueur.Position.Z)*/ new Vector3(96, 25, -96);
            Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);
            AjouterProjectile(positionPokéball, rotationObjet);
            base.Update(gameTime);
        }

        void AjouterProjectile(Vector3 positionPokéball, Vector3 rotationObjet)
        {
            if (GestionInput.EstNouveauClicGauche())
            {
                Projectile = new Pokeball(Game, 0.4f, rotationObjet, positionPokéball, RAYON_POKÉBALL, new Vector2(20, 20), "Pokeball", INTERVALLE_MAJ_STANDARD);
                Game.Components.Add(Projectile);
            }
        }

        private void GérerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Enter))
            {
                if(!(ÉtatJeu == ÉtatsJeu.COMBAT))
                {
                UploadSauvegarde();
                foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                {
                    t.ÀDétruire = true;
                }
                Game.Components.Add(new TexteFixe(Game, new Vector2(1, 1), "Saved Successfully"));
                }
                
            }

        }

        public void UploadSauvegarde()
        {
            List<string> Sauvegarde = new List<string>();
            Sauvegarde.Add(LeJoueur.Position.X.ToString());
            Sauvegarde.Add(LeJoueur.Position.Y.ToString());
            Sauvegarde.Add(LeJoueur.Position.Z.ToString());
            Sauvegarde.Add(LeJoueur[0].ToString());
            Sauvegarde.Add(LeJoueur[0].ToStringLev());

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
            int pokedexNumberAléatoire;// = générateurAléatoire.Next(1, POKEDEX_MAX);

            int[] pokemonPasValides = new int[] { 35, 41, 42, 77, 78, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 93, 95, 96, 97, 98, 99, 100, 101, 102, 103, 106, 108, 109, 110, 111, 113, 114, 116, 117, 118, 119, 120, 121, 124, 125, 126, 128, 131, 132, 135, 140, 141, 143 };

            //Ca crash aussi avec 64 pis 69, y en a qui sont invalides et pas dans le tableau pokemonPasValider

            //for (int i = 0; i < pokemonPasValides.Length; i++)
            //{
            //    if(pokedexNumberAléatoire == pokemonPasValides[i])
            //    {
            //        pokedexNumberAléatoire = générateurAléatoire.Next(1, POKEDEX_MAX);
            //    }
            //}

            //do
            //{
            //    pokedexNumberAléatoire = générateurAléatoire.Next(1, POKEDEX_MAX);
            //}
            //while (pokemonPasValides.Contains(pokedexNumberAléatoire));

            do
            {
                pokedexNumberAléatoire = générateurAléatoire.Next(1, POKEDEX_MAX);
            }
            while (!(pokedexNumberAléatoire < 30)); //pour que ça run pis qu'on puisse travailler sur autre chose en même temps

            PokemonRandom1Infos = new Pokemon(Game, pokedexNumberAléatoire, générateurAléatoire.Next(LeJoueur[0].Level - 3, LeJoueur[0].Level + 3));
            PokemonRandom1 = new ObjetDeBase(Game, PokemonRandom1Infos, TrouverDossierModèle(pokedexNumberAléatoire), ÉCHELLE_OBJET, Vector3.Zero, TrouverPositionRandom());
            Game.Components.Add(PokemonRandom1);
            PokemonSurLeTerrain.Add(PokemonRandom1);
        }

        private string TrouverDossierModèle(int pokedexNumber)
        {
            string local = pokedexNumber.ToString();
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
                    LeJoueur.Visible = true;
                    GérerCollision();
                    break;
                case ÉtatsJeu.COMBAT:
                    if (!(Game.Components.Contains(Game.Components.Where(c => c is Combat) as Combat)) && !Flags.Combat)
                    {
                        if (PokemonEnCollision.UnPokemon.EstEnVie)
                        {
                            LeJoueur.Visible = false;
                            Vector2 vecteurPosition = new Vector2(LeJoueur.Position.X - 1 + TerrainDeJeu.NbColonnes / 2, LeJoueur.Position.Z + 2 +  TerrainDeJeu.NbRangées / 2);
                            float posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
                            Vector3 positionjoueurpok = new Vector3(LeJoueur.Position.X + 2, posY, LeJoueur.Position.Z  + 2);

                            Vector2 vecteurPositionopponent = new Vector2(LeJoueur.Position.X + TerrainDeJeu.NbColonnes / 2, LeJoueur.Position.Z + TerrainDeJeu.NbRangées / 2);
                            float posYopponent = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
                            //ObjetDeBase PokemonLancer = new ObjetDeBase(Game, TrouverDossierModèle(LeJoueur[0].PokedexNumber), ÉCHELLE_OBJET, new Vector3(0, (float)(16 * Math.PI / 10), 0), new Vector3(LeJoueur.Position.X + 1, LeJoueur.Position.Y, LeJoueur.Position.Z + 1));
                            PokemonJoueur = new ObjetDeBase(Game, TrouverDossierModèle(LeJoueur.NextPokemonEnVie().PokedexNumber), ÉCHELLE_OBJET *3, new Vector3(0, (float)(8 * Math.PI / 5), 0), positionjoueurpok);

                            Game.Components.Add(new Afficheur3D(Game));
                            Game.Components.Add(PokemonJoueur);
                            PokemonEnCollision.Position = new Vector3(LeJoueur.Position.X - 2, posYopponent, LeJoueur.Position.Z + 2);
                            PokemonEnCollision.Rotation = new Vector3(0, -(float)( 7 *Math.PI / 5), 0);
                            PokemonSurLeTerrain[indexPokemonEnCollision] = PokemonEnCollision;
                            (PokemonSurLeTerrain[indexPokemonEnCollision] as ObjetDeBase).CalculerMonde();
                            (PokemonSurLeTerrain[indexPokemonEnCollision] as ObjetDeBase).Initialize();
                            Flags.Combat = true;
                            LeCombat = new Combat(Game, PositionBoxStandard, LeJoueur, PokemonEnCollision.UnPokemon, INTERVALLE_MAJ_STANDARD);
                            Game.Components.Add(LeCombat);
                        }

                        //ObjetDeBase PokemonLancer = new ObjetDeBase(Game,"09/09",ÉCHELLE_OBJET,new Vector3(0,(float)(16*Math.PI/10),0),new Vector3(LeJoueur.Position.X + 1,LeJoueur.Position.Y, LeJoueur.Position.Z +1 ));
                        //Game.Components.Add(new Afficheur3D(Game));
                        //Game.Components.Add(PokemonLancer);
                        //PokemonEnCollision.Position = new Vector3(PokemonLancer.Position.X -1 , PokemonLancer.Position.Y, PokemonLancer.Position.Z- 1);
                        //PokemonSurLeTerrain[indexPokemonEnCollision] = PokemonEnCollision;
                        //Flags.Combat = true;
                        //LeCombat = new Combat(Game, PositionBoxStandard, LeJoueur, new Pokemon(Game, 1/*no de pokédex du pokémon wild que l'on veut combattre*/), INTERVALLE_MAJ_STANDARD);
                        //Game.Components.Add(LeCombat);

                        //CaméraJeu.Cible = new Vector3(LeJoueur.Position.X + 3, LeJoueur.Position.Y + 3, LeJoueur.Position.Z);
                        //CaméraJeu.CréerPointDeVue(CaméraJeu.Position, CaméraJeu.Cible, CaméraJeu.OrientationVerticale);
                    }
                    if (!Combat.EnCombat)//Si le combat est terminé, on veut retourner à l'état Jeu3D avec le player et son modèle
                    {
                        //PokemonJoueur.Visible = false;
                        PokemonEnCollision.ÀDétruire = true;
                        PokemonJoueur.ÀDétruire = true;
                        ÉtatJeu = ÉtatsJeu.JEU3D;
                    }
                    if (Combat.EnCombat && LeCombat.GetPokemonEstChangé) //Ajouter si possible la condition que le pokémon est changé
                        PokemonJoueur.ChangerModèle(TrouverDossierModèle(LeCombat.NoPokédexUserPokemon));
                    //PokemonEnCollision.ChangerModèle(TrouverDossierModèle(LeCombat.NoPokédexOpponentPokemon));
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
                        indexPokemonEnCollision = PokemonSurLeTerrain.IndexOf(p);
                        PokemonEnCollision = PokemonSurLeTerrain[indexPokemonEnCollision];
                        ÉtatJeu = ÉtatsJeu.COMBAT;
                    }
                    if (!LeJoueur.EstEnCollision(p))
                    {
                        Flags.Combat = false;
                        //ÉtatJeu = ÉtatsJeu.JEU3D;
                    }
                }
            }
        }
    }
}
