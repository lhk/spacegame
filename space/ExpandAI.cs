
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space
{
    class ExpandAI : AI
    {
        public void Update(Control control, int playernumber)
        {
            IEnumerable<Planet> ownPlanets = from Planet p in control.planets where p.playernumber==playernumber select p;
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

            //find the weakest enemy and attack
            //Planet weakest = enemyPlanets.OrderBy(p => p.ships).First();
            Planet target = enemyPlanets.First();
            int ships = ownPlanets.Sum(p => p.ships);


            foreach (Planet p in ownPlanets) {
                Console.Out.WriteLine(p.ships);
                if (p.ships < target.ships) continue;
                if (p.ships < 2) return;
                int number = 1;
                control.Order(new SendOrder(p,target,number));

            }

        }
    }
}
