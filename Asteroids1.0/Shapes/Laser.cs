//***********************************************************************************
//Program:					Laser
//Description:	            Inherits Shapebase.
//                          Is laser for ship.
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
    /// <summary>
    /// Laser of the ship
    /// </summary>
    public class Laser : ShapeBase
    {
        //Model to hold the shape
        private GraphicsPath _model = new GraphicsPath();
        //If it is outside the field
        public bool outside { private set; get; } = false;

        /// <summary>
        /// Creats a new laser at the selected position. 
        /// </summary>
        /// <param name="scale">Scale ot draw at</param>
        /// <param name="positon">Position to start at</param>
        /// <param name="rotation">Rotation of the ship to set the x and y speed</param>
        public Laser(float scale, PointF positon, float rotation) : base(scale, positon)
        {
            _model = getLaser();
            _xSpeed = 20 * (float)Math.Sin(rotation * (Math.PI / 180));
            _ySpeed = -20 * (float)Math.Cos(rotation * (Math.PI / 180));
            _rot = rotation;
            _rotInc = 0;
        }

        /// <summary>
        /// Gets the path of the laser and translatesd it
        /// </summary>
        /// <returns>Translated graphics path</returns>
        public override GraphicsPath GetPath()
        {
            GraphicsPath laser = (GraphicsPath)_model.Clone();

            Matrix matrice = new Matrix();
            matrice.Translate(_pos.X, _pos.Y);
            //Scale/4 to be the right size compared to ship and rest
            matrice.Scale(_scale/4, _scale/4);
            matrice.Rotate(_rot);
            laser.Transform(matrice);
            return laser;
        }
        /// <summary>
        /// Gets a streched laser path to check for collision against.
        /// Fills in areas that the laser might skip on its travel path.
        /// </summary>
        /// <returns>Laser transformed to be longer</returns>
        public GraphicsPath GetRat()
        {
            GraphicsPath laser = (GraphicsPath)_model.Clone();
            Matrix matrice = new Matrix();
            matrice.Translate(_pos.X, _pos.Y);
            matrice.Scale(_scale/4, (float)(_scale*2));
            matrice.Rotate(_rot);
            laser.Transform(matrice);
            return laser;
        }

        public GraphicsPath GetRay()
        {
            GraphicsPath model = new GraphicsPath();

            model.AddLine(new PointF(_pos.X, _pos.Y), new PointF(_pos.X + _xSpeed, _pos.Y + _ySpeed));
            model.Widen(new Pen(Color.Green, 8));
            return model;
        }

        public override void Render(Color fillColor, BufferedGraphics bg)
        {
            //bg.Graphics.DrawPath(new Pen(fillColor), GetRay());
            bg.Graphics.FillPath(new SolidBrush(fillColor), GetPath());
        }

        /// <summary>
        /// If its outside, set bool so it will be removed.
        /// </summary>
        /// <param name="width">Width of play field</param>
        /// <param name="height"> Height of play field </param>
        public override void Tick(int width, int height)
        {
            base.Tick(width, height);
            if (_pos.X < 0 || _pos.X > width)
                outside = true;
            if (_pos.Y < 0 || _pos.Y > height)
                outside = true;
        }

        /// <summary>
        /// Points for a laser
        /// </summary>
        /// <returns>Graphics path of default laser</returns>
        public static GraphicsPath getLaser()
        {
            GraphicsPath model = new GraphicsPath();
            PointF[] pts = new PointF[28];

            pts[0] = new PointF(0, -5);
            pts[1] = new PointF(-1, -5);
            pts[2] = new PointF(-1, -4);
            pts[3] = new PointF(-2, -4);
            pts[4] = new PointF(-2, -3);
            pts[5] = new PointF(-3, -3);
            pts[6] = new PointF(-3, -0);
            pts[7] = new PointF(-2, -0);
            pts[8] = new PointF(-2, 2);
            pts[9] = new PointF(-1, 2);
            pts[10] = new PointF(-1, 4);
            pts[11] = new PointF(-0.5f, 4);
            pts[12] = new PointF(-0.5f, 6);
            pts[13] = new PointF(-0, 6);

            pts[14] = new PointF(0, 6);
            pts[15] = new PointF(0.5f, 6);
            pts[16] = new PointF(0.5f, 4);
            pts[17] = new PointF(1, 4);
            pts[18] = new PointF(1, 2);
            pts[19] = new PointF(2, 2);
            pts[20] = new PointF(2, -0);
            pts[21] = new PointF(3, -0);
            pts[22] = new PointF(3, -3);
            pts[23] = new PointF(2, -3);
            pts[24] = new PointF(2, -4);
            pts[25] = new PointF(1, -4);
            pts[26] = new PointF(1, -5);
            pts[27] = new PointF(0, -5);

            model.StartFigure();
            model.AddPolygon(pts);
            model.CloseFigure();
            return model;
        }

    }
}
