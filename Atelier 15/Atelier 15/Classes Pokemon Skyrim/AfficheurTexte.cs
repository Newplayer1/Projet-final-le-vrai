//Gabriel Paquette, Mars 2017
//Modifications 2 Avril 2017: Complet, plusieurs bug corrigés (début de ligne par un espace, défilement des lignes en fonction de la boite)
//              5 Avril 2017: Adapté aux modifications de EncodeurFont
//État: Fonctionnel, non nettoyé, on devrait peut-être augmenter la qualité de la police/cadre, le code est un peu dur à suivre par moments, peut-être renommer ou faire des propriétés de second ordre et/ou des prédicats
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
using System.Text;

namespace AtelierXNA
{

    public class AfficheurTexte : Cadre
    {
        const float INTERVALLE_MAJ_CLIGNOTANT = 45 / 60f;
        public bool Priorité { get; set; }
        string NomPolice { get; set; }
        protected Texture2D TexturePolice { get; private set; }
        List<Rectangle> RectangleSourcesPolice { get; set; }
        //byte[] MessageBytes { get; set; }
        List<int> MessageListInt { get; set; }

        int NBDeCaractèresParLigne => NBColonnes - 3; //Avant c'était - 2 mais - 3 pour laisser une colonne vide à la fin pour le clignotant
        int NBDeZonesDeTexte => (NBLignes - 2) / 2;//divisé par deux si double interligne


        int NBLettresAffichées { get; set; }
        int IndexDébutAffichage { get; set; }
        int NoDeLigneÀAfficher { get; set; }
        float IntervalMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float TempsÉcouléDepuisMAJClignotant { get; set; }
       
        InputManager GestionInput { get; set; }

        public static bool MessageEnCours { get; private set; }

        string Message { get; set; }
        static AfficheurTexte()
        {
            MessageEnCours = false;
        }

        public AfficheurTexte(Game game, Vector2 positionBox, int largeurBox, int hauteurBox, string message, float intervalMAJ)
            : base(game, positionBox, largeurBox, hauteurBox)
        {
            IntervalMAJ = intervalMAJ;
            NomPolice = "Police848x32";
            Message = message;
            //Message = "OAK: The quick brown fox jumps over the lazy dog! MAGIKARP used SPLASH! But nothing happened! (MAGITRASH) Enfin j'ai r<solu plusieurs bugs, c'est maintenant entierement g<n<rique au nb de la taille du cadre!! On ajoute le nb d'espaces nécessaires pour amener le mot sur la prochaine ligne. Si le total du nb de lettres est plus grand que le nb de lettres rendu a la fin de la ligne j, et que les deux lettres sont pas égal à deux caractères de suite signifie qu'un mot a été coupé. Nb max de charactère / mot = nbChar/ligne Augmenter le NBLettresAffichées jusqu'au max de la boite. Ensuite, si l'on clique, il faudra afficher la dernière ligne à la place de la première et continuer l'affichage dans la boucle de temps";
            //Message = "He used FLAMETHROWER  ";
        }


        public override void Initialize()
        {
            RectangleSourcesPolice = new List<Rectangle>();
            NBLettresAffichées = 0;
            TempsÉcouléDepuisMAJ = 0;
            TempsÉcouléDepuisMAJClignotant = 0;
            IndexDébutAffichage = 0;

            Priorité = true;
            if (MessageEnCours)//s'il y a un message en cours, celui-ci n'as pas la priorité
                Priorité = false;
            else
                MessageEnCours = true;

            BattleMenu.Wait = true;

            NoDeLigneÀAfficher = NBDeZonesDeTexte - 1;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            //MessageBytes = new byte[Message.Length - 1];
            MessageListInt = new List<int>();
            Clignotant = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            TexturePolice = Game.Content.Load<Texture2D>("Textures/" + NomPolice);
            CréerListeTiles();
            CréerListeMessageInt();
        }


        void CréerListeTiles()
        {
            for (int i = 0; i < (TexturePolice.Width / Cadre.TAILLE_TILE) - 1; i++)//Ligne A
            {
                RectangleSourcesPolice.Add(new Rectangle(Cadre.TAILLE_TILE * i, 0, Cadre.TAILLE_TILE, Cadre.TAILLE_TILE)); // A = 0; i = 0; j = 0
            }
            for (int i = 0; i < (TexturePolice.Width / Cadre.TAILLE_TILE) - 1; i++)//Ligne B
            {
                RectangleSourcesPolice.Add(new Rectangle(Cadre.TAILLE_TILE * i, Cadre.TAILLE_TILE, Cadre.TAILLE_TILE, Cadre.TAILLE_TILE)); // A = 0; i = 0; j = 0
            }
        }

        void CréerListeMessageInt()
        {
            EncoderMessage();
        }

        void EncoderMessage()
        {
            MessageListInt = EncodeurFont.ConvertirStringEnListeInt(Message);

            if (NBDeCaractèresParLigne > 5 && MessageListInt.Count > NBDeCaractèresParLigne)
                ModifierPourPasCouperDeMot(MessageListInt);
        }



