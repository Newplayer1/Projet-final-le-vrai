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
         new public bool EstEnVie => VérifierAlive();
        bool VérifierAlive()
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
                return Party[index]; //C'est correct, on veut pas faire de new, on veut changer directement ce pokémon (HP, etc)
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
            return Party.Find(pkm => pkm.EstEnVie); //Retourne le premier pokémon trouvé qui est encore en vie.
        }

        public void Heal()
        {
            foreach (Pokemon item in Party)
            {
                //item.AjouterHP(item.MaxHp);
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
        public void AddPokemon(Pokemon wild)//C'est correct, on modifie la référence originale du pokémon. Pas besoin de faire la copie, on veut conserver ses HP/niveau pis toute, pas besoin de prendre de l'espace
        {
            wild.EstSauvage = false;
            Party.Add(wild);
        }
        public Trainer(Game jeu, string nom, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
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
    }
}
