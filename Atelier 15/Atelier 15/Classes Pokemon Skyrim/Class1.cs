using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;


namespace AtelierXNA.Classes_Pokemon_Skyrim
{
    class Class1
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
        List<string> Type { get; set; }
        List<string> Type2 { get; set; }
        List<int> Number { get; set; }
        List<string> Renvoit { get; set; }
        public Class1()
        {
            InitialiserListes();
            InitialiserConnection();
            LireBaseDonnées();
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
            Type = new List<string>();
            Type2 = new List<string>();
            Renvoit = new List<string>();
            Pokemons = new List<List<string>>();
        }
        private void InitialiserConnection()
        {
            connection = new OleDbConnection();
            connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\201554646\Desktop\Base-de-données-Pokemon.accdb;
                                            Persist Security Info=False;";
            connection.Open();
            commande = new OleDbCommand();
            commande.Connection = connection;
            commande.CommandText = "SELECT * FROM [pokemon]";
        }
        private void LireBaseDonnées()
        {
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
                Type.Add(reader.GetString(8));
                Type2.Add(reader.GetString(9));
                Catch.Add(reader.GetInt32(10));
            }
            reader.Close();
        }
        private void RemplirListUltime()
        {
           
            for(int i = 0; i< Catch.Count; i++)
            {
                List<string> Tempo = new List<string>();
                Tempo.Add(Number[i].ToString());
                Tempo.Add(Name[i]);
                Tempo.Add(Attack[i].ToString());
                Tempo.Add(Defense[i].ToString());
                Tempo.Add(SpecialAttack[i].ToString());
                Tempo.Add(SpecialDefense[i].ToString());
                Tempo.Add(speed[i].ToString());
                Tempo.Add(Type[i]);
                Tempo.Add(Type2[i]);
                Tempo.Add(Catch[i].ToString());

                Pokemons.Add(Tempo);
            }
        }
        public List<string> AccessDonnéesPokemonStatsComplet(int PokedexNumber)
        {
            return Pokemons[PokedexNumber - 1];
        }
        //public int AccessDonnéesPokemonStatsInt(int PokedexNumber, int col)
        //{
        //    CheckerIntÉgalAQuoiInt(col);
        //    return 
        //}
        //public string AccessDonnéesPokemonStatsString(int PokedexNumber, int col)
        //{
        //    CheckerIntÉgalAQuoiString(col);
        //    return 
        //}
        // private List<int> CheckerIntÉgalAQuoiInt(int col)
        //{

        //}
        //private List<string> CheckerIntÉgalAQuoiString(int col)
        //{
        //    List<string> retour = new List<string>();
        //    if(col  == 8)
        //    {
        //        retour = Type;
        //    }
        //    if (col == 1)
        //    {
        //        retour = Name;
        //    }
        //    else // 9
        //    {
        //        retour = Type2;
        //    }
        //    return retour;
        //}
    }
}
