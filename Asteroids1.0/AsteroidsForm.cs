//***********************************************************************************
//Program:					Asteroids
//Description:	            Lame version of asteroids game
//Date:						March 24th 2017
//Authors:					Alexander Schneider
//Course:					CMPE2800
//Class:					CNTA02
//***********************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControlsLib;
using System.Drawing.Drawing2D;

namespace Asteroids1._0
{
    public partial class AsteroidsForm : Form
    {
        //Gets control from the keyboard or xbox controller
        private ControlInterface _control;
        //Ship the player controls
        private Ship _ship;
        //List of all lasers in the game
        private List<Laser> _shots;
        //LinkedList of all the rocks
        private LinkedList<Rock> _rocks;
        //List of all the background stars
        private List<Star> _stars;
        //Ship used to draw lives
        private Ship _livesShip = new Ship(2, new PointF(0, 0));

        //Count the shot position to line up with ships barrels. From 0-3
        private int _shotPos = 0;
        //Check for one shot per key press
        private bool _shotCheck = false;
        //Starting lives
        private int _lives = 3;
        //Starting scale
        private int _score = 0;
        //If the game is paused
        private bool _pause = false;
        //Makes the toggle so it doesnt pause and unpause as fast as tick runs
        private bool _pauseCheck = false;
        //State of the game; 3 states
        private gameStates _gameState = gameStates.menu;
        //Time for new rocks to spawn at. Decreases when a rock is spawned
        private int _rockSpawnTimer = 200;
        //Counts up per tick. Rocks are spawned when equal to rockSpawnTimer
        private int _rockTimer = 0;
        //Amount of lives gained to know when to add lives
        private int _livesGained = 0;

        //Lives the player starts with
        private const int _CstartLives = 3;
        //Score player starts with
        private const int _CstartScore = 0;
        //
        private const int _CstartlivesGained = 0;
        //How many points required for an extra life
        private const int _CextraLifePoints = 10000;
        //Font size for most UI elements. Larger text is based off the number
        private const int _CfontSize = 15;
        //Starting timer for rock spawn
        private const int _CstartRockSpawnTimer = 200;
        private const int _CstartRockTimer = 0;
        //Max amount of shots in game
        private const int _CmaxShots = 6;
        //Scale of the drawing in the game
        private const float _CgameScale = 2;
        //Amount of pixels shots need to be offset by to light up correctly
        private const float _CshotOff = 4* _CgameScale;

        //Random for spawning stars and rocks
        private static Random rand = new Random();

        //Enum for the state of the game
        enum gameStates { menu, gameOn, gameOver}
        //Possible size of rocks
        enum rockSize { small=1, medium, large }

        public AsteroidsForm()
        {
            InitializeComponent();
        }

        private void Asteroids_Load(object sender, EventArgs e)
        {
            //Create needed lists at launch
            _shots = new List<Laser>();
            _rocks = new LinkedList<Rock>();
            _stars = new List<Star>();
            //Start controls so player can play
            _control = new ControlInterface();
            
            //Add rocks and stars to the starting menu
            for (int r = 0; r < 10; r++)
            {
                _rocks.AddLast(new Rock(_CgameScale, new PointF(rand.Next(0, ClientRectangle.Width), rand.Next(0, ClientRectangle.Height))));
            }          
            for (int s = 0; s < 100; s++)
            {
                _stars.Add(new Star(_CgameScale, new PointF(rand.Next(0, ClientRectangle.Width), rand.Next(0, ClientRectangle.Height))));
            }
        }

        private void UI_TAst_Tick(object sender, EventArgs e)
        {
            //Every time tick, game moves
            gameTick();
        }

        private void AsteroidsForm_KeyDown(object sender, KeyEventArgs e)
        {
            //Update controls with keys being pressed
            _control.updateKeys(e, true);
        }

        private void AsteroidsForm_KeyUp(object sender, KeyEventArgs e)
        {
            //Update controls with keys that are released
            _control.updateKeys(e, false);
        }

