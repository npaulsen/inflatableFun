using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BibbleGame
{
    public class Bullet : MovingObject
    {
        private float mDamage = 5;
        private int mRebounds = 1;
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
            : base(g, BibbleGame.Statics.BulletTex, pos, orient, speed, 0.1f)
        {
            this.Owner = owner;
            this.mDamage = owner.BulletDamage;
        }

        public override bool Collide(Collidable c)
        {
            if (c is Bibble)
            {
                Bibble b = (Bibble) c;
                if (b.IsDead || Owner == b) return false;
                b.Damage(Damage);
                BibbleGame g = Game as BibbleGame;
                if (g != null)
                    g.Explosion(Position, 0.1f, false);
                return true;
            }
            else // bounce of, if stil bouncing
            {
                mRebounds--;
                Owner = null;
            }
            return mRebounds < 0;
        }
    }
}
