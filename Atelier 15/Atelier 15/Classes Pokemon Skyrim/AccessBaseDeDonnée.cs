// Classe pour lire la Base de données de pokémons à partir de Access
// Antoine Bourassa
//
// Fini le 15 mai 2017

using System.Collections.Generic;
using System.Data.OleDb;


namespace AtelierXNA
{
    internal class AccessBaseDeDonnée
    {
        private OleDbConnection connection { get; set; }
        private OleDbCommand commande { get; set; }
        List<List<string>> Pokemons { get; set; }
        List<List<int>> Weakness { get; set; }
        List<List<string>> TypeLevelAttaque { get; set; }
        List<List<string>> Attaque { get; set; }
        /// <summary>
        /// Constructeur qui lit les données de la base de données
        /// </summary>
        public AccessBaseDeDonnée()
        {
            InitialiserListes();
            InitialiserConnection();
            LireTablePokemons();
            LireTableWeaknessStrengh();
            LireTableTypeLevelAttack();
            LireTableAttack();
        }
        /// <summary>
        /// Création des objets de type List
        /// Je fait des list de list pour avoir l'effet d'un tableau
        /// </summary>
        private void InitialiserListes()
        {
            Pokemons = new List<List<string>>();
            Weakness = new List<List<int>>();
            TypeLevelAttaque = new List<List<string>>();
            Attaque = new List<List<string>>();
        }
        private void InitialiserConnection()
        {
            connection = new OleDbConnection();
            connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source= ../../../../../Base-de-données-Pokemon.accdb;
                                            Persist Security Info=False;";
            connection.Open();
            commande = new OleDbCommand();
            commande.Connection = connection;
        }
        /// <summary>
        /// Méthode de sauvegarde pour la position du joueur et les pokémons qu'il a sur lui
        /// Malheureusement, je n'ai pas réussi à faire cela générique, il faut donc savoir 
        /// que'est ce qu'il y a dans la list à sauvegarder pour sauvegarder
        /// </summary>
        /// <param name="ÉlémentsASauvegarder">list que la méthode recoit</param>
        public void Sauvegarder(List<string> ÉlémentsASauvegarder)
        {
            commande.CommandText = "Update Sauvegarde set PositionX = '" + ÉlémentsASauvegarder[0] +
                "',PositionY = '" + ÉlémentsASauvegarder[1] + "',PositionZ = '" + ÉlémentsASauvegarder[2] +

                "',Pokemonint1 = '" + ÉlémentsASauvegarder[3]  + "',PokemonLevel1 = '" + ÉlémentsASauvegarder[4] + 
                "' ,Pokemonint2 = '" + ÉlémentsASauvegarder[5] + "',PokemonLevel2 = '" + ÉlémentsASauvegarder[6] + 
                "',Pokemonint3 = '" + ÉlémentsASauvegarder[7] + "',PokemonLevel3 = '" + ÉlémentsASauvegarder[8] + 
                "',Pokemonint4 = '" + ÉlémentsASauvegarder[9] + "',PokemonLevel4 = '" + ÉlémentsASauvegarder[10] + 
                "',Pokemonint5 = '" + ÉlémentsASauvegarder[11] + "',PokemonLevel5 = '" + ÉlémentsASauvegarder[12] + 
                "',Pokemonint6 = '" + ÉlémentsASauvegarder[13] + "',PokemonLevel6 = '" + ÉlémentsASauvegarder[14] + "'";
            commande.ExecuteNonQuery(); // commande qui fait exécute la séquence texte que je lui est dit plus haut
        }
        /// <summary>
        /// Cette méthode lit tous les données et les ramene dans une list qu'elle renvoit
        /// Elle est public donc peut et doit être appeler par le constructeur de load game dans jeu
        /// </summary>
        /// <returns>La list ayant la positions et les pokémons du joueur qu'il avait sauvegarder précédemment</returns>
        public List<string> LoadSauvegarde()
        {
            commande.CommandText = "SELECT * FROM [Sauvegarde]"; // sélectionne la table à lire
            OleDbDataReader reader = commande.ExecuteReader();
            List<string> Tempo = new List<string>();
            while (reader.Read())
            {
                Tempo.Add(reader.GetString(0));
                Tempo.Add(reader.GetString(1));
                Tempo.Add(reader.GetString(2));
                Tempo.Add(reader.GetString(3));
                Tempo.Add(reader.GetString(4));
                Tempo.Add(reader.GetString(5));
                Tempo.Add(reader.GetString(6));
                Tempo.Add(reader.GetString(7));
                Tempo.Add(reader.GetString(8));
                Tempo.Add(reader.GetString(9));
            }
            reader.Close();
            return Tempo;
        }
        /// <summary>
        /// lit la table dans Access de pokemons 
        /// </summary>
        private void LireTablePokemons()
        {
            commande.CommandText = "SELECT * FROM [pokemon]"; // sélectionne la table à lire
            OleDbDataReader reader = commande.ExecuteReader();
            while (reader.Read())
            {
                List<string> Tempo = new List<string>();

                Tempo.Add(reader.GetInt16(0).ToString());
                Tempo.Add(reader.GetString(1));
                Tempo.Add(reader.GetInt16(2).ToString());
                Tempo.Add(reader.GetInt16(3).ToString());
                Tempo.Add(reader.GetInt16(4).ToString());
                Tempo.Add(reader.GetInt16(5).ToString());
                Tempo.Add(reader.GetInt16(6).ToString());
                Tempo.Add(reader.GetInt16(7).ToString());
                Tempo.Add(reader.GetString(8));
                Tempo.Add(reader.GetString(9));
                Tempo.Add(reader.GetInt16(10).ToString());
                Tempo.Add(reader.GetString(11));
                Tempo.Add(reader.GetInt16(12).ToString());
                Tempo.Add(reader.GetInt16(13).ToString());

                Pokemons.Add(Tempo);
            }
            reader.Close();
        }
        /// <summary>
        /// même chose que que la méthode précédente sauf pour le tableau des faiblesses et des forces des type des pokemons 
        /// Ex : ghost contre normal = 0% 
        /// Ex : fire contre water = 50 % 
        /// Ex : water contre fire = 200%
        /// Ex : normal contre électrique = 100%
        /// </summary>
        private void LireTableWeaknessStrengh()
        {
            commande.CommandText = "SELECT * FROM [Array Weakness and strengh]"; // sélectionne la table à lire
            OleDbDataReader reader = commande.ExecuteReader();

            while (reader.Read())
            {
                List<int> Tempo = new List<int>();

                Tempo.Add(reader.GetInt16(2));
                Tempo.Add(reader.GetInt16(3));
                Tempo.Add(reader.GetInt16(4));
                Tempo.Add(reader.GetInt16(5));
                Tempo.Add(reader.GetInt16(6));
                Tempo.Add(reader.GetInt16(7));
                Tempo.Add(reader.GetInt16(8));
                Tempo.Add(reader.GetInt16(9));
                Tempo.Add(reader.GetInt16(10));
                Tempo.Add(reader.GetInt16(11));
                Tempo.Add(reader.GetInt16(12));
                Tempo.Add(reader.GetInt16(13));
                Tempo.Add(reader.GetInt16(14));
                Tempo.Add(reader.GetInt16(15));
                Tempo.Add(reader.GetInt16(16));
                Tempo.Add(reader.GetInt16(17));
                Tempo.Add(reader.GetInt16(18));
                Tempo.Add(reader.GetInt16(19));

                Weakness.Add(Tempo);
            }
            reader.Close();
        }
        /// <summary>
        /// lit pokemon appprendre attaque à une tel niveau
        /// </summary>
        private void LireTableTypeLevelAttack()
        {
            commande.CommandText = "SELECT * FROM [Lien pokemon et attaque]"; // sélectionne la table à lire
            OleDbDataReader reader = commande.ExecuteReader();

            while (reader.Read())
            {
                List<string> Tempo = new List<string>();

                Tempo.Add(reader.GetInt16(0).ToString());
                Tempo.Add(reader.GetString(1));
                Tempo.Add(reader.GetInt16(2).ToString());
                Tempo.Add(reader.GetInt16(3).ToString());
                Tempo.Add(reader.GetInt16(4).ToString());
                Tempo.Add(reader.GetInt16(5).ToString());

                TypeLevelAttaque.Add(Tempo);
            }
            reader.Close();
        }
        /// <summary>
        /// lit chaque attaque avec effets puissancwe et etc.
        /// </summary>
        private void LireTableAttack()
        {
            commande.CommandText = "SELECT * FROM [Lien # et attaque]"; // sélectionne la table à lire
            OleDbDataReader reader = commande.ExecuteReader();

            while (reader.Read())
            {
                List<string> Tempo = new List<string>();

                Tempo.Add(reader.GetString(0));
                Tempo.Add(reader.GetString(1));
                Tempo.Add(reader.GetString(2));
                Tempo.Add(reader.GetString(3));
                Tempo.Add(reader.GetString(4));
                Tempo.Add(reader.GetString(5));

                Tempo.Add(reader.GetString(6));
                Tempo.Add(reader.GetString(7));
                Tempo.Add(reader.GetString(8));
                Tempo.Add(reader.GetString(9));
                Tempo.Add(reader.GetString(10));

                Attaque.Add(Tempo);
            }
            reader.Close();
        }
        /// <summary>
        /// trouve l'attaque que le pokémons apprend a une niveau quelconque
        /// </summary>
        /// <param name="numéroDuType">genre null est numéro 0</param>
        /// <param name="level">le niveau ou le pokémons est rendu</param>
        /// <returns>l'attaque qu'il apprend</returns>
        public string AccessDonnéesTypeLevelAttaque(int numéroDuType, int level)
        //level 0 = 3 et 4; level 10 = 5; et level 25 = 6
        {
            return TypeLevelAttaque[numéroDuType - 1][level - 1].ToString();
        }
        /// <summary>
        /// trouve la ligne complet dont l'attaque que le pokémons apprend a une niveau quelconque
        /// </summary>
        /// <param name="numéroDuType">genre null est numéro 0</</param>
        /// <returns>la ligne au complet, pas juste l'attaque en string</returns>
        public List<string> AccessDonnéesTypeLevelAttaque(int numéroDuType)
        {
            return TypeLevelAttaque[numéroDuType - 1];//[level - 1].ToString();
        }
        /// <summary>
        /// méthode qui fait juste éviter les brient d'encapsulation
        /// </summary>
        /// <param name="row"> la ligne nécessaire (donc le numéro du type de l'attque) Ex: null = 0</param>
        /// <returns> la ligne pour le type entrer</returns>
        public List<int> AccessDonnéesArrayWeaknessStrengh(int row)
        {
            return Weakness[row - 1];
        }
        /// <summary>
        /// même chose mais pour la ligne d'un pokémons 
        /// </summary>
        /// <param name="pokedexNumber"> le numéro du pokémon</param>
        /// <returns></returns>
        public List<string> AccessDonnéesPokemonStats(int pokedexNumber)
        {
            return Pokemons[pokedexNumber - 1];
        }
        /// <summary>
        /// même chose
        /// </summary>
        /// <param name="attaqueNumber"></param>
        /// <returns>la ligne des données pour une attaque</returns>
        public List<string> AccessDonnéesAttaqueStats(int attaqueNumber)
        {
            return Attaque[attaqueNumber - 1];
        }

        /// <summary>
        /// trouve la valeur en string 
        /// </summary>
        /// <param name="row">la ligne aka le type de l'attaquant</param>
        /// <param name="col"> la colonne aka le type recevant</param>
        /// <returns>le pourcentage que l'attaque a selon le type</returns>
        /// //Fonction access direct
        public int AccessDonnéesArrayWeaknessStrenghIndice(int row, int col)
        {
            return Weakness[row - 1][col - 1];
        }
    }
}