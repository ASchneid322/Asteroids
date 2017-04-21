//***********************************************************************************
//Program:					Scrap
//Description:	            Inherits Shapebase.
//                          A scrap of ship for death.
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
    /// Class is small pieces of the ship used on ship death.
    /// </summary>
    class Scrap : ShapeBase
    {
        //Model for the scrap
        private GraphicsPath _model = new GraphicsPath();
        //Color for the scrap.
        private Color _c;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale">Scale to render</param>
        /// <param name="pos">Starting positon</param>
        /// <param name="shape">Shape for the scrap to have</param>
        /// <param name="c">Color for the scrap to have</param>
        public Scrap(float scale, PointF pos, GraphicsPath shape, Color c) :base(scale, pos)
        {
            _model = (GraphicsPath)shape.Clone();
            _c = c;
        }
        /// <summary>
        /// Gets the model, transforms it and returns it.
        /// </summary>
        /// <returns>GraphicsPath of the transformed model</returns>
        public override GraphicsPath GetPath()
        {
            GraphicsPath scrap = (GraphicsPath)_model.Clone();

            Matrix matrice = new Matrix();
            matrice.Translate(_pos.X, _pos.Y);
            matrice.Scale(_scale, _scale);
            matrice.Rotate(_rot);
            scrap.Transform(matrice);
            return scrap;
        }
        /// <summary>
        /// Renders the scrap. Uses the set color instead of whats sent in
        /// </summary>
        /// <param name="fillColor">Not used</param>
        /// <param name="bg">BufferedGraphics to draw to</param>
        public override void Render(Color fillColor, BufferedGraphics bg)
        {
            base.Render(_c, bg);
        }
    }
}
