using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;


namespace AtelierXNA.Classes_Pokemon_Skyrim
{
    class AccessBaseDeDonnée
    {
        private OleDbConnection connection { get; set; }
        private OleDbCommand commande { get; set; }
        List<List<string>> Pokemons { get; set; }
        List<string> Name { get; set; }
        List<int> Attack { get; set; }
        List<int> Catch { get; set; }
        List<int> Hp { get; set; }
        List<int> Defense { get; set; }
        List<int> SpecialDefense { get; set; }
        List<int> SpecialAttack { get; set; }
        List<int> speed { get; set; }
        List<string> PokemonType { get; set; }
        List<string> PokemonType2 { get; set; }
        List<int> Number { get; set; }
        List<string> Renvoit { get; set; }
        List<int> TypeNumber { get; set; }
        int[,] ArrayWeakness { get; set; }
        int [,] TypeLevelAttack { get; set; }

        public AccessBaseDeDonnée()
        {
            InitialiserListes();
            InitialiserConnection();
            LireTablePokemons();
            LireTableWeaknessStrengh();
            LireTableTypeLevelAttack();
            RemplirListUltime();
        }
        private void InitialiserListes()
        {
            Name = new List<string>();
            Number = new List<int>();
            Attack = new List<int>();
            SpecialAttack = new List<int>();
            SpecialDefense = new List<int>();
            speed = new List<int>();
            Defense = new List<int>();
            Hp = new List<int>();
            Catch = new List<int>();
            PokemonType = new List<string>();
            PokemonType2 = new List<string>();
            Renvoit = new List<string>();
            Pokemons = new List<List<string>>();
            TypeNumber = new List<int>();
            ArrayWeakness = new int[17,17];
            TypeLevelAttack =  new int [,]; 
        }
        private void InitialiserConnection()
        {
            connection = new OleDbConnection();
            connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\201554646\Desktop\Base-de-données-Pokemon.accdb;
                                            Persist Security Info=False;";
            connection.Open();
            commande = new OleDbCommand();
            commande.Connection = connection;
        }
        private void LireTablePokemons()
        {
            commande.CommandText = "SELECT * FROM [pokemon]";
            OleDbDataReader reader = commande.ExecuteReader();
            while (reader.Read())
            {
                Number.Add(reader.GetInt32(0));
                Name.Add(reader.GetString(1));
                Hp.Add(reader.GetInt16(2));
                Attack.Add(reader.GetInt16(3));
                Defense.Add(reader.GetInt16(4));
                SpecialAttack.Add(reader.GetInt16(5));
                SpecialDefense.Add(reader.GetInt16(6));
                speed.Add(reader.GetInt16(7));
                PokemonType.Add(reader.GetString(8));
                PokemonType2.Add(reader.GetString(9));
                Catch.Add(reader.GetInt32(10));
            }
            reader.Close();
        }
        private void LireTableWeaknessStrengh()
        {
            commande.CommandText = "SELECT * FROM [Array Weakness and strengh]";
            OleDbDataReader reader = commande.ExecuteReader();
            int row = 0;
            while (reader.Read())
            {
                ArrayWeakness[row, 0] = reader.GetInt32(2);
                ArrayWeakness[row, 1] = reader.GetInt32(3);
                ArrayWeakness[row, 2] = reader.GetInt32(4);
                ArrayWeakness[row, 3] = reader.GetInt32(5);
                ArrayWeakness[row, 4] = reader.GetInt32(6);
                ArrayWeakness[row, 5] = reader.GetInt32(7);
                ArrayWeakness[row, 6] = reader.GetInt32(8);
                ArrayWeakness[row, 7] = reader.GetInt32(9);
                ArrayWeakness[row, 8] = reader.GetInt32(10);
                ArrayWeakness[row, 9] = reader.GetInt32(11);
                ArrayWeakness[row, 10] = reader.GetInt32(12);
                ArrayWeakness[row, 11] = reader.GetInt32(13);
                ArrayWeakness[row, 12] = reader.GetInt32(14);
                ArrayWeakness[row, 13] = reader.GetInt32(16);
                ArrayWeakness[row, 14] = reader.GetInt32(17);
                ArrayWeakness[row, 15] = reader.GetInt32(18);
                ArrayWeakness[row, 16] = reader.GetInt32(19);
                ArrayWeakness[row, 17] = reader.GetInt32(20);
                row++;
            }
            reader.Close();
        }
        private void RemplirListUltime()
        {            for(int i = 0; i< Catch.Count; i++)
            {
                List<string> Tempo = new List<string>();
                Tempo.Add(Number[i].ToString());
                Tempo.Add(Name[i]);
                Tempo.Add(Attack[i].ToString());
                Tempo.Add(Defense[i].ToString());
                Tempo.Add(SpecialAttack[i].ToString());
                Tempo.Add(SpecialDefense[i].ToString());
                Tempo.Add(speed[i].ToString());
                Tempo.Add(PokemonType[i]);
                Tempo.Add(PokemonType2[i]);
                Tempo.Add(Catch[i].ToString());

                Pokemons.Add(Tempo);
            }
        }
        public int AccessDonnéesArrayWeaknessStrengh(int row,  int col)
        {
            return ArrayWeakness[row,col];
        }
        public List<string> AccessDonnéesPokemonStats(int PokedexNumber)
        {
            return Pokemons[PokedexNumber - 1];
        }
    }
}
