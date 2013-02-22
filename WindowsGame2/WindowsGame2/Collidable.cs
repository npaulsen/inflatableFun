using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace BibbleGame
{
    interface Collidable : IGameComponent
    {
        Vector2 Position { get; }

        float Width { get; }
        float Height { get; }

        /// <summary>
        /// Collide
        /// </summary>
        /// <param name="b"></param>
        /// <returns> True, if <this>Component</this> was destroyed</returns>
        bool Collide(Bibble b);
    }
}
