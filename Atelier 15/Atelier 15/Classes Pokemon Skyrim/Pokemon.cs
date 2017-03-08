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
using System.Data.OleDb;

namespace AtelierXNA.Classes_Pokemon_Skyrim
{
    enum Status { NULL, BRN, FRZ, SLP, PSN, PAR }
    enum ExpGrowthClass { Fast = 800000, MediumFast = 1000000, MediumSlow = 1059860, Slow = 1250000}

    public class Pokemon : Vivant
    {

        const int MAX_LEVEL = 100;

        AccessBaseDeDonnée Database { get; set; }
        Random Générateur { get; set; }
        List<string> PokemonEnString { get; set; }
        int PokedexNumber { get; set; }
        string ExpGrowth => PokemonEnString[11];
        int BaseExp => int.Parse(PokemonEnString[12]);
        /*    
 *  doit storer:
 *  - stats d'un pokemon en fct du niveau (recalculé si level up)
 *  - bool pokémon appartient au user ou non
 *  - copie des stats (que combat va overwrite pour modifier les stats d'un pkmn)
 *  - list pour storer les 4 attaques
 *  - type (string)
 *  - HP (modifiable si pkmn center)
 *  - pts exp (modifiable par combat.cs)
 *  - status (brn, frz, slp, psn, par)
 *  - fonction attaquer ((seulement si pokemon du user, donc pas wild ou opponent)qui permet de prendre une des quatres attaques, retourne l'attaque choisie, fonction appelée par combat.cs sour la forme "User.Attack")
 *  - fonction défense (qui recoit les dommages calculés par combat.cs selon le type et attaques)
 *  - fonction riposter ((seulement si adverse) choisi une attaque aléatoire dans la liste d'attaques, appelé par combat "Opponent.Riposte" )
 *  - fonction évoluer (séquentiel)
 *  - fonction level up si exp atteint level complet: check if evolution, recalcul les stats, check if new move is learned
 *    */




        public int Level { get; private set; }
        int Exp { get; set; }

        public int HP { get; set; }
        int MaxHp { get; set; }

        public int Attack => Stats[0];
        public int Defense => Stats[1];
        public int SpecialAttack => Stats[2];
        public int SpecialDefense => Stats[3];
        public int Speed => Stats[4];

        List<int> Stats { get; set; }
        List<int> StatsFixes { get; set; }

        List<int> AttaquesList { get; set; }//Liste de 4 int référant à un numéro d'attaque
        bool EstSauvage { get; set; }
        bool EstÉchangé { get; set; }
        public string Type1 => PokemonEnString[8];
        public string Type2 => PokemonEnString[9];


 
        
        public Pokemon(Game game, int pokedexNumber, int level)
            : base(game)
        {
            PokedexNumber = pokedexNumber;
            Level = level;

            AttaquesList = new List<int>();
            AttaquesList.Add(0);
            AttaquesList.Add(-1);//Négatif siginife aucune attaque
            AttaquesList.Add(-1);
            AttaquesList.Add(-1);
            EstÉchangé = false;
            EstSauvage = false;
            //au lieu de juste mettre l'attaque 0, on pourrait mettre aléatoirement parmi les attaques disponibles du pokémon au niveau mentionné 
            PokemonEnString = Database.AccessDonnéesPokemonStats(pokedexNumber);
            CalculerStatsEtHP(Level);//AccessBaseDeDonnées pour remplir les valeurs de stats du pokémon selon son pokedex number et niveau
        }
        public Pokemon(Game game, int pokedexNumber, int level, List<int> attaques)
            : base(game)
        {
            PokedexNumber = pokedexNumber;
            Level = level;
            PokemonEnString = new List<string>();
            PokemonEnString = Database.AccessDonnéesPokemonStats(pokedexNumber);
            AttaquesList = new List<int>(attaques);
            EstÉchangé = false;
            EstSauvage = false;

            CalculerStatsEtHP(Level);//AccessBaseDeDonnées pour remplir les valeurs de stats du pokémon selon son pokedex number et niveau
        }

