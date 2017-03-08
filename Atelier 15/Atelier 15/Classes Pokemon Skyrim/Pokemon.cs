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

        AccessBaseDeDonn�e Database { get; set; }
        Random G�n�rateur { get; set; }
        List<string> PokemonEnString { get; set; }
        int PokedexNumber { get; set; }
        string ExpGrowth => PokemonEnString[11];
        int BaseExp => int.Parse(PokemonEnString[12]);
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
        bool Est�chang� { get; set; }
        public string Type1 => PokemonEnString[8];
        public string Type2 => PokemonEnString[9];


 
        
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
            Est�chang� = false;
            EstSauvage = false;
            //au lieu de juste mettre l'attaque 0, on pourrait mettre al�atoirement parmi les attaques disponibles du pok�mon au niveau mentionn� 
            PokemonEnString = Database.AccessDonn�esPokemonStats(pokedexNumber);
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
            Est�chang� = false;
            EstSauvage = false;

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
            if (Level < MAX_LEVEL && DoitLevelUp())
            {
                Ex�cuterS�quenceLevelUp();
            }
        }
        public int GiveExp()
        {
            float a = 1;
            float t = 1;

            if (!EstSauvage)
                a = 1.5f;
            if (Est�chang�)
                t = 1.5f;
            
            return (int)((a * t * BaseExp * Level) / (7));
        }

        void Ex�cuterS�quenceLevelUp()
        {
            Level++;
            V�rifierSi�volution();// ajouter int du niveau d'�volution
            V�rifierSiNouvelleAttaqueApprise();
            CalculerStatsEtHP(Level);//inclu R�tablirStats()
        }

        void V�rifierSi�volution(int niveau�volution)
        {
            if (Level >= niveau�volution)//if le niveau d'�volution a �t� atteint
            {
                ChangerPokedexNumber(PokedexNumber + 1);
                //Ex�cuter animation d'�volution?
            }
        }
        void V�rifierSiNouvelleAttaqueApprise()
        {
        }

        bool DoitLevelUp() //Selon les polyn�mes de Pokemon, comment on nommerait ces constantes si on devait en faire?
        {
            bool valeurV�rit� = false;
            int levelSuivant = Level + 1;

            if (ExpGrowth == ExpGrowthClass.Fast.ToString()) //Changer pour un switch case?
                valeurV�rit� = (Exp >= (int)(0.8 * Math.Pow(levelSuivant, 3)));

            else if (ExpGrowth == ExpGrowthClass.MediumFast.ToString())//Est-ce que ToString ram�ne le string ou le nombre en string?
                valeurV�rit� = (Exp >= (int)Math.Pow(levelSuivant, 3));

            else if (ExpGrowth == ExpGrowthClass.MediumSlow.ToString())
                valeurV�rit� = (Exp >= (int)(1.2 * Math.Pow(levelSuivant, 3) - 15 * Math.Pow(levelSuivant, 2) + 100 * levelSuivant - 140));
            
            else if (ExpGrowth == ExpGrowthClass.Slow.ToString())
                valeurV�rit� = (Exp >= (int)(1.25 * Math.Pow(levelSuivant, 3)));

            return valeurV�rit�;
        }

        public override void Initialize()
        {
            G�n�rateur = new Random();//?
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
