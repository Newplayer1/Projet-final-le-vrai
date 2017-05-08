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


namespace AtelierXNA
{
    enum ExpGrowthClass { Fast = 800000, MediumFast = 1000000, MediumSlow = 1059860, Slow = 1250000 }
    public class Pokemon : Microsoft.Xna.Framework.GameComponent
    {

        const int MAX_LEVEL = 100;
        public const int MAX_LETTRES_NOM_POKEMON = 10;
        public bool EstSauvage { get; set; }

        public int MaxHp { get; private set; }
        public int HP { get; private set; }
        public bool EstEnVie => HP > 0;

        public int Attack => Stats[0];
        public int Defense => Stats[1];
        public int SpecialAttack => Stats[2];
        public int SpecialDefense => Stats[3];
        public int Speed => Stats[4];

        List<int> Stats { get; set; }
        List<int> StatsFixes { get; set; }

        public int PokedexNumber { get; private set; }
        public int NbAttaques => AttaquesList.Count;
        public int CatchRate => int.Parse(PokemonEnString[10]);
        public int Level { get; private set; }
        ExpGrowthClass ExpGrowth { get; set; }
        int BaseExp => int.Parse(PokemonEnString[12]);
        public int Exp { get; private set; }

        List<Attaque> AttaquesList { get; set; }
        public Attaque this[int index]
        {
            get
            {
                return AttaquesList[index];
            }
        }

        public string Nom => PokemonEnString[1];
        List<string> PokemonEnString { get; set; } 
        List<string> LearnsetEnString { get; set; }
        public string Type1 => PokemonEnString[8];
        public string Type2 => PokemonEnString[9];
        int Type1EnInt => (int)Enum.Parse(typeof(PokemonTypes), Type1);

        AccessBaseDeDonn�e Database { get; set; }

        public Pokemon(Game jeu, int pokedexNumber)
            : base(jeu)
        {
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            PokemonEnString = Database.AccessDonn�esPokemonStats(pokedexNumber);
            LearnsetEnString = Database.AccessDonn�esTypeLevelAttaque(Type1EnInt);

            PokedexNumber = pokedexNumber;
            Level = 5;

            CalculerStatsEtHP(Level);
            HP = MaxHp;
            EstSauvage = true;
            AttribuerAttaquesParD�faut();
        }
        public Pokemon(Game jeu, int pokedexNumber, Trainer player)
            : base(jeu)
        {
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            PokemonEnString = Database.AccessDonn�esPokemonStats(pokedexNumber);
            LearnsetEnString = Database.AccessDonn�esTypeLevelAttaque(Type1EnInt);

            PokedexNumber = pokedexNumber;
            Level = 5;

            CalculerStatsEtHP(Level);
            HP = MaxHp;
            EstSauvage = false;
            AttribuerAttaquesParD�faut();
        }
        public Pokemon(Game jeu, int pokedexNumber, int level)
            : base(jeu)
        {
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            PokemonEnString = Database.AccessDonn�esPokemonStats(pokedexNumber);
            LearnsetEnString = Database.AccessDonn�esTypeLevelAttaque(Type1EnInt);


            PokedexNumber = pokedexNumber;
            Level = level;

            CalculerStatsEtHP(Level);
            HP = MaxHp;
            EstSauvage = true;
            AttribuerAttaquesParD�faut();
        }
        public Pokemon(Game jeu, int pokedexNumber, int level, Trainer player)
            : base(jeu)
        {
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            PokemonEnString = Database.AccessDonn�esPokemonStats(pokedexNumber);
            LearnsetEnString = Database.AccessDonn�esTypeLevelAttaque(Type1EnInt);
            
            PokedexNumber = pokedexNumber;
            Level = level;

            CalculerStatsEtHP(Level);
            HP = MaxHp;
            EstSauvage = false;
            AttribuerAttaquesParD�faut();
        }

