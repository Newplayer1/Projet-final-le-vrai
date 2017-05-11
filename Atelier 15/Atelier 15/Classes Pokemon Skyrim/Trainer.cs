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
    public class Trainer : Vivant
    {
        List<Pokemon> Party { get; set; }
        List<int> Inventaire { get; set; }
        public string Nom { get; private set; }
        public int GetNbPokemon => Party.Count;
         new public bool EstEnVie => V�rifierAlive();
        bool V�rifierAlive()
        {
            bool valeur = false;
            foreach (Pokemon item in Party)
            {
                if (item.EstEnVie)
                    valeur = true;
            }
            return valeur;
        }


        public Pokemon this[int index]
        {
            get
            {
                return Party[index]; //C'est correct, on veut pas faire de new, on veut changer directement ce pok�mon (HP, etc)
            }
        }
        public Pokemon GetPokemon(int i)
        {
            return new Pokemon(Party[i]);
        }
        public Pokemon NextPokemonEnVie()
        {
            return Party.Find(pkm => pkm.EstEnVie); //Retourne le premier pok�mon trouv� qui est encore en vie.
        }

        public void Heal()
        {
            foreach (Pokemon p in Party)
            {
                p.AjouterHP(p.MaxHp);
            }
        }
        public void AddPokemon(int pokedexNumber)
        {
            Party.Add(new Pokemon(Game, pokedexNumber, this));
        }
        public void AddPokemon(int pokedexNumber, int level)
        {
            Party.Add(new Pokemon(Game, pokedexNumber, level, this));
        }
        public void AddPokemon(Pokemon wild)//C'est correct, on modifie la r�f�rence originale du pok�mon. Pas besoin de faire la copie, on veut conserver ses HP/niveau pis toute, pas besoin de prendre de l'espace
        {
            wild.EstSauvage = false;
            Party.Add(wild);
        }
        public Trainer(Game jeu, string nom, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
            Nom = nom;
            Party = new List<Pokemon>();
        }
        
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void ChangerListe(Pokemon PremierDeLaListe)
        {
            Party.Remove(PremierDeLaListe);
            Party.Insert(0, PremierDeLaListe);
        }
    }
}
