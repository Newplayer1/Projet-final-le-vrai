//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;


//namespace AtelierXNA.Classes_Pokemon_Skyrim
//{

//    public class ChoixPokemonDébut : Microsoft.Xna.Framework.DrawableGameComponent
//    {
//        const float INTERVALLE_MAJ_STANDARD = 0.01f;
//        SpriteBatch spriteBatch { get; set; }

//        RessourcesManager<Texture2D> GestionnaireDeTextures {get; set;}
//        GraphicsDeviceManager graphics { get; set; }

//        public ChoixPokemonDébut(Game game)
//            : base(game)
//        {
//            // TODO: Construct any child components here
//        }

//        public override void Initialize()
//        {

//            graphics = Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager;
//            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
//            ChoisirPokemonDépart();
//            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
//            base.Initialize();
//        }
//        void ChoisirPokemonDépart()
//        {
//            //foreach(GameComponent p in Game.Components)
//            //{
//            //    if(p is PageTitre)
//            //    Game.Components.Remove(p);
//            //}


//            Game.Components.Add(charmander);


//            Game.Components.Add(bulbusaur);


//            Game.Components.Add(squirtle);

//        }

//        public override void Update(GameTime gameTime)
//        {
//            MouseState mouse = Mouse.GetState();
            

//            base.Update(gameTime);
//        }
//        public override void Draw(GameTime gameTime)
//        {
//            GraphicsDevice.Clear(Color.Black);
//            spriteBatch.Begin();
//                    squirtle.Draw(spriteBatch);
//                    bulbusaur.Draw(spriteBatch);
//                    charmander.Draw(spriteBatch);
//            spriteBatch.End();
//            base.Draw(gameTime);
//        }
//    }
//}
