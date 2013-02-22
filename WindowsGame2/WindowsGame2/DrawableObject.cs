using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BibbleGame
{
    public class DrawableObject : DrawableGameComponent
    {
        Texture2D mSpriteTexture;

    //    public int Width = 30;
    //    public int Height = 30;

        private float mZoom = 1.0f;

        Vector2 mPosition = new Vector2(100, 100); // position of centre
        Vector2 mPaintCornerOffset;
        float mDirection = 0;//-(float)Math.PI / 2;
        private Color mColor = Color.White;


        #region Properties
        public Vector2 Position { get { return mPosition; } set { mPosition = value; } }
        public Texture2D SpriteTexture
        {
            get { return mSpriteTexture; }
            set {
                mSpriteTexture = value;
                RecalculatePaintCornerOffset();
            }
        }
        public float Orientation
        {
            get { return mDirection - (float)Math.PI / 2.0f; }
            set
            {
                mDirection = value + (float)Math.PI / 2.0f;
                RecalculatePaintCornerOffset();
            } // TODO stay in range, performance
        }
        public Vector2 PaintCorner { get { return mPosition + mPaintCornerOffset; } set { mPosition -= mPaintCornerOffset; } }
        public Color Color { get { return mColor; } set { mColor = value; } }
        private float mWidth;
        public float Width { get { return mWidth; } }
        private float mHeight;
        public float Height { get { return mHeight; } }
        public float Zoom
        {
            get { return mZoom; }
            set
            {
                mZoom = value; // TODO range control
                mWidth = mWidth * value;
                mHeight = mHeight * value;
            }
        }
        #endregion

        public DrawableObject(Texture2D tex, Vector2 pos, Game g, float scale) : base(g) {
            mSpriteTexture = tex;
            mPosition = pos; // TODO Property?
            mWidth = mSpriteTexture.Bounds.Width * scale;
            mHeight = mSpriteTexture.Bounds.Height * scale;
            mZoom = scale;
            RecalculatePaintCornerOffset();
        }
        public DrawableObject(Texture2D tex, Vector2 pos, Game g) : this(tex, pos, g, 1.0f) { }

        public override void Draw(GameTime gt) {
            //  base.Draw(gt); //TODO necessary?
            if (!(Game is BibbleGame))
            {
                base.Draw(gt);
            }
            else
            {
                BibbleGame game = Game as BibbleGame;
                game.SpriteBatch.Draw(mSpriteTexture, PaintCorner, null /* entire texture */
                    , this.Color, mDirection, new Vector2(0, 0), mZoom, SpriteEffects.None, 0);
            }
        }

        protected void RecalculatePaintCornerOffset()
        {
            Vector2 offset = new Vector2(-Width / 2.0f, -Height / 2.0f);
            offset = Vector2.Transform(offset, Matrix.CreateRotationZ(mDirection));
            mPaintCornerOffset = offset;
        }

    }
}
