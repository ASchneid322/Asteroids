//***********************************************************************************
//Program:					Ship
//Description:	            Inherits Shapebase.
//                          Ship for the player to use
//Date:						March 24th 2017
//Authors:					Alexander Schneider
//Course:					CMPE2800
//Class:					CNTA02
//***********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Asteroids1._0
{
    /// /////////////////////////////
    /// Ship class 
    /// ////////////////////////////
    public class Ship : ShapeBase
    {
        //The models for all the parts of the ship.
        private GraphicsPath _model = new GraphicsPath();
        private GraphicsPath _modelRed = new GraphicsPath();
        private GraphicsPath _thrust = new GraphicsPath();
        private GraphicsPath _blue = new GraphicsPath();
        //Left and Right of ship used in death animations.
        private GraphicsPath _modelL = new GraphicsPath();
        private GraphicsPath _modelR = new GraphicsPath();

        //How fast the ship turns
        public float _turnSpeed;
        //How fast the ship moves
        public float _moveSpeed;
        //If thrust is on
        public bool thrust;
        //Alpha for ship. Used when taking hits
        public int alpha = 255;
        //If the ship is dead
        private bool dead = false;
        //List of parts for death sequence
        List<Scrap> deathParts;

        public Ship(float scale, PointF position) : base(scale, position)
        {
            //Gets all the models used for rendering the ship.
            _model = getShipWhite();
            _thrust = getShipThrust();
            _modelRed = getShipRed();
            _blue = getShipBlue();
            _modelL = getShipWhiteL();
            _modelR = getShipWhiteR();
            //Turns speed of ship      
            _turnSpeed = 5;
            //Moves speed
            _moveSpeed = 5;
            //Current rotation
            _rot = 0;
            //How much to change rotation by
            _rotInc = 0;
            //Speed in direction
            _xSpeed = 0;
            _ySpeed = 0;
        }
        
        /// <summary>
        /// Sets positon of the ship
        /// </summary>
        /// <param name="newPos"></param>
        public void setPos(PointF newPos)
        {
            _pos = newPos;
        }

        /// <summary>
        /// Renders the ship based off of its state.
        /// </summary>
        /// <param name="fillColor"></param>
        /// <param name="bg"></param>
        public override void Render(Color fillColor, BufferedGraphics bg)
        {
            if (dead)
            {
                //Render each part of its death
                deathParts.ForEach(s => s.Render(Color.Red, bg));
            }
            else
            {
                //If moving forward, draw thruster
                if (thrust)
                    bg.Graphics.FillPath(new SolidBrush(Color.FromArgb(alpha, Color.Red)), ThrustPath());
                //Draw the three colored regions that make up the ship
                bg.Graphics.FillPath(new SolidBrush(Color.FromArgb(alpha, Color.White)), ShipPath());
                bg.Graphics.FillRegion(new SolidBrush(Color.FromArgb(alpha, Color.Red)), RedRegion());
                bg.Graphics.FillRegion(new SolidBrush(Color.FromArgb(alpha, Color.Blue)), BlueRegion());
                //If invulnerable, draw a barrier.
                if (alpha<255)
                    bg.Graphics.DrawEllipse(new Pen(Color.FromArgb(255-alpha, Color.Aqua)), _pos.X - _tileSize / 2, _pos.Y - _tileSize / 2, _tileSize, _tileSize );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void Tick(int width, int height)
        {
            //New coordinates if needs to be moved
            float newX = _pos.X;
            float newY = _pos.Y;

            //Increase the alpha if not at 255.
            if (alpha < 255)
                alpha += 2;
            else if (alpha > 255)
                alpha = 255;

            //If outside x, reverse direction and set to the bounds of the window.
            if (_pos.X > width)
            {
                newX = width;
            }
            else if (_pos.X < 0)
            {
                newX = 0;
            }
            //If outside Y, reverse direction and set to the bounds of the window.
            if (_pos.Y > height)
            {
                newY = height;
            }
            else if (_pos.Y < 0)
            {
                newY = 0;
            }
            //Set the new position if needed
            _pos = new PointF(newX + _xSpeed, newY + _ySpeed);
            if (dead)
            {
                deathParts.ForEach(s => s.Tick(width,height));
            }
        }

        /// <summary>
        /// If the player is out of lives, set the ship to dead.
        /// Add parts to the list for death animation
        /// </summary>
        /// <param name="lives"></param>
        public void shipDeath(int lives)
        {
            if (lives == 0)
            {
                dead = true;
                //Add parts the makeup the ship as scraps to fly around
                deathParts = new List<Scrap>();
                deathParts.Add(new Scrap(_scale, _pos, RedPath(0, 0), Color.Red));
                deathParts.Add(new Scrap(_scale, _pos, RedPath(0, 0), Color.Red));
                deathParts.Add(new Scrap(_scale, _pos, RedPath(0, 0), Color.Red));
                deathParts.Add(new Scrap(_scale, _pos, RedPath(0, 0), Color.Red));
                deathParts.Add(new Scrap(_scale, _pos, BluePath(0, 0), Color.Blue));
                deathParts.Add(new Scrap(_scale, _pos, BluePath(0, 0), Color.Blue));
                deathParts.Add(new Scrap(_scale, _pos, BluePath(0, 0), Color.Blue));
                deathParts.Add(new Scrap(_scale, _pos, ShipPathL(), Color.White));
                deathParts.Add(new Scrap(_scale, _pos, ShipPathR(), Color.White));
            }
            //If still have lives, set alpha to 99.
            else
                alpha = 99;
        }

        //Gets the ships rotation
        public float shipRotation()
        {
            return _rot;
        }

        /// <summary>
        /// Turns the ship to the left
        /// </summary>
        public void rotateLeft()
        {
            _rot -= _turnSpeed;
        }
        /// <summary>
        /// Turns the ship to the right
        /// </summary>
        public void rotateRight()
        {
            _rot += _turnSpeed;
        }
        /// <summary>
        /// Moves the ship forward with some math.
        /// </summary>
        public void moveForward()
        {
            _pos = new PointF(_pos.X - (float)Math.Sin(-_rot * (Math.PI / 180)) * _moveSpeed
                , _pos.Y - (float)Math.Cos(-_rot * (Math.PI / 180)) * _moveSpeed);
        }

        /// <summary>
        /// Gets the model, translates it to the correct spot and returns
        /// </summary>
        /// <returns>GraphisPath of translated ship</returns>
        public GraphicsPath ShipPath()
        {
            GraphicsPath tri = (GraphicsPath)_model.Clone();

            Matrix matrice = new Matrix();

            matrice.Translate(_pos.X, _pos.Y);
            matrice.Scale(_scale, _scale);
            matrice.Rotate(_rot);
            tri.Transform(matrice);

            return tri;
        }

        /// <summary>
        /// Graphics path of half the ship.
        /// </summary>
        /// <returns>Transformed graphics path</returns>
        public GraphicsPath ShipPathL()
        {
            GraphicsPath tri = (GraphicsPath)_modelL.Clone();

            Matrix matrice = new Matrix();

            matrice.Rotate(_rot);
            tri.Transform(matrice);

            return tri;
        }

        /// <summary>
        /// Graphics path of half the ship.
        /// </summary>
        /// <returns>Transformed graphics path</returns>
        public GraphicsPath ShipPathR()
        {
            GraphicsPath tri = (GraphicsPath)_modelR.Clone();

            Matrix matrice = new Matrix();

            matrice.Rotate(_rot);
            tri.Transform(matrice);

            return tri;
        }

        /// <summary>
        /// Adds a bunch of RedPaths together and unions them
        /// </summary>
        /// <returns>Region of all the red paths.</returns>
        public Region RedRegion()
        {
            GraphicsPath tri = RedPath(0, 0);
            Region r = new Region(tri);

            r.Union(RedPath(1, 1));
            r.Union(RedPath(-1, 1));

            r.Union(RedPath(4, -2 ));
            r.Union(RedPath(-4, -2));

            r.Union(RedPath(7, 0));
            r.Union(RedPath(-7, 0));

            r.Union(RedPath(-2, 4));
            r.Union(RedPath(-2, 5));
            r.Union(RedPath(-3, 5));

            r.Union(RedPath(2, 4));
            r.Union(RedPath(2, 5));
            r.Union(RedPath(3, 5));

            Matrix matrice = new Matrix();
            matrice.Translate(_pos.X, _pos.Y);
            matrice.Rotate(_rot);
            matrice.Scale(_scale, _scale);
            r.Transform(matrice);
            return r;
        }

        /// <summary>
        /// Adds a bunch of BluePaths together and unions them
        /// </summary>
        /// <returns>Region of all the blue paths.</returns>
        public Region BlueRegion()
        {
            GraphicsPath tri = BluePath(3, -1);
            Region r = new Region(tri);
            r.Union(BluePath(4, 0));
            r.Union(BluePath(-4, 0));
            r.Union(BluePath(-3, -1));

            Matrix matrice = new Matrix();
            matrice.Translate(_pos.X, _pos.Y);
            matrice.Rotate(_rot);
            matrice.Scale(_scale, _scale);
            r.Transform(matrice);
            return r;
        }

        /// <summary>
        /// Gets the thrust and adds it to the ship
        /// </summary>
        /// <returns>Translated thrust path</returns>
        public GraphicsPath ThrustPath()
        {
            GraphicsPath tri = (GraphicsPath)_thrust.Clone();

            Matrix matrice = new Matrix();

            matrice.Translate(_pos.X, _pos.Y);
            matrice.Scale(_scale/2, _scale/2);
            matrice.Rotate(_rot);
            tri.Transform(matrice);

            return tri;
        }

        /// <summary>
        /// Gets the red model, and moves it to the specified spots
        /// </summary>
        /// <param name="xMove"></param>
        /// <param name="yMove"></param>
        /// <returns>Red path moved to the correct spot</returns>
        public GraphicsPath RedPath(float xMove, float yMove)
        {
            GraphicsPath tri = (GraphicsPath)_modelRed.Clone();
            Matrix matrice = new Matrix();
            matrice.Translate(xMove, yMove);
            tri.Transform(matrice);
            return tri;
        }
        /// <summary>
        /// Gets the blue model, and moves it to the specified spots
        /// </summary>
        /// <param name="xMove"></param>
        /// <param name="yMove"></param>
        /// <returns>Blue path moved to the correct spot</returns>
        public GraphicsPath BluePath(float xMove, float yMove, float rot = 0)
        {
            GraphicsPath tri = (GraphicsPath)_blue.Clone();
            Matrix matrice = new Matrix();
            matrice.Translate(xMove, yMove);
            matrice.Rotate(rot);
            tri.Transform(matrice);
            return tri;
        }

        /// <summary>
        /// Points to draw the ship.
        /// </summary>
        /// <returns>Graphics path of the ship</returns>
        public static GraphicsPath getShipWhite()
        {
            //New model to store the path
            GraphicsPath model = new GraphicsPath();
            PointF[] pts = new PointF[53];
            pts[0] = new PointF(0, -9);
            pts[1] = new PointF(-0.5f, -9);
            pts[2] = new PointF(-0.5f, -6);
            pts[3] = new PointF(-1.5f, -6);
            pts[4] = new PointF(-1.5f, -6);
            pts[5] = new PointF(-1.5f, -2);
            pts[6] = new PointF(-2.5f, -2);
            pts[7] = new PointF(-2.5f, -1);
            pts[8] = new PointF(-4.5f, -1);
            pts[9] = new PointF(-4.5f, 2);
            pts[10] = new PointF(-5.5f, 2);
            pts[11] = new PointF(-5.5f, 3);
            pts[12] = new PointF(-6.5f, 3);
            pts[13] = new PointF(-6.5f, 1);
            pts[14] = new PointF(-7.5f, 1);
            pts[15] = new PointF(-7.5f, 7);
            pts[16] = new PointF(-6.5f, 7);
            pts[17] = new PointF(-6.5f, 6);
            pts[18] = new PointF(-5.5f, 6);
            pts[19] = new PointF(-5.5f, 5);
            pts[20] = new PointF(-4.5f, 5);
            pts[21] = new PointF(-4.5f, 4);
            pts[22] = new PointF(-1.5f, 4);
            pts[23] = new PointF(-1.5f, 5);
            pts[24] = new PointF(-0.5f, 5);
            pts[25] = new PointF(-0.5f, 7);
            pts[26] = new PointF(0, 7);
            pts[27] = new PointF(0.5f, 7);
            pts[28] = new PointF(0.5f, 5);
            pts[29] = new PointF(1.5f, 5);
            pts[30] = new PointF(1.5f, 4);
            pts[31] = new PointF(4.5f, 4);
            pts[32] = new PointF(4.5f, 5);
            pts[33] = new PointF(5.5f, 5);
            pts[34] = new PointF(5.5f, 6);
            pts[35] = new PointF(6.5f, 6);
            pts[36] = new PointF(6.5f, 7);
            pts[37] = new PointF(7.5f, 7);
            pts[38] = new PointF(7.5f, 1);
            pts[39] = new PointF(6.5f, 1);
            pts[40] = new PointF(6.5f, 3);
            pts[41] = new PointF(5.5f, 3);
            pts[42] = new PointF(5.5f, 2);
            pts[43] = new PointF(4.5f, 2);
            pts[44] = new PointF(4.5f, -1);
            pts[45] = new PointF(2.5f, -1);
            pts[46] = new PointF(2.5f, -2);
            pts[47] = new PointF(1.5f, -2);
            pts[48] = new PointF(1.5f, -6);
            pts[49] = new PointF(1.5f, -6);
            pts[50] = new PointF(0.5f, -6);
            pts[51] = new PointF(0.5f, -9);
            pts[52] = new PointF(0, -9);
            model.StartFigure();
            model.AddPolygon(pts);
            model.CloseFigure();
            return model;
        }

        /// <summary>
        /// Left half of ship
        /// </summary>
        /// <returns>Left half of ship</returns>
        public static GraphicsPath getShipWhiteL()
        {
            //New model to store the path
            GraphicsPath model = new GraphicsPath();
            PointF[] pts = new PointF[53];
            pts[26] = new PointF(0, 7);
            pts[27] = new PointF(0.5f, 7);
            pts[28] = new PointF(0.5f, 5);
            pts[29] = new PointF(1.5f, 5);
            pts[30] = new PointF(1.5f, 4);
            pts[31] = new PointF(4.5f, 4);
            pts[32] = new PointF(4.5f, 5);
            pts[33] = new PointF(5.5f, 5);
            pts[34] = new PointF(5.5f, 6);
            pts[35] = new PointF(6.5f, 6);
            pts[36] = new PointF(6.5f, 7);
            pts[37] = new PointF(7.5f, 7);
            pts[38] = new PointF(7.5f, 1);
            pts[39] = new PointF(6.5f, 1);
            pts[40] = new PointF(6.5f, 3);
            pts[41] = new PointF(5.5f, 3);
            pts[42] = new PointF(5.5f, 2);
            pts[43] = new PointF(4.5f, 2);
            pts[44] = new PointF(4.5f, -1);
            pts[45] = new PointF(2.5f, -1);
            pts[46] = new PointF(2.5f, -2);
            pts[47] = new PointF(1.5f, -2);
            pts[48] = new PointF(1.5f, -6);
            pts[49] = new PointF(1.5f, -6);
            pts[50] = new PointF(0.5f, -6);
            pts[51] = new PointF(0.5f, -9);
            pts[52] = new PointF(0, -9);
            model.StartFigure();
            model.AddPolygon(pts);
            model.CloseFigure();
            return model;
        }

        /// <summary>
        /// Right half of ship
        /// </summary>
        /// <returns>Right half of ship</returns>
        public static GraphicsPath getShipWhiteR()
        {
            //New model to store the path
            GraphicsPath model = new GraphicsPath();
            PointF[] pts = new PointF[53];
            pts[0] = new PointF(0, -9);
            pts[1] = new PointF(-0.5f, -9);
            pts[2] = new PointF(-0.5f, -6);
            pts[3] = new PointF(-1.5f, -6);
            pts[4] = new PointF(-1.5f, -6);
            pts[5] = new PointF(-1.5f, -2);
            pts[6] = new PointF(-2.5f, -2);
            pts[7] = new PointF(-2.5f, -1);
            pts[8] = new PointF(-4.5f, -1);
            pts[9] = new PointF(-4.5f, 2);
            pts[10] = new PointF(-5.5f, 2);
            pts[11] = new PointF(-5.5f, 3);
            pts[12] = new PointF(-6.5f, 3);
            pts[13] = new PointF(-6.5f, 1);
            pts[14] = new PointF(-7.5f, 1);
            pts[15] = new PointF(-7.5f, 7);
            pts[16] = new PointF(-6.5f, 7);
            pts[17] = new PointF(-6.5f, 6);
            pts[18] = new PointF(-5.5f, 6);
            pts[19] = new PointF(-5.5f, 5);
            pts[20] = new PointF(-4.5f, 5);
            pts[21] = new PointF(-4.5f, 4);
            pts[22] = new PointF(-1.5f, 4);
            pts[23] = new PointF(-1.5f, 5);
            pts[24] = new PointF(-0.5f, 5);
            pts[25] = new PointF(-0.5f, 7);
            pts[26] = new PointF(0, 7);
            model.StartFigure();
            model.AddPolygon(pts);
            model.CloseFigure();
            return model;
        }

        /// <summary>
        /// Square redpart of the ship.
        /// </summary>
        /// <returns>Square redpart of the ship</returns>
        public static GraphicsPath getShipRed()
        {
            GraphicsPath model = new GraphicsPath();
            PointF[] pts = new PointF[20];
            pts[0] = new PointF(0, 1);
            pts[1] = new PointF(0.5f, 1);
            pts[2] = new PointF(0.5f, -1);
            pts[3] = new PointF(-0.5f, -1);
            pts[4] = new PointF(-0.5f, 1);
            pts[5] = new PointF(0, 1);
            model.StartFigure();
            model.AddPolygon(pts);
            model.CloseFigure();
            return model;
        }
        /// <summary>
        /// Points for thrust of the ship
        /// </summary>
        public static GraphicsPath getShipThrust()
        {
            //New model to store the path
            GraphicsPath model = new GraphicsPath();
            PointF[] pts = new PointF[4];
            pts[0] = new PointF(0, 4);
            pts[1] = new PointF(10, 4);
            pts[2] = new PointF(0, 30);
            pts[3] = new PointF(-10, 4);
            model.StartFigure();
            model.AddPolygon(pts);
            model.CloseFigure();
            return model;
        }
        /// <summary>
        /// Blue parts of the ship
        /// </summary>
        public static GraphicsPath getShipBlue()
        {
            GraphicsPath model = new GraphicsPath();
            PointF[] pts = new PointF[4];
            pts[0] = new PointF(-0.5f, 0);
            pts[1] = new PointF(0.5f, 0);
            pts[2] = new PointF(0.5f, 1);
            pts[3] = new PointF(-0.5f, 1);
            model.StartFigure();
            model.AddPolygon(pts);
            model.CloseFigure();
            return model;
        }
    }
}
