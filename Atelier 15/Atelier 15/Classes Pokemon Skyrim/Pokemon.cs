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

        AccessBaseDeDonn�e Database { get; set; }
        Random G�n�rateur { get; set; }
        List<string> PokemonEnString { get; set; }

        /*    
 *  doit storer:
 *  - stats d'un pokemon en fct du niveau (recalcul� si level up)
 *  - bool pok�mon appartient au user ou non
 *  - copie des stats (que combat va overwrite pour modifier les stats d'un pkmn)
 *  - list pour storer les 4 attaques
 *  - type (string)
 *  - HP (modifiable si pkmn center)
 *  - pts exp (modifiable par combat.cs)
 *  - status (brn, frz, slp, psn, par)
 *  - fonction attaquer ((seulement si pokemon du user, donc pas wild ou opponent)qui permet de prendre une des quatres attaques, retourne l'attaque choisie, fonction appel�e par combat.cs sour la forme "User.Attack")
 *  - fonction d�fense (qui recoit les dommages calcul�s par combat.cs selon le type et attaques)
 *  - fonction riposter ((seulement si adverse) choisi une attaque al�atoire dans la liste d'attaques, appel� par combat "Opponent.Riposte" )
 *  - fonction �voluer (s�quentiel)
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

        List<int> AttaquesList { get; set; }//Liste de 4 int r�f�rant � un num�ro d'attaque
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
            AttaquesList.Add(-1);//N�gatif siginife aucune attaque
            AttaquesList.Add(-1);
            AttaquesList.Add(-1);

            //au lieu de juste mettre l'attaque 0, on pourrait mettre al�atoirement parmi les attaques disponibles du pok�mon au niveau mentionn� 

            CalculerStatsEtHP(Level);//AccessBaseDeDonn�es pour remplir les valeurs de stats du pok�mon selon son pokedex number et niveau
        }
        public Pokemon(Game game, int pokedexNumber, int level, List<int> attaques)
            : base(game)
        {
            PokedexNumber = pokedexNumber;
            Level = level;
            PokemonEnString = new List<string>();
            PokemonEnString = Database.AccessDonn�esPokemonStats(pokedexNumber);
            AttaquesList = new List<int>(attaques);

            CalculerStatsEtHP(Level);//AccessBaseDeDonn�es pour remplir les valeurs de stats du pok�mon selon son pokedex number et niveau
        }

        void CalculerStatsEtHP(int level)//Refaire � chaque level up
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
            R�tablirStats();
        }

        void ChangerPokedexNumber(int nouveauPkdexNumber)// � faire si on �volue
        {
            PokedexNumber = nouveauPkdexNumber;
            PokemonEnString = Database.AccessDonn�esPokemonStats(PokedexNumber);
        }

        public int Attaquer()//Temp, on va retourner le num�ro de l'attaque, pas le stat "Attack"
        {
            return Attack;
        }
        public void D�fendre(int pointsDeDamage)
        {
            HP = HP - pointsDeDamage;
            //si la vie est � z�ro, mettre EstEnVie � z�ro
            if (HP <= 0)
            {
                EstEnVie = false; //H�rit� de Vivant, mis public dans vivant
                HP = 0;
            }
        }
        public int AttaqueAl�atoire()//Temp
        {
            int attaqueAl�atoire = 0;
            int index = 0;

            do
            {
                index = G�n�rateur.Next(0, 4);
                attaqueAl�atoire = AttaquesList[index];
            }
            while (AttaquesList[index] < 0) ;

            return attaqueAl�atoire;
        }


        public void R�tablirStats()//Apr�s chaque combat (et level up) TR�S IMPORTANT
        {
            Stats = new List<int>(StatsFixes);
        }

        public void GainExp(int valeur)
        {
            Exp = Exp + valeur;
            //si Exp d�passe un threshold, faire le level up : check if evolution, recalcul les stats, check if new move is learned
        }

        public override void Initialize()
        {
            G�n�rateur = new Random();
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