        private void AsteroidsForm_MouseClick(object sender, MouseEventArgs e)
        {
            _rocks.AddLast(new Rock(_CgameScale, e.Location, (int)rockSize.large, 0));
        }
        /// ////////
        /// Called every tick. Checks if game is paused, and if not calls all the needed game ticks.
        /// Renders everything.
        /// ///////
        private void gameTick()
        {
            //Game has ended if no livse
            if (_lives == 0)
            {
                _gameState = gameStates.gameOver;
            }
            //If the game is on, check if it should be paused
            if (_control.pause && _gameState==gameStates.gameOn)
            {
                //Check to pause/unpause on only a new press of the key
                if (_pauseCheck)
                {
                    _pause = !_pause;
                    _pauseCheck = false;
                }
            }
            else
            {
                _pauseCheck = true;
            }
            //If not paused, check controls and tick the game
            if (!_pause)
            {
                checkControls();
                gameTicks();
            }
            //Render everything
            gameRender();
        }

        /// <summary>
        /// Checks the controls from the Control Interface 
        /// If being pressed, update 
        /// </summary>
        private void checkControls()
        {
            //Dont bother updating if game isnt on
            if (_gameState == gameStates.gameOn)
            {
                _ship.thrust = _control.up;
                if (_control.left)
                {
                    _ship.rotateLeft();
                }
                else if (_control.right)
                {
                    _ship.rotateRight();
                }
                if (_control.up)
                    _ship.moveForward();

                if (_control.fire)
                {
                    if (_shotCheck && _shots.Count<_CmaxShots)
                    {
                        _shots.Add(new Laser(_CgameScale, getShotPoint(_shotPos), _ship.shipRotation()));
                        if (_shotPos == 3)
                            _shotPos = 0;
                        else
                            _shotPos++;
                        _shotCheck = false;
                    }
                }
                else
                {
                    _shotCheck = true;
                }
            }
            //If on menu, shoot to start
            else
            {
                if (_control.fire)
                {
                    gameStart();
                    //So you dont immediately shoot when game starts
                    _shotCheck = false;
                }
            }
        }

        /// <summary>
        /// All the ticks for every object in the game.
        /// Checks if they are in the approriate gamestate and updates.
        /// </summary>
        private void gameTicks()
        {
            //Check if any extra lives are needed
            extraLives();
            //Check if any new rocks need to be spawned
            if (_gameState == gameStates.gameOn)
                spawnRocks();
            //Tick on the ship, shots and remove shots outside of the field
            //Check collision between lasers and rocks, and the ship and rocks.
            if (_gameState == gameStates.gameOn || _gameState == gameStates.gameOver)
            {
                _ship.Tick(ClientSize.Width, ClientSize.Height);
                _shots.ForEach(l => l.Tick(ClientSize.Width, ClientSize.Height));
                _shots.RemoveAll(l => l.outside);
                checkCollision();
                shipCollision();
            }
            //Tick all the objects that happen all the time
            _stars.ForEach(s => s.Tick(ClientSize.Width, ClientSize.Height));
            foreach (Rock r in _rocks)
                r.Tick(ClientSize.Width, ClientSize.Height);
        }

