//Gabriel Paquette, 3 Avril 2017
//Modifications 3 Avril 2017: créé, ajout de NBCaractèresParLigne/NbDeZonesDeTexte et de la base du cadre
//              5 Avril 2017: Adapté aux modifications de EncodeurFont, IndexSélectionné mis static
//              22 Avril 2017: Ajout d'une propriété indexée (comme ça si on demande UnAfficheurChoix[0], on renvoie la première string du choix), et fonction pour modifier les choix
//État: En cours, non complet, ajouter fct pour add un choix? (implique de refaire le cadre...)
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
        //public bool Priorité { get; set; } remplacer par juste enabled/disabled
        int NBDeCaractèresParLigne => NBColonnes - 4; //toute la largeur moins le cadre (2), moins le curseur (1) pis moins l'espace entre curseur et cadre (1)
        int NBDeZonesDeTexte => NBLignes - 2; //Toute la hauteur moins le cadre (2) (dans AfficheurTexte on avait mis /2 pour que l'on skip une ligne)
        
        List<List<int>> MessagesEnInt { get; set; }
        List<string> MessagesEnString { get; set; }
        List<int> CurrentMessageInt { get; set; }

        public string this[int index]
        {
            get
            {
                return MessagesEnString[index]; //C'est correct sans new? parce qu'avec new ça fuck, string a pas de constructeur de copie
            }
        }

        string NomPolice { get; set; }
        Texture2D TexturePolice { get; set; }
        List<Rectangle> RectangleSourcesPolice { get; set; }
        byte[] MessageBytes { get; set; }
        int NoDeLigneÀAfficher { get; set; }


        InputManager GestionInput { get; set; }

        public /*static*/ int IndexSélectionné { get; private set; }
        
        //static AfficheurChoix()
        //{
        //    IndexSélectionné = 0; //initialisation de l'index choisi à 0. 
        //    //Ne pas oublier: si l'on a un menu cascades (genre un afficheur choix qui apparait à cause d'un autre afficheur choix), il faudrait garder en backup l'ancien Index, sinon, si on fait back, on va revenir à l'ancien afficheurChoix mais avec un différent index... (possibilité de bug si on a un index trop haut dans l'afficheur que l'on vient de fermer comparé à l'afficheur précédent)
        //}

        public AfficheurChoix(Game game, Vector2 positionBox, int largeurBox, int hauteurBox, List<string> message, float intervalMAJ)
            : base(game, positionBox, largeurBox, hauteurBox)
        {
            MessagesEnString = new List<string>(message);//Utile?
            NomPolice = "Police848x32";
        }
        
        public override void Initialize()
        {
            IndexSélectionné = 0; //à garder ici, parce que si l'on crée un nouveau, on réinitialise l'index sélectionné
            NoDeLigneÀAfficher = 0;
            //Priorité = false;
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
            CréerListeTiles();
            CréerListesMessagesInt();
        }
        void CréerListeTiles()
        {
            RectangleSourcesPolice = EncodeurFont.CréerListeRectangleSources(TexturePolice, Cadre.TAILLE_TILE);
        }
        void CréerListesMessagesInt()
        {
            EncoderMessages();
        }
        void EncoderMessages()
        {
            int i = 0;
            foreach (string m in MessagesEnString)
            {
                string message = m;
                if (message.Count() > NBDeCaractèresParLigne)//un choix ne peut être plus long que la ligne.
                {
                    message = "?";
                }
                MessagesEnInt[i++] = EncodeurFont.ConvertirStringEnListeInt(message);
            }
        }
        public void ModifierChoix(List<string> nouveauxChoix)//Si un pokémon level up lors d'un combat et apprend une nouvelle attaque, il faudra réinitialiser les choix présents dans FightChoix
        {
            MessagesEnString = nouveauxChoix;
            InitialiserMessagesEnInt();
            CréerListesMessagesInt();
        }
        public override void Update(GameTime gameTime)
        {
            if (!AfficheurTexte.MessageEnCours)//S'il n'y a pas de message en cours, on gère le clavier
                GérerClavier();
            base.Update(gameTime);
        }
        void GérerClavier()//fait juste choisir up/down, on devra peut-être ajouter left/right un jour
        {
            if ((GestionInput.EstNouvelleTouche(Keys.W) || GestionInput.EstNouveauUp_menucombat()) && IndexSélectionné > 0)
                IndexSélectionné--;
            if ((GestionInput.EstNouvelleTouche(Keys.S) || GestionInput.EstNouveauDown_menucombat()) && IndexSélectionné < NBDeZonesDeTexte - 1)
                IndexSélectionné++;

            //la classe qui utilise AfficheurChoix devra updater le choix fait selon l'IndexSélectionné (important d'être fait AVANT de créer un autre AfficheurChoix)
            //if (GestionInput.EstNouvelleTouche(Keys.A))
                //ÀDétruire = true; //?
        }

        public override void Draw(GameTime gameTime)
        {
            if (!AfficheurTexte.MessageEnCours)
            {
                base.Draw(gameTime);

            GestionSprites.Begin();
            NoDeLigneÀAfficher = 0;
            
            for (int j = 1; j <= MessagesEnInt.Count; j++)
            {
                int k = 0;
                CurrentMessageInt = MessagesEnInt[NoDeLigneÀAfficher];
                for (int i = POSITION_CURSEUR + 1; i < CurrentMessageInt.Count + POSITION_CURSEUR + 1 && i < NBColonnes - 1; i++)//on commence à écrire juste après le cadre, l'espace et le curseur et entre l'autre cadre. On écrit pour la longueur du message
                {
                    GestionSprites.Draw(TexturePolice, Positions[i, j], RectangleSourcesPolice[CurrentMessageInt[k++]], Color.White);
                }
                NoDeLigneÀAfficher++;
            }

            GestionSprites.Draw(TextureCadre, Positions[POSITION_CURSEUR, IndexSélectionné + 1], RectangleSourcesCadre[10], Color.White);
            GestionSprites.End();
            }

        }
    }
}
