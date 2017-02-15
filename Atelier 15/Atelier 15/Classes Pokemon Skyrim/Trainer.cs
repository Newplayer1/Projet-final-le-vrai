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
        //codifier une fonction qui lance un pok�mon sur la carte, genre Throw pour que dans combat: User.Throw(PokemonsSurLui{0})
        bool valeurVie = true;
        public List<Pokemon> PokemonsSurLui { get; private set; }
        int age;
        string sexe;

        public Trainer(Game game, string nom, int age)
            : base(game, nom)
        {
            Age = age;
            Sexe = sexe;
            PokemonsSurLui = new List<Pokemon>();
        }
        public int Age
        {
            get { return age; }
            private set
            {
                if(value > 0)
                age = value;
            }
        }
        public string Sexe
        {
            get { return sexe; }
            private set
            {
                if (value.Trim().ToUpper() == "MALE" || value.Trim().ToUpper() == "FEMELE")
                    sexe = value;
                else
                    throw new Exception();
            }
        }

        public override bool EstEnVie()
        {
            int cpt = 0;
            foreach (Pokemon p in PokemonsSurLui/*liste de pokemons que le trainer a !!!!!!*/)
            {
                if (!(p.EstEnVie()))
                {
                    ++cpt;
                }
            }
            if (cpt == PokemonsSurLui.Count)
                valeurVie = false;

            return valeurVie;
        }
        public override void Initialize()
        {
            
            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            G�rerLesInputs();
            base.Update(gameTime);
        }
        private void G�rerLesInputs()
        {
            if (/*bouton de lancer de la pokeball est pes�*/ false)
            {
                LancerPokeball();
            }
        }

        private void LancerPokeball()
        {
            Game.Components.Add(new Pokeball(Game, 1, Position, Vector3.Zero, 1, new Vector2(10, 10), "Pokeball", Atelier.INTERVALLE_MAJ_STANDARD));


        }

        private void InitialiserPokeball(object position)
        {
            throw new NotImplementedException();
        }
    }
}