        /// <summary>
        /// Spawns new rocks in the game based off of a timer.
        /// </summary>
        private void spawnRocks()
        {
            //If the timer reaches 0, time to spawn a new rock.
            if (_rockTimer <= 0)
            {
                PointF point = new PointF(rand.Next(0, ClientRectangle.Width), rand.Next(0, ClientRectangle.Height));
                _rocks.AddLast(new Rock(_CgameScale, point, (int)rockSize.large, 0));
                //Minmimum spawn time is 20 ticks.
                if (_rockSpawnTimer!=20)
                    _rockSpawnTimer--;
                //Reset the rock timer to the top
                _rockTimer = _rockSpawnTimer;
            }
            //Move timer to get closer to new rock spawn.
            _rockTimer--;
        }
        /// <summary>
        /// Checks the score if the player needs a new life.
        /// </summary>
        private void extraLives()
        {
            if (_score / _CextraLifePoints == _livesGained+1)
            {
                _lives++;
                _livesGained++;
            }
        }
        /// <summary>
        /// If a new game is started, reset everything
        /// </summary>
        private void gameStart()
        {
            //New lists and ship
            _shots = new List<Laser>();
            _ship = new Ship(_CgameScale, new PointF(ClientSize.Width / 2, ClientSize.Height / 2));
            _rocks = new LinkedList<Rock>();
            _stars = new List<Star>();
            //Adde new background stars
            for (int s = 0; s < 100; s++)
            {
                _stars.Add(new Star(_CgameScale, new PointF(rand.Next(0,ClientRectangle.Width), rand.Next(0, ClientRectangle.Height))));
            }
            //Set these to the correct values based on the consts
            _score = _CstartScore;
            _lives = _CstartLives;
            _rockSpawnTimer = _CstartRockSpawnTimer;
            _rockTimer = _CstartRockTimer;
            _gameState = gameStates.gameOn;
        }

        /// <summary>
        /// Renders everything in the game.
        /// Checks the states between menu, gameOver and gameOn.
        /// Renders what is needed in each mode.
        /// </summary>
        private void gameRender()
        {
            using (BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                using (BufferedGraphics bg = bgc.Allocate(CreateGraphics(), ClientRectangle))
                {
                    //Stars, Rocks and Shots(So they continue to exist during death) are always rendered.
                    _stars.ForEach(s => s.Render(Color.White, bg));
                    foreach (Rock r in _rocks)
                        r.Render(Color.Brown, bg);                  
                    _shots.ForEach(l => l.Render(Color.Green, bg));

                    //During the game, ship, the score screen are rendered.
                    if (_gameState == gameStates.gameOn)
                    {
                        _ship.Render(Color.White, bg);
                        screenRender(bg);
                        //If paused, draw paused on screen
                        if (_pause)
                            gamePause(bg);
                    }
                    //Draw ship, menu and gameOver info if gameOver state.
                    else if (_gameState == gameStates.gameOver)
                    {
                        _ship.Render(Color.White, bg);
                        menu(bg);
                        gameOver(bg);
                    }
                    //Draw menu if on menu.
                    else if (_gameState == gameStates.menu)
                    {
                        menu(bg);
                    }
                    //Render the background buffer
                    bg.Render();
                }
            }
        }

        /// <summary>
        /// Gets a point for new lasers so they align with the barrels.
        /// </summary>
        /// <param name="sp">Position for the shot to occur in. Between 0-3</param>
        /// <returns></returns>
        private PointF getShotPoint(int sp)
        {
            if (sp == 0)
                return new PointF(_ship._pos.X + (float)(_CshotOff * Math.Cos(_ship.shipRotation() * (Math.PI / 180))), 
                    _ship._pos.Y + (float)(5 * Math.Sin(_ship.shipRotation() * (Math.PI / 180))));
            else if (sp == 1)
                return new PointF(_ship._pos.X + (float)(-_CshotOff * Math.Cos(_ship.shipRotation() * (Math.PI / 180))), 
                    _ship._pos.Y + (float)(-5 * Math.Sin(_ship.shipRotation() * (Math.PI / 180))));
            else if (sp == 2)
                return new PointF(_ship._pos.X + (float)((_CshotOff*2) * Math.Cos(_ship.shipRotation() * (Math.PI / 180))), 
                    _ship._pos.Y + (float)(10 * Math.Sin(_ship.shipRotation() * (Math.PI / 180))));
            else
                return new PointF(_ship._pos.X + (float)((-_CshotOff*2) * Math.Cos(_ship.shipRotation() * (Math.PI / 180))), 
                    _ship._pos.Y + (float)(-10 * Math.Sin(_ship.shipRotation() * (Math.PI / 180))));

        }

