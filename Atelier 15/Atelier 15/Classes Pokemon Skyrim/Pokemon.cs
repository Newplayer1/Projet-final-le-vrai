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
        /*    
 *  doit storer:
 *  - stats d'un pokemon en fct du niveau (recalculé si level up)
 *  - bool pokémon appartient au user ou non
 *  - copie des stats (que combat va overwrite pour modifier les stats d'un pkmn)
 *  - list pour storer les 4 attaques
 *  - type (vector2 qu'on va caster en int)
 *  - pts de vie (modifiable si pkmn center)
 *  - pts exp (modifiable par combat.cs)
 *  - status (brn, frz, slp, psn, par)
 *  - fonction attaquer ((seulement si pokemon du user, donc pas wild ou opponent)qui permet de prendre une des quatres attaques, retourne l'attaque choisie, fonction appelée par combat.cs sour la forme "User.Attack")
 *  - fonction défense (qui recoit les dommages calculés par combat.cs selon le type et attaques)
 *  - fonction riposter ((seulement si adverse) choisi une attaque aléatoire dans la liste d'attaques, appelé par combat "Opponent.Riposte" )
 *  - fonction évoluer (séquentiel)
 *  - fonction level up si exp atteint level complet: check if evolution, recalcul les stats, check if new move is learned
 *    */


        bool valeurVie = true;
        List<string> TypesRecu;
        List<string> TypesOriginals;

        const int MAX_POKEDEX_NUMBER = 151;
        const int MIN_POKEDEX_NUMBER = 0;
        const int MAX_NIVEAU = 100;
        const int MIN_NIVEAU = 0;

        int pokedexNumber;
        
        
        float poids;
        string type1;
        string type2;
        string nomSprite;
        public int Level { get; private set; }
        int Exp { get; set; }

        public int HP => Stats[0];
        public int Attack => Stats[1];
        public int Defense => Stats[2];
        public int SpecialAttack => Stats[3];
        public int SpecialDefense => Stats[4];
        public int Speed => Stats[5];

        List<int> Stats { get; set; }
        List<int> StatsFixes { get; set; }


        bool EstSauvage { get; set; }

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
            get { return Level; }
            set
            {
                if(!(value > MAX_NIVEAU || value >= MIN_NIVEAU))
                Level = value;
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
            get { return HP; }
            set
            {
                int valeur = ConditionVie(value);
                Stats[0] = valeur;
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
        public int Attaquer()
        {
            return Attack;
        }
        public void Défendre(int pointsDeDamage)
        {
            PtsVie = PtsVie - pointsDeDamage;
        }
        public int AttaqueAléatoire()
        {
            return Attack;
        }

        public void ChangerSpeed(int valeur)
        {
            Stats[5] = valeur;
        }

        public void GainExp(int valeur)
        {
            Exp = Exp + valeur;
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
