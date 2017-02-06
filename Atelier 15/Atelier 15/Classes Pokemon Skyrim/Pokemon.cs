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
    public class Pokemon : Vivant
    {
        bool valeurVie = true;
        List<string> TypesRecu;
        List<string> TypesOriginals;

        const int MAX_POKEDEX_NUMBER = 151;
        const int MIN_POKEDEX_NUMBER = 0;
        const int MAX_NIVEAU = 100;
        const int MIN_NIVEAU = 0;

        int pokedexNumber;
        int ptsVie;
        int niveau;
        float poids;
        string type1;
        string type2;
        string nomSprite;



        public string Type1 { get; private set; }
        public string Type2 { get; private set; }
        public string Types
        {
            get { return type1; }
            set
            {
                Checker1ou2type(value);
            }
        }

        protected float Poids
        {
            get { return poids; }
            set
            {
                if (value > 0)
                {
                    poids = value;
                }
            }
        }
        protected string NomSprite
        {
            get { return nomSprite; }
            set
            {
                if (!(string.IsNullOrEmpty(value)))
                {
                    nomSprite = value;
                }
            }
        }

        protected int Niveau
        {
            get { return niveau; }
            set
            {
                if(!(value > MAX_NIVEAU || value >= MIN_NIVEAU))
                niveau = value;
                else
                {
                    throw new Exception();
                }
            }
        }
        public override bool EstEnVie()
        {
            return valeurVie;
        }
        protected int PtsVie
        {
            get { return ptsVie; }
            set
            {
                int valeur = ConditionVie(value);
                    ptsVie = valeur;
            }
        }
        private int ConditionVie(int vie)
        {
            if (vie <= 0)
            {
                vie = 0;
                valeurVie = false;
            }
            return vie;
        }

        protected int PokedexNumber
        {
            get { return pokedexNumber; }
            set
            {
                if (value <= 151 && value > 0)
                {
                    pokedexNumber = value;
                }
            }
        }
        public Pokemon(Game game, string nom, float poids,string types, string nomSprite)
            : base(game, nom)
        {
            TypesRecu = new List<string>();
            TypesOriginals = new List<string>();
            Poids = poids;
            Types = types;
            NomSprite = nomSprite;
        }
        // peut etre dans comnbat pis ici onn va utiliser des vectors
        private  void Checker1ou2type(string types)
        {
            TypesOriginals.Add("FLYING");
            TypesOriginals.Add("FIRE");
            TypesOriginals.Add("WATER");
            TypesOriginals.Add("FIGHTING");
            TypesOriginals.Add("NORMAL");
            TypesOriginals.Add("POISON");
            TypesOriginals.Add("GRASS");
            TypesOriginals.Add("ELETRIC");
            TypesOriginals.Add("PSYCHIC");
            TypesOriginals.Add("GROUND");
            TypesOriginals.Add("ICE");
            TypesOriginals.Add("ROCK");
            TypesOriginals.Add("BUG");
            TypesOriginals.Add("DRAGON");
            TypesOriginals.Add("GHOST");
            TypesOriginals.Add("DARK");
            TypesOriginals.Add("STEEL");

            TypesRecu = types.Split(',').ToList();
            if (TypesRecu.Count == 1)
            {
                AssocierLesTypes(TypesRecu[0], Type1);
            }
            else
                if (TypesRecu.Count == 2)
            {
                AssocierLesTypes(TypesRecu[0],Type1);
                AssocierLesTypes(TypesRecu[1], Type2);
            }
            else throw new Exception();
        }
        private void AssocierLesTypes(string types, string propriété)
        {
            foreach (string t in TypesOriginals)
            {
                if (t == types.Trim().ToUpper())
                    propriété = types;
            }
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
