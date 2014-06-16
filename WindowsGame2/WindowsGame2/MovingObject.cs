using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BibbleGame
{
    public abstract class MovingObject : DrawableObject, Collidable
    {
        private const float VEL_LOSS = 15f;
        private const float TURN_VEL_LOSS = 15f;
        const float MAX_MAX_SPEED = 20;
        const float MAX_ACCELERATION = 30;

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
        float mAccIntensity = 1;
        float mBreakIntensity = 1;
        float mMaxTurnAngle = 10f;
        float mTurnIntensity = 0;
        float mTurnAccVal = 30f;
        float mMovementDirection = 0f;

        Vector2 lastPos;

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
            set { mMaxSpeed = value > MAX_MAX_SPEED? MAX_MAX_SPEED : value; }
        }
        public virtual float MaxAcceleration
        {
            get { return mAccVal; }
            set { mAccVal = value > MAX_ACCELERATION? MAX_ACCELERATION : value; }
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
        public virtual float MovementDirection
        {
            get { return mMovementDirection - (float)Math.PI / 2.0f; }
            set
            {
                mMovementDirection = value + (float)Math.PI / 2.0f;
                //RecalculatePaintCornerOffset();
            } // copypaste from DrawableObject.Orientation
        }
        public virtual Vector2 LastPos
        {
            get { return lastPos; }
        }

        #endregion

        public MovingObject(Game g, Texture2D tex, Vector2 pos, float orient, float speed)
            : this(g, tex, pos, orient, speed, 1.0f) { }

        public MovingObject(Game g, Texture2D tex, Vector2 pos, float orient, float speed, float scale)
            : base(tex, pos, g, scale)
        {
            this.Orientation = orient;
            this.MovementDirection = orient;
            this.Speed = speed;
            this.lastPos = pos;
        }

        public MovingObject(Game g, Texture2D tex, Vector2 pos) : base(tex,pos,g) {
            this.lastPos = pos;
        }
 
  
        public void Accelerate()
        {
            mAcc = true;
            mAccIntensity = 1;
        }

        public void Accelerate(float intensity)
        {
            mAcc = true;
            mAccIntensity = intensity;
        }

        public void Break()
        {
            mBreak = true;
        }

        public void Break(float intensity)
        {
            mBreak = true;
            mBreakIntensity = intensity;
        }
        
        public void Turn(float intensity)
        {
            if (intensity < 0)
                mTurnLeft = true;
            else
                mTurnRight = true;
            mTurnIntensity = intensity;
        }

        public override void Update(GameTime gt)
        {
            float factor = gt.ElapsedGameTime.Milliseconds / 1000.0f;
            float newMovementSpeed = 0;
            if (mAcc && !mBreak)
                newMovementSpeed = mAccIntensity * MaxAcceleration * factor;
            else if (mBreak && !mAcc)
                newMovementSpeed = - mBreakIntensity * MaxAcceleration * factor;
           //resistance
            float a = Math.Abs(Speed) < VelocityLoss ? Math.Abs(Speed) : VelocityLoss;
            this.Speed -= a * Math.Sign(this.Speed) * factor;
            

            if (mTurnLeft && !mTurnRight)
                this.TurnSpeed += mTurnIntensity * mTurnAccVal * factor;
            else if (!mTurnLeft && mTurnRight)
                this.TurnSpeed += mTurnIntensity * mTurnAccVal * factor;
            else
                this.TurnSpeed = 0;
            this.Orientation += TurnSpeed * factor;

            mAcc = mBreak = mTurnLeft = mTurnRight = false;

            Vector2 oldMvmt = new Vector2(Speed * (float)Math.Cos(MovementDirection), 
                Speed * (float)Math.Sin(MovementDirection));
            Vector2 newMvmt = new Vector2(newMovementSpeed * (float)Math.Cos(Orientation),
            newMovementSpeed * (float)Math.Sin(Orientation));

            Vector2 resulting = oldMvmt + newMvmt;
            this.Speed = resulting.Length();
            this.MovementDirection = (float)Math.Atan2(resulting.Y, resulting.X);
            lastPos = Position;
            Position += oldMvmt + newMvmt;
        }


        public abstract bool Collide(Collidable b);
    }
}
