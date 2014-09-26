using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace space
{
    // Singleton Object that controls the ships and planets
	// all AIs communicate with this object
	// all updates on the game state are done here
    class Control
    {


        public List<Planet> planets;
        public List<Ship> ships;
        public List<AI> ais;
        public List<Explosion> explosions;

        // these orders are messages between the AIs and the control singleton.
        // A SendOrder specifies an origin planet, a destination planet and the number of ships
        public List<SendOrder> sendOrders;


        // Monogame specific stuff for drawing
        public Texture2D shipTexture;
        public Texture2D planetTexture;
        public Texture2D explosionTexture;
        public SpriteFont font;

        public SoundEffect explosionSound;

        public int screenWidth;
        public int screenHeight;

		/// minimum and maximum size of planets, purely visual, no gameplay effect
		public int minPlanetRadius=10;
		public int maxPlanetRadius=30;

		// number of planets per player
		public int planetCount=20;

		// number of initial ships per planet
		public int startShips=5;

		// number of milliseconds to produce a new ship
		public int shipDelay=1000;

        public Control(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            sendOrders = new List<SendOrder>();

            // initialize the game
            // create random planets and start the ai
            planets = new List<Planet>();
            Random r = new Random();
            for (int i = 0; i < planetCount; i++) {
                Planet testPlanet = new Planet();
                testPlanet.playernumber = 1;
                testPlanet.radius = r.Next(minPlanetRadius,maxPlanetRadius);
                testPlanet.ships = startShips;
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
            for (int i = 0; i < planetCount; i++)
            {
                Planet testPlanet = new Planet();
                testPlanet.playernumber = 2;
                testPlanet.radius = r.Next(minPlanetRadius, maxPlanetRadius);
                testPlanet.ships = startShips;
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
            JoinAI ai1 = new JoinAI();
            JoinAI ai2 = new JoinAI();
            ais.Add(ai1);
            ais.Add(ai2);

            explosions = new List<Explosion>();
        }


        public bool Send(SendOrder order)
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
                explosions.Add(new Explosion(ship.position));
                //explosionSound.Play();
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

        // The Update method of the Game1 object is called 60 times a second. And it calls the update method on its control instance.
        // This Update method actually updates the game state
        public void Update(GameTime gameTime) 
        {
            // update the planets
            foreach (Planet planet in planets) 
            {
				if (planet.playernumber == 0)
					continue;
                // time to add a new ship ?
                planet.timer += gameTime.ElapsedGameTime.Milliseconds;
                if (planet.timer > shipDelay) {
                    planet.timer = 0;
                    planet.ships++;
                }
            }

            // update the AIs
            // some dirty trick here. The AIs get a reference to the control singleton and their playernumber as parameters to their update method. 
            // The AIs shouldn't store any persistent record of the situation but just query the current state from the control reference
            // and then determine their planets and ships with their playernumbers.
            // The playernumber is the position in the list of AIs.
            // If an AI is added or removed during runtime, this could mean that players are "swapped"
            int i = 1;
            foreach (AI ai in ais) 
            {
                ai.Update(this, i);
                i++;
            }

            // the AIs have made their decisions and created the according sendorders.
            // process them
            foreach (SendOrder order in sendOrders) 
            {
                Send(order.from, order.to, order.amount);
            }
            sendOrders.RemoveAll(p => true);

            // move the ships
            // if a ship touches its target planet, it can land. That means it is removed from the list of ships and the planet is updated
            // but the list of ships must not be modified in the foreach loop or an exception occurs.
            // therefore the landed ships are stored in a separate list and removed after the foreach loop finishes.
            List<Ship> landedShips=new List<Ship>();
            foreach(Ship ship in ships)
            {
                if (ship.target != null)
                {
                    // is the ship touching the planet ?
                    if (Vector2.Distance(ship.position, ship.target.position) > ship.target.radius)
                    {
                        // move the ship towards the target
                        Vector2 diff = ship.target.position - ship.position;
                        diff.Normalize();
                        diff = Vector2.Multiply(diff, ((float)gameTime.ElapsedGameTime.Milliseconds) / 10);
                        ship.position += diff;

                        // rotate the ship to face its target
                        Vector2 direction = ship.target.position - ship.position;
                        ship.rotation = (float)Math.Atan2(direction.Y, direction.X);
                    }
                    else {
                        // the ship is within the planets radius, it must land
                        landedShips.Add(ship);
                    }
                }
            }
            foreach(Ship ship in landedShips){
                Land(ship.target,ship);
            }

			foreach (Explosion explosion in explosions) {
				explosion.Update (gameTime);
			}


            List<Explosion> expiredExplosions = new List<Explosion>();
            foreach (Explosion explosion in explosions) 
            {
                if (explosion.expired) expiredExplosions.Add(explosion);
            }
            foreach (Explosion explosion in expiredExplosions) 
            {
                explosions.Remove(explosion);
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
            foreach (Explosion explosion in explosions) 
            {

                spriteBatch.Draw(explosionTexture, new Rectangle((int)explosion.position.X-25, (int)explosion.position.Y-25, 50, 50), explosion.drawRect, Color.White);
            }
        }
    }
}