        public override void Initialize()
        {
            ExpGrowth = (ExpGrowthClass)Enum.Parse(typeof(ExpGrowthClass), PokemonEnString[11]);
            base.Initialize();
        }
        void AttribuerAttaquesParD�faut()
        {
            //Attribuer attaques selon level et type
            AttaquesList = new List<Attaque>();
            int atkBase = int.Parse(LearnsetEnString[2]);
            int premi�reAttaqueType = int.Parse(LearnsetEnString[4]);
            int secondeAttaqueType = int.Parse(LearnsetEnString[5]);

            AttaquesList.Add(new Attaque(Game, atkBase));
            AttaquesList.Add(new Attaque(Game, atkBase + 1));

            if (Level >= 10)
            {
                AttaquesList.Add(new Attaque(Game, premi�reAttaqueType));
                if (Level >= 25)
                    AttaquesList.Add(new Attaque(Game, secondeAttaqueType));
            }
        }

        void CalculerStatsEtHP(int level)//Refaire � chaque level up, a faire lorsque Access bien impl�ment�
        {
            MaxHp = (2 * (int.Parse(PokemonEnString[2]) + 2) * level) / 100 + level + 10;
            StatsFixes = new List<int>();
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
        public void R�tablirStats()//Apr�s chaque combat (et level up) TR�S IMPORTANT
        {
            Stats = new List<int>(StatsFixes);
        }

        void ChangerPokedexNumber(int nouveauPkdexNumber)// � faire si on �volue
        {
            PokedexNumber = nouveauPkdexNumber;
            PokemonEnString = Database.AccessDonn�esPokemonStats(PokedexNumber);
        }

        //public int AttaqueAl�atoire()//Temp
        //{
        //    Attaque attaqueAl�atoire;
        //    int index = 0;

        //    do
        //    {
        //        index = G�n�rateur.Next(0, 4);
        //        attaqueAl�atoire = AttaquesList[index];
        //    }
        //    while (AttaquesList[index] < 0);

        //    return attaqueAl�atoire;
        //}
        public bool GainExp(int valeur)
        {
            bool aAugment�DeNiveau = false;
            Exp = Exp + 3*valeur;
            //si Exp d�passe un threshold, faire le level up : check if evolution, recalcul les stats, check if new move is learned
            if (Level < MAX_LEVEL && DoitLevelUp())
            {
                Ex�cuterS�quenceLevelUp();
                aAugment�DeNiveau = true;
            }
            return aAugment�DeNiveau;
        }
        bool DoitLevelUp() //Selon les polyn�mes de Pokemon, comment on nommerait ces constantes si on devait en faire?
        {
            bool valeurV�rit� = false;
            int levelSuivant = Level + 1;

            switch (ExpGrowth)
            {
                case ExpGrowthClass.Fast:
                    valeurV�rit� = (Exp >= (int)(0.8 * Math.Pow(levelSuivant, 3)));
                    break;
                case ExpGrowthClass.MediumFast:
                    valeurV�rit� = (Exp >= (int)Math.Pow(levelSuivant, 3));
                    break;
                case ExpGrowthClass.MediumSlow:
                    valeurV�rit� = (Exp >= (int)(1.2 * Math.Pow(levelSuivant, 3) - 15 * Math.Pow(levelSuivant, 2) + 100 * levelSuivant - 140));
                    break;
                case ExpGrowthClass.Slow:
                    valeurV�rit� = (Exp >= (int)(1.25 * Math.Pow(levelSuivant, 3)));
                    break;
            }
            return valeurV�rit�;
        }
        void Ex�cuterS�quenceLevelUp()
        {
            Level++;
            //V�rifierSi�volution(NiveauEvolution);
            //V�rifierSiNouvelleAttaqueApprise();
            CalculerStatsEtHP(Level);//inclu R�tablirStats()
        }
        public int GiveExp()
        {
            float a = 1;

            if (!EstSauvage)
                a = 1.5f;
            int exp =  (int)((a * BaseExp * Level) / (7));
            return exp;
        }
        public void AjouterHP(int value)//Effet d'un item ou d'une attaque
        {
            HP += value;
            if (HP > MaxHp)
                HP = MaxHp;
        }

        public void D�fendre(int value)//Recevoir les points de damage
        {
            HP -= value;
            if (HP < 0)
                HP = 0;
        }
        public override string ToString()
        {
            return Nom + " Lvl." + Level.ToString();
        }
        public string ToStringSauvegarde()
        {
            return PokedexNumber.ToString();
        }
        public string ToStringLev()
        {
            return Level.ToString();
        }
        public string VieToString()
        {
            return HP.ToString() + "/" + MaxHp.ToString() + " HP";
        }
    }
}
