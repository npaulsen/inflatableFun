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
        Dummy, Health, Speed, Acceleration, BulletSpeed, BulletDamage, BulletSplit, MineDamage, MineOuterRadius, Killer
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
            Init(t, value);
        }

        public Item(Game g, Vector2 pos, ItemType t)
            : base(BibbleGame.Statics.BoxTex, pos, g, 1 / 8.0f)
        {
            int value = 0;
            switch (t)
            {
                case ItemType.Acceleration: value = 3; break;
                case ItemType.BulletDamage: value = 1; break;
                case ItemType.BulletSpeed: value = 10; break;
                case ItemType.BulletSplit: value = 1; break;
                case ItemType.Health: value = 25; break;
                case ItemType.MineDamage: value = 15; break;
                case ItemType.MineOuterRadius: value = 10; break;
                case ItemType.Speed: value = 2; break;
                case ItemType.Killer: value = 30; break;
            }
            Init(t, value);
        }

        private void Init(ItemType t, int val)
        {
            mType = t;
            mValue = val;

            if (t == ItemType.Killer)
                Color = Color.Black;
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
                case ItemType.Killer:
                    BibbleGame bg = Game as BibbleGame;
                    if (bg == null) break;
                    bg.Detonate(new Mine(Position, null, BibbleGame.Statics.MineTex, bg));
                    break;
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
                case ItemType.Killer: return 2f;
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
