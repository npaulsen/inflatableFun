using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BibbleGame
{
    public class MovingObject : DrawableObject
    {
        private const float VEL_LOSS = 15f;
        private const float TURN_VEL_LOSS = 15f;

        float mSpeed = 0f;
        float mTurnSpeed = 0f;
        bool mAcc;
        bool mBreak;
        bool mTurnLeft;
        bool mTurnRight;
        /* behaviour pseudo-constants */
        float mMaxSpeed = 10;
        float mMinSpeed = -2;
        float mAccVal = 15;
        float mBreakVal = 35;
        float mMaxTurnAngle = 10f;
        float mTurnAccVal = 30f;

        #region Properties 
        public virtual float Speed
        {
            get { return mSpeed; }
            set { mSpeed = (value > MaxSpeed) ? MaxSpeed : (value < -2) ? -2 : value; }
        }
        public virtual float TurnSpeed
        {
            get { return mTurnSpeed; }
            set { mTurnSpeed = (value > MaxTurnAngle) ? MaxTurnAngle : (value < -MaxTurnAngle) ? -MaxTurnAngle : value; }
        }
        public virtual float MinSpeed
        {
            get { return mMinSpeed; }
            set { mMinSpeed = value; }
        }
        public virtual float MaxSpeed
        {
            get { return mMaxSpeed; }
            set { mMaxSpeed = value; }
        }
        public virtual float MaxAcceleration
        {
            get { return mAccVal; }
            set { mAccVal = value; }
        }
        public virtual float MaxNegAcceleration
        {
            get { return mBreakVal; }
            set { mBreakVal = value; }
        }
        public virtual float MaxTurnAngle
        {
            get { return mMaxTurnAngle; }
            set { mMaxTurnAngle = value; }
        }
        public virtual float VelocityLoss
        {
            get { return VEL_LOSS; }
        }
        public virtual float TurnVelocityLoss
        {
            get { return TURN_VEL_LOSS; }
        }

        #endregion

        public MovingObject(Game g, Texture2D tex, Vector2 pos, float orient, float speed)
            : this(g, tex, pos, orient, speed, 1.0f) { }

        public MovingObject(Game g, Texture2D tex, Vector2 pos, float orient, float speed, float scale)
            : base(tex, pos, g, scale)
        {
            this.Orientation = orient;
            this.Speed = speed;
        }

        public MovingObject(Game g, Texture2D tex, Vector2 pos) : base(tex,pos,g) {}
 
  
        public void Accelerate()
        {
            mAcc = true;
        }

        public void Break()
        {
            mBreak = true;
        }
        
        public void Turn(bool left)
        {
            if (left)
                mTurnLeft = true;
            else
                mTurnRight = true;
        }

        public override void Update(GameTime gt)
        {
            float factor = gt.ElapsedGameTime.Milliseconds / 1000.0f;
            if (mAcc && !mBreak)
                this.Speed += mAccVal * factor;
            else if (mBreak && !mAcc)
                this.Speed -= mBreakVal * factor;
            else if (!mBreak && !mBreak)
            {
                float a = Math.Abs(Speed) < VelocityLoss ? Math.Abs(Speed) : VelocityLoss;
                this.Speed -= a * Math.Sign(this.Speed) * factor;
            }

            if (mTurnLeft && !mTurnRight)
                this.TurnSpeed -= mTurnAccVal * factor;
            else if (!mTurnLeft && mTurnRight)
                this.TurnSpeed += mTurnAccVal * factor;
            else
                this.TurnSpeed = 0;
            this.Orientation += TurnSpeed * factor;

            mAcc = mBreak = mTurnLeft = mTurnRight = false;

            Position = new Vector2(Position.X + Speed * (float)Math.Cos(Orientation),
            Position.Y + Speed * (float)Math.Sin(Orientation));

        }
    }
}