        void CalculerStatsEtHP(int level)//Refaire à chaque level up
        {
            MaxHp = (2 * (int.Parse(PokemonEnString[2]) + 2) * level) / 100 + level + 10;

            float stat = 0;
            int baseStat = 0;
            for (int i = 1; i <= 5; i++)
            {
                baseStat = int.Parse(PokemonEnString[2 + i]);

                stat = (2 * (baseStat + 2) * level) / 100 + 5;
                StatsFixes.Add((int)stat);
            }
            RétablirStats();
        }

        void ChangerPokedexNumber(int nouveauPkdexNumber)// à faire si on évolue
        {
            PokedexNumber = nouveauPkdexNumber;
            PokemonEnString = Database.AccessDonnéesPokemonStats(PokedexNumber);
        }

        public int Attaquer()//Temp, on va retourner le numéro de l'attaque, pas le stat "Attack"
        {
            return Attack;
        }
        public void Défendre(int pointsDeDamage)
        {
            HP = HP - pointsDeDamage;
            //si la vie est à zéro, mettre EstEnVie à zéro
            if (HP <= 0)
            {
                EstEnVie = false; //Hérité de Vivant, mis public dans vivant
                HP = 0;
            }
        }
        public int AttaqueAléatoire()//Temp
        {
            int attaqueAléatoire = 0;
            int index = 0;

            do
            {
                index = Générateur.Next(0, 4);
                attaqueAléatoire = AttaquesList[index];
            }
            while (AttaquesList[index] < 0) ;

            return attaqueAléatoire;
        }


        public void RétablirStats()//Après chaque combat (et level up) TRÈS IMPORTANT
        {
            Stats = new List<int>(StatsFixes);
        }

        public void GainExp(int valeur)
        {
            Exp = Exp + valeur;
            //si Exp dépasse un threshold, faire le level up : check if evolution, recalcul les stats, check if new move is learned
            if (Level < MAX_LEVEL && DoitLevelUp())
            {
                ExécuterSéquenceLevelUp();
            }
        }
        public int GiveExp()
        {
            float a = 1;
            float t = 1;

            if (!EstSauvage)
                a = 1.5f;
            if (EstÉchangé)
                t = 1.5f;
            
            return (int)((a * t * BaseExp * Level) / (7));
        }

        void ExécuterSéquenceLevelUp()
        {
            Level++;
            VérifierSiÉvolution();// ajouter int du niveau d'évolution
            VérifierSiNouvelleAttaqueApprise();
            CalculerStatsEtHP(Level);//inclu RétablirStats()
        }

        void VérifierSiÉvolution(int niveauÉvolution)
        {
            if (Level >= niveauÉvolution)//if le niveau d'évolution a été atteint
            {
                ChangerPokedexNumber(PokedexNumber + 1);
                //Exécuter animation d'évolution?
            }
        }
        void VérifierSiNouvelleAttaqueApprise()
        {
        }

        bool DoitLevelUp() //Selon les polynômes de Pokemon, comment on nommerait ces constantes si on devait en faire?
        {
            bool valeurVérité = false;
            int levelSuivant = Level + 1;

            if (ExpGrowth == ExpGrowthClass.Fast.ToString()) //Changer pour un switch case?
                valeurVérité = (Exp >= (int)(0.8 * Math.Pow(levelSuivant, 3)));

            else if (ExpGrowth == ExpGrowthClass.MediumFast.ToString())//Est-ce que ToString ramène le string ou le nombre en string?
                valeurVérité = (Exp >= (int)Math.Pow(levelSuivant, 3));

            else if (ExpGrowth == ExpGrowthClass.MediumSlow.ToString())
                valeurVérité = (Exp >= (int)(1.2 * Math.Pow(levelSuivant, 3) - 15 * Math.Pow(levelSuivant, 2) + 100 * levelSuivant - 140));
            
            else if (ExpGrowth == ExpGrowthClass.Slow.ToString())
                valeurVérité = (Exp >= (int)(1.25 * Math.Pow(levelSuivant, 3)));

            return valeurVérité;
        }

        public override void Initialize()
        {
            Générateur = new Random();//?
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
