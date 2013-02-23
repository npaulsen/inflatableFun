using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BibbleGame
{
    class ItemSpawner : GameComponent
    {
        #region Fields and Properties
        const int DEFAULT_SPAWN_MS = 20000;
        private int mCountDownMS;
        private int mSpawnTime;

        public int SpawnTime
        {
            get { return mSpawnTime; }
            set { mSpawnTime = value; }
        }
        private bool mPaused = false;
        /// <summary>
        /// Pauses the generation of items
        /// </summary>
        public bool IsPaused
        {
            get { return mPaused; }
            set { mPaused = value; }
        }
        #endregion

        public ItemSpawner(BibbleGame game) : this(game, DEFAULT_SPAWN_MS) { }

        public ItemSpawner(BibbleGame game, int spawnTime)
            : base(game)
        {
            SpawnTime = spawnTime;
            mCountDownMS = spawnTime;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsPaused) return; // pause, dont update counter

            int ms = gameTime.ElapsedGameTime.Milliseconds;
            if (mCountDownMS <= ms)
            { // spawn an item
                mCountDownMS = SpawnTime;
                BibbleGame g = Game as BibbleGame;
                if (g != null)
                {
                    g.AddItem(new Item(g, new Vector2(-1, -1)));
                }
            }
            else
                mCountDownMS -= ms;
        }

        /// <summary>
        /// Delay spawning items by ms milliseconds
        /// </summary>
        /// <param name="ms"> delay count in milliseconds</param>
        public void Delay(int ms)
        {
            mCountDownMS += ms;
        }
    }
}
