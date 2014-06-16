using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BibbleGame
{
    class SimpleCollidableLine : DrawableGameComponent, Collidable
    {
        public Vector2 From, To;
        public bool Closed; // no shots through it
        
        public SimpleCollidableLine(Game g, Vector2 from, Vector2 to, bool closed)
            : base(g)
        {
            From = from;
            To = to;
            Closed = closed;
        }

        public SimpleCollidableLine(Game g, Vector2 from, Vector2 to) : this(g, from, to, true) { }

        public override void Draw(GameTime gt)
        {
            //  base.Draw(gt); //TODO necessary?
            if (Game is BibbleGame)
            {

                BibbleGame game = Game as BibbleGame;

                DrawLine(game, //draw line
                    From, //start of line
                    To //end of line
                );
            }
            base.Draw(gt);
        }

        void DrawLine(BibbleGame g, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            g.SpriteBatch.Draw(g.LinePixel,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

        public Vector2 Position
        {
            get { throw new NotImplementedException(); }
        }

        public float Width
        {
            get { throw new NotImplementedException(); }
        }

        public float Height
        {
            get { throw new NotImplementedException(); }
        }

        public bool Collide(Collidable b)
        { //nothing happens to the line
            return false;
        }
    }
}


