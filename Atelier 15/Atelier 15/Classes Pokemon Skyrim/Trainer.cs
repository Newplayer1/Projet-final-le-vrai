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
        
        public List<string> GetNomPokemon()
        {
            List<string> listeRetour = new List<string>();
            foreach (Pokemon item in Party)
            {
                listeRetour.Add(item.Nom);
            }
            return listeRetour;
        }
        public List<string> GetLVLPokemon()
        {
            List<string> listeRetour = new List<string>();
            foreach (Pokemon item in Party)
            {
                listeRetour.Add(item.Level.ToString());
            }
            return listeRetour;
        }
        public List<string> GetType1Pokemon()
        {
            List<string> listeRetour = new List<string>();
            foreach (Pokemon item in Party)
            {
                //listeRetour.Add(item.Type1);
            }
            return listeRetour;
            
        }
        public List<string> GetType2Pokemon()
        {
            List<string> listeRetour = new List<string>();
            foreach (Pokemon item in Party)
            {
                //listeRetour.Add(item.Type2);
            }
            return listeRetour;
        }
        public List<string> GetHPPokemon()
        {
            List<string> listeRetour = new List<string>();
            foreach (Pokemon item in Party)
            {
                listeRetour.Add(item.VieToString());
            }
            return listeRetour;
        }


        public Pokemon NextPokemonEnVie()
        {
            return Party.Find(pkm => pkm.EstEnVie); //Retourne le premier pok�mon trouv� qui est encore en vie.
        }

        public void Heal()
        {
            foreach (Pokemon item in Party)
            {
                //item.AjouterHP(item.MaxHp);
            }
        }
        public void AddPokemon(int choix)
        {

        }
               public Trainer(Game jeu, string nom, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
            Nom = nom;
            Party = new List<Pokemon>();
        }
        
        public override void Initialize()
        {
            Party.Add(new Pokemon(Game, 6, 20, this));
            Party.Add(new Pokemon(Game, 5, 100, this));
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
