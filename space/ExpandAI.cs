using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space
{
    // the simplest AI possible.
    class ExpandAI : AI
    {
        public void Update(Control control, int playernumber)
        {
            IEnumerable<Planet> ownPlanets = from Planet p in control.planets where p.playernumber == playernumber select p;
            if (!ownPlanets.Any())
            {
                // apparently we have lost all our planets.
                return;
            }
            IEnumerable<Planet> enemyPlanets = from Planet p in control.planets where p.playernumber != playernumber select p;
            if (!enemyPlanets.Any())
            {
                // all planets belong to us. or there are just no planets at all.
                return;
            }

            //just attack one of the enemies
            Planet target = closestEnemies(control.planets, playernumber).First();


            foreach (Planet p in ownPlanets)
            {
                Console.Out.WriteLine(p.ships);
                if (p.ships < target.ships) continue;
                if (p.ships < 2) return;
                int number = 1;
                control.Send(new SendOrder(p, target, number));

            }
        }
        public IEnumerable<Planet> closestEnemies(IEnumerable<Planet> planets,int playernumber){
            IEnumerable<Planet> enemies = from Planet p in planets where p.playernumber != playernumber select p;
            IEnumerable<Planet> allies=from Planet p in planets where p.playernumber==playernumber select p;
            enemies = from Planet p in enemies orderby Vector2.Distance((allies.Aggregate((a,b)=>Vector2.Distance(a.position,p.position)<Vector2.Distance(b.position,p.position)?a:b)).position,p.position) select p;
            return enemies;
        }
    }
}
