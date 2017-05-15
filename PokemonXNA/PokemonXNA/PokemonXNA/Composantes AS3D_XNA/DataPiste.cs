using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System;

namespace AtelierXNA
{
    class DataPiste
    {
        const float LARGEUR_PISTE = 8;
        string NomFichierSplineX { get; set; }
        string NomFichierSplineY { get; set; }

        List<float> SplineX { get; set; }
        List<float> SplineY { get; set; }
        
        List<Vector2> PointsCube { get; set; }
        List<Vector2> PointsDePatrouille { get; set; }
        List<Vector2> PointsBordures { get; set; }
        List<Vector2> BordureExtérieure { get; set; }
        List<Vector2> BordureIntérieure { get; set; }
        
        public Vector2 PositionAvatar { get; private set; }


        public DataPiste(string nomFichierSplineX, string nomFichierSplineY)
        {
            NomFichierSplineX = nomFichierSplineX;
            NomFichierSplineY = nomFichierSplineY;

            SplineX = new List<float>();
            SplineY = new List<float>();
            
            CréerListes();
        }

        void CréerListes()
        {
            RemplirListe(SplineX, NomFichierSplineX);
            RemplirListe(SplineY, NomFichierSplineY);

            CréerPointCube();
            PositionAvatar = new Vector2(PointsCube[0].X, PointsCube[0].Y);

            CréerPointsPatrouille();
            CréerBordures();
        }

        void RemplirListe(List<float> liste, string nomFichier)
        {
            char[] séparateur = new char[] { '\t' };
            StreamReader streamReader = new StreamReader("Content/" + nomFichier);

            while (!streamReader.EndOfStream)
            {
                string ligneLue = streamReader.ReadLine();
                string[] champsLus = ligneLue.Split(séparateur);
                for (int i = 0; i < champsLus.Length; ++i)
                {
                    liste.Add(float.Parse(champsLus[i]));
                }
            }
        }

        void CréerPointCube()
        {
            PointsCube = new List<Vector2>();
            
            int t = 0;
            for (int i = 0; i < SplineX.Count - 3; i += 4)
            {
                float pointCubeX = SplineX[i] + SplineX[i + 1] * (float)Math.Pow(t, 3) + SplineX[i + 2] * (float)Math.Pow(t, 2) + SplineX[i + 3] * t;
                float pointCubeY = SplineY[i] + SplineY[i + 1] * (float)Math.Pow(t, 3) + SplineY[i + 2] * (float)Math.Pow(t, 2) + SplineY[i + 3] * t;

                PointsCube.Add(new Vector2(pointCubeX, pointCubeY) * 32f);
                ++t;
            }
        }

        void CréerBordures()
        {
            CréerPointsBordures();
            CréerBordureIntérieure();
            CréerBordureExtérieure();
        }

        void CréerBordureIntérieure()
        {
            BordureIntérieure = new List<Vector2>();

            Vector2 distance;
            for (int i = 0; i < PointsBordures.Count - 1; ++i)
            {
                if (i == PointsBordures.Count - 1)
                    distance = new Vector2(PointsBordures[0].X - PointsBordures[i].X, PointsBordures[0].Y - PointsBordures[i].Y);
                else
                    distance = new Vector2(PointsBordures[i + 1].X - PointsBordures[i].X, PointsBordures[i + 1].Y - PointsBordures[i].Y);

                Vector2 VecteurIntérieur = new Vector2(distance.Y, -distance.X);

                VecteurIntérieur.Normalize();
                VecteurIntérieur *= LARGEUR_PISTE;

                BordureIntérieure.Add(new Vector2(PointsBordures[i].X + VecteurIntérieur.X, PointsBordures[i].Y + VecteurIntérieur.Y));
            }
            BordureIntérieure.Add(BordureIntérieure[0]);
        }

