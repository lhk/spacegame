using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space
{
    // the simplest AI possible.
    class JoinAI : AI
    {
        public void Update(Control control, int playernumber)
        {
            IEnumerable<Planet> allies = from Planet p in control.planets where p.playernumber == playernumber select p;
            if (!allies.Any())
            {
                // apparently we have lost all our planets.
                return;
            }
            IEnumerable<Planet> enemies = from Planet p in control.planets where p.playernumber != playernumber select p;
            if (!enemies.Any())
            {
                // all planets belong to us. or there are just no planets at all.
                return;
            }


            foreach (Planet p in allies)
            {
                //attack the closest enemy
                List<Planet> ally=new List<Planet>();
                ally.Add(p);

                Planet target = closestEnemies(enemies, ally, playernumber).First();

                Console.Out.WriteLine(p.ships);
                //if (p.ships < target.ships) continue;
                if (p.ships < 2) return;
                int number = 1;
                control.Order(new SendOrder(p, target, number));

            }
        }
        public IEnumerable<Planet> closestEnemies(IEnumerable<Planet> enemies, IEnumerable<Planet> allies, int playernumber)
        {
            enemies = from Planet p in enemies orderby Vector2.Distance((allies.Aggregate((a, b) => Vector2.Distance(a.position, p.position) < Vector2.Distance(b.position, p.position) ? a : b)).position, p.position) select p;
            return enemies;
        }

        public IEnumerable<Planet> closestAllies(IEnumerable<Planet> allies, Planet ally)
        {
            allies = from Planet p in allies orderby Vector2.Distance(p.position, ally.position) where p.position != ally.position select p;
            return allies;
        }
    }
}
