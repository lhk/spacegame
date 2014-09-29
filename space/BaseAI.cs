using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space
{
	abstract class BaseAI: AI
	{
		/// <summary>
		/// For debug purposes. With this font, the AI can draw messages
		/// </summary>
		private SpriteFont Font;
		public SpriteFont font{
			get{return Font;}
			set{Font=value;}
		}
		public int playernumber{ get; set;}
		public Control control{ get; set; }

		virtual public void Update(){
		}
		virtual public void Draw(SpriteBatch spriteBatch){
		}

		/// <summary>
		/// Returns a list of all enemies sorted by ascending distance
		/// </summary>
		/// <returns>List of enemies</returns>
		/// <param name="planets">All planets</param>
		/// <param name="playernumber">Playernumber.</param>
		public IEnumerable<Planet> closestEnemies(IEnumerable<Planet> planets,int playernumber){
			IEnumerable<Planet> enemies = from Planet p in planets where p.playernumber != playernumber select p;
			IEnumerable<Planet> allies=from Planet p in planets where p.playernumber==playernumber select p;

			// this is some linq magic
			// look at all enemies
			// order them by the distance
			// by what distance ?
			// look at all allies, compare them one by one, drop the ally that is farer away from the current enemy
			// this behaviour is implemented by aggregate
			// finally one ally remains, it's the ally closest to the enemy
			// => the distance to this ally is the smallest distance to any of the allies
			enemies = from Planet p in enemies
				orderby Vector2.Distance((allies.Aggregate((a,b)=>Vector2.Distance(a.position,p.position)<Vector2.Distance(b.position,p.position)?a:b)).position,p.position) 
					select p;
			return enemies;
		}

		/// <summary>
		/// Returns a list of enemies sorted by ascending distance to the given group of planets
		/// </summary>
		/// <returns>List of enemies</returns>
		/// <param name="planets">All planets</param>
		/// <param name="allies">allies</param>
		/// <param name="playernumber">Playernumber.</param>
		public IEnumerable<Planet> closestEnemies(IEnumerable<Planet> planets, IEnumerable<Planet> allies,int playernumber){
			IEnumerable<Planet> enemies = from Planet p in planets where p.playernumber != playernumber select p;
			foreach (Planet p in allies)
				if (p.playernumber != playernumber)
					throw new Exception ("closestEnemis was given a group of allied planets: in this group are planets with different playernumbers");
			enemies = from Planet p in enemies orderby Vector2.Distance((allies.Aggregate((a,b)=>Vector2.Distance(a.position,p.position)<Vector2.Distance(b.position,p.position)?a:b)).position,p.position) select p;
			return enemies;
		}

		/// <summary>
		/// Returns a list of the allied planets, ordered by ascending distance to the specified ally
		/// </summary>
		/// <returns>List of allies</returns>
		/// <param name="allies">Allies.</param>
		/// <param name="ally">Ally.</param>
		public IEnumerable<Planet> closestAllies(IEnumerable<Planet> allies, Planet ally)
		{
			allies = from Planet p in allies orderby Vector2.Distance(p.position, ally.position) where p.position != ally.position select p;
			return allies;
		}
	}
}

