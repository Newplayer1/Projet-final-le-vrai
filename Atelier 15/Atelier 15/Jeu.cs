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
        const int POKEDEX_MAX = 35;
        const float RAYON_POKÉBALL = 0.25f;
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
        //Pokemon_B test { get; set; }
        ÉtatsJeu ÉtatJeu { get; set; }
        TexteFixe ÉtatJeuTexte { get; set; }
        Random générateurAléatoire { get; set; }
        List<ObjetDeBase> PokemonSurLeTerrain { get; set; }
        ObjetDeBase PokemonEnCollision { get; set; }
        int indexPokemonEnCollision;
        
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
            RegarderLeNombreDePokemonAAjouter();
            PokemonSurLeTerrain = new List<ObjetDeBase>();
        }

        private void RegarderLeNombreDePokemonAAjouter()
        {
            int cpt = 0;
            while (cpt < 6)
            {
                if (Database.LoadSauvegarde()[cpt + 3] == "Empty")
                {
                    cpt = 6;
                }
                else
                {
                    LeJoueur.AddPokemon(int.Parse(Database.LoadSauvegarde()[cpt + 3]), int.Parse(Database.LoadSauvegarde()[cpt + 4]));
                    cpt = cpt + 2;
                }
            }
        }

        public override void Initialize()
        {
            générateurAléatoire = new Random();
            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            //Vector3 positionObjet = new Vector3(100, 20, -100);
            ÉtatJeu = ÉtatsJeu.JEU3D;
            ÉtatJeuTexte = new TexteFixe(Game, new Vector2(5, 5), "GameState : " + ÉtatJeu.ToString());
            //LoadSauvegarde();
            Game.Components.Add(new ArrièrePlan(Game, "BackGroundNuage"));
            Game.Components.Add(new Afficheur3D(Game));
            TerrainDeJeu = new TerrainAvecBase(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 17, 256), "TerrainPokemon", "DétailsTerrain", 5, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(TerrainDeJeu);
            Game.Services.AddService(typeof(TerrainAvecBase), TerrainDeJeu);

            //Game.Services.AddService(typeof(Pokeball), Projectile);

            Game.Components.Add(LeJoueur);
            Game.Services.AddService(typeof(Player), LeJoueur);
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as CaméraSubjective;
            (CaméraJeu as CaméraSubjective).Cible = new Vector3(LeJoueur.Position.X, LeJoueur.Position.Y + 5, LeJoueur.Position.Z);
            PositionBoxStandard = new Vector2(0, Game.Window.ClientBounds.Height - Cadre.TAILLE_TILE * 6);
            Game.Components.Add(ÉtatJeuTexte);
        }
        public override void Update(GameTime gameTime)
        {
            ÉtatJeuTexte.RemplacerMessage("GameState : " + ÉtatJeu.ToString());
            
            GérerClavier();
            Vector3 positionPokéball = new Vector3(LeJoueur.Position.X + 1.2f, LeJoueur.Position.Y + 0.8f, LeJoueur.Position.Z);
            Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);
            AjouterProjectile(positionPokéball, rotationObjet);

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

            EnleverProjectile();
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
        void EnleverProjectile()
        {
            int nbPOkeballactives = Game.Components.Count(x => x is Pokeball);
            for (int i = Game.Components.Count - 1; i >= 0; --i)
            {
                if (Game.Components[i] is Pokeball && nbPOkeballactives == 5) 
                {
                    //PROBLÈME : la cinquième balle ajouté va disparaitre tout de suite.. donc le cinquieme clic fait rien théoriquement
                        Game.Components.RemoveAt(i);
                }
            }
        }
    
        //no entiendo porque la pokeball no desaparece cuando esta a bajo de menos diez
        

        private void GérerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.H))
            {
                    LeJoueur.Heal();
                    Game.Components.Add(new AfficheurTexte(Game, new Vector2(PositionBoxStandard.X, PositionBoxStandard.Y), Cadre.LARGEUR_BOX_STANDARD, Cadre.HAUTEUR_BOX_STANDARD, "All Pokemon has been healed", INTERVALLE_MAJ_STANDARD));

            }
            if (GestionInput.EstNouvelleTouche(Keys.Enter))
            {
                if (!(ÉtatJeu == ÉtatsJeu.COMBAT))
                {
                    UploadSauvegarde();
                    foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                    {
                        t.ÀDétruire = true;
                    }
                    Game.Components.Add(new AfficheurTexte(Game, new Vector2(PositionBoxStandard.X, PositionBoxStandard.Y), Cadre.LARGEUR_BOX_STANDARD, Cadre.HAUTEUR_BOX_STANDARD, "Upload Sauvegarde", INTERVALLE_MAJ_STANDARD));
                }

            }

        }

        public void UploadSauvegarde()
        {
            List<string> Sauvegarde = new List<string>();
            Sauvegarde.Add(LeJoueur.Position.X.ToString());
            Sauvegarde.Add(LeJoueur.Position.Y.ToString());
            Sauvegarde.Add(LeJoueur.Position.Z.ToString());
            int compteur = 0;
            while (compteur < LeJoueur.GetNbPokemon)
            {
                Sauvegarde.Add(LeJoueur[compteur].ToStringSauvegarde());
                Sauvegarde.Add(LeJoueur[compteur].ToStringLev());
                compteur++;
            }
            while (compteur < 6)
            {
                Sauvegarde.Add("Empty");
                Sauvegarde.Add("Empty");
                compteur++;
            }

            Database.Sauvegarder(Sauvegarde);
        }
        private void AjoutPokemonsRandom()
        {

            //int pokedexNumberAléatoire = générateurAléatoire.Next(1, POKEDEX_MAX);
            //int pokedexNumberAléatoire = NumeroPokemonValides();
            int numpok = NumeroPokemonValides();
            PokemonRandom1Infos = new Pokemon(Game, numpok , générateurAléatoire.Next(LeJoueur[0].Level - 3, LeJoueur[0].Level + 3));
            PokemonRandom1 = new ObjetDeBase(Game, PokemonRandom1Infos, TrouverDossierModèle(numpok), ÉCHELLE_OBJET * 2, Vector3.Zero, TrouverPositionRandom());
            Game.Components.Add(PokemonRandom1);
            PokemonSurLeTerrain.Add(PokemonRandom1);



        }
        private int NumeroPokemonValides()
        {
            //List< int > pokemonnopeValides = new List<int> { 35, 41, 42, 60, 77, 78, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 93, 95, 96, 97, 98, 99, 100, 101, 102, 103, 106, 108, 109, 110, 111, 113, 114, 116, 117, 118, 119, 120, 121, 124, 125, 126, 128, 131, 132, 135, 140, 141, 143 };
            List<int> pokemonValides = new List<int> { /*1,2,3,4,5,6,7,8,9,*/10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,94,104,105,107,112,115,122,123,127,129,130,133,134,136,137,138,139,142,144,145,146,147,148,149,150,151};

            int pokedexNumberAléatoire = générateurAléatoire.Next(1, pokemonValides.Count);
            int pokemonchoisi = pokemonValides[pokedexNumberAléatoire];
            //int a = 99;

            //do
            //{
            //    pokedexNumberAléatoire = générateurAléatoire.Next(1, POKEDEX_MAX);
            //} while ((pokemonPasValides.Contains(pokedexNumberAléatoire)) || (pokedexNumberAléatoire < 80 && pokedexNumberAléatoire > 34));
            return pokemonchoisi;
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

            //grandir échelle si besoin 
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
                            Vector2 vecteurPosition = new Vector2(LeJoueur.Position.X - 1 + TerrainDeJeu.NbColonnes / 2, LeJoueur.Position.Z + 2 + TerrainDeJeu.NbRangées / 2);
                            float posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
                            Vector3 positionjoueurpok = new Vector3(LeJoueur.Position.X + 2, posY, LeJoueur.Position.Z + 2);

                            Vector2 vecteurPositionopponent = new Vector2(LeJoueur.Position.X + TerrainDeJeu.NbColonnes / 2, LeJoueur.Position.Z + TerrainDeJeu.NbRangées / 2);
                            float posYopponent = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRangées - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
                            //ObjetDeBase PokemonLancer = new ObjetDeBase(Game, TrouverDossierModèle(LeJoueur[0].PokedexNumber), ÉCHELLE_OBJET, new Vector3(0, (float)(16 * Math.PI / 10), 0), new Vector3(LeJoueur.Position.X + 1, LeJoueur.Position.Y, LeJoueur.Position.Z + 1));
                            PokemonJoueur = new ObjetDeBase(Game, TrouverDossierModèle(LeJoueur.NextPokemonEnVie().PokedexNumber), ÉCHELLE_OBJET * 3, new Vector3(0, (float)(8 * Math.PI / 5), 0), positionjoueurpok);

                            Game.Components.Add(new Afficheur3D(Game));
                            Game.Components.Add(PokemonJoueur);
                            PokemonEnCollision.Position = new Vector3(LeJoueur.Position.X - 6, posYopponent + 2, LeJoueur.Position.Z);
                            PokemonEnCollision.Rotation = new Vector3(0, -(float)(7 * Math.PI / 5), 0);
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
                if (!(p is Player) && LeJoueur.EstEnVie)
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
            if (Game.Components.Contains(Projectile))
            {
                foreach (ObjetDeBase o in Game.Components.Where(r => r is ObjetDeBase))
                {
                    if (!(o is Player))
                    {
                        if (Projectile.EstEnCollision(o))
                        {
                            LeCombat.TryCatchWildPokemon(LeJoueur, o.UnPokemon);
                            ÉtatJeu = ÉtatsJeu.JEU3D;
                        }
                    }

                }
            }
        }
    }
}
