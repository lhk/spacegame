using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space
{
    // the simplest AI possible.
    class JoinAI : BaseAI
    {
        override public void Update(Control control, int playernumber)
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
                control.Send(new SendOrder(p, target, number));

            }
        }
    }
}
