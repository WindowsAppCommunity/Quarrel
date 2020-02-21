// Copyright (c) Quarrel. All rights reserved.

using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Quarrel.Helpers
{
    /// <summary>
    /// Class to assist with rendering the DiscordStatus graph.
    /// </summary>
    public class ChartRenderer
    {
        /// <summary>
        /// Gets or sets the width of a point on the graph.
        /// </summary>
        public float StepSize { get; set; }

        /// <summary>
        /// Render the graph without data.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        /// <param name="args">EventArgs.</param>
        public void RenderAxes(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var width = (float)canvas.ActualWidth;
            var height = (float)canvas.ActualHeight;
            var midWidth = (float)(width * .5);
            var midHeight = (float)(height * .5);

            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                // Horizontal line
                cpb.BeginFigure(new Vector2(0, midHeight));
                cpb.AddLine(new Vector2(width, midHeight));
                cpb.EndFigure(CanvasFigureLoop.Open);

                // Horizontal line arrow
                cpb.BeginFigure(new Vector2(width - 10, midHeight - 3));
                cpb.AddLine(new Vector2(width, midHeight));
                cpb.AddLine(new Vector2(width - 10, midHeight + 3));
                cpb.EndFigure(CanvasFigureLoop.Open);

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), Windows.UI.Colors.Gray, 1);
            }

            args.DrawingSession.DrawText("0", 5, midHeight - 30, Windows.UI.Colors.Gray);
            args.DrawingSession.DrawText(canvas.ActualWidth.ToString(), width - 50, midHeight - 30, Windows.UI.Colors.Gray);

            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                // Vertical line
                cpb.BeginFigure(new Vector2(midWidth, 0));
                cpb.AddLine(new Vector2(midWidth, height));
                cpb.EndFigure(CanvasFigureLoop.Open);

                // Vertical line arrow
                cpb.BeginFigure(new Vector2(midWidth - 3, 10));
                cpb.AddLine(new Vector2(midWidth, 0));
                cpb.AddLine(new Vector2(midWidth + 3, 10));
                cpb.EndFigure(CanvasFigureLoop.Open);

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), Windows.UI.Colors.Gray, 1);
            }

            args.DrawingSession.DrawText("0", midWidth + 5, height - 30, Windows.UI.Colors.Gray);
            args.DrawingSession.DrawText("1", midWidth + 5, 5, Windows.UI.Colors.Gray);
        }

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

            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
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

        /// <summary>
        /// Render values as Columns.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        /// <param name="args">Event Args.</param>
        /// <param name="columnAvgDataRange">Not sure.</param>
        /// <param name="columnWidth">Width to render columns.</param>
        /// <param name="data">Data (as doubles).</param>
        public void RenderAveragesAsColumns(CanvasControl canvas, CanvasDrawEventArgs args, int columnAvgDataRange, float columnWidth, List<double> data)
        {
            var padding = .5 * (columnAvgDataRange - columnWidth);

            for (int start = 0; start < data.Count; start += columnAvgDataRange)
            {
                double total = 0;
                var range = Math.Min(columnAvgDataRange, data.Count - start);

                for (int i = start; i < start + range; i++)
                {
                    total += data[i];
                }

                args.DrawingSession.FillRectangle(
                    start + (float)padding,
                    (float)(canvas.ActualHeight * (1 - (total / range))),
                    columnWidth,
                    (float)(canvas.ActualHeight * (total / range)),
                    Windows.UI.Colors.WhiteSmoke);
            }
        }

        /// <summary>
        /// Render values in a PieChart.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        /// <param name="args">Event Args.</param>
        /// <param name="pieValues">Data (as doubles).</param>
        /// <param name="palette">Colors for data (in order of size).</param>
        public void RenderAveragesAsPieChart(CanvasControl canvas, CanvasDrawEventArgs args, List<double> pieValues, List<Color> palette)
        {
            var total = pieValues.Sum();

            var w = (float)canvas.ActualWidth;
            var h = (float)canvas.ActualHeight;
            var midx = w / 2;
            var midy = h / 2;
            var padding = 50;
            var lineOffset = 20;
            var r = (Math.Min(w, h) / 2) - padding;

            float angle = 0f;
            var center = new Vector2(midx, midy);

            for (int i = 0; i < pieValues.Count; i++)
            {
                float sweepAngle = (float)(2 * Math.PI * pieValues[i] / total);
                var arcStartPoint = new Vector2((float)(midx + (r * Math.Sin(angle))), (float)(midy - (r * Math.Cos(angle))));

                using (var cpb = new CanvasPathBuilder(args.DrawingSession))
                {
                    cpb.BeginFigure(center);
                    cpb.AddLine(arcStartPoint);
                    cpb.AddArc(new Vector2(midx, midy), r, r, angle - (float)(Math.PI / 2), sweepAngle);
                    cpb.EndFigure(CanvasFigureLoop.Closed);
                    args.DrawingSession.FillGeometry(CanvasGeometry.CreatePath(cpb), palette[i % palette.Count]);
                }

                angle += sweepAngle;
            }

            angle = 0f;

            var lineBrush = new CanvasSolidColorBrush(args.DrawingSession, Windows.UI.Colors.Black);

            for (int i = 0; i < pieValues.Count; i++)
            {
                float sweepAngle = (float)(2 * Math.PI * pieValues[i] / total);
                var midAngle = angle + (sweepAngle / 2);
                var isRightHalf = midAngle < Math.PI;
                var isTopHalf = midAngle <= Math.PI / 2 || midAngle >= Math.PI * 3 / 2;
                var p0 = new Vector2((float)(midx + ((r - lineOffset) * Math.Sin(midAngle))), (float)(midy - ((r - lineOffset) * Math.Cos(midAngle))));
                var p1 = new Vector2((float)(midx + ((r + lineOffset) * Math.Sin(midAngle))), (float)(midy - ((r + lineOffset) * Math.Cos(midAngle))));
                var p2 = isRightHalf ? new Vector2(p1.X + 50, p1.Y) : new Vector2(p1.X - 50, p1.Y);

                using (var cpb = new CanvasPathBuilder(args.DrawingSession))
                {
                    cpb.BeginFigure(p0);
                    cpb.AddLine(p1);
                    cpb.AddLine(p2);
                    cpb.EndFigure(CanvasFigureLoop.Open);

                    args.DrawingSession.DrawGeometry(
                        CanvasGeometry.CreatePath(cpb),
                        lineBrush,
                        1);
                }

                args.DrawingSession.DrawText(
                    pieValues[i].ToString("F2"),
                    p1,
                    Windows.UI.Colors.Black,
                    new CanvasTextFormat
                    {
                        HorizontalAlignment = isRightHalf ? CanvasHorizontalAlignment.Left : CanvasHorizontalAlignment.Right,
                        VerticalAlignment = isTopHalf ? CanvasVerticalAlignment.Bottom : CanvasVerticalAlignment.Top,
                        FontSize = 18,
                    });

                angle += sweepAngle;
            }
        }

        /// <summary>
        /// Render data along with a moving average.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        /// <param name="args">Event Args.</param>
        /// <param name="color">Color of lines.</param>
        /// <param name="thickness">Thickness of lines.</param>
        /// <param name="movingAverageRange">range of values.</param>
        /// <param name="data">Data (as doubles).</param>
        public void RenderMovingAverage(CanvasControl canvas, CanvasDrawEventArgs args, Color color, float thickness, int movingAverageRange, List<double> data)
        {
            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                cpb.BeginFigure(new Vector2(0, (float)(canvas.ActualHeight * (1 - data[0]))));

                double total = data[0];

                int previousRangeLeft = 0;
                int previousRangeRight = 0;

                for (int i = 1; i < data.Count; i++)
                {
                    var range = Math.Max(0, Math.Min(movingAverageRange / 2, Math.Min(i, data.Count - 1 - i)));
                    int rangeLeft = i - range;
                    int rangeRight = i + range;

                    for (int j = previousRangeLeft; j < rangeLeft; j++)
                    {
                        total -= data[j];
                    }

                    for (int j = previousRangeRight + 1; j <= rangeRight; j++)
                    {
                        total += data[j];
                    }

                    previousRangeLeft = rangeLeft;
                    previousRangeRight = rangeRight;

                    cpb.AddLine(new Vector2(i, (float)(canvas.ActualHeight * (1 - (total / ((range * 2) + 1))))));
                }

                cpb.EndFigure(CanvasFigureLoop.Open);

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), color, thickness);
            }
        }
    }
}
