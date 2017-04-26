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

        public string Nom => PokemonEnString[1];//C'est le quel pour le nom? (mais surtout, où est passée la database??)
        AccessBaseDeDonnée Database { get; set; }
        Random Générateur { get; set; }
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

        List<int> AttaquesList { get; set; }//Liste de 4 int référant à un numéro d'attaque
        bool EstSauvage { get; set; }
        public string Type1 => PokemonEnString[8];
        public string Type2 => PokemonEnString[9];


        public Pokemon_B(Game game, int pokedexNumber, int level, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            PokedexNumber = pokedexNumber;
            Level = level;

            AttaquesList = new List<int>();
            AttaquesList.Add(28);
            AttaquesList.Add(29);//Négatif siginife aucune attaque
            AttaquesList.Add(-1);
            AttaquesList.Add(-1);
            EstSauvage = false;
            Status = Status.NULL;
            //au lieu de juste mettre l'attaque 0, on pourrait mettre aléatoirement parmi les attaques disponibles du pokémon au niveau mentionné 
            Database = new AccessBaseDeDonnée();
            PokemonEnString = Database.AccessDonnéesPokemonStats(pokedexNumber);
            CalculerStatsEtHP(Level);//AccessBaseDeDonnées pour remplir les valeurs de stats du pokémon selon son pokedex number et niveau
        }
        public Pokemon_B(Game game, int pokedexNumber, int level, List<int> attaques, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(game, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            PokedexNumber = pokedexNumber;
            Level = level;


            PokemonEnString = new List<string>();
            AttaquesList = new List<int>(attaques);

            EstSauvage = false;
            Status = Status.NULL;
            CalculerStatsEtHP(Level);//AccessBaseDeDonnées pour remplir les valeurs de stats du pokémon selon son pokedex number et niveau
        }

        void CalculerStatsEtHP(int level)//Refaire à chaque level up, a faire lorsque Access bien implémenté
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
            RétablirStats();
        }

        void ChangerPokedexNumber(int nouveauPkdexNumber)// à faire si on évolue
        {
            PokedexNumber = nouveauPkdexNumber;
            PokemonEnString = Database.AccessDonnéesPokemonStats(PokedexNumber);
        }

        public int Attaquer()//Temp, on va retourner le numéro de l'attaque, pas le stat "Attack"
        {
            return Attack;
        }
        public void Défendre(int pointsDeDamage)
        {
            HP = HP - pointsDeDamage;
            //si la vie est à zéro, mettre EstEnVie à zéro
            if (HP <= 0)
            {
                EstEnVie = false; //Hérité de Vivant, mis public dans vivant
                HP = 0;
            }
        }
        public int AttaqueAléatoire()//Temp
        {
            int attaqueAléatoire = 0;
            int index = 0;

            do
            {
                index = Générateur.Next(0, 4);
                attaqueAléatoire = AttaquesList[index];
            }
            while (AttaquesList[index] < 0);

            return attaqueAléatoire;
        }


        public void RétablirStats()//Après chaque combat (et level up) TRÈS IMPORTANT
        {
            Stats = new List<int>(StatsFixes);
        }

        public void GainExp(int valeur)
        {
            Exp = Exp + valeur;
            //si Exp dépasse un threshold, faire le level up : check if evolution, recalcul les stats, check if new move is learned
            if (Level < MAX_LEVEL && DoitLevelUp())
            {
                ExécuterSéquenceLevelUp();
            }
        }
        public int GiveExp()
        {
            float a = 1;

            if (!EstSauvage)
                a = 1.5f;
            return (int)((a * BaseExp * Level) / (7));
        }

        void ExécuterSéquenceLevelUp()
        {
            Level++;
            VérifierSiÉvolution(NiveauEvolution);
            VérifierSiNouvelleAttaqueApprise();
            CalculerStatsEtHP(Level);//inclu RétablirStats()
        }

        void VérifierSiÉvolution(int niveauÉvolution)
        {
            if ((niveauÉvolution > 0) && (Level >= niveauÉvolution))//if le niveau d'évolution a été atteint
            {
                ChangerPokedexNumber(PokedexNumber + 1);
                //Exécuter animation d'évolution?
            }

            else if (niveauÉvolution < 0 && (Level >= NIVEAU_EVOLUTION_EEVEE))//Gestion du cas spécial Eevee ici
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
        void VérifierSiNouvelleAttaqueApprise()//? wut
        {

        }

        bool DoitLevelUp() //Selon les polynômes de Pokemon, comment on nommerait ces constantes si on devait en faire?
        {
            bool valeurVérité = false;
            int levelSuivant = Level + 1;

            switch (ExpGrowth)
            {
                case ExpGrowthClass.Fast:
                    valeurVérité = (Exp >= (int)(0.8 * Math.Pow(levelSuivant, 3)));
                    break;
                case ExpGrowthClass.MediumFast:
                    valeurVérité = (Exp >= (int)Math.Pow(levelSuivant, 3));
                    break;
                case ExpGrowthClass.MediumSlow:
                    valeurVérité = (Exp >= (int)(1.2 * Math.Pow(levelSuivant, 3) - 15 * Math.Pow(levelSuivant, 2) + 100 * levelSuivant - 140));
                    break;
                case ExpGrowthClass.Slow:
                    valeurVérité = (Exp >= (int)(1.25 * Math.Pow(levelSuivant, 3)));
                    break;
            }
            return valeurVérité;


            //if (ExpGrowth == ExpGrowthClass.Fast.ToString()) //Changer pour un switch case?
            //    valeurVérité = (Exp >= (int)(0.8 * Math.Pow(levelSuivant, 3)));

            //else if (ExpGrowth == ExpGrowthClass.MediumFast.ToString())//Est-ce que ToString ramène le string ou le nombre en string?
            //    valeurVérité = (Exp >= (int)Math.Pow(levelSuivant, 3));

            //else if (ExpGrowth == ExpGrowthClass.MediumSlow.ToString())
            //    valeurVérité = (Exp >= (int)(1.2 * Math.Pow(levelSuivant, 3) - 15 * Math.Pow(levelSuivant, 2) + 100 * levelSuivant - 140));

            //else if (ExpGrowth == ExpGrowthClass.Slow.ToString())
            //    valeurVérité = (Exp >= (int)(1.25 * Math.Pow(levelSuivant, 3)));
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
            RétablirStats();
            SetStatus("null");
        }

        public override void Initialize()
        {
            Générateur = Game.Services.GetService(typeof(Random)) as Random;
            Database = Game.Services.GetService(typeof(AccessBaseDeDonnée)) as AccessBaseDeDonnée;
            Database = new AccessBaseDeDonnée();

            PokemonEnString = Database.AccessDonnéesPokemonStats(PokedexNumber);

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