        void ModifierPourPasCouperDeMot(List<int> message)
        {
            for (int j = 1; j <= message.Count / (NBDeCaractèresParLigne); j++)
            {

                if (message.Count > NBDeCaractèresParLigne * j && (message[NBDeCaractèresParLigne * j] != 26 && message[NBDeCaractèresParLigne * j - 1] != 26)) //Si le total du nb de lettres est plus grand que le nb de lettres rendu a la fin de la ligne j, et que les deux lettres sont pas égal à " ", deux caractères de suite signifie qu'un mot a été coupé. Nb max de charactère / mot = nbChar/ligne
                {
                    int index = message.LastIndexOf(26, NBDeCaractèresParLigne * j, NBDeCaractèresParLigne); //on trouve où est le dernier espace 
                    int nbEspaceAjouter = NBDeCaractèresParLigne * j - index; //On calcule combien d'espaces qu'on doit ajouter pour que le mot coupé aille à la ligne suivante
                    for (int i = 0; i < nbEspaceAjouter - 1 && index >= 0; i++)
                    {
                        message.Insert(index, 26);//On ajoute le nb d'espaces nécessaires pour amener le mot sur la prochaine ligne
                    }
                }
                if (message.Count > NBDeCaractèresParLigne * j && message[j * NBDeCaractèresParLigne] == 26)//Trim
                {
                    message.RemoveAt(j * NBDeCaractèresParLigne);
                }

            }
        }
        //void TrimDébutToutesLesLignes() //Enlever l'espace au début de la ligne
        //{
        //    for (int j = 1; j < MessageListInt.Count / NBDeCaractèresParLigne; j++)//tant que j est plus petit que le total des lettres divisé par le nb de lettres dans une ligne. (pourquoi - 2?)
        //    {
        //        if (MessageListInt.Count > NBDeCaractèresParLigne * j && MessageListInt[j * NBDeCaractèresParLigne] == 26) //si la lettre à la position d'un début de ligne est un espace
        //        {
        //            MessageListInt.RemoveAt(j * NBDeCaractèresParLigne + 1);//On retire cet espace
        //        }
        //    }
        //}


        public override void Update(GameTime gameTime)
        {
            //Augmenter le NBLettresAffichées jusqu'au max de la boite. Ensuite, si l'on clique, il faudra afficher la dernière ligne à la place de la première et continuer l'affichage dans la boucle de temps

            if (Priorité)
            {
                MessageEnCours = true;
                GérerClavier();
                EffectuerMiseÀJour(gameTime);
            }
                
            else
                Priorité = !MessageEnCours; //tant qu'on a un message en cours, on ne veut pas avoir la priorité

            
            base.Update(gameTime);
        }

        void GérerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.A) && ((NBLettresAffichées == MessageListInt.Count) || NBLettresAffichées == NBDeCaractèresParLigne * (NoDeLigneÀAfficher + 1))) //Si la condition du clignotant est remplie ou que l'on a fini d'afficher le message
            {
                if (NBLettresAffichées == MessageListInt.Count)//Si toutes les lettres ont été affichées, et que A est pesé, on supprime le message puisqu'il a rempli sa fonction
                {
                    MessageEnCours = false;
                    ÀDétruire = true;
                }
                //NBLettresAffichées = NBDeCaractèresParLigne;//Permet d'écrire juste la ligne du dessous, juste elle est animée
                IndexDébutAffichage = NBLettresAffichées - NBDeCaractèresParLigne;//NBDeCaractèresParLigne * NoDeLigneÀAfficher;
                //if (MessageListInt.Count < NBLettresAffichées)
                NoDeLigneÀAfficher = NoDeLigneÀAfficher + (NBDeZonesDeTexte - 1);//pas trop sur ici

                Clignotant = false;
            }
        }
        void EffectuerMiseÀJour(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            TempsÉcouléDepuisMAJClignotant += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalMAJ)
            {
                if (NBLettresAffichées < MessageListInt.Count && !(NBLettresAffichées == NBDeCaractèresParLigne * (NoDeLigneÀAfficher + 1))/*NBLettresAffichées < NBDeCaractèresParLigne * (NoDeLigneÀAfficher + 1)*/) //On augmente le maximum de lettres à afficher tant qu'y en reste et tant que la boite n'est pas pleine
                    NBLettresAffichées++;

                //if (NBLettresAffichées == NBDeCaractèresParLigne * (NoDeLigneÀAfficher + 1)) //Si la box est remplie (y doit avoir des espaces qui comblent le "vide"), on affiche le clignotant
                //    Clignotant = !Clignotant; //Mettre dans un autre intervalle de temps pour ralentir?

                TempsÉcouléDepuisMAJ = 0;
            }

            if (TempsÉcouléDepuisMAJClignotant >= INTERVALLE_MAJ_CLIGNOTANT)
            {
                if (NBLettresAffichées == NBDeCaractèresParLigne * (NoDeLigneÀAfficher + 1)) //Si la box est remplie (y doit avoir des espaces qui comblent le "vide"), on affiche le clignotant
                    Clignotant = !Clignotant; //Mettre 
                TempsÉcouléDepuisMAJClignotant = 0;
            }
            //Clignotant = false;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Priorité)
            {
                base.Draw(gameTime);
                DrawAfficheur();
            }
        }

        private void DrawAfficheur()
        {
            GestionSprites.Begin();

            int k = IndexDébutAffichage;
            for (int j = 1; j <= NBDeZonesDeTexte; j++)
            {
                DrawLigneTexte(j, ref k);
            }
            if (Clignotant)
                GestionSprites.Draw(TextureCadre, Positions[NBColonnes - 2, NBLignes - 2], RectangleSourcesCadre[9], Color.White);

            GestionSprites.End();
        }

        void DrawLigneTexte(int j, ref int k)//On écrit toutes les lettres une seule fois, donc on veut passer k par référence pour garder le compteur
        {
            for (int i = 1; i < NBDeCaractèresParLigne + 1 && k < NBLettresAffichées && k >= 0; i++)
            {
                GestionSprites.Draw(TexturePolice, Positions[i, 2 * j], RectangleSourcesPolice[MessageListInt[k++]], Color.White);
            }//Pour le choix d'attaques, on met pas 2*j. Juste pour dialogues, skip une ligne sur deux donne l'effet d'origine
        }
    }
}