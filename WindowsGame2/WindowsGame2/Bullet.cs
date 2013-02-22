using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BibbleGame
{
    public class Bullet : MovingObject, Collidable
    {
        private float mDamage = 5;
        public Bibble Owner;

        public float Damage { get { return mDamage; } }
        public override float MaxSpeed
        {
            get { return 30; }
        }
        public override float VelocityLoss
        {
            get
            {
                return 0;
            }
        }

        public Bullet(Game g, Bibble owner, Vector2 pos, float orient, float speed)
            : base(g, BibbleGame.Statics.BulletTex, pos, orient, speed, 3/40.0f)
        {
            this.Owner = owner;
        }

        public bool Collide(Bibble b)
        {
            if (b.IsDead || Owner == b) return false;
            b.Damage(Damage);
            BibbleGame g = Game as BibbleGame;
            if (g != null)
                g.Explosion(b.Position, 0.1f, false);
            return true;
        }
    }
}
