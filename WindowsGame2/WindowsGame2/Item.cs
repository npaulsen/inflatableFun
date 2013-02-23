using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BibbleGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BibbleGame
{
    /* Item Type is via enum to have a common constructor for all items */
    public enum ItemType
    {
        Dummy, Health, Speed, Acceleration, BulletSpeed, BulletDamage, BulletSplit, MineDamage, MineOuterRadius
    }
    public class Item : DrawableObject, Collidable
    {
        private ItemType mType;
        private int mValue;

        public int Value
        {
            get { return mValue; }
        }
        public ItemType Type { get { return mType; } }

        public Item(Game g, Vector2 pos) : this(g, pos, ChooseType()) { }

        public Item(Game g, Vector2 pos, ItemType t, int value) : base(BibbleGame.Statics.BoxTex, pos, g, 1/8.0f) 
        {
            mType = t;
            mValue = value;
        }

        public Item(Game g, Vector2 pos, ItemType t)
            : base(BibbleGame.Statics.BoxTex, pos, g, 1 / 8.0f)
        {
            mType = t;
            switch (t)
            {
                case ItemType.Acceleration: mValue = 3; break;
                case ItemType.BulletDamage: mValue = 1; break;
                case ItemType.BulletSpeed: mValue = 10; break;
                case ItemType.BulletSplit: mValue = 1; break;
                case ItemType.Health: mValue = 25; break;
                case ItemType.MineDamage: mValue = 15; break;
                case ItemType.MineOuterRadius: mValue = 10; break;
                case ItemType.Speed: mValue = 2; break;
            }
        }


        public virtual bool Collide(Bibble b)
        {
            switch (Type)
            {
                case ItemType.Acceleration: b.MaxAcceleration += Value; break;
                case ItemType.BulletDamage: b.BulletDamage += Value; break;
                case ItemType.BulletSpeed: b.BulletSpeed += Value; break;
                case ItemType.BulletSplit: b.SplitBulletMode += Value; break;
                case ItemType.Health: b.Health += Value; break;
                case ItemType.MineDamage: b.MineDamage += Value; break;
                case ItemType.MineOuterRadius: b.MineOuterRadius += Value; break;
                case ItemType.Speed: b.MaxSpeed += Value; break;
            }
            BibbleGame.Statics.quack.Play(.1f, 0, 0);
            return true;
        }
        /// <summary>
        /// Get a probability coefficient for the Item type. 1.0f is normal, .5f half, 2f double probability
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetProbability(ItemType t)
        {
            switch (t)
            {
                case ItemType.Health: return 5f;
                default: return 1f;
            }
        }

        public static ItemType ChooseType()
        {
            float sum = 0;
            foreach (ItemType t in Enum.GetValues(typeof(ItemType)))
            {
                sum += GetProbability(t);
            }
            float r = (float)BibbleGame.Random.NextDouble() * sum;
            foreach (ItemType t in Enum.GetValues(typeof(ItemType)))
            {
                float prob = GetProbability(t);
                if (r < prob)
                    return t;
                r -= prob;
            }
            //TODO error handling
            return ItemType.Dummy;
        }

        // debug:
#if DEBUG
        public override void Draw(GameTime gt)
        {
            base.Draw(gt);
            BibbleGame g = Game as BibbleGame;
            g.SpriteBatch.DrawString(g.SpriteFont, Type.ToString(), Position, Color.Blue);
        }
#endif
    }
}
