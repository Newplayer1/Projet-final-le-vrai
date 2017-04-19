﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.Sql;


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

        public AccessBaseDeDonnée()
        {
            InitialiserListes();
            InitialiserConnection();
            LireTablePokemons();
            LireTableWeaknessStrengh();
            LireTableTypeLevelAttack();
            LireTableAttack();
        }
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
            connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Antoine\Desktop\Base-de-données-Pokemon.accdb;
                                            Persist Security Info=False;";
            connection.Open();
            commande = new OleDbCommand();
            commande.Connection = connection;
        }

        public void Sauvegarder(List<string> ÉlémentsASauvegarder)
        {
            commande.CommandText = "Update Sauvegarde set PositionX = '" +ÉlémentsASauvegarder[0] + "',PositionY = '" + ÉlémentsASauvegarder[1] + "',PositionZ = '" + ÉlémentsASauvegarder[2] + "'";
            commande.ExecuteNonQuery();
        }
        public List<string> LoadSauvegarde()
        {
            commande.CommandText = "SELECT * FROM [Sauvegarde]";
            OleDbDataReader reader = commande.ExecuteReader();
            List<string> Tempo = new List<string>();
            while (reader.Read())
            {

                Tempo.Add(reader.GetString(0));
                Tempo.Add(reader.GetString(1));
                Tempo.Add(reader.GetString(2));
            }
            reader.Close();
            return Tempo;
        }
        private void LireTablePokemons()
        {
            commande.CommandText = "SELECT * FROM [pokemon]";
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
        private void LireTableWeaknessStrengh()
        {
            commande.CommandText = "SELECT * FROM [Array Weakness and strengh]";
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
        private void LireTableTypeLevelAttack()
        {
            commande.CommandText = "SELECT * FROM [Lien pokemon et attaque]";
            OleDbDataReader reader = commande.ExecuteReader();

            while (reader.Read())
            {
                List<string> Tempo = new List<string>();

                Tempo.Add(reader.GetInt16(0).ToString());
                Tempo.Add(reader.GetString(1));
                Tempo.Add(reader.GetString(2));
                Tempo.Add(reader.GetInt16(3).ToString());
                Tempo.Add(reader.GetInt16(4).ToString());

                TypeLevelAttaque.Add(Tempo);
            }
            reader.Close();
        }
        private void LireTableAttack()
        {
            commande.CommandText = "SELECT * FROM [Lien # et attaque]";
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
        public string AccessDonnéesTypeLevelAttaque(int numéroDuType, int level)
        //level 0 = 3 level 10 = 4 et level 25 = 5
        {
            return TypeLevelAttaque[numéroDuType - 1][level - 1].ToString();
        }
        public List<int> AccessDonnéesArrayWeaknessStrengh(int row)
        {
            return Weakness[row - 1];
        }
        public List<string> AccessDonnéesPokemonStats(int pokedexNumber)
        {
            return Pokemons[pokedexNumber - 1];
        }
        public List<string> AccessDonnéesAttaqueStats(int attaqueNumber)
        {
            return Attaque[attaqueNumber - 1];
        }


        //Fonction access direct
        public int AccessDonnéesArrayWeaknessStrenghIndice(int row, int col)
        {
            return Weakness[row - 1][col - 1];
        }
    }
}