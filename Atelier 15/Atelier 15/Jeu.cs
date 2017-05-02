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
    enum �tatsJeu { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        const float INTERVALLE_MAJ_STANDARD = 1 / 60f;
        const int POKEMON_SUR_LE_TERRAIN = 25;
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float �CHELLE_OBJET = 0.004f;
        const int POKEDEX_MAX = 152;
        const int RAYON_POK�BALL = 1;
        Vector2 PositionBoxStandard { get; set; }
        ObjetDeBase PokemonJoueur { get; set; }
        Player LeJoueur { get; set; }
        Pokeball Projectile { get; set; }
        InputManager GestionInput { get; set; }
        AccessBaseDeDonn�e Database { get; set; }
        Combat LeCombat { get; set; }
        Cam�raSubjective Cam�raJeu { get; set; }
        TerrainAvecBase TerrainDeJeu { get; set; }
        ObjetDeBase PokemonRandom1 { get; set; }
        Pokemon PokemonRandom1Infos { get; set; }
        Pokemon_B test { get; set; }
        �tatsJeu �tatJeu { get; set; }
        TexteFixe �tatJeuTexte { get; set; }
        Random g�n�rateurAl�atoire { get; set; }
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
            /*DataBase = new AccessBaseDeDonn�e();*/
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI / 4, 0);
            Vector3 positionCPU = new Vector3(96, 18f, -30);
            LeJoueur = new Player(Game, "El_guapo", �CHELLE_OBJET, rotationObjet, positionCPU, INTERVALLE_MAJ_STANDARD, 1f);
            //LeJoueur.PokemonsDansLesMains = new List<int>();

            //LeJoueur.PokemonsDansLesMains.Add(choix);
            LeJoueur.AddPokemon(choix); //tout les trainers ont une liste de pok�mon, la liste va dans trainer (et faut jamais mettre une liste publique)
            PokemonSurLeTerrain = new List<ObjetDeBase>();
            UploadSauvegarde();
        }
        public Jeu(Game game, List<string> Sauvegarde)
            : base(game)
        {
            /*DataBase = new AccessBaseDeDonn�e();*/
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            Vector3 rotationObjet = new Vector3(0, -(float)Math.PI / 4, 0);
            LeJoueur = new Player(Game, "El_guapo", �CHELLE_OBJET, rotationObjet, new Vector3(float.Parse(Database.LoadSauvegarde()[0]), float.Parse(Database.LoadSauvegarde()[1]), float.Parse(Database.LoadSauvegarde()[2])), INTERVALLE_MAJ_STANDARD, 1f);
            //LeJoueur.AddPokemon(int.Parse(Database.LoadSauvegarde()[3]), int.Parse(Database.LoadSauvegarde()[4]));
            PokemonSurLeTerrain = new List<ObjetDeBase>();
        }
        public override void Initialize()
        {
            g�n�rateurAl�atoire = new Random();
            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            //Vector3 positionObjet = new Vector3(100, 20, -100);
            DebugAfficheurTexteA = new AfficheurTexte(Game, new Vector2(2, 2), 32, 6, "This is the first box. The pokemon used Tackle!", INTERVALLE_MAJ_STANDARD);

            DebugAfficheurTexteB = new AfficheurTexte(Game, new Vector2(2, 2), 32, 6, "It's not very effective.", INTERVALLE_MAJ_STANDARD);

            DebugAfficheurTexteC = new AfficheurTexte(Game, new Vector2(2, 2), 32, 6, "This is the third box. The pokemon used Tackle!", INTERVALLE_MAJ_STANDARD);

            DebugAfficheurTexteD = new AfficheurTexte(Game, new Vector2(2, 2), 32, 6, "OH MY GAWD IT'S SUPER EFFECTIVE!", INTERVALLE_MAJ_STANDARD);

            �tatJeu = �tatsJeu.JEU3D;
            �tatJeuTexte = new TexteFixe(Game, new Vector2(5, 5), "GameState : " + �tatJeu.ToString());
            //LoadSauvegarde();
            Game.Components.Add(new Arri�rePlan(Game, "BackGroundNuage"));
            Game.Components.Add(new Afficheur3D(Game));
            TerrainDeJeu = new TerrainAvecBase(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 17, 256), "TerrainPokemon", "D�tailsTerrain", 5, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(TerrainDeJeu);
            Game.Services.AddService(typeof(TerrainAvecBase), TerrainDeJeu);
            Game.Components.Add(LeJoueur);
            Game.Services.AddService(typeof(Player), LeJoueur);
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Cam�raJeu = Game.Services.GetService(typeof(Cam�ra)) as Cam�raSubjective;
            PositionBoxStandard = new Vector2(0, Game.Window.ClientBounds.Height - Cadre.TAILLE_TILE * 6);

            LeJoueur.AddPokemon(02, 50);
            Game.Components.Add(�tatJeuTexte);
            Game.Components.Add(DebugAfficheurTexteA);
            Game.Components.Add(DebugAfficheurTexteB);
            Game.Components.Add(DebugAfficheurTexteC); Game.Components.Add(DebugAfficheurTexteD);
        }
        public override void Update(GameTime gameTime)
        {
            �tatJeuTexte.RemplacerMessage("GameState : " + �tatJeu.ToString());

            G�rerClavier();
            //G�rerTransition();
            G�rer�tat();
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
            Vector3 positionPok�ball = /*new Vector3(Joueur.Position.X, Joueur.Position.Y + 5, Joueur.Position.Z)*/ new Vector3(96, 25, -96);
            Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);
            AjouterProjectile(positionPok�ball, rotationObjet);
            base.Update(gameTime);
        }

        void AjouterProjectile(Vector3 positionPok�ball, Vector3 rotationObjet)
        {
            if (GestionInput.EstNouveauClicGauche())
            {
                Projectile = new Pokeball(Game, 0.4f, rotationObjet, positionPok�ball, RAYON_POK�BALL, new Vector2(20, 20), "Pokeball", INTERVALLE_MAJ_STANDARD);
                Game.Components.Add(Projectile);
            }
        }

        private void G�rerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Enter))
            {
                if(!(�tatJeu == �tatsJeu.COMBAT))
                {
                UploadSauvegarde();
                foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                {
                    t.�D�truire = true;
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

        private void G�rerTransition()
        {
            switch (�tatJeu)
            {
                case �tatsJeu.JEU3D:
                    break;
                    //case �tats.COMBAT:
                    //    G�rerTransitionCombat();
                    //    break;
                    //case �tats.MAISON:
                    //    G�rerTransitionMaison();
                    //    break;
                    //case �tats.GYM:
                    //    G�rerTransitionGym();
                    //    break;
                    //case �tats.FIN:
                    //    G�rerTransitionFin();
                    //    break;
                    //default:
                    //    break;
            }
        }
        private void AjoutPokemonsRandom()
        {
            int pokedexNumberAl�atoire;// = g�n�rateurAl�atoire.Next(1, POKEDEX_MAX);

            int[] pokemonPasValides = new int[] { 35, 41, 42, 77, 78, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 93, 95, 96, 97, 98, 99, 100, 101, 102, 103, 106, 108, 109, 110, 111, 113, 114, 116, 117, 118, 119, 120, 121, 124, 125, 126, 128, 131, 132, 135, 140, 141, 143 };

            //Ca crash aussi avec 64 pis 69, y en a qui sont invalides et pas dans le tableau pokemonPasValider

            //for (int i = 0; i < pokemonPasValides.Length; i++)
            //{
            //    if(pokedexNumberAl�atoire == pokemonPasValides[i])
            //    {
            //        pokedexNumberAl�atoire = g�n�rateurAl�atoire.Next(1, POKEDEX_MAX);
            //    }
            //}

            //do
            //{
            //    pokedexNumberAl�atoire = g�n�rateurAl�atoire.Next(1, POKEDEX_MAX);
            //}
            //while (pokemonPasValides.Contains(pokedexNumberAl�atoire));

            do
            {
                pokedexNumberAl�atoire = g�n�rateurAl�atoire.Next(1, POKEDEX_MAX);
            }
            while (!(pokedexNumberAl�atoire < 30)); //pour que �a run pis qu'on puisse travailler sur autre chose en m�me temps

            PokemonRandom1Infos = new Pokemon(Game, pokedexNumberAl�atoire, g�n�rateurAl�atoire.Next(LeJoueur[0].Level - 3, LeJoueur[0].Level + 3));
            PokemonRandom1 = new ObjetDeBase(Game, PokemonRandom1Infos, TrouverDossierMod�le(pokedexNumberAl�atoire), �CHELLE_OBJET, Vector3.Zero, TrouverPositionRandom());
            Game.Components.Add(PokemonRandom1);
            PokemonSurLeTerrain.Add(PokemonRandom1);
        }

        private string TrouverDossierMod�le(int pokedexNumber)
        {
            string local = pokedexNumber.ToString();
            if (local.Count() == 1)
            {
                local = "0" + local;
            }
            string nomMod = local + '/' + local;
            return nomMod;

            //float �chelleObjTest = 0.01f;

            //if (local1 == 14.ToString())
            //{
            //    �chelleObjTest = �CHELLE_OBJET * 10;
            //}
        }


        private Vector3 TrouverPositionRandom()
        {
            float positionRandomX = g�n�rateurAl�atoire.Next(-TerrainDeJeu.NbColonnes / 2, TerrainDeJeu.NbColonnes / 2);
            float positionRandomY = g�n�rateurAl�atoire.Next(-TerrainDeJeu.NbRang�es / 2, TerrainDeJeu.NbRang�es / 2);

            //positionRandomX = MathHelper.Max(MathHelper.Min(positionRandomX, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);
            //positionRandomY = MathHelper.Max(MathHelper.Min(positionRandomY, TerrainDeJeu.NbColonnes / 2), -TerrainDeJeu.NbColonnes / 2);

            Vector2 position = new Vector2(positionRandomX + TerrainDeJeu.NbColonnes / 2, positionRandomY + TerrainDeJeu.NbRang�es / 2);

            float hauteur = (TerrainDeJeu.GetPointSpatial((int)Math.Round(position.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(position.Y, 0)) + Vector3.Zero).Y;

            return new Vector3(positionRandomX, hauteur, positionRandomY);

        }
        private void G�rer�tat()
        {
            switch (�tatJeu)
            {
                case �tatsJeu.JEU3D:
                    LeJoueur.Visible = true;
                    G�rerCollision();
                    break;
                case �tatsJeu.COMBAT:
                    if (!(Game.Components.Contains(Game.Components.Where(c => c is Combat) as Combat)) && !Flags.Combat)
                    {
                        if (PokemonEnCollision.UnPokemon.EstEnVie)
                        {
                            LeJoueur.Visible = false;
                            Vector2 vecteurPosition = new Vector2(LeJoueur.Position.X - 1 + TerrainDeJeu.NbColonnes / 2, LeJoueur.Position.Z + 2 +  TerrainDeJeu.NbRang�es / 2);
                            float posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
                            Vector3 positionjoueurpok = new Vector3(LeJoueur.Position.X + 2, posY, LeJoueur.Position.Z  + 2);

                            Vector2 vecteurPositionopponent = new Vector2(LeJoueur.Position.X + TerrainDeJeu.NbColonnes / 2, LeJoueur.Position.Z + TerrainDeJeu.NbRang�es / 2);
                            float posYopponent = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
                            //ObjetDeBase PokemonLancer = new ObjetDeBase(Game, TrouverDossierMod�le(LeJoueur[0].PokedexNumber), �CHELLE_OBJET, new Vector3(0, (float)(16 * Math.PI / 10), 0), new Vector3(LeJoueur.Position.X + 1, LeJoueur.Position.Y, LeJoueur.Position.Z + 1));
                            PokemonJoueur = new ObjetDeBase(Game, TrouverDossierMod�le(LeJoueur.NextPokemonEnVie().PokedexNumber), �CHELLE_OBJET *3, new Vector3(0, (float)(8 * Math.PI / 5), 0), positionjoueurpok);

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

                        //ObjetDeBase PokemonLancer = new ObjetDeBase(Game,"09/09",�CHELLE_OBJET,new Vector3(0,(float)(16*Math.PI/10),0),new Vector3(LeJoueur.Position.X + 1,LeJoueur.Position.Y, LeJoueur.Position.Z +1 ));
                        //Game.Components.Add(new Afficheur3D(Game));
                        //Game.Components.Add(PokemonLancer);
                        //PokemonEnCollision.Position = new Vector3(PokemonLancer.Position.X -1 , PokemonLancer.Position.Y, PokemonLancer.Position.Z- 1);
                        //PokemonSurLeTerrain[indexPokemonEnCollision] = PokemonEnCollision;
                        //Flags.Combat = true;
                        //LeCombat = new Combat(Game, PositionBoxStandard, LeJoueur, new Pokemon(Game, 1/*no de pok�dex du pok�mon wild que l'on veut combattre*/), INTERVALLE_MAJ_STANDARD);
                        //Game.Components.Add(LeCombat);

                        //Cam�raJeu.Cible = new Vector3(LeJoueur.Position.X + 3, LeJoueur.Position.Y + 3, LeJoueur.Position.Z);
                        //Cam�raJeu.Cr�erPointDeVue(Cam�raJeu.Position, Cam�raJeu.Cible, Cam�raJeu.OrientationVerticale);
                    }
                    if (!Combat.EnCombat)//Si le combat est termin�, on veut retourner � l'�tat Jeu3D avec le player et son mod�le
                    {
                        //PokemonJoueur.Visible = false;
                        PokemonEnCollision.�D�truire = true;
                        PokemonJoueur.�D�truire = true;
                        �tatJeu = �tatsJeu.JEU3D;
                    }
                    if (Combat.EnCombat && LeCombat.GetPokemonEstChang�) //Ajouter si possible la condition que le pok�mon est chang�
                        PokemonJoueur.ChangerMod�le(TrouverDossierMod�le(LeCombat.NoPok�dexUserPokemon));
                    //PokemonEnCollision.ChangerMod�le(TrouverDossierMod�le(LeCombat.NoPok�dexOpponentPokemon));
                    break;
                    //case �tats.GYM:
                    //    G�rerVitesseD�placement();
                    //    G�rerComputer();
                    //    G�rerCombat();
                    //    break;
                    //default: //�tats.FIN:
                    //    Fin();
                    //    SauvegardeAuto();
                    //    break;
                    //
            }
        }

        private void G�rerCollision()
        {
            foreach (ObjetDeBase p in Game.Components.Where(r => r is ObjetDeBase))
            {
                if (!(p is Player))
                {
                    if (LeJoueur.EstEnCollision(p))
                    {
                        indexPokemonEnCollision = PokemonSurLeTerrain.IndexOf(p);
                        PokemonEnCollision = PokemonSurLeTerrain[indexPokemonEnCollision];
                        �tatJeu = �tatsJeu.COMBAT;
                    }
                    if (!LeJoueur.EstEnCollision(p))
                    {
                        Flags.Combat = false;
                        //�tatJeu = �tatsJeu.JEU3D;
                    }
                }
            }
        }
    }
}
