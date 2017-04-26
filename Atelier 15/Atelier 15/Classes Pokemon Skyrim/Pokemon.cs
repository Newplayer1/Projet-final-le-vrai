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
    public class Pokemon : Microsoft.Xna.Framework.GameComponent
    {
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

        int PokedexNumber { get; set; }
        public int Level { get; private set; }
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
        public string Type1 => PokemonEnString[8];
        public string Type2 => PokemonEnString[9];

        AccessBaseDeDonn�e Database { get; set; }

        public Pokemon(Game jeu, int pokedexNumber)
            : base(jeu)
        {
            AttaquesList = new List<Attaque>();
            AttaquesList.Add(new Attaque(Game, 1));
            AttaquesList.Add(new Attaque(Game, 2));
            AttaquesList.Add(new Attaque(Game, 3));
            AttaquesList.Add(new Attaque(Game, 4));

            PokedexNumber = pokedexNumber;
            Level = 50;
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            PokemonEnString = Database.AccessDonn�esPokemonStats(pokedexNumber);
            CalculerStatsEtHP(Level);
            HP = MaxHp;

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
        public string VieToString()
        {
            return HP.ToString() + "/" + MaxHp.ToString() + " HP";
        }
    }
}
