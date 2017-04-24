//Gabriel Paquette, 3 Avril 2017
//Modifications 3 Avril 2017: créé, ajout de la fonction EncoderASCIIEnFont
//              5 Avril 2017: Ajout de la fonction ConvertirStringEnListeInt (comme ça tout ce qui a lien avec l'encodage est ici, un seul endroit)
//État: Fonctionnel, tout les "if" dans l'encodage sont vraiment affreux
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA
{
    public static class EncodeurFont
    {

        public static List<int> ConvertirStringEnListeInt(string message)
        {
            List<int> messageListeInt = new List<int>();
            char[] tab = message.ToCharArray();
            byte [] messageBytes = new byte[message.Length - 1];
            messageBytes = Encoding.ASCII.GetBytes(tab);
            foreach (byte b in messageBytes)
            {
                messageListeInt.Add(EncoderASCIIEnFont(b));//On prend les char en ASCII, on met les numéros des caractères (dec) pour qu'y soient équivalents aux cases de la font.
            }
            return messageListeInt;
        }
        public static int EncoderASCIIEnFont(byte b)
        {
            int noDeTile = b;
            //Première ligne de la texture de la police
            if (b >= 97 && b <= 122) //Les minuscules
                noDeTile = b - 97;
            if (b == 32)
                noDeTile = b - 6; //l'espace
            if (b >= 65 && b <= 90) //Les majuscules
                noDeTile = b - 38;

            //Deuxième ligne
            if (b >= 49 && b <= 57)//de 1 à 9
                noDeTile = b + 3;
            else if (b == 48)//0
                noDeTile = b + 13;
            else if (b == 46)//.
                noDeTile = b + 16;
            else if (b == 58)//:
                noDeTile = b + 5;
            else if (b == 44)//,
                noDeTile = b + 20;
            else if (b == 59)//;
                noDeTile = b + 6;
            else if (b == 39)//'
                noDeTile = b + 28;
            else if (b == 40 || b == 47)//( ou /
                noDeTile = b + 30;
            else if (b == 33)//!
                noDeTile = b + 38;
            else if (b == 63)//?
                noDeTile = b + 9;
            else if (b == 41 || b == 43)//) ou + 
                noDeTile = b + 32;
            else if (b == 45)// -
                noDeTile = b + 31;
            else if (b == 61)// =
                noDeTile = b + 17;
            else if (b == 60 || b == 62)// é, mais qu'on écrit < ou à mais qu'on écrit >
                noDeTile = b + 19;

            //(apparamment les accents sont pas pris en compte par l'encodeur ascii de visual studio...)

            return noDeTile;
        }
    }
}
