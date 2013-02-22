using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BibbleGame;
using Microsoft.Xna.Framework;

namespace BibbleGame
{
    class Item : DrawableObject, Collidable
    {
        public Item(Vector2 pos, Game g) : base(BibbleGame.Statics.BoxTex, pos, g, 1/8.0f) { }


        public bool Collide(Bibble b)
        {
            b.Health += 5;
            return true;
        }
    }

}
