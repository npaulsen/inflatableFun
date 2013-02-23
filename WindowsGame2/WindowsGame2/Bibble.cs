using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BibbleGame
{
    public class Bibble : MovingObject
    {
        const int SPAWN_TIME = 5000;
        const int BLINK_TIME = 250;
        static Color BLINK_COLOR = Color.Red;
        const float DEFAULT_BULLET_SPEED = 20;
        const int DEFAULT_BULLET_DAMAGE = 5;
        const float DEFAULT_MINE_DAMAGE = 50;
        const float MAX_MINE_DAMAGE = 80;
        const float DEFAULT_MINE_OUTER_RADIUS = 100;
        const float MAX_MINE_OUTER_RADIUS = 600;

        public Color BibbleColor;

        Mine mMine;

        BibbleGame game;

        private int mHealth = 100;
        private int mMaxHealth = 150;
        public int Deaths = 0;

        public bool IsDead;
        private bool mWasHit = false;
        public int mLastHitMS = 0;

        public int SpawnMS = 0;

        private float mBulletSpeed = DEFAULT_BULLET_SPEED;
        private int mBulletDamage = DEFAULT_BULLET_DAMAGE;
        private int mSplitBulletMode = 1;
        private float mSplitBulletAngle = .25f;
        private float mMineDamage = DEFAULT_MINE_DAMAGE;
        private float mMineOuterRadius = DEFAULT_MINE_OUTER_RADIUS;
        public int Health
        {
            get { return mHealth; }
            set { mHealth =  value > MaxHealth? MaxHealth: value < 0? 0 : value; }
        }
        public int MaxHealth
        {
            get { return mMaxHealth; }
            set { mMaxHealth = value; }
        }
        public float MineOuterRadius
        {
            get { return mMineOuterRadius; }
            set { mMineOuterRadius = value > MAX_MINE_OUTER_RADIUS? MAX_MINE_OUTER_RADIUS : value; }
        }
        public float MineDamage
        {
            get { return mMineDamage; }
            set { mMineDamage = value > MAX_MINE_DAMAGE? MAX_MINE_DAMAGE : value; }
        }
        public float SplitBulletAngle
        {
            get { return mSplitBulletAngle; }
            set { mSplitBulletAngle = value; }
        }

        public int SplitBulletMode
        {
            get { return mSplitBulletMode; }
            set { mSplitBulletMode = value; }
        }
        public int BulletDamage
        {
            get { return mBulletDamage; }
            set { mBulletDamage = value; }
        }
        public float BulletSpeed
        {
            get { return mBulletSpeed; }
            set { mBulletSpeed = value; }
        }
       

        public Bibble(BibbleGame game, Texture2D tex) : this(game, tex, Color.White) { }

        public Bibble(BibbleGame game, Texture2D tex, Color color)
            : base(game, tex, new Vector2(111,111))
        {
            this.Color = color;
            BibbleColor = color;
            this.game = game;
        }

        public new void Update(GameTime gt)
        {
            int ems = gt.ElapsedGameTime.Milliseconds;
            if (IsDead)
            {
                SpawnMS -= (ems > SpawnMS) ? SpawnMS : ems;
                if (SpawnMS <= 0)
                    Reset();
                return;
            }
            else
            {
                if (mWasHit)
                {
                    mLastHitMS = BLINK_TIME;
                    Color = BLINK_COLOR;
                    mWasHit = false;
                }
                mLastHitMS -= (ems > mLastHitMS) ? mLastHitMS : ems;
                if (mLastHitMS <=0)
                    Color = BibbleColor;
                base.Update(gt);
            }
        }

        /// <summary>
        /// ONLY FOR SQUARE YET!!!
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        internal bool IsInside(Rectangle rectangle)
        {
            Vector2 offset = new Vector2(Width / 2.0f, Height / 2.0f);
            offset = Vector2.Transform(offset, Matrix.CreateRotationZ(Orientation));
            // SHITTY!!!
            Vector2 vertex = PaintCorner;
            float xmax = vertex.X, xmin = vertex.X, ymin = vertex.Y, ymax = vertex.Y;
            for (int i = 1; i <= 3; i++)
            {
                offset = Vector2.Transform(offset, Matrix.CreateRotationZ((float)Math.PI/2));
                vertex = Position - offset;
                xmax = (vertex.X > xmax) ? vertex.X : xmax;
                xmin = (vertex.X < xmin) ? vertex.X : xmin;
                ymax = (vertex.Y > ymax) ? vertex.Y : ymax;
                ymin = (vertex.Y < ymin) ? vertex.Y : ymin;
            }
            //leftmost, upmost, rightmost, downmost

            return rectangle.Intersects(new Rectangle((int)xmin, (int)ymin, (int)(xmax - xmin), (int)(ymax - ymin)));

        }

        internal void MineAction()
        {
            if (mMine == null)
            {
                if (IsDead) return;
                mMine = new Mine(Position, this, BibbleGame.Statics.MineTex, game);
                game.addMine(mMine);
            }
            else
            {
                game.Detonate(mMine);
                mMine = null;
            }
        }
        internal void ShootAction()
        {
            if (IsDead) return;
            float bSpeed = BulletSpeed <= Speed + 1 ? Speed + 1 : BulletSpeed; // Bullets always faster than actual speed
            game.addBullet(new Bullet(game, this, this.Position, this.Orientation, bSpeed));
            for (int i = 1; i <= SplitBulletMode; i++)
            {
                game.addBullet(new Bullet(game, this, this.Position,
                    this.Orientation + i * SplitBulletAngle, bSpeed));
                game.addBullet(new Bullet(game, this, this.Position,
                    this.Orientation - i * SplitBulletAngle, bSpeed));
            }
            
            BibbleGame.Statics.laser.Play(.2f, 0, 1);
        }

        internal void Damage(float p)
        {
            if (p <= 0 || IsDead) return;

            mWasHit = true;
            this.Health -= (int)p;
            if (this.Health <= 0)
            {
                this.Health = 0;
                Deaths++;
                game.Explosion(Position, 1.0f, true);
                BibbleGame.Statics.laugh.Play();
                IsDead = true;
                SpawnMS = SPAWN_TIME;
            }
        }

        internal void Reset() {
            this.Health = 100;
            this.Speed = 0;
            this.Orientation = 0;
            this.IsDead = false;
            this.Position = new Vector2(BibbleGame.Random.Next(100,500), BibbleGame.Random.Next(100,300));
            this.Color = BibbleColor;
            this.mWasHit = false;
        }

        public override void Draw(GameTime gt)
        {
            base.Draw(gt);
            if (Health < 30)
                game.Smoke(Position);
        }
    }
}
