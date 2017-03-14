using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA.Classes_Pokemon_Skyrim
{

    enum PokemonTypes { Null, Normal, Fire, Water, Electric, Grass, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel }
    public class Attaque : Microsoft.Xna.Framework.GameComponent
    {
        AccessBaseDeDonnée Database { get; set; }
        List<string> AttaqueEnString { get; set; }
        List<int> Weaknesses { get; set; }

        public int NuméroAttaque { get; set; }
        public string Name => AttaqueEnString[1];
        public int Power => int.Parse(AttaqueEnString[2]);

        int AttackType { get; set; }

        public Attaque(Game jeu, int attaqueNumber)
            : base(jeu)
        {
            AttaqueEnString = new List<string>();
            Weaknesses = new List<int>();
            NuméroAttaque = attaqueNumber;
        }

        public override void Initialize()
        {
            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;

            AttaqueEnString = Database.AccessDonnéesAttaqueStats(NuméroAttaque);

            AttackType = (int)Enum.Parse(typeof(PokemonTypes), AttaqueEnString[4]);// comme "AttackType = (int)Type.Water;", le type vaut 3. Ici, on va chercher le num du type de l'attaque peu importe l'attaque
            Weaknesses = Database.AccessDonnéesArrayWeaknessStrengh(AttackType);

            base.Initialize();
        }

        public float GetTypeMultiplier(string premierType, string secondType)
        {
            int indexPremierType = (int)Enum.Parse(typeof(PokemonTypes), premierType);
            int indexSecondType = (int)Enum.Parse(typeof(PokemonTypes), secondType);

            int multiplierPremierType = Weaknesses[indexPremierType + 2];//+2 parce qu'on skip la colonne du numéro de type et du nom de type
            int multiplierSecondType = Weaknesses[indexSecondType + 2];

            return (multiplierPremierType / 100) * (multiplierSecondType / 100);
        }
        /*//from Microsoft
        public class EnumTest
        {
            enum Days { Sun, Mon, Tue, Wed, Thu, Fri, Sat };

            static void Main()
            {
                int x = (int)Days.Sun;
                int y = (int)Days.Fri;
                Console.WriteLine("Sun = {0}", x);
                Console.WriteLine("Fri = {0}", y);
            }
        }
        /* Output:
           Sun = 0
           Fri = 5
        */
    }
}
