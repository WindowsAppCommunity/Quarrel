// Quarrel © 2022

using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;

namespace Quarrel.SubPages.DiscordStatus
{
    public partial class DiscordStatusPage
    {
        /// <summary>
        /// Gets or sets the width of a point on the graph.
        /// </summary>
        public float StepSize { get; set; }
        
        /// <summary>
        /// Render the data.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        /// <param name="args">EventArgs.</param>
        /// <param name="color">Color to render the lines with.</param>
        /// <param name="thickness">Thickness of the lines.</param>
        /// <param name="data">Data Points.</param>
        /// <param name="renderArea">Render area under line.</param>
        /// <param name="max">Largest Y scale to render for.</param>
        public void RenderData(CanvasControl canvas, CanvasDrawEventArgs args, Color color, float thickness, List<double> data, bool renderArea, double max)
        {
            if (data.Count == 0)
            {
                return;
            }

            // Each data point gets equal area
            StepSize = Convert.ToSingle(canvas.ActualWidth / data.Count);

            using var cpb = new CanvasPathBuilder(args.DrawingSession);
            
            // Begin at bottom
            cpb.BeginFigure(new Vector2(0, (float)(canvas.ActualHeight * (1 - (data[0] / max)))));

            // Add data
            for (int i = 1; i < data.Count; i++)
            {
                cpb.AddLine(new Vector2(StepSize * i, (float)(canvas.ActualHeight * (1 - (data[i] / max)))));
            }

            if (renderArea)
            {
                cpb.AddLine(new Vector2(data.Count, (float)canvas.ActualHeight));
                cpb.AddLine(new Vector2(0, (float)canvas.ActualHeight));
                cpb.EndFigure(CanvasFigureLoop.Closed);

                // Draw shape
                args.DrawingSession.FillGeometry(CanvasGeometry.CreatePath(cpb), Windows.UI.Colors.LightGreen);
            }
            else
            {
                cpb.EndFigure(CanvasFigureLoop.Open);

                // Draw line
                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), color, thickness);
            }
        }
    }
}
