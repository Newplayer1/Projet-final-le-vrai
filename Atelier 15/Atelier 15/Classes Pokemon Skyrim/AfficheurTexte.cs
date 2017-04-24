//Gabriel Paquette, Mars 2017
//Modifications 2 Avril 2017: Complet, plusieurs bug corrig�s (d�but de ligne par un espace, d�filement des lignes en fonction de la boite)
//              5 Avril 2017: Adapt� aux modifications de EncodeurFont
//�tat: Fonctionnel, non nettoy�, on devrait peut-�tre augmenter la qualit� de la police/cadre, le code est un peu dur � suivre par moments, peut-�tre renommer ou faire des propri�t�s de second ordre et/ou des pr�dicats
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
        public bool Priorit� { get; set; }
        string NomPolice { get; set; }
        protected Texture2D TexturePolice { get; private set; }
        List<Rectangle> RectangleSourcesPolice { get; set; }
        //byte[] MessageBytes { get; set; }
        List<int> MessageListInt { get; set; }

        int NBDeCaract�resParLigne => NBColonnes - 3; //Avant c'�tait - 2 mais - 3 pour laisser une colonne vide � la fin pour le clignotant
        int NBDeZonesDeTexte => (NBLignes - 2) / 2;//divis� par deux si double interligne


        int NBLettresAffich�es { get; set; }
        int IndexD�butAffichage { get; set; }
        int NoDeLigne�Afficher { get; set; }
        float IntervalMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        float Temps�coul�DepuisMAJClignotant { get; set; }
       
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
            //Message = "OAK: The quick brown fox jumps over the lazy dog! MAGIKARP used SPLASH! But nothing happened! (MAGITRASH) Enfin j'ai r<solu plusieurs bugs, c'est maintenant entierement g<n<rique au nb de la taille du cadre!! On ajoute le nb d'espaces n�cessaires pour amener le mot sur la prochaine ligne. Si le total du nb de lettres est plus grand que le nb de lettres rendu a la fin de la ligne j, et que les deux lettres sont pas �gal � deux caract�res de suite signifie qu'un mot a �t� coup�. Nb max de charact�re / mot = nbChar/ligne Augmenter le NBLettresAffich�es jusqu'au max de la boite. Ensuite, si l'on clique, il faudra afficher la derni�re ligne � la place de la premi�re et continuer l'affichage dans la boucle de temps";
            //Message = "He used FLAMETHROWER  ";
        }


        public override void Initialize()
        {
            RectangleSourcesPolice = new List<Rectangle>();
            NBLettresAffich�es = 0;
            Temps�coul�DepuisMAJ = 0;
            Temps�coul�DepuisMAJClignotant = 0;
            IndexD�butAffichage = 0;

            Priorit� = true;
            if (MessageEnCours)//s'il y a un message en cours, celui-ci n'as pas la priorit�
                Priorit� = false;
            else
                MessageEnCours = true;

            BattleMenu.Wait = true;

            NoDeLigne�Afficher = NBDeZonesDeTexte - 1;
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
            Cr�erListeTiles();
            Cr�erListeMessageInt();
        }


        void Cr�erListeTiles()
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

        void Cr�erListeMessageInt()
        {
            EncoderMessage();
        }

        void EncoderMessage()
        {
            MessageListInt = EncodeurFont.ConvertirStringEnListeInt(Message);

            if (NBDeCaract�resParLigne > 5 && MessageListInt.Count > NBDeCaract�resParLigne)
                ModifierPourPasCouperDeMot(MessageListInt);
        }



        void ModifierPourPasCouperDeMot(List<int> message)
        {
            for (int j = 1; j <= message.Count / (NBDeCaract�resParLigne); j++)
            {

                if (message.Count > NBDeCaract�resParLigne * j && (message[NBDeCaract�resParLigne * j] != 26 && message[NBDeCaract�resParLigne * j - 1] != 26)) //Si le total du nb de lettres est plus grand que le nb de lettres rendu a la fin de la ligne j, et que les deux lettres sont pas �gal � " ", deux caract�res de suite signifie qu'un mot a �t� coup�. Nb max de charact�re / mot = nbChar/ligne
                {
                    int index = message.LastIndexOf(26, NBDeCaract�resParLigne * j, NBDeCaract�resParLigne); //on trouve o� est le dernier espace 
                    int nbEspaceAjouter = NBDeCaract�resParLigne * j - index; //On calcule combien d'espaces qu'on doit ajouter pour que le mot coup� aille � la ligne suivante
                    for (int i = 0; i < nbEspaceAjouter - 1 && index >= 0; i++)
                    {
                        message.Insert(index, 26);//On ajoute le nb d'espaces n�cessaires pour amener le mot sur la prochaine ligne
                    }
                }
                if (message.Count > NBDeCaract�resParLigne * j && message[j * NBDeCaract�resParLigne] == 26)//Trim
                {
                    message.RemoveAt(j * NBDeCaract�resParLigne);
                }

            }
        }
        //void TrimD�butToutesLesLignes() //Enlever l'espace au d�but de la ligne
        //{
        //    for (int j = 1; j < MessageListInt.Count / NBDeCaract�resParLigne; j++)//tant que j est plus petit que le total des lettres divis� par le nb de lettres dans une ligne. (pourquoi - 2?)
        //    {
        //        if (MessageListInt.Count > NBDeCaract�resParLigne * j && MessageListInt[j * NBDeCaract�resParLigne] == 26) //si la lettre � la position d'un d�but de ligne est un espace
        //        {
        //            MessageListInt.RemoveAt(j * NBDeCaract�resParLigne + 1);//On retire cet espace
        //        }
        //    }
        //}


        public override void Update(GameTime gameTime)
        {
            //Augmenter le NBLettresAffich�es jusqu'au max de la boite. Ensuite, si l'on clique, il faudra afficher la derni�re ligne � la place de la premi�re et continuer l'affichage dans la boucle de temps

            if (Priorit�)
            {
                MessageEnCours = true;
                G�rerClavier();
                EffectuerMise�Jour(gameTime);
            }
                
            else
                Priorit� = !MessageEnCours; //tant qu'on a un message en cours, on ne veut pas avoir la priorit�

            
            base.Update(gameTime);
        }

        void G�rerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.A) && ((NBLettresAffich�es == MessageListInt.Count) || NBLettresAffich�es == NBDeCaract�resParLigne * (NoDeLigne�Afficher + 1))) //Si la condition du clignotant est remplie ou que l'on a fini d'afficher le message
            {
                if (NBLettresAffich�es == MessageListInt.Count)//Si toutes les lettres ont �t� affich�es, et que A est pes�, on supprime le message puisqu'il a rempli sa fonction
                {
                    MessageEnCours = false;
                    �D�truire = true;
                }
                //NBLettresAffich�es = NBDeCaract�resParLigne;//Permet d'�crire juste la ligne du dessous, juste elle est anim�e
                IndexD�butAffichage = NBLettresAffich�es - NBDeCaract�resParLigne;//NBDeCaract�resParLigne * NoDeLigne�Afficher;
                //if (MessageListInt.Count < NBLettresAffich�es)
                NoDeLigne�Afficher = NoDeLigne�Afficher + (NBDeZonesDeTexte - 1);//pas trop sur ici

                Clignotant = false;
            }
        }
        void EffectuerMise�Jour(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            Temps�coul�DepuisMAJClignotant += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalMAJ)
            {
                if (NBLettresAffich�es < MessageListInt.Count && !(NBLettresAffich�es == NBDeCaract�resParLigne * (NoDeLigne�Afficher + 1))/*NBLettresAffich�es < NBDeCaract�resParLigne * (NoDeLigne�Afficher + 1)*/) //On augmente le maximum de lettres � afficher tant qu'y en reste et tant que la boite n'est pas pleine
                    NBLettresAffich�es++;

                //if (NBLettresAffich�es == NBDeCaract�resParLigne * (NoDeLigne�Afficher + 1)) //Si la box est remplie (y doit avoir des espaces qui comblent le "vide"), on affiche le clignotant
                //    Clignotant = !Clignotant; //Mettre dans un autre intervalle de temps pour ralentir?

                Temps�coul�DepuisMAJ = 0;
            }

            if (Temps�coul�DepuisMAJClignotant >= INTERVALLE_MAJ_CLIGNOTANT)
            {
                if (NBLettresAffich�es == NBDeCaract�resParLigne * (NoDeLigne�Afficher + 1)) //Si la box est remplie (y doit avoir des espaces qui comblent le "vide"), on affiche le clignotant
                    Clignotant = !Clignotant; //Mettre 
                Temps�coul�DepuisMAJClignotant = 0;
            }
            //Clignotant = false;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Priorit�)
            {
                base.Draw(gameTime);
                DrawAfficheur();
            }
        }

        private void DrawAfficheur()
        {
            GestionSprites.Begin();

            int k = IndexD�butAffichage;
            for (int j = 1; j <= NBDeZonesDeTexte; j++)
            {
                DrawLigneTexte(j, ref k);
            }
            if (Clignotant)
                GestionSprites.Draw(TextureCadre, Positions[NBColonnes - 2, NBLignes - 2], RectangleSourcesCadre[9], Color.White);

            GestionSprites.End();
        }

        void DrawLigneTexte(int j, ref int k)//On �crit toutes les lettres une seule fois, donc on veut passer k par r�f�rence pour garder le compteur
        {
            for (int i = 1; i < NBDeCaract�resParLigne + 1 && k < NBLettresAffich�es && k >= 0; i++)
            {
                GestionSprites.Draw(TexturePolice, Positions[i, 2 * j], RectangleSourcesPolice[MessageListInt[k++]], Color.White);
            }//Pour le choix d'attaques, on met pas 2*j. Juste pour dialogues, skip une ligne sur deux donne l'effet d'origine
        }
    }
}