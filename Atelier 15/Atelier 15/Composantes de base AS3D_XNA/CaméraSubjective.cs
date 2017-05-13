using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace AtelierXNA
{
    public class CaméraSubjective : Caméra
    {

        public Vector2 Souris { get; private set; }
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const double ANGLE_MAX = MathHelper.Pi/12;
        const float ACCÉLÉRATION = 0.001f;
        const float VITESSE_INITIALE_ROTATION = 1.5f;
        const float VITESSE_INITIALE_TRANSLATION = 0.05f;
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
        const float RAYON_COLLISION = 1f;
        public Vector3 Direction { get; set; }
        public Vector3 Latéral { get; private set; }
        float VitesseTranslation { get; set; }
        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }
        float VitesseRotation { get; set; }


        Vector3 Angle { get; set; }

        TerrainAvecBase Terrain { get; set; }


        bool estEnZoom;
        bool EstEnZoom
        {
            get { return estEnZoom; }
            set
            {
                float ratioAffichage = Game.GraphicsDevice.Viewport.AspectRatio;
                estEnZoom = value;
                if (estEnZoom)
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF / 2, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
                else
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
            }
        }

        public CaméraSubjective(Game jeu, Vector3 positionCaméra, Vector3 cible, Vector3 orientation, float intervalleMAJ)
           : base(jeu)
        {
            IntervalleMAJ = intervalleMAJ;
            CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            CréerPointDeVue(positionCaméra, cible, orientation);
            EstEnZoom = false;
        }

        public override void Initialize()
        {
            VitesseRotation = VITESSE_INITIALE_ROTATION;
            VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
            Angle = new Vector3(DELTA_LACET, DELTA_TANGAGE, DELTA_ROULIS);
            GérerLacet();
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
            // float nbRangées = Terrain.NbRangées;

        }

        protected override void CréerPointDeVue()
        {
            Latéral = Vector3.Cross(Direction, OrientationVerticale);
            Latéral = Vector3.Normalize(Latéral);
            Vue = Matrix.CreateLookAt(Position, Position + Direction, OrientationVerticale);
            GénérerFrustum();
        }

        public override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
        {
            Position = position;
            Cible = cible;
            OrientationVerticale = Vector3.Normalize(orientation);
            Direction = Vector3.Normalize(Cible - Position); 
            CréerPointDeVue();
        }
        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            GestionClavier();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                CréerPointDeVue();
                TempsÉcouléDepuisMAJ = 0;
            }
                Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
            base.Update(gameTime);
        }
        
        private int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? 1 : 0;
        }

        private void GérerAccélération()
        {
            int valAccélération = (GérerTouche(Keys.Subtract) + GérerTouche(Keys.OemMinus)) - (GérerTouche(Keys.Add) + GérerTouche(Keys.OemPlus));
            if (valAccélération != 0)
            {
                IntervalleMAJ += ACCÉLÉRATION * valAccélération;
                IntervalleMAJ = MathHelper.Max(INTERVALLE_MAJ_STANDARD, IntervalleMAJ);
            }
        }

        public void GérerDéplacement(float déplacementDirection, float déplacementLatéral)
        {
            EffectuerDéplacementDirec(déplacementDirection);
            EffectuerDéplacementLat(déplacementLatéral);
        }


        void EffectuerDéplacementDirec(float déplacement)
        {
            if (déplacement != 0)
            {
                Position += Direction * déplacement;
            }
        }

        void EffectuerDéplacementLat(float déplacement)
        {
            if (déplacement != 0)
            {
                Position += Latéral * déplacement;
            }
        }

        
        void GérerRotation()
        {
            GérerLacet();
            GérerTangage();
            GérerRoulis();
        }

        void GérerLacet()
        {
            // Gestion du lacet (yaw)
            // À compléter
            //if (GestionInput.EstEnfoncée(Keys.Left))
            //{
            //    Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(OrientationVerticale, Angle.X * VitesseRotation));

            //    Latéral = Vector3.Cross(OrientationVerticale, Direction);
            //    Latéral = Vector3.Normalize(Latéral);
            //}

            //if (GestionInput.EstEnfoncée(Keys.Right))
            //{
            //    Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(OrientationVerticale, Angle.X * -VitesseRotation));

            //    Latéral = Vector3.Cross(OrientationVerticale, Direction);
            //    Latéral = Vector3.Normalize(Latéral);
            //}

        }

        void GérerTangage()
        {
            // Gestion du tangage
            // À compléter
            if (GestionInput.EstEnfoncée(Keys.Up))
            {
                OrientationVerticale = Vector3.Transform(OrientationVerticale, Matrix.CreateFromAxisAngle(Latéral, Angle.Z * VitesseRotation));
                Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Latéral, Angle.X * VitesseRotation));

                Latéral = Vector3.Cross(OrientationVerticale, Direction);
                Latéral = Vector3.Normalize(Latéral);
            }

            if (GestionInput.EstEnfoncée(Keys.Down))
            {
                OrientationVerticale = Vector3.Transform(OrientationVerticale, Matrix.CreateFromAxisAngle(Latéral, Angle.Z * -VitesseRotation));
                Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Latéral, Angle.X * -VitesseRotation));

                Latéral = Vector3.Cross(OrientationVerticale, Direction);
                Latéral = Vector3.Normalize(Latéral);
            }
        }

        void GérerRoulis()
        {
            if (GestionInput.EstEnfoncée(Keys.PageDown))
            {
                OrientationVerticale = Vector3.Transform(OrientationVerticale, Matrix.CreateFromAxisAngle(Direction, Angle.Z * -VitesseRotation));

                Latéral = Vector3.Cross(OrientationVerticale, Direction);
                Latéral = Vector3.Normalize(Latéral);
            }

            if (GestionInput.EstEnfoncée(Keys.PageUp))
            {
                OrientationVerticale = Vector3.Transform(OrientationVerticale, Matrix.CreateFromAxisAngle(Direction, Angle.Z * VitesseRotation));

                Latéral = Vector3.Cross(OrientationVerticale, Direction);
                Latéral = Vector3.Normalize(Latéral);
            }
        }

        void GestionClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Z))
            {
                EstEnZoom = !EstEnZoom;
            }
        }
    }
}






/*
 * 
 * Dans le constructeur 
 * 
 *  MoveTo(position, rotation);

 * 
    private void UpdateLookAt()
        {
            Matrix rotationMatrix = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y);
            Vector3 lookAtOffSet = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

            cameraLookAt = cameraPosition + lookAtOffSet;

        }

 * 
 * 
 * Avant la méthode update 
 * 
 * 
 * 
   private Vector3 PreviewMove(Vector3 amount)
        {
            Matrix rotate = Matrix.CreateRotationY(Rotation.Y); // cameraRoatation.Y???

            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);

            return Position + movement;
        }

        void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }

 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * Au début de la méthode update 
 * 
 *             float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentMouseState = Mouse.GetState();
 * 
 * 
 * Dans la fin de la méthode Update
 
     
                 float deltaX;
            float deltaY;

            if(currentMouseState != previousMouseState)
            {
                deltaX = currentMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
                deltaY = currentMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                mouseRotationBuffer.X -= 0.01f * deltaX * dt;
                mouseRotationBuffer.Y -= 0.01f * deltaY * dt;

                if(mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));

                if (mouseRotationBuffer.Y < MathHelper.ToRadians(75.0f))
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));

                Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(mouseRotationBuffer.X), 0);

                deltaX = 0;
                deltaY = 0;
            }

            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            previousMouseState = currentMouseState;
     
     
     
     
     
     
     
     
     
     
     */
