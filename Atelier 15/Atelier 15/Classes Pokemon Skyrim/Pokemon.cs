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
        const int LEVEL_PAR_DÉFAUT = 15;
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
        public int NiveauÉvolution => int.Parse(PokemonEnString[13]);
        public int Level { get; private set; }
        ExpGrowthClass ExpGrowth { get; set; }
        int BaseExp => int.Parse(PokemonEnString[12]);
        public float Exp { get; private set; }

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

        AccessBaseDeDonnée Database { get; set; }

        public Pokemon(Game jeu, int pokedexNumber)
            : base(jeu)
        {
            CréerDatabase(pokedexNumber);
            Level = LEVEL_PAR_DÉFAUT;

            AttribuerAttaquesParDéfaut();
            CalculerStatsEtHP(Level);
            Exp = 0;
            Exp = CalculerExpTotal(Level);
            HP = MaxHp;
            EstSauvage = true;
        }

        

        public Pokemon(Game jeu, int pokedexNumber, Trainer player)
            : base(jeu)
        {
            CréerDatabase(pokedexNumber);
            
            Level = LEVEL_PAR_DÉFAUT;
            Exp = 0;
            AttribuerAttaquesParDéfaut();
            CalculerStatsEtHP(Level);
            Exp = CalculerExpTotal(Level);
            HP = MaxHp;
            EstSauvage = false;
        }

        private void CréerDatabase(int pokedexNumber)
        {
            PokedexNumber = pokedexNumber;
            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
            PokemonEnString = Database.AccessDonnéesPokemonStats(PokedexNumber);
            LearnsetEnString = Database.AccessDonnéesTypeLevelAttaque(Type1EnInt);
            
            ExpGrowth = (ExpGrowthClass)Enum.Parse(typeof(ExpGrowthClass), PokemonEnString[11]);
        }

        public Pokemon(Game jeu, int pokedexNumber, int level)
            : base(jeu)
        {
            CréerDatabase(pokedexNumber);
            
            Level = level;
            Exp = 0;
            AttribuerAttaquesParDéfaut();
            CalculerStatsEtHP(Level);
            Exp = CalculerExpTotal(Level);
            HP = MaxHp;
            EstSauvage = true;
        }
        public Pokemon(Game jeu, int pokedexNumber, int level, Trainer player)
            : base(jeu)
        {
            CréerDatabase(pokedexNumber);
            Level = level;
            Exp = 0;
            AttribuerAttaquesParDéfaut();
            CalculerStatsEtHP(Level);
            Exp = CalculerExpTotal(Level);
            HP = MaxHp;
            EstSauvage = false;
        }
        public Pokemon(Pokemon copie)
            :base(copie.Game)
        {
            CréerDatabase(copie.PokedexNumber);
            Level = copie.Level;
            AttribuerAttaquesParDéfaut();
            CalculerStatsEtHP(Level);
            //CalculerExpTotal(Level);
            HP = copie.HP;
            EstSauvage = false;
            Exp = copie.Exp;
        }

        public override void Initialize()
        {
            ExpGrowth = (ExpGrowthClass)Enum.Parse(typeof(ExpGrowthClass), PokemonEnString[11]);//fonctionne pas dans l'initialize
            base.Initialize();
        }
        void AttribuerAttaquesParDéfaut()
        {
            //Attribuer attaques selon level et type
            AttaquesList = new List<Attaque>();
            int atkBase = int.Parse(LearnsetEnString[2]);
            int premièreAttaqueType = int.Parse(LearnsetEnString[4]);
            int secondeAttaqueType = int.Parse(LearnsetEnString[5]);

            AttaquesList.Add(new Attaque(Game, atkBase));
            AttaquesList.Add(new Attaque(Game, atkBase + 1));

            if (Level >= 10)
            {
                AttaquesList.Add(new Attaque(Game, premièreAttaqueType));
                if (Level >= 25)
                    AttaquesList.Add(new Attaque(Game, secondeAttaqueType));
            }
        }

        void CalculerStatsEtHP(int level)//Refaire à chaque level up, a faire lorsque Access bien implémenté
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
            RétablirStats();
        }
        public float CalculerExpTotal(int level)
        {
            float expTotal = 0;
            switch (ExpGrowth)
            {
                case ExpGrowthClass.Fast:
                    expTotal += CalculerExpFast(level);
                    break;
                case ExpGrowthClass.MediumFast:
                    expTotal += CalculerExpMediumFast(level);
                    break;
                case ExpGrowthClass.MediumSlow:
                    expTotal += CalculerExpMediumSlow(level);
                    break;
                case ExpGrowthClass.Slow:
                    expTotal += CalculerExpSlow(level);
                    break;
            }
            return expTotal;
        }
        public void RétablirStats()//Après chaque combat (et level up) TRÈS IMPORTANT
        {
            Stats = new List<int>(StatsFixes);
        }

        void ChangerPokedexNumber(int nouveauPkdexNumber)// à faire si on évolue
        {
            PokedexNumber = nouveauPkdexNumber;
            PokemonEnString = Database.AccessDonnéesPokemonStats(PokedexNumber);
        }

        //public int AttaqueAléatoire()//Temp
        //{
        //    Attaque attaqueAléatoire;
        //    int index = 0;

        //    do
        //    {
        //        index = Générateur.Next(0, 4);
        //        attaqueAléatoire = AttaquesList[index];
        //    }
        //    while (AttaquesList[index] < 0);

        //    return attaqueAléatoire;
        //}
        public void GainExp(float valeur)
        {
            Exp = Exp + valeur;

            AfficheurTexte messageC = new AfficheurTexte(Game, Jeu.PositionBoxMessage, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, Nom + " gained " + ((int)valeur).ToString() + " exp.", Jeu.INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(messageC);
            //si Exp dépasse un threshold, faire le level up : check if evolution, recalcul les stats, check if new move is learned
            if (Level < MAX_LEVEL && DoitLevelUp())
            {
                ExécuterSéquenceLevelUp();
            }
            
        }
        bool DoitLevelUp() //Selon les polynômes de Pokemon, comment on nommerait ces constantes si on devait en faire?
        {
            bool valeurVérité = false;

            if (CalculerExpTotal(Level + 1) <= Exp)
                valeurVérité = true;
            return valeurVérité;
        }
        float CalculerExpFast(int level)
        {
            return (float)(0.8 * Math.Pow(level, 3));
        }
        float CalculerExpMediumFast(int level)
        {
            return (float)Math.Pow(level, 3);
        }
        float CalculerExpMediumSlow(int level)
        {
            return (float)(1.2 * Math.Pow(level, 3) - 15 * Math.Pow(level, 2) + 100 * level - 140);
        }
        float CalculerExpSlow(int level)
        {
            return (float)(1.25 * Math.Pow(level, 3));
        }
        void ExécuterSéquenceLevelUp()
        {
            Level++;
            Game.Components.Add(new AfficheurTexte(Game, Jeu.PositionBoxMessage, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, Nom + " grew to level " + Level + "!", Jeu.INTERVALLE_MAJ_STANDARD));

            Évoluer(NiveauÉvolution);
            VérifierSiNouvelleAttaqueApprise();
            CalculerStatsEtHP(Level);//inclu RétablirStats()
        }

        void Évoluer(int niveauEvolution)
        { 
            if (Level >= niveauEvolution)
            {
                string ancienNom = Nom;
                ChangerPokedexNumber(++PokedexNumber);
                
                Game.Components.Add(new AfficheurTexte(Game, Jeu.PositionBoxMessage, Cadre.LARGEUR_BOX_STANDARD, Cadre.HAUTEUR_BOX_STANDARD, ancienNom + " evolved into " + Nom + "!" , Atelier.INTERVALLE_MAJ_STANDARD));
            }
        }
        void VérifierSiNouvelleAttaqueApprise()
        {
            if (Level == 10)
            {
                Attaque nouvelleAttaque = new Attaque(Game, int.Parse(LearnsetEnString[4]));
                AttaquesList.Add(nouvelleAttaque);
                //AttribuerAttaquesParDéfaut();
                Game.Components.Add(new AfficheurTexte(Game, Jeu.PositionBoxMessage, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, Nom + " learned " + nouvelleAttaque.Name + "!", Jeu.INTERVALLE_MAJ_STANDARD));
            }
            else if (Level == 25)
            {
                Attaque nouvelleAttaque = new Attaque(Game, int.Parse(LearnsetEnString[5]));
                AttaquesList.Add(nouvelleAttaque);
                //AttribuerAttaquesParDéfaut();
                Game.Components.Add(new AfficheurTexte(Game, Jeu.PositionBoxMessage, Jeu.LargeurBoxMessage, Jeu.HauteurBoxMessage, Nom + " learned " + nouvelleAttaque.Name + "!", Jeu.INTERVALLE_MAJ_STANDARD));
            }
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

        public void Défendre(int value)//Recevoir les points de damage
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
