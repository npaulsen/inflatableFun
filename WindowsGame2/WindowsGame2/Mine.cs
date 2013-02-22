using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BibbleGame
{
    public class Mine : DrawableObject
    {
        public Bibble owner;
        public float Radius = 50;
        public float OuterRadius = 100;
        public float InnerDamage = 50;

        public Mine(Vector2 pos, Bibble owner, Texture2D tex, Game g) : base(tex, pos, g)
        {
            this.owner = owner;
        }

        public override void Draw(GameTime gt) {
            base.Draw(gt);
            BibbleGame g = Game as BibbleGame;
            g.SpriteBatch.Draw(BibbleGame.Statics.Circle50, new Vector2(Position.X - Radius, Position.Y - Radius), null, owner.Color, 0, new Vector2(0, 0), Radius / 50, SpriteEffects.None, 0);
            Color light = owner.Color;
            light.A /= 3;
            g.SpriteBatch.Draw(BibbleGame.Statics.Circle100, new Vector2(Position.X - OuterRadius, Position.Y - OuterRadius),
                    null, light, 0, new Vector2(0, 0), OuterRadius / 100, SpriteEffects.None, 0);
            
        }
    }
}
