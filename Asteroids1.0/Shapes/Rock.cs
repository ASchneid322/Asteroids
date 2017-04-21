//***********************************************************************************
//Program:					Rock
//Description:	            Inherits Shapebase.
//                          Is a rock for the game.
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
    /// Rock class. Inhertics from ShapeBase class
    /// Creates a shapebase with a the shape of a rock which
    /// is defined as a shape with 4-12 sides, and a random
    /// vertex distance from the center for each point.
    /// /////////////////////////////
    public class Rock : ShapeBase
    {
        //Model for the rock to have.
        public GraphicsPath _model = new GraphicsPath();
        public bool wrapping = false;
        public bool doubleWrap = false;
        public int alpha;
        PointF wrapper = new PointF(500, 500);

        public int size;
        /// ///////////////////////
        ///Uses constructor from baseclass with no changes.
        /// ///////////////////////
        public Rock(float scale, PointF position, int sizeMulti = 1, int alpha = 50) : base(scale, position)
        {
            size = sizeMulti;
            _model = generateShape(s_rnd.Next(4, 13), _tileSize * sizeMulti, 0.5f);
            //_defModel = _model;
        }

        /// ///////////////////////
        /// GetPath() gets the current form the the shape,
        /// gets the rock shape and transforms it to 
        /// in the correct rotation and translation.
        /// Returns the graphicsPath.
        /// 
        /// If wrapping, will append more graphics paths.
        /// ///////////////////////

        public override GraphicsPath GetPath()
        {
            GraphicsPath tri = (GraphicsPath)_model.Clone();
            GraphicsPath wrap;
            Matrix matrice = new Matrix();
            matrice.Translate(_pos.X, _pos.Y);
            matrice.Rotate(_rot);
            matrice.Scale(_scale / 2, _scale / 2);
            tri.Transform(matrice);

            //If need to wrap, will add a new circle on the other side
            if (wrapping)
            {
                wrap = (GraphicsPath)_model.Clone();
                matrice = new Matrix();
                matrice.Translate(wrapper.X, wrapper.Y);
                matrice.Rotate(_rot);
                matrice.Scale(_scale / 2, _scale / 2);
                wrap.Transform(matrice);
                tri.AddPath(wrap, true);
                //If wrapping on both ways, will add more paths in all corners.
                if (doubleWrap)
                {
                    wrap = (GraphicsPath)_model.Clone();
                    matrice = new Matrix();
                    matrice.Translate(wrapper.X, _pos.Y);
                    matrice.Rotate(_rot);
                    matrice.Scale(_scale / 2, _scale / 2);
                    wrap.Transform(matrice);
                    tri.AddPath(wrap, true);

                    wrap = (GraphicsPath)_model.Clone();
                    matrice = new Matrix();
                    matrice.Translate(_pos.X, wrapper.Y);
                    matrice.Rotate(_rot);
                    matrice.Scale(_scale / 2, _scale / 2);
                    wrap.Transform(matrice);
                    tri.AddPath(wrap, true);
                }
            }
            return tri;
        }

        /// <summary>
        /// Renders the rock. Adds alpha to the set color.
        /// </summary>
        /// <param name="fillColor">Not used</param>
        /// <param name="bg">BufferedGraphics to draw to</param>
        public override void Render(Color fillColor, BufferedGraphics bg)
        {
            base.Render(Color.FromArgb(alpha, fillColor), bg);
        }

        /// <summary>
        /// Tick the rock.
        /// Check if the rock is outside the window, if it is halfway on the edge, will turn on wrapping
        /// If it is fully out of the window, it will be moved to the other side.
        /// Will increase the alpha of the rock if it is not 255.
        /// </summary>
        /// <param name="width">Width of window</param>
        /// <param name="height">Height of window</param>
        public override void Tick(int width, int height)
        {
            base.Tick(width, height);

            //If wrapping
            bool wrapX = false;
            bool wrapY = false;

            //Floats for the coords of the new position.
            float newX = _pos.X;
            float newY = _pos.Y;

            //Increase the alpha.
            if (alpha < 125)
                alpha += 5;
            else if (alpha < 210)
                alpha += 2;
            if (alpha > 210)
                alpha = 255;

            //Check if the rock is far outside or on the edge of X area.
            //If far outside, turn off wrapX
            if (_pos.X < (0 - (_tileSize * size) / 2))
            {
                _pos = new PointF(_pos.X + width, _pos.Y);
                wrapX = false;
            }
            else if (_pos.X > width + (_tileSize * size) / 2)
            {
                _pos = new PointF(_pos.X - width, _pos.Y);
                wrapX = false;
            }
            //If on the edges, turn on X wrap
            else if (_pos.X < (_tileSize * size) / 2)
            {
                wrapX = true;
                newX = _pos.X + width;
            }
            else if (_pos.X > width - (_tileSize * size) / 2)
            {
                wrapX = true;
                newX = _pos.X - width;
            }
            //If neither, no wrapping
            else
            {
                wrapX = false;
            }

            //Check if the rock is far outside or on the edge of Y area.
            //If far outside, turn off wrapY
            if (_pos.Y < 0 - (_tileSize * size) / 2)
            {
                _pos = new PointF(_pos.X, _pos.Y + height);
                wrapY = false;
            }
            else if (_pos.Y > height + (_tileSize * size) / 2)
            {
                _pos = new PointF(_pos.X, _pos.Y - height);
                wrapY = false;
            }
            //If on the edges, turn on Y wrap
            else if (_pos.Y < 0 + (_tileSize * size) / 2)
            {
                wrapY = true;
                newY = _pos.Y + height;
            }
            else if (_pos.Y > height - (_tileSize * size) / 2)
            {
                wrapY = true;
                newY = _pos.Y - height;
            }
            //If neither, no wrapping
            else
            {
                wrapY = false;
            }

            //If one direction is wrapping
            if (wrapY || wrapX)
            {
                //Turn on class wrapping bool for use during render.
                wrapping = true;
                //Gets position the wrap needs to be at
                wrapper = new PointF(newX, newY);
                //If both are wrapping, doubleWrap for 4 corner render
                if (wrapY && wrapX)
                    doubleWrap = true;
                else
                    doubleWrap = false;
            }
            //Wrapping false if both are off.
            else
            {
                wrapping = false;
            }
        }
    }
}
