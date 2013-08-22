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
        private List<Planet> planets;
        private List<Ship> ships;
        public Texture2D shipTexture;
        public Texture2D planetTexture;
        public SpriteFont font;
        public int screenWidth;
        public int screenHeight;

        public Control(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            // initialize the game
            // create random planets and start the ai
            // for now just a testPlanet planet and a testPlanet testShip

            Planet testPlanet = new Planet();
            testPlanet.playernumber = 2;
            testPlanet.id = 1;
            testPlanet.position = new Vector2(screenWidth/2, screenHeight/2);
            testPlanet.radius = 50;
            testPlanet.ships = 0;

            planets = new List<Planet>();
            planets.Add(testPlanet);

            Ship testShip = new Ship();
            testShip.playernumber = 5;
            testShip.position = new Vector2(300,0);
            testShip.target = testPlanet;

            ships = new List<Ship>();
            ships.Add(testShip);
        }

        // method that an AI can call. Returns true if successful,
        public bool Send(Planet from, Planet to, int amount, int playernumber)
        {

            if (from.ships < amount)
            {
                return false;
            }

            // create the ships
            for (int i = 0; i < amount; i++)
            {
                Ship ship = new Ship();
                ship.position = from.position;
                ship.playernumber = playernumber;
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
                            float dot = Vector2.Dot(Vector2.UnitX, diff);
                            Console.Out.WriteLine(dot);
                            Console.Out.WriteLine(diff.Length());
                            double angle = Math.Acos(dot / diff.Length());
                            ship.rotation = angle;
                            Console.Out.WriteLine(angle);
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
