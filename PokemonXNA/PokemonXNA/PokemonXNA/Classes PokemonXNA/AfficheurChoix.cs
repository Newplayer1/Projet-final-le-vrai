//Gabriel Paquette, 3 Avril 2017
//Modifications 3 Avril 2017: cr��, ajout de NBCaract�resParLigne/NbDeZonesDeTexte et de la base du cadre
//              5 Avril 2017: Adapt� aux modifications de EncodeurFont, IndexS�lectionn� mis static
//              22 Avril 2017: Ajout d'une propri�t� index�e (comme �a si on demande UnAfficheurChoix[0], on renvoie la premi�re string du choix), et fonction pour modifier les choix
//�tat: En cours, non complet, ajouter fct pour add un choix? (implique de refaire le cadre...)
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
    public class AfficheurChoix : Cadre
    {
        const int POSITION_CURSEUR = 1;
        //public bool Priorit� { get; set; } remplacer par juste enabled/disabled
        int NBDeCaract�resParLigne => NBColonnes - 4; //toute la largeur moins le cadre (2), moins le curseur (1) pis moins l'espace entre curseur et cadre (1)
        int NBDeZonesDeTexte => NBLignes - 2; //Toute la hauteur moins le cadre (2) (dans AfficheurTexte on avait mis /2 pour que l'on skip une ligne)
        
        List<List<int>> MessagesEnInt { get; set; }
        List<string> MessagesEnString { get; set; }
        List<int> CurrentMessageInt { get; set; }

        public string this[int index]
        {
            get
            {
                return MessagesEnString[index]; //C'est correct sans new? parce qu'avec new �a fuck, string a pas de constructeur de copie
            }
        }

        string NomPolice { get; set; }
        Texture2D TexturePolice { get; set; }
        List<Rectangle> RectangleSourcesPolice { get; set; }
        byte[] MessageBytes { get; set; }
        int NoDeLigne�Afficher { get; set; }


        InputManager GestionInput { get; set; }

        public /*static*/ int IndexS�lectionn� { get; private set; }
        
        //static AfficheurChoix()
        //{
        //    IndexS�lectionn� = 0; //initialisation de l'index choisi � 0. 
        //    //Ne pas oublier: si l'on a un menu cascades (genre un afficheur choix qui apparait � cause d'un autre afficheur choix), il faudrait garder en backup l'ancien Index, sinon, si on fait back, on va revenir � l'ancien afficheurChoix mais avec un diff�rent index... (possibilit� de bug si on a un index trop haut dans l'afficheur que l'on vient de fermer compar� � l'afficheur pr�c�dent)
        //}

        public AfficheurChoix(Game game, Vector2 positionBox, int largeurBox, int hauteurBox, List<string> message, float intervalMAJ)
            : base(game, positionBox, largeurBox, hauteurBox)
        {
            MessagesEnString = new List<string>(message);//Utile?
            NomPolice = "Police848x32";
        }
        
        public override void Initialize()
        {
            IndexS�lectionn� = 0; //� garder ici, parce que si l'on cr�e un nouveau, on r�initialise l'index s�lectionn�
            NoDeLigne�Afficher = 0;
            //Priorit� = false;
            RectangleSourcesPolice = new List<Rectangle>();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;

            InitialiserMessagesEnInt();


            base.Initialize();
        }

        void InitialiserMessagesEnInt()
        {
            MessagesEnInt = new List<List<int>>();
            foreach (string message in MessagesEnString)//pour chaque message, on ajoute une liste de int
            {
                MessagesEnInt.Add(new List<int>());
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            TexturePolice = Game.Content.Load<Texture2D>("Textures/" + NomPolice);
            Cr�erListeTiles();
            Cr�erListesMessagesInt();
        }
        void Cr�erListeTiles()
        {
            RectangleSourcesPolice = EncodeurFont.Cr�erListeRectangleSources(TexturePolice, Cadre.TAILLE_TILE);
        }
        void Cr�erListesMessagesInt()
        {
            EncoderMessages();
        }
        void EncoderMessages()
        {
            int i = 0;
            foreach (string m in MessagesEnString)
            {
                string message = m;
                if (message.Count() > NBDeCaract�resParLigne)//un choix ne peut �tre plus long que la ligne.
                {
                    message = "?";
                }
                MessagesEnInt[i++] = EncodeurFont.ConvertirStringEnListeInt(message);
            }
        }
        public void ModifierChoix(List<string> nouveauxChoix)//Si un pok�mon level up lors d'un combat et apprend une nouvelle attaque, il faudra r�initialiser les choix pr�sents dans FightChoix
        {
            MessagesEnString = nouveauxChoix;
            InitialiserMessagesEnInt();
            Cr�erListesMessagesInt();
        }
        public override void Update(GameTime gameTime)
        {
            if (!AfficheurTexte.MessageEnCours)//S'il n'y a pas de message en cours, on g�re le clavier
                G�rerClavier();
            base.Update(gameTime);
        }
        void G�rerClavier()//fait juste choisir up/down, on devra peut-�tre ajouter left/right un jour
        {
            if ((GestionInput.EstNouvelleTouche(Keys.W) || GestionInput.EstNouveauUp_menucombat()) && IndexS�lectionn� > 0)
                IndexS�lectionn�--;
            if ((GestionInput.EstNouvelleTouche(Keys.S) || GestionInput.EstNouveauDown_menucombat()) && IndexS�lectionn� < NBDeZonesDeTexte - 1)
                IndexS�lectionn�++;

            //la classe qui utilise AfficheurChoix devra updater le choix fait selon l'IndexS�lectionn� (important d'�tre fait AVANT de cr�er un autre AfficheurChoix)
            //if (GestionInput.EstNouvelleTouche(Keys.A))
                //�D�truire = true; //?
        }

        public override void Draw(GameTime gameTime)
        {
            if (!AfficheurTexte.MessageEnCours)
            {
                base.Draw(gameTime);

            GestionSprites.Begin();
            NoDeLigne�Afficher = 0;
            
            for (int j = 1; j <= MessagesEnInt.Count; j++)
            {
                int k = 0;
                CurrentMessageInt = MessagesEnInt[NoDeLigne�Afficher];
                for (int i = POSITION_CURSEUR + 1; i < CurrentMessageInt.Count + POSITION_CURSEUR + 1 && i < NBColonnes - 1; i++)//on commence � �crire juste apr�s le cadre, l'espace et le curseur et entre l'autre cadre. On �crit pour la longueur du message
                {
                    GestionSprites.Draw(TexturePolice, Positions[i, j], RectangleSourcesPolice[CurrentMessageInt[k++]], Color.White);
                }
                NoDeLigne�Afficher++;
            }

            GestionSprites.Draw(TextureCadre, Positions[POSITION_CURSEUR, IndexS�lectionn� + 1], RectangleSourcesCadre[10], Color.White);
            GestionSprites.End();
            }

        }
    }
}
