//Gabriel Paquette, 3 Avril 2017
//Modifications 3 Avril 2017: créé, ajout de la fonction EncoderASCIIEnFont
//              5 Avril 2017: Ajout de la fonction ConvertirStringEnListeInt (comme ça tout ce qui a lien avec l'encodage est ici, un seul endroit)
//État: Fonctionnel, tout les "if" dans l'encodage sont vraiment affreux
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                noDeTile = b + 4;
            else if (b == 48)//0
                noDeTile = b + 14;
            else if (b == 46)//.
                noDeTile = b + 17;
            else if (b == 58)//:
                noDeTile = b + 6;
            else if (b == 44)//,
                noDeTile = b + 21;
            else if (b == 59)//;
                noDeTile = b + 7;
            else if (b == 39)//'
                noDeTile = b + 29;
            else if (b == 40 || b == 47)//( ou /
                noDeTile = b + 31;
            else if (b == 33)//!
                noDeTile = b + 39;
            else if (b == 63)//?
                noDeTile = b + 10;
            else if (b == 41 || b == 43)//) ou + 
                noDeTile = b + 33;
            else if (b == 45)// -
                noDeTile = b + 32;
            else if (b == 61)// =
                noDeTile = b + 18;
            else if (b == 60 || b == 62)// é, mais qu'on écrit < ou à mais qu'on écrit >
                noDeTile = b + 20;

            //(apparamment les accents sont pas pris en compte par l'encodeur ascii de visual studio...)

            return noDeTile;
        }

        public static List<Rectangle> CréerListeRectangleSources(Texture2D image, int tailleTile)
        {
            List<Rectangle> listeRetour = new List<Rectangle>();
            for (int j = 0; j < (image.Height / tailleTile); j++)
            {
                for (int i = 0; i < (image.Width / tailleTile); i++)
                {
                    listeRetour.Add(new Rectangle(tailleTile * i, tailleTile * j, tailleTile, tailleTile)); // A = 0; i = 0; j = 0 // A² = 1; i = 1; j = 0 // A³ = 2
                }
            }

            return listeRetour;
        }
    }
}
