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
        const int POKEDEX_MAX = 35;
        const float RAYON_POK�BALL = 0.25f;
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
        //Pokemon_B test { get; set; }
        �tatsJeu �tatJeu { get; set; }
        TexteFixe �tatJeuTexte { get; set; }
        Random g�n�rateurAl�atoire { get; set; }
        List<ObjetDeBase> PokemonSurLeTerrain { get; set; }
        ObjetDeBase PokemonEnCollision { get; set; }
        int indexPokemonEnCollision;
        
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
            g�n�rateurAl�atoire = new Random();
            Vector3 positionObjet = new Vector3(96, 16.37255f, -96);
            //Vector3 positionObjet = new Vector3(100, 20, -100);
            �tatJeu = �tatsJeu.JEU3D;
            �tatJeuTexte = new TexteFixe(Game, new Vector2(5, 5), "GameState : " + �tatJeu.ToString());
            //LoadSauvegarde();
            Game.Components.Add(new Arri�rePlan(Game, "BackGroundNuage"));
            Game.Components.Add(new Afficheur3D(Game));
            TerrainDeJeu = new TerrainAvecBase(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 17, 256), "TerrainPokemon", "D�tailsTerrain", 5, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(TerrainDeJeu);
            Game.Services.AddService(typeof(TerrainAvecBase), TerrainDeJeu);

            //Game.Services.AddService(typeof(Pokeball), Projectile);

            Game.Components.Add(LeJoueur);
            Game.Services.AddService(typeof(Player), LeJoueur);
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Cam�raJeu = Game.Services.GetService(typeof(Cam�ra)) as Cam�raSubjective;
            (Cam�raJeu as Cam�raSubjective).Cible = new Vector3(LeJoueur.Position.X, LeJoueur.Position.Y + 5, LeJoueur.Position.Z);
            PositionBoxStandard = new Vector2(0, Game.Window.ClientBounds.Height - Cadre.TAILLE_TILE * 6);
            Game.Components.Add(�tatJeuTexte);
        }
        public override void Update(GameTime gameTime)
        {
            �tatJeuTexte.RemplacerMessage("GameState : " + �tatJeu.ToString());
            
            G�rerClavier();
            Vector3 positionPok�ball = new Vector3(LeJoueur.Position.X + 1.2f, LeJoueur.Position.Y + 0.8f, LeJoueur.Position.Z);
            Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);
            AjouterProjectile(positionPok�ball, rotationObjet);

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

            EnleverProjectile();
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
        void EnleverProjectile()
        {
            int nbPOkeballactives = Game.Components.Count(x => x is Pokeball);
            for (int i = Game.Components.Count - 1; i >= 0; --i)
            {
                if (Game.Components[i] is Pokeball && nbPOkeballactives == 5) 
                {
                    //PROBL�ME : la cinqui�me balle ajout� va disparaitre tout de suite.. donc le cinquieme clic fait rien th�oriquement
                        Game.Components.RemoveAt(i);
                }
            }
        }
    
        //no entiendo porque la pokeball no desaparece cuando esta a bajo de menos diez
        

        private void G�rerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.H))
            {
                    LeJoueur.Heal();
                    Game.Components.Add(new AfficheurTexte(Game, new Vector2(PositionBoxStandard.X, PositionBoxStandard.Y), Cadre.LARGEUR_BOX_STANDARD, Cadre.HAUTEUR_BOX_STANDARD, "All Pokemon has been healed", INTERVALLE_MAJ_STANDARD));

            }
            if (GestionInput.EstNouvelleTouche(Keys.Enter))
            {
                if (!(�tatJeu == �tatsJeu.COMBAT))
                {
                    UploadSauvegarde();
                    foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                    {
                        t.�D�truire = true;
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

            //int pokedexNumberAl�atoire = g�n�rateurAl�atoire.Next(1, POKEDEX_MAX);
            //int pokedexNumberAl�atoire = NumeroPokemonValides();
            int numpok = NumeroPokemonValides();
            PokemonRandom1Infos = new Pokemon(Game, numpok , g�n�rateurAl�atoire.Next(LeJoueur[0].Level - 3, LeJoueur[0].Level + 3));
            PokemonRandom1 = new ObjetDeBase(Game, PokemonRandom1Infos, TrouverDossierMod�le(numpok), �CHELLE_OBJET * 2, Vector3.Zero, TrouverPositionRandom());
            Game.Components.Add(PokemonRandom1);
            PokemonSurLeTerrain.Add(PokemonRandom1);



        }
        private int NumeroPokemonValides()
        {
            //List< int > pokemonnopeValides = new List<int> { 35, 41, 42, 60, 77, 78, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 93, 95, 96, 97, 98, 99, 100, 101, 102, 103, 106, 108, 109, 110, 111, 113, 114, 116, 117, 118, 119, 120, 121, 124, 125, 126, 128, 131, 132, 135, 140, 141, 143 };
            List<int> pokemonValides = new List<int> { /*1,2,3,4,5,6,7,8,9,*/10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,94,104,105,107,112,115,122,123,127,129,130,133,134,136,137,138,139,142,144,145,146,147,148,149,150,151};

            int pokedexNumberAl�atoire = g�n�rateurAl�atoire.Next(1, pokemonValides.Count);
            int pokemonchoisi = pokemonValides[pokedexNumberAl�atoire];
            //int a = 99;

            //do
            //{
            //    pokedexNumberAl�atoire = g�n�rateurAl�atoire.Next(1, POKEDEX_MAX);
            //} while ((pokemonPasValides.Contains(pokedexNumberAl�atoire)) || (pokedexNumberAl�atoire < 80 && pokedexNumberAl�atoire > 34));
            return pokemonchoisi;
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

            //grandir �chelle si besoin 
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
                            Vector2 vecteurPosition = new Vector2(LeJoueur.Position.X - 1 + TerrainDeJeu.NbColonnes / 2, LeJoueur.Position.Z + 2 + TerrainDeJeu.NbRang�es / 2);
                            float posY = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
                            Vector3 positionjoueurpok = new Vector3(LeJoueur.Position.X + 2, posY, LeJoueur.Position.Z + 2);

                            Vector2 vecteurPositionopponent = new Vector2(LeJoueur.Position.X + TerrainDeJeu.NbColonnes / 2, LeJoueur.Position.Z + TerrainDeJeu.NbRang�es / 2);
                            float posYopponent = (TerrainDeJeu.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), TerrainDeJeu.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
                            //ObjetDeBase PokemonLancer = new ObjetDeBase(Game, TrouverDossierMod�le(LeJoueur[0].PokedexNumber), �CHELLE_OBJET, new Vector3(0, (float)(16 * Math.PI / 10), 0), new Vector3(LeJoueur.Position.X + 1, LeJoueur.Position.Y, LeJoueur.Position.Z + 1));
                            PokemonJoueur = new ObjetDeBase(Game, TrouverDossierMod�le(LeJoueur.NextPokemonEnVie().PokedexNumber), �CHELLE_OBJET * 3, new Vector3(0, (float)(8 * Math.PI / 5), 0), positionjoueurpok);

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
                if (!(p is Player) && LeJoueur.EstEnVie)
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
            if (Game.Components.Contains(Projectile))
            {
                foreach (ObjetDeBase o in Game.Components.Where(r => r is ObjetDeBase))
                {
                    if (!(o is Player))
                    {
                        if (Projectile.EstEnCollision(o))
                        {
                            LeCombat.TryCatchWildPokemon(LeJoueur, o.UnPokemon);
                            �tatJeu = �tatsJeu.JEU3D;
                        }
                    }

                }
            }
        }
    }
}
