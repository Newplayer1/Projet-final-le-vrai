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
    public class Pokemon
    {
        public int MaxHp => 20;
        public int HP { get; private set; }
        public bool EstEnVie => HP > 0;
        public int Attack => 20;
        public int Defense => 20;
        public int SpecialAttack => 20;
        public int SpecialDefense => 20;
        public int Speed => 20;


        public int Level { get; private set; }
        List<int> AttaquesList { get; set; }
        public int this[int index]
        {
            get
            {
                return AttaquesList[index];
            }
        }

        public string Nom { get; private set; }
        public Pokemon(string nom)
        {
            AttaquesList = new List<int>();
            AttaquesList.Add(0);
            AttaquesList.Add(1);
            AttaquesList.Add(2);
            AttaquesList.Add(-1);
            Nom = nom;
            HP = 20;
            Level = 10;
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
        public string VieToString()
        {
            return HP.ToString() + "/" + MaxHp.ToString() + " HP";
        }
    }
}
