using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Discord_UWP.Classes
{

    class ChartRenderer
    {
        public void RenderAxes(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var width = (float)canvas.ActualWidth;
            var height = (float)(canvas.ActualHeight);
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

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), Colors.Gray, 1);
            }

            args.DrawingSession.DrawText("0", 5, midHeight - 30, Colors.Gray);
            args.DrawingSession.DrawText(canvas.ActualWidth.ToString(), width - 50, midHeight - 30, Colors.Gray);

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

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), Colors.Gray, 1);
            }

            args.DrawingSession.DrawText("0", midWidth + 5, height - 30, Colors.Gray);
            args.DrawingSession.DrawText("1", midWidth + 5, 5, Colors.Gray);
        }

        public void RenderData(CanvasControl canvas, CanvasDrawEventArgs args, Color color, float thickness, List<double> data, bool renderArea, double max)
        {
            if (data.Count == 0) return;
            var stepsize = Convert.ToSingle(canvas.ActualWidth / data.Count);
            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                cpb.BeginFigure(new Vector2(0, (float)(canvas.ActualHeight * (1 - data[0]/max))));

                for (int i = 1; i < data.Count; i++)
                {
                    cpb.AddLine(new Vector2(stepsize*i, (float)(canvas.ActualHeight * (1 - data[i]/max))));
                }

                if (renderArea)
                {
                    cpb.AddLine(new Vector2(data.Count, (float)canvas.ActualHeight));
                    cpb.AddLine(new Vector2(0, (float)canvas.ActualHeight));
                    cpb.EndFigure(CanvasFigureLoop.Closed);
                    args.DrawingSession.FillGeometry(CanvasGeometry.CreatePath(cpb), Colors.LightGreen);
                }
                else
                {
                    cpb.EndFigure(CanvasFigureLoop.Open);
                    args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), color, thickness);
                }
            }
        }

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
                    (float)(canvas.ActualHeight * (1 - total / range)),
                    columnWidth,
                    (float)(canvas.ActualHeight * (total / range)),
                    Colors.WhiteSmoke);
            }
        }

        public void RenderAveragesAsPieChart(CanvasControl canvas, CanvasDrawEventArgs args, List<double> pieValues, List<Color> palette)
        {
            var total = pieValues.Sum();

            var w = (float)canvas.ActualWidth;
            var h = (float)canvas.ActualHeight;
            var midx = w / 2;
            var midy = h / 2;
            var padding = 50;
            var lineOffset = 20;
            var r = Math.Min(w, h) / 2 - padding;

            float angle = 0f;
            var center = new Vector2(midx, midy);

            for (int i = 0; i < pieValues.Count; i++)
            {
                float sweepAngle = (float)(2 * Math.PI * pieValues[i] / total);
                var arcStartPoint = new Vector2((float)(midx + r * Math.Sin(angle)), (float)(midy - r * Math.Cos(angle)));

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

            var lineBrush = new CanvasSolidColorBrush(args.DrawingSession, Colors.Black);

            for (int i = 0; i < pieValues.Count; i++)
            {
                float sweepAngle = (float)(2 * Math.PI * pieValues[i] / total);
                var midAngle = angle + sweepAngle / 2;
                var isRightHalf = midAngle < Math.PI;
                var isTopHalf = midAngle <= Math.PI / 2 || midAngle >= Math.PI * 3 / 2;
                var p0 = new Vector2((float)(midx + (r - lineOffset) * Math.Sin(midAngle)), (float)(midy - (r - lineOffset) * Math.Cos(midAngle)));
                var p1 = new Vector2((float)(midx + (r + lineOffset) * Math.Sin(midAngle)), (float)(midy - (r + lineOffset) * Math.Cos(midAngle)));
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
                    Colors.Black,
                    new CanvasTextFormat
                    {
                        HorizontalAlignment = isRightHalf ? CanvasHorizontalAlignment.Left : CanvasHorizontalAlignment.Right,
                        VerticalAlignment = isTopHalf ? CanvasVerticalAlignment.Bottom : CanvasVerticalAlignment.Top,
                        FontSize = 18
                    });

                angle += sweepAngle;
            }
        }

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

                    cpb.AddLine(new Vector2(i, (float)(canvas.ActualHeight * (1 - total / (range * 2 + 1)))));
                }

                cpb.EndFigure(CanvasFigureLoop.Open);

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), color, thickness);
            }
        }
    }
}
