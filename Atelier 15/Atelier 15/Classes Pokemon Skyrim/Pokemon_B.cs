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
    public enum Status { NULL, BRN, FRZ, SLP, PSN, PAR }
    //enum ExpGrowthClass { Fast = 800000, MediumFast = 1000000, MediumSlow = 1059860, Slow = 1250000 }

    public class Pokemon_B : Vivant
    {
        const int MAX_LEVEL = 100;
        const int NIVEAU_EVOLUTION_EEVEE = 25;

        public string Nom => PokemonEnString[1];//C'est le quel pour le nom? (mais surtout, o� est pass�e la database??)
        AccessBaseDeDonn�e Database { get; set; }
        Random G�n�rateur { get; set; }
        List<string> PokemonEnString { get; set; }
        int PokedexNumber { get; set; }
        ExpGrowthClass ExpGrowth { get; set; } //Enum.Parse(typeof(ExpGrowthClass), PokemonEnString[11]);//PokemonEnString[11]
        Status Status { get; set; }
        int BaseExp { get; set; }
        int NiveauEvolution { get; set; }

        public int this[int index]
        {
            get
            {
                return AttaquesList[index];
            }
        }

        public int Level { get; private set; }
        int Exp { get; set; }

        public int HP { get; set; }
        int MaxHp { get; set; }

        public int Attack => Stats[0];
        public int Defense => Stats[1];
        public int SpecialAttack => Stats[2];
        public int SpecialDefense => Stats[3];
        public int Speed => Stats[4];

        List<int> Stats { get; set; }
        List<int> StatsFixes { get; set; }

        List<int> AttaquesList { get; set; }//Liste de 4 int r�f�rant � un num�ro d'attaque
        bool EstSauvage { get; set; }
        public string Type1 => PokemonEnString[8];
        public string Type2 => PokemonEnString[9];


        public Pokemon_B(Game game, int pokedexNumber, int level, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
            PokedexNumber = pokedexNumber;
            Level = level;

            AttaquesList = new List<int>();
            AttaquesList.Add(28);
            AttaquesList.Add(29);//N�gatif siginife aucune attaque
            AttaquesList.Add(-1);
            AttaquesList.Add(-1);
            EstSauvage = false;
            Status = Status.NULL;
            //au lieu de juste mettre l'attaque 0, on pourrait mettre al�atoirement parmi les attaques disponibles du pok�mon au niveau mentionn� 
            Database = new AccessBaseDeDonn�e();
            PokemonEnString = Database.AccessDonn�esPokemonStats(pokedexNumber);
            CalculerStatsEtHP(Level);//AccessBaseDeDonn�es pour remplir les valeurs de stats du pok�mon selon son pokedex number et niveau
        }
        public Pokemon_B(Game game, int pokedexNumber, int level, List<int> attaques, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
            PokedexNumber = pokedexNumber;
            Level = level;


            PokemonEnString = new List<string>();
            AttaquesList = new List<int>(attaques);

            EstSauvage = false;
            Status = Status.NULL;
            CalculerStatsEtHP(Level);//AccessBaseDeDonn�es pour remplir les valeurs de stats du pok�mon selon son pokedex number et niveau
        }

        void CalculerStatsEtHP(int level)//Refaire � chaque level up, a faire lorsque Access bien impl�ment�
        {
            MaxHp = (2 * (int.Parse(PokemonEnString[2]) + 2) * level) / 100 + level + 10;
            StatsFixes = new List<int>();
            float stat = 0;
            int baseStat = 0;
            for (int i = 1; i <= 5; i++)
            {
                baseStat = int.Parse(PokemonEnString[2 + i]);

                stat = (2 * (baseStat + 2) * level) / 100 + 5;
                StatsFixes.Add((int)stat);
            }
            R�tablirStats();
        }

        void ChangerPokedexNumber(int nouveauPkdexNumber)// � faire si on �volue
        {
            PokedexNumber = nouveauPkdexNumber;
            PokemonEnString = Database.AccessDonn�esPokemonStats(PokedexNumber);
        }

        public int Attaquer()//Temp, on va retourner le num�ro de l'attaque, pas le stat "Attack"
        {
            return Attack;
        }
        public void D�fendre(int pointsDeDamage)
        {
            HP = HP - pointsDeDamage;
            //si la vie est � z�ro, mettre EstEnVie � z�ro
            if (HP <= 0)
            {
                EstEnVie = false; //H�rit� de Vivant, mis public dans vivant
                HP = 0;
            }
        }
        public int AttaqueAl�atoire()//Temp
        {
            int attaqueAl�atoire = 0;
            int index = 0;

            do
            {
                index = G�n�rateur.Next(0, 4);
                attaqueAl�atoire = AttaquesList[index];
            }
            while (AttaquesList[index] < 0);

            return attaqueAl�atoire;
        }


        public void R�tablirStats()//Apr�s chaque combat (et level up) TR�S IMPORTANT
        {
            Stats = new List<int>(StatsFixes);
        }

        public void GainExp(int valeur)
        {
            Exp = Exp + valeur;
            //si Exp d�passe un threshold, faire le level up : check if evolution, recalcul les stats, check if new move is learned
            if (Level < MAX_LEVEL && DoitLevelUp())
            {
                Ex�cuterS�quenceLevelUp();
            }
        }
        public int GiveExp()
        {
            float a = 1;

            if (!EstSauvage)
                a = 1.5f;
            return (int)((a * BaseExp * Level) / (7));
        }

        void Ex�cuterS�quenceLevelUp()
        {
            Level++;
            V�rifierSi�volution(NiveauEvolution);
            V�rifierSiNouvelleAttaqueApprise();
            CalculerStatsEtHP(Level);//inclu R�tablirStats()
        }

        void V�rifierSi�volution(int niveau�volution)
        {
            if ((niveau�volution > 0) && (Level >= niveau�volution))//if le niveau d'�volution a �t� atteint
            {
                ChangerPokedexNumber(PokedexNumber + 1);
                //Ex�cuter animation d'�volution?
            }

            else if (niveau�volution < 0 && (Level >= NIVEAU_EVOLUTION_EEVEE))//Gestion du cas sp�cial Eevee ici
            {
                //if (HauteurDuJoueur <= (TerrainAvecBase.HAUTEUR_MAXIMALE / 3f))
                //{
                //    ChangerPokedexNumber(PokedexNumber + 1);
                //}
                //else if (HauteurDuJoueur <= 2 * (TerrainAvecBase.HAUTEUR_MAXIMALE / 3f))
                //{
                //    ChangerPokedexNumber(PokedexNumber + 2);
                //}
                //else if (HauteurDuJoueur <= TerrainAvecBase.HAUTEUR_MAXIMALE)
                //{
                //    ChangerPokedexNumber(PokedexNumber + 3);
                //}
            }
        }
        void V�rifierSiNouvelleAttaqueApprise()//? wut
        {

        }

        bool DoitLevelUp() //Selon les polyn�mes de Pokemon, comment on nommerait ces constantes si on devait en faire?
        {
            bool valeurV�rit� = false;
            int levelSuivant = Level + 1;

            switch (ExpGrowth)
            {
                case ExpGrowthClass.Fast:
                    valeurV�rit� = (Exp >= (int)(0.8 * Math.Pow(levelSuivant, 3)));
                    break;
                case ExpGrowthClass.MediumFast:
                    valeurV�rit� = (Exp >= (int)Math.Pow(levelSuivant, 3));
                    break;
                case ExpGrowthClass.MediumSlow:
                    valeurV�rit� = (Exp >= (int)(1.2 * Math.Pow(levelSuivant, 3) - 15 * Math.Pow(levelSuivant, 2) + 100 * levelSuivant - 140));
                    break;
                case ExpGrowthClass.Slow:
                    valeurV�rit� = (Exp >= (int)(1.25 * Math.Pow(levelSuivant, 3)));
                    break;
            }
            return valeurV�rit�;


            //if (ExpGrowth == ExpGrowthClass.Fast.ToString()) //Changer pour un switch case?
            //    valeurV�rit� = (Exp >= (int)(0.8 * Math.Pow(levelSuivant, 3)));

            //else if (ExpGrowth == ExpGrowthClass.MediumFast.ToString())//Est-ce que ToString ram�ne le string ou le nombre en string?
            //    valeurV�rit� = (Exp >= (int)Math.Pow(levelSuivant, 3));

            //else if (ExpGrowth == ExpGrowthClass.MediumSlow.ToString())
            //    valeurV�rit� = (Exp >= (int)(1.2 * Math.Pow(levelSuivant, 3) - 15 * Math.Pow(levelSuivant, 2) + 100 * levelSuivant - 140));

            //else if (ExpGrowth == ExpGrowthClass.Slow.ToString())
            //    valeurV�rit� = (Exp >= (int)(1.25 * Math.Pow(levelSuivant, 3)));
        }

        public void SetStatus(int value)
        {
            Status = (Status)value;
        }
        public void SetStatus(string value)
        {
            Status = (Status)Enum.Parse(typeof(Status), value.Trim().ToUpper());
        }
        public Status GetStatus()
        {
            return Status;
        }

        public void AjouterHP(int value)//Effet d'un item ou d'une attaque
        {
            HP += value;
            if (HP > MaxHp)
                HP = MaxHp;
        }

        public void FullRestore() //par un item ou par pokemon center
        {
            AjouterHP(MaxHp);
            R�tablirStats();
            SetStatus("null");
        }

        public override void Initialize()
        {
            G�n�rateur = Game.Services.GetService(typeof(Random)) as Random;
            Database = Game.Services.GetService(typeof(AccessBaseDeDonn�e)) as AccessBaseDeDonn�e;
            Database = new AccessBaseDeDonn�e();

            PokemonEnString = Database.AccessDonn�esPokemonStats(PokedexNumber);

            ExpGrowth = (ExpGrowthClass)Enum.Parse(typeof(ExpGrowthClass), PokemonEnString[11]);

            BaseExp = int.Parse(PokemonEnString[12]);
            NiveauEvolution = int.Parse(PokemonEnString[13]);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
