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
    enum Status { NUL, BRN, FRZ, SLP, PSN, PAR }
    public class Pokemon : Vivant
    {
        const int MAX_POKEDEX_NUMBER = 151;
        const int MIN_POKEDEX_NUMBER = 0;
        const int MAX_NIVEAU = 100;
        const int MIN_NIVEAU = 0;

        AccessBaseDeDonnée Database { get; set; }
        Random Générateur { get; set; }
        List<string> PokemonEnString { get; set; }

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

        int PokedexNumber { get; set; }
        

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

        public string Type1 => PokemonEnString[8];
        public string Type2 => PokemonEnString[9];

        
        protected int Niveau
        {
            get { return Level; }
            set
            {
                if(!(value > MAX_NIVEAU || value >= MIN_NIVEAU))
                Level = value;
                else
                {
                    throw new Exception();
                }
            }
        }
 
        
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

            //au lieu de juste mettre l'attaque 0, on pourrait mettre aléatoirement parmi les attaques disponibles du pokémon au niveau mentionné 

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
        }

        public override void Initialize()
        {
            Générateur = new Random();
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
