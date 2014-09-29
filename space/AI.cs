using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace space
{
    interface AI
    {
        void Update();
		void Draw(SpriteBatch spriteBatch);
		SpriteFont font {
			get;
			set;
		}
		int playernumber{get;set;}
		Control control{get;set;}
    }
}
