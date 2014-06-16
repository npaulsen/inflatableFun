using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace BibbleGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BibbleGame : Microsoft.Xna.Framework.Game
    {
        #region Fields and Properties
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteBatch spriteBatchEffects;

        Bibble bib1, bib2;

        List<Bibble> bibbles;

        public struct Defaults
        {
            public Texture2D BibbleTex;
            public Texture2D BulletTex;
            public Texture2D MineTex;
            public Texture2D Circle50;//TODO circles via singleton pattern
            public Texture2D Circle100;
            public Texture2D Circle150;
            public Texture2D BoxTex;
            public Texture2D BackgroundTex;

            public SoundEffect laser;
            public SoundEffect laugh;
            public SoundEffect explosion;
            public SoundEffect comic;
            public SoundEffect quack;
        }
        public static Defaults Statics;

        ExplosionParticleSystem explosion;
        ExplosionSmokeParticleSystem smoke;
        SmokePlumeParticleSystem smokePlume;

        List<Bullet> bullets;
        List<Item> items;
        List<SimpleCollidableLine> lines;

        KeyboardState lastKeyboard;

        GamePadState lastGamePadOne;
        GamePadState lastGamePadTwo;

        SpriteFont font, fontHuge;

        public Texture2D LinePixel;

        // a random number generator that the whole sample can share.
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        public SpriteBatch SpriteBatch { get { return spriteBatch; } }
        public SpriteBatch SpriteBatchEffects { get { return spriteBatchEffects; } }
        public SpriteFont SpriteFont { get { return font; } }
        #endregion

        public BibbleGame()
        {
            Window.AllowUserResizing = true;
            IsFixedTimeStep = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // create the particle systems and add them to the components list.
            // we should never see more than one explosion at once
            explosion = new ExplosionParticleSystem(this, 1);
            Components.Add(explosion);

            // but the smoke from the explosion lingers a while.
            smoke = new ExplosionSmokeParticleSystem(this, 2);
            Components.Add(smoke);

            // we'll see lots of these effects at once; this is ok
            // because they have a fairly small number of particles per effect.
            smokePlume = new SmokePlumeParticleSystem(this, 15);
            Components.Add(smokePlume);

            bullets = new List<Bullet>(); // TODO collidable class for trash or explosives
            items = new List<Item>();
            lines = new List<SimpleCollidableLine>();
           }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            bib1 = new Bibble(this, Statics.BibbleTex);
            bib1.Zoom = 60f / Statics.BibbleTex.Width;
            Color c = Color.Purple;
            c.A = 0x6F;
            bib2 = new Bibble(this, Statics.BibbleTex, c);
            bib2.Zoom = 60f / Statics.BibbleTex.Width;
            //TODO: list of all damageable objects
            bibbles = new List<Bibble>();
            bibbles.Add(bib1);
            bibbles.Add(bib2);
            this.Components.Add(new ItemSpawner(this, 30000));

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // SpriteBatch for Explosions, Smoke etc
            spriteBatchEffects = new SpriteBatch(GraphicsDevice);

            Statics.BibbleTex = this.Content.Load<Texture2D>("chick");
            Statics.BulletTex = this.Content.Load<Texture2D>("200px-Soccer_ball");
            Statics.MineTex = this.Content.Load<Texture2D>("mine");
            Statics.Circle50 = CreateCircle(50);
            Statics.Circle100 = CreateCircle(100);
            Statics.Circle150 = CreateCircle(150);
            Statics.BackgroundTex = this.Content.Load<Texture2D>("water");
            Statics.BoxTex = this.Content.Load<Texture2D>("health");

            Statics.laser = this.Content.Load<SoundEffect>("laser1");
            Statics.laugh = this.Content.Load<SoundEffect>("laugh1");
            Statics.explosion = this.Content.Load<SoundEffect>("explosionSound");
            Statics.comic = this.Content.Load<SoundEffect>("comic");
            Statics.quack = this.Content.Load<SoundEffect>("duck-quack4");
            font = Content.Load<SpriteFont>("font");
            fontHuge = Content.Load<SpriteFont>("fontHuge");

            LinePixel = new Texture2D(GraphicsDevice, 1, 1);
            LinePixel.SetData<Color>(
                new Color[] { Color.White });// fill the texture with white
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            #region controls

            //Keyboard

            //Player 1: Turn right
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                bib1.Turn(1.0f);
            //Player 1: Turn left
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                bib1.Turn(-1.0f);
            //Player 1: Accelerate
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                bib1.Accelerate();
            //Player 1: Break
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                bib1.Break();

            //Player 2: Turn right
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                bib2.Turn(1.0f);
            //Player 2: Turn left
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                bib2.Turn(-1.0f);
            //Player 2: Accelerate
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                bib2.Accelerate();
            //Player 2: Break
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                bib2.Break();

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                LaunchItem();

            //GamePad

            //Exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Player 1: Turn right
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0)
                bib1.Turn(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);
            //Player 1: Turn left
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < 0)
                bib1.Turn(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);
            //Player 1: Accelerate
            if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0)
                bib1.Accelerate(GamePad.GetState(PlayerIndex.One).Triggers.Right);
            //Player 1: Break
            if (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0.1)
                bib1.Break(GamePad.GetState(PlayerIndex.One).Triggers.Left);

            //Player 2: Turn right
            if (GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X > 0)
                bib2.Turn(GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X);
            //Player 2: Turn left
            if (GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X < 0)
                bib2.Turn(GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X);
            //Player 2: Accelerate
            if (GamePad.GetState(PlayerIndex.Two).Triggers.Right > 0.1)
                bib2.Accelerate(GamePad.GetState(PlayerIndex.Two).Triggers.Right);
            //Player 2: Break
            if (GamePad.GetState(PlayerIndex.Two).Triggers.Left > 0.1)
                bib2.Break(GamePad.GetState(PlayerIndex.Two).Triggers.Left);

            #endregion


            bib1.Update(gameTime);
            bib2.Update(gameTime);
            if (lastKeyboard != null)
            {
                if ((lastKeyboard.IsKeyDown(Keys.M) && Keyboard.GetState().IsKeyUp(Keys.M)) || (lastGamePadOne.IsButtonDown(Buttons.A) && GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.A)))
                {
                    bib1.MineAction();
                }
                if ((lastKeyboard.IsKeyDown(Keys.N) && Keyboard.GetState().IsKeyUp(Keys.N)) || (lastGamePadOne.IsButtonDown(Buttons.X) && GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.X)))
                {
                    bib1.ShootAction();
                }
                if ((lastKeyboard.IsKeyDown(Keys.LeftShift) && Keyboard.GetState().IsKeyUp(Keys.LeftShift)) || (lastGamePadTwo.IsButtonDown(Buttons.A) && GamePad.GetState(PlayerIndex.Two).IsButtonUp(Buttons.A)))
                {
                    bib2.MineAction();
                }
                if ((lastKeyboard.IsKeyDown(Keys.CapsLock) && Keyboard.GetState().IsKeyUp(Keys.CapsLock)) || (lastGamePadTwo.IsButtonDown(Buttons.X) && GamePad.GetState(PlayerIndex.Two).IsButtonUp(Buttons.X)))
                {
                    bib2.ShootAction();
                }
                if ((lastKeyboard.IsKeyDown(Keys.U) && Keyboard.GetState().IsKeyUp(Keys.U)))
                {
                    List<SimpleCollidableLine> l = EdgeScene.getEdgeData(this);
                    if (l.Count > 0)
                    {
                        resetLines();
                        addLines(l);
                    }
                }
            }
            CheckCollisions();

            base.Update(gameTime);
            lastKeyboard = Keyboard.GetState();
            lastGamePadOne = GamePad.GetState(PlayerIndex.One);
            lastGamePadTwo = GamePad.GetState(PlayerIndex.Two);

        }

        private void LaunchItem()
        {
            Item i = new Item(this, new Vector2(-1, -1));
            AddItem(i);
        }

        public void AddItem(Item i)
        {
            if (i.Position == new Vector2(-1, -1))
            { // guess random, free position
                int tries;
                for (tries = 25; tries > 0; tries--)
                {
                    i.Position = new Vector2(Random.Next(50, Window.ClientBounds.Width - 50),
                                        Random.Next(50, Window.ClientBounds.Height - 50));
                    bool collides = false;
                    foreach (Bibble b in bibbles)
                        if ((b.Position - i.Position).Length() * 2 < b.Width + i.Width + 100)
                        {
                            collides = true;
                            break; // too close to a player
                        }
                    // TODO: other
                    if (!collides) break; // tries >= 1

                }
                if (tries <= 0)
                    return; // failed to find a valid spawn position
            }
            Components.Add(i);
            items.Add(i);

        }

        private void CheckCollisions()
        {

            /* Vector2 mdis = bib1.Position - bib2.Position;
             angle = (float) Math.Asin(mdis.Y / mdis.Length());
             angle -= bib1.Orientation;
             if (angle < Math.Atan(bib1.Width / bib1.Height))
                  angle = (float)(bib1.Width / 2.0 / Math.Sin(angle)); 
              */

            List<DrawableObject> list = new List<DrawableObject>(bullets); // allocation every call...
            list.AddRange(items);
            List<Collidable> delete = new List<Collidable>();

            foreach (Collidable b in list)
            {
                if (b.Position.X < -100 || b.Position.Y < -100
                    || b.Position.X > this.Window.ClientBounds.Width + 100
                    || b.Position.Y > this.Window.ClientBounds.Height + 100)
                {
                    delete.Add(b);
                    continue;
                }
                foreach (Bibble bib in bibbles)
                {
                    if (bib.IsDead) continue;
                    Vector2 mdis = bib.Position - b.Position;
                    if (mdis.Length() * 2 < bib.Width + b.Width)
                    {
                        if (b.Collide(bib))
                        {
                            delete.Add(b);
                        }

                    }
                }
            }

            foreach (SimpleCollidableLine l in lines)
            {
                foreach (Bibble b in bibbles)
                {
                    CircleLineIntersect(b, l);
                }
                if (l.Closed)
                {
                    foreach (Bullet b in bullets)
                    {
                        if (CircleLineIntersect(b, l))
                        {
                            delete.Add(b);
                            Explosion(b.Position, 0.05f, false);
                        }
                    }
                }
            }
            foreach (Collidable c in delete)
            { // bad design: need to know all kinds
                Components.Remove(c);
                if (c is Bullet)
                    bullets.Remove((Bullet)c);
                else if (c is Item)
                    items.Remove((Item)c);
            }
        }

        private bool CircleLineIntersect(MovingObject o, SimpleCollidableLine boundry)
        {
            Vector2 A = boundry.From;
            Vector2 B = boundry.To;
            Vector2 C = o.Position;
            float r = (o.Width - 5) / 2; // approx. radius of bibble
            // compute the euclidean distance between A and B
            float lab = Vector2.Distance(A, B);
            // compute the direction vector D from A to B
            Vector2 D = new Vector2((B.X - A.X) / lab, (B.Y - A.Y) / lab);
            // check, whether the centre of o has passed the line within one update cycle
            // the object must be fast, else, in the last collision check(s) it would have
            // touched the line!
            if (Vector2.Distance(o.LastPos, o.Position) > r) // this object is fast!
            {
                Vector2 C2 = o.LastPos;
                float ua = (C.X - C2.X) * (A.Y - C2.Y) - (C.Y - C2.Y) * (A.X - C2.X);
                float ub = (B.X - A.X) * (A.Y - C2.Y) - (B.Y - A.Y) * (A.X - C2.X);
                float denominator = (C.Y - C2.Y) * (B.X - A.X) - (C.X - C2.X) * (B.Y - A.Y);

                if (Math.Abs(denominator) <= 0.00001f)
                {
                    if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f && o.Collide(boundry))
                    {
                        //arbitrary intersectionpoint
                        // (A + B) / 2;
                        return true;
                    }
                }
                else
                {
                    ua /= denominator;
                    ub /= denominator;

                    if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                    {
                        // crossing point E
                        Vector2 Es = new Vector2(A.X + ua * (B.X - A.X), A.Y + ua * (B.Y - A.Y));
                        // compute the euclidean distance from E to C
                        float lecs = (float)Math.Sqrt((Es.X - C.X) * (Es.X - C.X) + (Es.Y - C.Y) * (Es.Y - C.Y));
                        //intersectionPoint.X = A.X + ua * (B.X - A.X);
                        //intersectionPoint.Y = A.Y + ua * (B.Y - A.Y);
                        if (o.Collide(boundry))
                        {
                            o.Position = Es;
                            return true;
                        }
                        else
                        {
                            //position and orientation correction
                            Vector2 resulting;
                            Vector2 oldDirection = new Vector2((float)Math.Cos(o.MovementDirection), (float)Math.Sin(o.MovementDirection));
                            Vector2 normal = new Vector2(-D.Y, D.X);
                            Vector2.Reflect(ref oldDirection, ref normal, out resulting);
                            if (resulting.Length() != 1f)
                            {
                                resulting.Normalize();
                            }
                            o.MovementDirection = (float)Math.Atan2(resulting.Y, resulting.X);
                            // move it back distance r-lec and furth r-lec again (in the new direction) 
                            o.Position = Es + resulting * lecs;
                            return false;
                        }
                    }
                }
            }

            if (Vector2.Distance(A, C) + Vector2.Distance(B, C) - 2 * r > lab)
            {
                return false;
            }

            
            // Now the line equation is x = Dx*t + Ax, y = Dy*t + Ay with 0 <= t <= 1.
            // compute the value t of the closest point to the circle center (Cx, Cy)
            float t = D.X * (C.X - A.X) + D.Y * (C.Y - A.Y);
            // This is the projection of C on the line from A to B.
            // compute the coordinates of the point E on line and closest to C
            Vector2 E = new Vector2(t * D.X + A.X, t * D.Y + A.Y);
            // compute the euclidean distance from E to C
            float lec = (float)Math.Sqrt((E.X - C.X) * (E.X - C.X) + (E.Y - C.Y) * (E.Y - C.Y));
            // test if the line intersects or tangents the circle
            if (lec < r) // TODO lec = r
            {
                /*    // compute distance from t to circle intersection point
                    float dt = (float)Math.Sqrt(r * r - lec * lec);
                    // compute first intersection point
                    Vector2 F = new Vector2((t - dt) * D.X + A.X, (t - dt) * D.Y + A.Y);
                    // compute second intersection point
                    Vector2 G = new Vector2((t + dt) * D.X + A.X, (t + dt) * D.Y + A.Y);*/
                if (o.Collide(boundry))
                {
                    Explosion(E, 0.05f, false); // ugly style
                    return true;
                }
                else
                {
                    Vector2 resulting;
                    Vector2 oldDirection = new Vector2((float)Math.Cos(o.MovementDirection), (float)Math.Sin(o.MovementDirection));
                    if (Vector2.Distance(C - oldDirection, E) < Vector2.Distance(C + oldDirection, E)) return false;
                    Vector2 normal = new Vector2(-D.Y, D.X);
                    Vector2.Reflect(ref oldDirection, ref normal, out resulting);
                    if (resulting.Length() != 1f)
                    {
                        resulting.Normalize();
                    }
                    o.MovementDirection = (float)Math.Atan2(resulting.Y, resulting.X);
                    // move it back distance r-lec and furth r-lec again (in the new direction) 
                    o.Position -= oldDirection * (r - lec);
                    o.Position += resulting * (r - lec);
                    return false;
                }
            }

            else
            {
                // line doesn't touch circle
                return false;
            }
        }

        public void addMine(Mine m)
        {
            Components.Add(m);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        { // http://gamedev.stackexchange.com/questions/24298/performance-architectural-implications-of-calling-spritebatch-begin-end-in-many
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            // int screenWidth = Window.ClientBounds.Width, screenHeight = Window.ClientBounds.Height;

            SpriteBatch sbg = new Microsoft.Xna.Framework.Graphics.SpriteBatch(this.GraphicsDevice);
            sbg.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sbg.Draw(Statics.BackgroundTex, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .9f);
            sbg.End();
            spriteBatch.Begin();

            base.Draw(gameTime); // calls components, they can use open SpriteBatch

            drawBib(bib1, gameTime);
            drawBib(bib2, gameTime);

            String s = String.Format("Bibble 1: {0}% {1}", bib1.Health, bib1.Deaths);
            spriteBatch.DrawString(font, s, new Vector2(51, 51), Color.FromNonPremultiplied(0, 0, 0, 0x8F));
            spriteBatch.DrawString(font, s, new Vector2(50, 50), bib1.Color);

            s = String.Format("Bibble 2: {0}% {1}", bib2.Health, bib2.Deaths);

            spriteBatch.DrawString(font, s, new Vector2(Window.ClientBounds.Width - 249, 51), Color.FromNonPremultiplied(0, 0, 0, 0x8F));
            spriteBatch.DrawString(font, s, new Vector2(Window.ClientBounds.Width - 250, 50), bib2.Color);


            spriteBatch.End();

        }


        private void drawBib(Bibble bib, GameTime gt)
        {
            if (bib.IsDead)
            {
                spriteBatch.DrawString(fontHuge, ((int)(bib.SpawnMS / 1000 + 1)).ToString(), new Vector2(300, 300), bib.Color);
                return;
            }
            int screenWidth = Window.ClientBounds.Width, screenHeight = Window.ClientBounds.Height;

            if (bib.IsInside(new Rectangle(0, 0, screenWidth, screenHeight)))
            {
                //Vector2 paintCorner = PaintCorner(bib.Width, bib.Height, bib.Orientation, bib.Position);
                //spriteBatch.Draw(Statics.BibbleTex, bib.PaintCorner, Statics.BibbleTex.Bounds, bib.Color, bib.Orientation, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                bib.Draw(gt);
                /* // Red DOT for paintcorner debug
                Texture2D dummyTexture = new Texture2D(GraphicsDevice, 3, 3);
                dummyTexture.SetData(new Color[] { Color.Red, Color.Red, Color.Red, Color.Red, Color.Black, Color.Red, Color.Red, Color.Red, Color.Red });
                spriteBatch.Draw(dummyTexture, bib.PaintCorner - new Vector2(1, 1), Color.Red); 
                 */
            }
            else
            {
                Vector2 position = bib.Position;
                if (position.X < 0) position.X = 25;
                if (position.X > screenWidth) position.X = screenWidth - 25;
                if (position.Y < 0) position.Y = 25;
                if (position.Y > screenHeight) position.Y = screenHeight - 25;
                float thumbFactor = (float) (3 / Math.Log(Vector2.Distance(position, bib.Position)));
                position = PaintCorner((int)(bib.Width * thumbFactor), (int)(bib.Height * thumbFactor), bib.Orientation + (float)Math.PI/2f, position);
                spriteBatch.Draw(Statics.BibbleTex, position, Statics.BibbleTex.Bounds, bib.Color, bib.Orientation + (float)Math.PI / 2f, new Vector2(0, 0), bib.Zoom * thumbFactor, SpriteEffects.None, 0);
            }


        }

        private Vector2 PaintCorner(int width, int height, float orientation, Vector2 center)
        {
            Vector2 offset = new Vector2(width * .5f, height * 0.5f);
            offset = Vector2.Transform(offset, Matrix.CreateRotationZ(orientation));
            return center - offset;
        }

        internal void Detonate(Mine mMine)
        {
            foreach (Bibble b in bibbles)
            {
                float dist = Vector2.Distance(b.Position, mMine.Position);
                //TODO move to model
                if (dist < mMine.Radius)
                    b.Damage(mMine.InnerDamage);
                else if (dist < mMine.OuterRadius)
                    b.Damage(mMine.InnerDamage * (mMine.OuterRadius - dist) / (mMine.OuterRadius - mMine.Radius));
            }
            Explosion(mMine.Position, .7f, false);

            Components.Remove(mMine);
        }

        public Texture2D CreateCircle(int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                for (int i = 0; i < 5; i++)
                {
                    int x = (int)Math.Round((radius - i) + (radius - i) * Math.Cos(angle)) + i;
                    int y = (int)Math.Round((radius - i) + (radius - i) * Math.Sin(angle)) + i;

                    data[y * outerRadius + x + 1] = Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xA0 / (i + 1));
                }
            }

            texture.SetData(data);
            return texture;
        }
        public void Explosion(Vector2 pos, float size, bool smoking)
        {
            explosion.Size = smoke.Size = size;
            explosion.AddParticles(pos);
            if (smoking)
                smoke.AddParticles(pos);
            Statics.explosion.Play(size, 0, 0);
        }

        public void Smoke(Vector2 pos)
        {
            smokePlume.Size = 0.5f;
            smokePlume.AddParticles(pos);
        }

        internal void addBullet(Bullet b)
        {
            bullets.Add(b);
            Components.Add(b);
        }

        internal void resetLines()
        {
            foreach (SimpleCollidableLine l in lines)
            Components.Remove(l);
            lines.Clear();
            int yMax = Window.ClientBounds.Height;
            addLine(new SimpleCollidableLine(this, new Vector2(0, 0), new Vector2(Window.ClientBounds.Width, 0)));
            addLine(new SimpleCollidableLine(this, new Vector2(Window.ClientBounds.Width, 0), new Vector2(Window.ClientBounds.Width, yMax)));
            addLine(new SimpleCollidableLine(this, new Vector2(Window.ClientBounds.Width, yMax), new Vector2(0, yMax)));
            addLine(new SimpleCollidableLine(this, new Vector2(0, yMax), new Vector2(0, 0)));
            addLine(new SimpleCollidableLine(this, new Vector2(150,150), new Vector2(Window.ClientBounds.Width - 150, 150)));
        }

        internal void addLine(SimpleCollidableLine l)
        {
            lines.Add(l);
            Components.Add(l);
        }
        internal void addLines(List<SimpleCollidableLine> l)
        {
            foreach (SimpleCollidableLine s in l) {
                addLine(s);
            }
        }
    }
}
