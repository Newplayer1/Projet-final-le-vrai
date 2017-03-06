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

namespace AtelierXNA.Classes_Pokemon_Skyrim
{
    public class Pokemon : Vivant
    {
        const int MAX_POKEDEX_NUMBER = 151;
        const int MIN_POKEDEX_NUMBER = 0;
        const int MAX_NIVEAU = 100;
        const int MIN_NIVEAU = 0;

        /*    
 *  doit storer:
 *  - stats d'un pokemon en fct du niveau (recalcul� si level up)
 *  - bool pok�mon appartient au user ou non
 *  - copie des stats (que combat va overwrite pour modifier les stats d'un pkmn)
 *  - list pour storer les 4 attaques
 *  - type (vector2 qu'on va caster en int)
 *  - pts de vie (modifiable si pkmn center)
 *  - pts exp (modifiable par combat.cs)
 *  - status (brn, frz, slp, psn, par)
 *  - fonction attaquer ((seulement si pokemon du user, donc pas wild ou opponent)qui permet de prendre une des quatres attaques, retourne l'attaque choisie, fonction appel�e par combat.cs sour la forme "User.Attack")
 *  - fonction d�fense (qui recoit les dommages calcul�s par combat.cs selon le type et attaques)
 *  - fonction riposter ((seulement si adverse) choisi une attaque al�atoire dans la liste d'attaques, appel� par combat "Opponent.Riposte" )
 *  - fonction �voluer (s�quentiel)
 *  - fonction level up si exp atteint level complet: check if evolution, recalcul les stats, check if new move is learned
 *    */

            
        List<string> TypesRecu;
        List<string> TypesOriginals;



        int PokedexNumber { get; set; }
        
        float poids;
        string type1;
        string type2;
        string nomSprite;

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
 




        public Pokemon(Game game, int pokedexNumber, int level)
            : base(game)
        {
            PokedexNumber = pokedexNumber;
            Level = level;

            AttaquesList = new List<int>();
            AttaquesList.Add(0);
            AttaquesList.Add(-1);//N�gatif siginife aucune attaque
            AttaquesList.Add(-1);
            AttaquesList.Add(-1);

            //au lieu de juste mettre l'attaque 0, on pourrait mettre al�atoirement parmi les attaques disponibles du pok�mon au niveau mentionn� 

            CalculerStatsEtHP(PokedexNumber, Level);//AccessBaseDeDonn�es pour remplir les valeurs de stats du pok�mon selon son pokedex number et niveau
        }
        public Pokemon(Game game, int pokedexNumber, int level, List<int> attaques)
            : base(game)
        {
            PokedexNumber = pokedexNumber;
            Level = level;
            AttaquesList = new List<int>(attaques);

            CalculerStatsEtHP(PokedexNumber, Level);//AccessBaseDeDonn�es pour remplir les valeurs de stats du pok�mon selon son pokedex number et niveau
        }

        void CalculerStatsEtHP(int pokedexNumber, int level)//inclu HP
        {
            //AccessBaseDeDonn�es pour remplir les valeurs de stats du pok�mon selon son pokedex number et niveau
            //Utiliser la formule de calcul des stats et HP en fonction du base stat et du niveau
            //Remplir la liste Stats, mais surtout StatsFixes

            //  Stat = (2 * (BaseStat + 2) * Level)/100 + 5
            //  HP = (2 * (BaseStat + 2) * Level)/100 + Level + 10


        }

        public int Attaquer()//Temp
        {
            return Attack;
        }
        public void D�fendre(int pointsDeDamage)
        {
            HP = HP - pointsDeDamage;
            //si la vie est � z�ro, mettre EstEnVie � z�ro
            if (HP <= 0)
            {
                EstEnVie = false; //H�rit� de Vivant, mis protected dans vivant
                HP = 0;
            }
        }
        public int AttaqueAl�atoire()//Temp
        {
            return Attack;
        }


        public void R�tablirStats()//Apr�s chaque combat TR�S IMPORTANT
        {
            Stats = StatsFixes;
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
