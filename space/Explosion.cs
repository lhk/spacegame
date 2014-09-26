using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space
{
    class Explosion
    {
        public Explosion(Vector2 position) 
        {
            this.position = position;
        }

        public Vector2 position;
        public int width=88;
        public int height=93;
        private int frame;
        private int totalFrames=43;
		private float timer=0;
		private float frameDuration=0.1f;

		public void Update(GameTime gameTime){
			timer += gameTime.ElapsedGameTime.Milliseconds;
			if (timer > frameDuration) {
				frame++;
				timer = 0;
			}
		}
        public bool expired {
            set { }
            get { return frame == totalFrames; }
        }
        public Rectangle drawRect{
            set { }
            get { return new Rectangle(width * (frame-1), 0, width, height); }
        }
    }
}