        /// <summary>
        /// Checks if the lasers and rocks are hitting. If they are they are removed from the game.
        /// Rocks that are not minimum size will spawn new rocks.
        /// </summary>
        private void checkCollision()
        {
            Graphics g = CreateGraphics();
            List<Laser> hits = new List<Laser>();
            //Look at each laser
            foreach (Laser l in _shots)
            {
                //Get the list of rocks that are close to the lasers.
                var close = from rock in _rocks
                            where ((distance(l._pos, rock._pos)) < (ShapeBase._tileSize) || rock.wrapping)
                            select rock;
                if (close.Count() > 0)
                {
                    //Check if any of the shapes are intersecting and adds them to a new collection.
                    var intersect = from n in close
                                    where !((intersectTest(n.GetPath(), l.GetRay())).IsEmpty(g))
                                    select n;
                    //If intersections were found
                    if (intersect.Count() > 0)
                    {
                        //Add new rocks based off of their size and increase the score.
                        //Then remove all the rocks
                        foreach (Rock r in intersect.ToList())
                        {
                            if (r.size == (int)rockSize.large)
                            {
                                for (int add = 0; add < 2; add++)
                                {
                                    _rocks.AddLast(new Rock(_CgameScale, r._pos, (int)rockSize.medium));
                                }
                                _score = _score + 100;
                            }
                            if (r.size == (int)rockSize.medium)
                            {
                                for (int add = 0; add < 3; add++)
                                {
                                    _rocks.AddLast(new Rock(_CgameScale, r._pos, (int)rockSize.small));
                                }
                                _score = _score + 200;
                            }
                            if (r.size == (int)rockSize.small)
                            {
                                _score = _score + 300;
                            }
                            _rocks.Remove(r);
                        }
                        //List of all the lasers that have hits targets
                        hits.Add(l);
                    }
                }
            }
            //Remove those lasers from the game
            hits.ForEach(l => _shots.Remove(l));
        }

