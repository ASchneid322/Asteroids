//***********************************************************************************
//Program:					Star
//Description:	            Inherits Shapebase.
//                          Background star for the game
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
    /// Star class.
    /// Background stars that pulsate and spin.
    /// </summary>
    class Star : ShapeBase
    {
        //Model of the star
        public GraphicsPath _model = new GraphicsPath();
        //Alpha to draw with
        public int alpha;
        //If it is increasing or decreasing in alpha
        private bool glow = false;

        /// <summary>
        /// Makes a new star with a shape from generateShape.
        /// Random alpha between 50 and 254
        /// </summary>
        /// <param name="scale">Scale of the star to draw</param>
        /// <param name="pos">Position to draw the star in</param>
        public Star(float scale, PointF pos) : base(scale, pos)
        {
            _model = generateShape(s_rnd.Next(9, 13), _tileSize / 15, 0.5f);
            alpha = s_rnd.Next(50, 255);
        }

        /// <summary>
        /// Gets the model, transforms it and returns it.
        /// </summary>
        /// <returns>GraphicsPath of the transformed model</returns>
        public override GraphicsPath GetPath()
        {
            GraphicsPath tri = (GraphicsPath)_model.Clone();
            Matrix matrice = new Matrix();
            matrice.Translate(_pos.X, _pos.Y);
            matrice.Rotate(_rot);
            matrice.Scale(_scale / 2, _scale / 2);
            tri.Transform(matrice);
            return tri;
        }

        /// <summary>
        /// Renders the star. Adds alpha to the set color.
        /// </summary>
        /// <param name="fillColor">Not used</param>
        /// <param name="bg">BufferedGraphics to draw to</param>
        public override void Render(Color fillColor, BufferedGraphics bg)
        {
            base.Render(Color.FromArgb(alpha, fillColor), bg);
        }

        /// <summary>
        /// Tick the star. Changes the alpha and the rotation.
        /// </summary>
        /// <param name="width">Width of window</param>
        /// <param name="height">Height of window</param>
        public override void Tick(int width, int height)
        {
            _rot += _rotInc/4;
            if (glow)
                alpha += 2;
            else
                alpha -= 2;
            if (alpha > 250)
                glow = false;
            else if (alpha < 25)
                glow = true;
        }
    }
}
