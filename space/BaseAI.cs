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
		virtual public void Update(Control control, int playernumber){
		}

		//TODO add caching, this is really tough on the cpu. nasty performance for more than 20 planets
		// returns a list of enemy planets, ordered by the distance to the closest planet of the player
		// planets : all planets
		// playernumber : this players number
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

		// returns a list of enemy planets, ordered by the distance to the group of planets given
		public IEnumerable<Planet> closestEnemies(IEnumerable<Planet> planets, IEnumerable<Planet> allies,int playernumber){
			IEnumerable<Planet> enemies = from Planet p in planets where p.playernumber != playernumber select p;
			foreach (Planet p in allies)
				if (p.playernumber != playernumber)
					throw new Exception ("closestEnemis was given a group of allied planets: in this group are planets with different playernumbers");
			enemies = from Planet p in enemies orderby Vector2.Distance((allies.Aggregate((a,b)=>Vector2.Distance(a.position,p.position)<Vector2.Distance(b.position,p.position)?a:b)).position,p.position) select p;
			return enemies;
		}

		public IEnumerable<Planet> closestAllies(IEnumerable<Planet> allies, Planet ally)
		{
			allies = from Planet p in allies orderby Vector2.Distance(p.position, ally.position) where p.position != ally.position select p;
			return allies;
		}
	}
}