        /// <summary>
        /// Checks if the ship had collided with rocks
        /// </summary>
        private void shipCollision()
        {
            Graphics g = CreateGraphics();
            //The ships alpha is <255 if it has been hit recently and is invulnerable.
            if (_ship.alpha > 254)
            {
                //Chcek if any rocks are close
                var close = from rock in _rocks
                            where (rock.alpha == 255 && ((distance(_ship._pos, rock._pos)) < (ShapeBase._tileSize*rock.size) || rock.wrapping))
                            select rock;
                if (close.Count() > 0)
                {
                    //Check if any of the shapes are intersecting and adds them to a new collection.
                    var intersect = from rock in close
                                    where !((intersectTest(rock.GetPath(), _ship.ShipPath())).IsEmpty(g))
                                    select rock;
                    //If intersections were found, reduce lives and check if the ship is dead.
                    if (intersect.Count() > 0)
                    {
                        _lives--;
                        _ship.shipDeath(_lives);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the distance between two points
        /// </summary>
        /// <param name="posA">First Point</param>
        /// <param name="posB">Second Point</param>
        /// <returns>The distance between them as a float</returns>
        private float distance(PointF posA, PointF posB)
        {
            return Math.Abs((float)Math.Sqrt((Math.Pow((posA.X - posB.X), 2) + Math.Pow((posA.Y - posB.Y), 2))));
        }

        //Returns the region of intersection between two graphics paths.
        private Region intersectTest(GraphicsPath firstPath, GraphicsPath secondPath)
        {
            Region reg = new Region(firstPath);
            reg.Intersect(secondPath);
            return reg;
        }
        /// <summary>
        /// Renders the HUD while the game is being played
        /// </summary>
        /// <param name="bg">BufferedGraphics to draw to</param>
        private void screenRender(BufferedGraphics bg)
        {
            //Add score, rocks and lives to the top corner.
            PointF pos = new PointF(-20, 100);
            Font f = new Font("Broadway", _CfontSize);
            Brush b = new SolidBrush(Color.White);
            bg.Graphics.DrawString("Score: " + _score, f, b, new Point(0, 0));        
            bg.Graphics.DrawString("Rocks: " + _rocks.Count(), f, b, new Point(0, 25));
            bg.Graphics.DrawString("Lives: " + _lives, f, b, new Point(0, 50));

            //If lives is less than 4, show lives as spaceships.
            if (_lives <= 4)
            {
                for (int s = 0; s < _lives; s++)
                {
                    pos = new PointF(pos.X + 50, 100);
                    _livesShip.setPos(pos);
                    _livesShip.Render(Color.White, bg);
                }
            }
            //If higher, draw one ship the the number as a multiple (Ship x5)
            else
            {
                pos = new PointF(pos.X + 50, 100);
                _livesShip.setPos(pos);
                _livesShip.Render(Color.White, bg);
                bg.Graphics.DrawString("x" + _lives, f, b, new Point(55, 75));
            }
        }

        /// <summary>
        /// Show paused on the screen so the user knows
        /// </summary>
        /// <param name="bg">BufferedGraphics to draw to</param>
        private void gamePause(BufferedGraphics bg)
        {
            Font f = new Font("Broadway", _CfontSize);
            Brush b = new SolidBrush(Color.White);
            bg.Graphics.DrawString("Paused", f, b, new Point(455, 200));
        }

        /// <summary>
        /// Main menu. Writes information the user needs to play.
        /// </summary>
        /// <param name="bg">BufferedGraphics to draw to</param>
        private void menu(BufferedGraphics bg)
        {
            //Draw title in a bigger font.
            Font f = new Font("Broadway", _CfontSize*3);
            Brush b = new SolidBrush(Color.White);
            bg.Graphics.DrawString("ASTEROIDS", f, b, new Point(325, 50));

            //Rest of the info in smaller font.
            f = new Font("Broadway", _CfontSize);
            bg.Graphics.DrawString("Press Fire to Begin", f, b, new Point(390, 125));
            //Keyboard controls
            bg.Graphics.DrawString("Keyboard Controls", f, b, new Point(150, 400));
            bg.Graphics.DrawString("Forward . . . W", f, b, new Point(150, 450));
            bg.Graphics.DrawString("Turn Left . . . A", f, b, new Point(150, 500));
            bg.Graphics.DrawString("Turn Right . .D", f, b, new Point(150, 550));
            bg.Graphics.DrawString("Fire . . . . Space", f, b, new Point(150, 600));
            bg.Graphics.DrawString("Pause . . . . P", f, b, new Point(150, 650));
            //Xbox controls
            bg.Graphics.DrawString("Xbox Controls", f, b, new Point(675, 400));
            bg.Graphics.DrawString("Forward . . . Dpad Up", f, b, new Point(675, 450));
            bg.Graphics.DrawString("Turn Left . . . Dpad Left", f, b, new Point(675, 500));
            bg.Graphics.DrawString("Turn Right . .Dpad Right", f, b, new Point(675, 550));
            bg.Graphics.DrawString("Fire . . . . A", f, b, new Point(675, 600));
            bg.Graphics.DrawString("Pause . . . . Start", f, b, new Point(675, 650));
        }
        /// <summary>
        /// If the game is over, draw gameover and score to user.
        /// </summary>
        /// <param name="bg">BufferedGraphics to draw to</param>
        private void gameOver(BufferedGraphics bg)
        {
            Font f = new Font("Broadway", _CfontSize * 3);
            Brush b = new SolidBrush(Color.White);
            bg.Graphics.DrawString("Game Over", f, b, new Point(320, 200));
            bg.Graphics.DrawString("Score: " + _score, f, b, new Point(330, 300));
        }
    }
}
