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
        public new bool EstEnVie => VérifierAlive();
        public string Nom { get; private set; }

        public Pokemon this[int index]
        {
            get
            {
                return Party[index]; //C'est correct, on veut pas faire de new, on veut changer directement ce pokémon (HP, etc)
            }
        }

        bool VérifierAlive()
        {
            bool valeur = true;
            foreach (Pokemon item in Party)
            {
                if (!item.EstEnVie)
                    valeur = false;
            }
            return valeur;
        }

        public Trainer(Game jeu, string nom, string nomModèle, float échelle, Vector3 rotation, Vector3 position, float intervallleMAJ)
            : base(jeu, nomModèle, échelle, rotation, position)
        {
            Nom = nom;
        }
        
        public override void Initialize()
        {
            //Party.Add(new Pokemon(Game, "Magikarp"));

            Party = new List<Pokemon>();
            base.Initialize();
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void AddPokemon(Pokemon newPokemon)
        {
            Party.Add(newPokemon);//Peut-être add new Pokemon(newPokemon);? (non, pas nécessairement. Inutile car on veut pas créer plein de copies inutiliement)
        }
        public void AddPokemon(int pokedexNumber)//on devra faire un constructeur de pokémon qui accepte juste un int
        {
            Party.Add(new Pokemon(Game, pokedexNumber, 5));
        }

        public void RemovePokemon(int index) //Donne juste le choix d'enlever un numéro. Et si on faisait aussi possible d'enlever en donnant le nom du pokémon?
        {
            Party.RemoveAt(index);
        }
    }
}
