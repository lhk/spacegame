using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space
{
    // the simplest AI possible.
    class ExpandAI : BaseAI
    {
        override public void Update(Control control, int playernumber)
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
    }
}
