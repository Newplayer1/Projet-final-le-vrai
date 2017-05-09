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


namespace AtelierXNA
{
   public class CalculateurFPS : Microsoft.Xna.Framework.GameComponent
   {
      public string ChaîneFPS { get; private set; }
      float IntervalleMAJ { get; set; }
      float TempsÉcouléDepuisMAJ { get; set; }
      int CptFrames { get; set; }


      public CalculateurFPS(Game game, float intervalleMAJ)
         : base(game)
      {
         IntervalleMAJ = intervalleMAJ;
      }

      public override void Initialize()
      {
         TempsÉcouléDepuisMAJ = 0;
         CptFrames = 0;
         ChaîneFPS = "";
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
         ++CptFrames;
         TempsÉcouléDepuisMAJ += tempsÉcoulé;
         if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
         {
            MettreÀJourFPS();
            TempsÉcouléDepuisMAJ = 0;
         }
      }

      private void MettreÀJourFPS()
      {
         float valFPS = CptFrames / TempsÉcouléDepuisMAJ;
         ChaîneFPS = valFPS.ToString("0");
         CptFrames = 0;
         Game.Window.Title = ChaîneFPS;
      }
   }
}