        void CréerBordureExtérieure()
        {
            BordureExtérieure = new List<Vector2>();

            Vector2 distance;
            for (int i = 0; i < PointsBordures.Count - 1; ++i)
            {
                if (i == PointsBordures.Count - 1)
                    distance = new Vector2(PointsBordures[0].X - PointsBordures[i].X, PointsBordures[0].Y - PointsBordures[i].Y);
                else
                    distance = new Vector2(PointsBordures[i + 1].X - PointsBordures[i].X, PointsBordures[i + 1].Y - PointsBordures[i].Y);

                Vector2 VecteurExtérieur = new Vector2(-distance.Y, distance.X);
                
                VecteurExtérieur.Normalize();
                VecteurExtérieur *= LARGEUR_PISTE;
                
                BordureExtérieure.Add(new Vector2(PointsBordures[i].X + VecteurExtérieur.X, PointsBordures[i].Y + VecteurExtérieur.Y));
            }
            BordureExtérieure.Add(BordureExtérieure[0]);
        }
        
        void CréerPointsBordures()
        {
            PointsBordures = new List<Vector2>();
            int k = 0;
            for (int i = 0; i < SplineX.Count - 3; i += 4)
            {
                for (int j = 0; j < 20; ++j)
                {
                    float l = k + j / 20f;

                    float pointBordureX = SplineX[i] + SplineX[i + 1] * (float)Math.Pow(l, 3) + SplineX[i + 2] * (float)Math.Pow(l, 2) + SplineX[i + 3] * l;
                    float pointBordureY = SplineY[i] + SplineY[i + 1] * (float)Math.Pow(l, 3) + SplineY[i + 2] * (float)Math.Pow(l, 2) + SplineY[i + 3] * l;

                    PointsBordures.Add(new Vector2(pointBordureX, pointBordureY) * 32f);
                }
                ++k;
            }
        }

        void CréerPointsPatrouille()
        {
            PointsDePatrouille = new List<Vector2>();

            Vector2 delta;
            for (int i = 0; i < PointsCube.Count; ++i)
            {
                PointsDePatrouille.Add(PointsCube[i]);

                if (i == PointsCube.Count - 1) //Si on atteint le dernier, on va prendre le premier ([0])
                    delta = new Vector2((PointsCube[0].X - PointsCube[i].X), (PointsCube[0].Y - PointsCube[i].Y));
                else
                {
                    delta = new Vector2((PointsCube[i + 1].X - PointsCube[i].X), (PointsCube[i + 1].Y - PointsCube[i].Y));

                    //PointsDePatrouille.Add(Vector2.SmoothStep(PointsCube[i], PointsCube[i + 1], 1));
                    //PointsDePatrouille.Add(Vector2.SmoothStep(PointsCube[i], PointsCube[i + 1], 2));
                    //PointsDePatrouille.Add(Vector2.SmoothStep(PointsCube[i], PointsCube[i + 1], 3));

                    //PointsDePatrouille.Add(Vector2.Lerp(PointsCube[i], PointsCube[i + 1], 1));
                    //PointsDePatrouille.Add(Vector2.Lerp(PointsCube[i], PointsCube[i + 1], 2));
                    //PointsDePatrouille.Add(Vector2.Lerp(PointsCube[i], PointsCube[i + 1], 3));

                //Lerp et SmoothStep n'interpolent pas la courbe, ils ne font pas des points de courbe...
                }

                //Points "interpolés" (fonctionne pas, pourtant on recalcule toujours la direction)
                PointsDePatrouille.Add(PointsCube[i] + (delta * 1 / 4f));
                PointsDePatrouille.Add(PointsCube[i] + (delta * 2 / 4f));
                PointsDePatrouille.Add(PointsCube[i] + (delta * 3 / 4f));
            }
        }
        
        List<Vector2> ListeCopie(List<Vector2> liste)
        {
            List<Vector2> copie = new List<Vector2>(liste.Count);
            foreach (Vector2 vecteur in liste)
                copie.Add(new Vector2(vecteur.X, vecteur.Y));

            return copie;
        }

        public List<Vector2> GetPointsDePatrouille()
        {
            return ListeCopie(PointsDePatrouille);
        }

        public List<Vector2> GetBordureIntérieure()
        {
            return ListeCopie(BordureIntérieure);
        }

        public List<Vector2> GetBordureExtérieure()
        {
            return ListeCopie(BordureExtérieure);
        }

        public List<Vector2> GetPointsCube()
        {
            return ListeCopie(PointsCube);
        }
    }
}
