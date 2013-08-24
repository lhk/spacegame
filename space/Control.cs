using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space
{
    class Control
    {
        public List<Planet> planets;
        public List<Ship> ships;
        public List<AI> ais;

        public List<SendOrder> sendOrders;



        public Texture2D shipTexture;
        public Texture2D planetTexture;
        public SpriteFont font;

        public int screenWidth;
        public int screenHeight;

        public bool once = true;

        public Control(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            sendOrders = new List<SendOrder>();

            // initialize the game
            // create random planets and start the ai

            planets = new List<Planet>();
            Random r = new Random();
            for (int i = 0; i < 5; i++) {
                Planet testPlanet = new Planet();
                testPlanet.playernumber = 1;
                testPlanet.radius = r.Next(10,50);
                testPlanet.ships = 5;
                testPlanet.timer = 0;

                Vector2 position = new Vector2(r.Next(testPlanet.radius, screenWidth / 2-testPlanet.radius), r.Next(testPlanet.radius, screenHeight-testPlanet.radius));
                while(planets.Exists(p=>Vector2.Distance(p.position,position)<p.radius+testPlanet.radius))
                {
                    position = new Vector2(r.Next(testPlanet.radius, screenWidth / 2 - testPlanet.radius), r.Next(testPlanet.radius, screenHeight - testPlanet.radius));
                }
                testPlanet.position = position;

                planets.Add(testPlanet);
            }
            r = new Random();
            for (int i = 0; i < 5; i++)
            {
                Planet testPlanet = new Planet();
                testPlanet.playernumber = 2;
                testPlanet.radius = r.Next(10, 50);
                testPlanet.ships = 5;
                testPlanet.timer = 0;

                Vector2 position = new Vector2(r.Next(screenWidth / 2 + testPlanet.radius, screenWidth - testPlanet.radius), r.Next(screenWidth / 2 + testPlanet.radius, screenHeight - testPlanet.radius));
                while (planets.Exists(p => Vector2.Distance(p.position, position) < p.radius + testPlanet.radius))
                {
                    position = new Vector2(r.Next(screenWidth / 2 + testPlanet.radius, screenWidth - testPlanet.radius), r.Next(screenWidth / 2 + testPlanet.radius, screenHeight - testPlanet.radius));
                }
                testPlanet.position = position;
                Console.Out.WriteLine(position);

                planets.Add(testPlanet);
            }

            ships = new List<Ship>();

            ais = new List<AI>();
            ExpandAI eai1 = new ExpandAI();
            ExpandAI eai2 = new ExpandAI();
            ais.Add(eai1);
            ais.Add(eai2);
        }


        public bool Order(SendOrder order)
        {
            sendOrders.Add(order);
            return true;
        }

        private bool Send(Ship ship, Planet target) 
        {
            ship.target = target;
            return true;
        }

        // method that an AI can call. Returns true if successful,
        private bool Send(Planet from, Planet to, int amount)
        {

            if (from.ships < amount)
            {
                return false;
            }

            // create the ships
            Random r = new Random();
            from.ships = from.ships - amount;
            for (int i = 0; i < amount; i++)
            {
                Ship ship = new Ship();
                ship.position = from.position+new Vector2(r.Next(-20,20),r.Next(-20,20));
                ship.playernumber = from.playernumber;
                ship.target = to;

                ships.Add(ship);
            }

            return true;
        }

        public bool Land(Planet planet, Ship ship)
        {
            if (planet.playernumber == ship.playernumber) {
                // the ship lands on an allied planet
                planet.ships++;
                ships.Remove(ship);
            }
            else if (planet.ships > 0)
            {
                // the ship attacks an enemies planet
                planet.ships--;
                if (planet.ships == 0) 
                {
                    // all forces on the planet are destroyed
                    planet.playernumber = 0;
                }
                ships.Remove(ship);
            }
            else if (planet.ships == 0) 
            {
                // the planet was empty and is now conquered
                planet.ships++;
                planet.playernumber = ship.playernumber;
                ships.Remove(ship);
            }
            return true;
        }

        public void Update(GameTime gameTime) 
        {
            // update the planets
            foreach (Planet planet in planets) 
            {
                planet.timer += gameTime.ElapsedGameTime.Milliseconds;
                if (planet.timer > 1000) {
                    planet.timer = 0;
                    planet.ships++;
                }
            }

            // start the AI
            int i = 1;
            foreach (AI ai in ais) 
            {
                ai.Update(this, i);
                i++;
            }

            foreach (SendOrder order in sendOrders) 
            {
                Send(order.from, order.to, order.amount);
            }
            sendOrders.RemoveAll(p => true);

            // move the ships
            List<Ship> landedShips=new List<Ship>();
            foreach(Ship ship in ships)
            {
                if (ship.target != null)
                {
                    if (Vector2.Distance(ship.position, ship.target.position) > ship.target.radius)
                    {

                        Vector2 diff = ship.target.position - ship.position;
                        diff.Normalize();
                        diff = Vector2.Multiply(diff, ((float)gameTime.ElapsedGameTime.Milliseconds) / 10);
                        //Console.Out.WriteLine(diff);
                        ship.position += diff;
                        //Console.Out.WriteLine(testShip.position);

                        //now rotate the testShip, unless it is very close to the target.
                        if (Vector2.Distance(ship.position, ship.target.position) > ship.target.radius)
                        {

                            Vector2 direction = ship.target.position - ship.position;
                            ship.rotation = (float)Math.Atan2(direction.Y, direction.X);

                        }
                    }
                    else {
                        landedShips.Add(ship);
                    }
                }
            }
            foreach(Ship ship in landedShips){
                Land(ship.target,ship);
            }

            if (gameTime.TotalGameTime.Seconds > 5 && once) 
            {
                once = false;
                Send(planets.First<Planet>(), planets.Last<Planet>(), 5);
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Planet planet in planets)
            {
                Color color=Color.Khaki;
                switch (planet.playernumber) 
                {
                    case 0: color = Color.Gray; break;
                    case 1: color = Color.Red; break;
                    case 2: color = Color.Blue; break;
                    case 3: color = Color.Green; break;
                    case 4: color = Color.Gold; break;
                    case 5: color = Color.HotPink; break;
                }
                Rectangle drawRect = new Rectangle((int)planet.position.X - planet.radius, (int)planet.position.Y - planet.radius,(int) planet.radius*2,(int) planet.radius*2);
                spriteBatch.Draw(planetTexture, drawRect, color);
                spriteBatch.DrawString(font, ""+planet.ships, planet.position, color);
            }
            foreach (Ship ship in ships)
            {
                Color color=Color.Khaki;
                switch (ship.playernumber)
                {
                    case 0: color = Color.Gray; break;
                    case 1: color = Color.Red; break;
                    case 2: color = Color.Blue; break;
                    case 3: color = Color.Green; break;
                    case 4: color = Color.Gold; break;
                    case 5: color = Color.HotPink; break;
                }
                Rectangle drawRect = new Rectangle((int)(ship.position.X - ship.size / 2), (int)(ship.position.Y - ship.size / 2), (int)ship.size, (int)ship.size);
                spriteBatch.Draw(shipTexture, drawRect,null , color, (float)ship.rotation, new Vector2(ship.size / 2, ship.size / 2), SpriteEffects.None, 1);
            }
        }
    }
}
