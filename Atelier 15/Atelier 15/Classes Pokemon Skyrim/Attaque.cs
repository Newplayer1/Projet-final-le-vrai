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
using System.Data.OleDb;

namespace AtelierXNA
{
    enum PokemonTypes { Null, Normal, Fire, Water, Electric, Grass, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel }
    public class Attaque : Microsoft.Xna.Framework.GameComponent
    {
        AccessBaseDeDonnée Database { get; set; }
        List<string> AttaqueEnString { get; set; }
        List<int> Weaknesses { get; set; }

        public int NuméroAttaque { get; set; }
        public string Name => AttaqueEnString[1];
        public int Power => int.Parse(AttaqueEnString[2]); //va crasher joyeusement si non numérique
        public int Accuracy => int.Parse(AttaqueEnString[3]); //tjrs numérique, ok
        public int EffectAccuracy => int.Parse(AttaqueEnString[6]);
        public string DescriptionEffet => AttaqueEnString[7];
        public int Coefficient => int.Parse(AttaqueEnString[8]);
        int AttackType { get; set; }

        public Attaque(Game jeu, int attaqueNumber)
            : base(jeu)
        {
            AttaqueEnString = new List<string>();
            Weaknesses = new List<int>();
            NuméroAttaque = attaqueNumber;
            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
            AttaqueEnString = Database.AccessDonnéesAttaqueStats(NuméroAttaque + 1);

            AttackType = (int)Enum.Parse(typeof(PokemonTypes), AttaqueEnString[4]);// comme "AttackType = (int)Type.Water;", le type vaut 3. Ici, on va chercher le num du type de l'attaque peu importe l'attaque
            Weaknesses = Database.AccessDonnéesArrayWeaknessStrengh(AttackType);
        }

        public override void Initialize()
        {
            //Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
            //AttaqueEnString = Database.AccessDonnéesAttaqueStats(NuméroAttaque);


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

        //category
        public bool EstUneAttaqueSpéciale()
        {
            bool value = false;

            if (AttaqueEnString[5].ToUpper() == "SPECIAL")
                value = true;

            return value;
        }
        public bool EstUneAttaquePhysique()
        {
            bool value = false;

            if (AttaqueEnString[5].ToUpper() == "PHYSIC")
                value = true;

            return value;
        }
        public bool EstUneAttaqueStatus()
        {
            bool value = false;

            if (AttaqueEnString[5].ToUpper() == "status")
                value = true;

            return value;
        }
        //


        //
        public bool EstUneAttaqueAvecBasePowerValide()
        {
            bool value = false;

            if (!(AttaqueEnString[2].ToUpper() == "null"))
                value = true;

            return value;
        }
        public bool EstUneAttaqueAvecEffectAccuracyValide()
        {
            bool value = false;

            if (!(AttaqueEnString[6].ToUpper() == "null"))
                value = true;

            return value;
        }
        public bool AUnEffetNonNull()
        {
            bool value = false;

            if (!(AttaqueEnString[7].ToUpper() == "null"))
                value = true;

            return value;
        }
        public bool AUnEffetSurPlusieursTours()
        {
            bool value = false;

            if (!(AttaqueEnString[9].ToUpper() == "null"))
                value = true;

            return value;
        }

        public override string ToString()
        {
            return Name;
        }
        //AccessBaseDeDonnée Database { get; set; }
        //List<string> AttaqueEnString { get; set; }
        //List<int> Weaknesses { get; set; }

        //public int NuméroAttaque { get; set; }
        //public string Name => AttaqueEnString[1];
        //public int Power => int.Parse(AttaqueEnString[2]);

        //int AttackType { get; set; }

        //public Attaque(Game jeu, int attaqueNumber)
        //    : base(jeu)
        //{
        //    AttaqueEnString = new List<string>();
        //    Weaknesses = new List<int>();
        //    NuméroAttaque = attaqueNumber;
        //}

        //public override void Initialize()
        //{
        //    Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
        //    AttaqueEnString = Database.AccessDonnéesAttaqueStats(NuméroAttaque);

        //    AttackType = (int)Enum.Parse(typeof(PokemonTypes), AttaqueEnString[4]);// comme "AttackType = (int)Type.Water;", le type vaut 3. Ici, on va chercher le num du type de l'attaque peu importe l'attaque
        //    Weaknesses = Database.AccessDonnéesArrayWeaknessStrengh(AttackType);

        //    base.Initialize();
        //}

        //public float GetTypeMultiplier(string premierType, string secondType)
        //{
        //    int indexPremierType = (int)Enum.Parse(typeof(PokemonTypes), premierType);
        //    int indexSecondType = (int)Enum.Parse(typeof(PokemonTypes), secondType);

        //    int multiplierPremierType = Weaknesses[indexPremierType + 2];//+2 parce qu'on skip la colonne du numéro de type et du nom de type
        //    int multiplierSecondType = Weaknesses[indexSecondType + 2];

        //    return (multiplierPremierType / 100) * (multiplierSecondType / 100);
        //}
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
