using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BibbleGame;
using Microsoft.Xna.Framework;

namespace BibbleGame
{
    public abstract class Item : DrawableObject, Collidable
    {
        public Item(Vector2 pos, Game g) : base(BibbleGame.Statics.BoxTex, pos, g, 1/8.0f) { }


        public virtual bool Collide(Bibble b)
        {
            BibbleGame.Statics.quack.Play(.1f, 0, 0);
            return true;
        }
    }

    public class HealthItem : Item
    {
        private const int DEFAULT_HEALTH = 25;
        private int mHealth;
        public HealthItem(Vector2 pos, Game g, int health)
            : base(pos, g)
        {
            mHealth = health;
        }
        public HealthItem(Vector2 pos, Game g) : this(pos, g, DEFAULT_HEALTH) { }

        public override bool Collide(Bibble b)
        {
            b.Health += mHealth;
            return base.Collide(b);
        }
    }

}
