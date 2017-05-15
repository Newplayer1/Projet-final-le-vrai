using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
   public class InputManager : Microsoft.Xna.Framework.GameComponent
   {
      Keys[] AnciennesTouches { get; set; }
      Keys[] NouvellesTouches { get; set; }
      KeyboardState ÉtatClavier { get; set; }
      MouseState AncienÉtatSouris { get; set; }
      MouseState NouvelÉtatSouris { get; set; }
      GamePadState AncienÉtatGamepad { get; set; }
      GamePadState NouvelÉtatGamepad { get; set; }
      GamePadState ÉtatGamePad { get; set; }

        public InputManager(Game game)
         : base(game)
      { }

      public override void Initialize()
      {
         NouvellesTouches = new Keys[0];
         AnciennesTouches = NouvellesTouches;
         NouvelÉtatSouris = Mouse.GetState();
         AncienÉtatSouris = NouvelÉtatSouris;

         NouvelÉtatGamepad = GamePad.GetState(PlayerIndex.One);
         AncienÉtatGamepad = NouvelÉtatGamepad;
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         AnciennesTouches = NouvellesTouches;
         ÉtatClavier = Keyboard.GetState();
         NouvellesTouches = ÉtatClavier.GetPressedKeys();

            ÉtatGamePad = GamePad.GetState(PlayerIndex.One);


         if (EstSourisActive)
         {
            ActualiserÉtatSouris();
         }
         if(GamePad.GetState(PlayerIndex.One).IsConnected) 
         {
            ActualiserÉtatGamepad();
         }
      }

      public bool EstClavierActivé
      {
         get { return NouvellesTouches.Length > 0; }
      }

      public bool EstEnfoncée(Keys touche)
      {
         return ÉtatClavier.IsKeyDown(touche);
      }

      public bool EstPenché()
      {
            return ÉtatGamePad.ThumbSticks.Right.X != 0;
      }

      public bool EstNouvelleTouche(Keys touche)
      {
         int nbTouches = AnciennesTouches.Length;
         bool estNouvelleTouche = ÉtatClavier.IsKeyDown(touche);
         int i = 0;
         while (i < nbTouches && estNouvelleTouche)
         {
            estNouvelleTouche = AnciennesTouches[i] != touche;
            ++i;
         }
         return estNouvelleTouche;
      }
      void ActualiserÉtatGamepad() 
      {
            AncienÉtatGamepad = NouvelÉtatGamepad;
            NouvelÉtatGamepad = GamePad.GetState(PlayerIndex.One);
      }
      public void ActualiserÉtatSouris()
      {
         AncienÉtatSouris = NouvelÉtatSouris;
         NouvelÉtatSouris = Mouse.GetState();
      }

      public bool EstSourisActive
      {
         get { return Game.IsMouseVisible; }
      }

      public bool EstAncienClicDroit()
      {
         return NouvelÉtatSouris.RightButton == ButtonState.Pressed && 
                AncienÉtatSouris.RightButton == ButtonState.Pressed;
      }

      public bool EstAncienClicGauche()
      {
         return NouvelÉtatSouris.LeftButton == ButtonState.Pressed && AncienÉtatSouris.LeftButton == ButtonState.Pressed;
      }

      public bool EstNouveauClicDroit()
      {
         return NouvelÉtatSouris.RightButton == ButtonState.Pressed && AncienÉtatSouris.RightButton == ButtonState.Released;
      }

      public bool EstNouveauClicGauche()
      {
         return NouvelÉtatSouris.LeftButton == ButtonState.Pressed && 
                AncienÉtatSouris.LeftButton == ButtonState.Released;
      }
        public bool EstNouveauA()
        {
            return NouvelÉtatGamepad.Buttons.A == ButtonState.Pressed &&
                   AncienÉtatGamepad.Buttons.A == ButtonState.Released;
        }
        public bool EstBougerEnX()
        {
            return NouvelÉtatGamepad.ThumbSticks.Right.X != AncienÉtatGamepad.ThumbSticks.Right.X;
        }
        
        public bool EstNouveauRightshoulder_pokeball()
        {
            return NouvelÉtatGamepad.Buttons.RightShoulder == ButtonState.Pressed &&
                   AncienÉtatGamepad.Buttons.RightShoulder == ButtonState.Released;
        }
        public bool EstNouveauB_back()
        {
            return NouvelÉtatGamepad.Buttons.B == ButtonState.Pressed &&
                   AncienÉtatGamepad.Buttons.B == ButtonState.Released;
        }
        public bool EstNouveauStart_save()
        {
            return NouvelÉtatGamepad.Buttons.Start == ButtonState.Pressed &&
                   AncienÉtatGamepad.Buttons.Start == ButtonState.Released;
        }
        public bool EstNouveauSelect_heal()
        {
            return NouvelÉtatGamepad.Buttons.Back  == ButtonState.Pressed &&
                   AncienÉtatGamepad.Buttons.Back == ButtonState.Released;
        }
        public bool EstNouveauDown_menucombat()
        {
            return NouvelÉtatGamepad.DPad.Down == ButtonState.Pressed &&
                   AncienÉtatGamepad.DPad.Down == ButtonState.Released;
        }
        public bool EstNouveauUp_menucombat()
        {
            return NouvelÉtatGamepad.DPad.Up == ButtonState.Pressed &&
                   AncienÉtatGamepad.DPad.Up == ButtonState.Released;
        }
        public bool EstNouveauX_newpokemon()
        {
            return NouvelÉtatGamepad.Buttons.X == ButtonState.Pressed &&
                   AncienÉtatGamepad.Buttons.X == ButtonState.Released;
        }
        public bool EstNouveauY_inventaire()
        {
            return NouvelÉtatGamepad.Buttons.Y == ButtonState.Pressed &&
                   AncienÉtatGamepad.Buttons.Y == ButtonState.Released;
        }

        public Point GetPositionSouris()
      {
         return new Point(NouvelÉtatSouris.X, NouvelÉtatSouris.Y);
      }
   }
}