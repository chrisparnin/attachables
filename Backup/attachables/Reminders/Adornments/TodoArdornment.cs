using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text.Tagging;

namespace TodoArdornment
{
    ///<summary>
    ///TodoArdornment places red boxes behind all the "A"s in the editor window
    ///</summary>
    public class TodoArdornment
    {
        IAdornmentLayer _layer;
        IWpfTextView _view;
        Brush _brush;
        Pen _pen;
        ITagAggregator<TodoGlyphTag> _createTagAggregator;

        public TodoArdornment(IWpfTextView view, ITagAggregator<TodoGlyphTag> aggregrator)
        {
            _view = view;
            _layer = view.GetAdornmentLayer("TodoArdornment");
            _createTagAggregator = aggregrator;

            //Listen to any event that changes the layout (text changes, scrolling, etc)
            _view.LayoutChanged += OnLayoutChanged;

            //Create the pen and brush to color the box behind the a's
            Brush brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            brush.Freeze();
            Brush penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            Pen pen = new Pen(penBrush, 0.5);
            pen.Freeze();

            _brush = brush;
            _pen = pen;
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                this.CreateVisuals(line);
            }
        }

        /// <summary>
        /// Within the given line add the scarlet box behind the a
        /// </summary>
        private void CreateVisuals(ITextViewLine line)
        {
            //grab a reference to the lines in the current TextView 
            IWpfTextViewLineCollection textViewLines = _view.TextViewLines;
            int start = line.Start;
            int end = line.End;

            foreach (var tag in this._createTagAggregator.GetTags(line.Extent))
            {
                foreach (var span in tag.Span.GetSpans(_view.TextSnapshot))
                {
                    SetBoundary(textViewLines, span);
                }
            }
        }

        public void SetBoundary(IWpfTextViewLineCollection textViewLines, SnapshotSpan span)
        {
            Geometry g = textViewLines.GetMarkerGeometry(span);
            if (g != null)
            {
                GeometryDrawing drawing = new GeometryDrawing(_brush, _pen, g);
                drawing.Freeze();

                DrawingImage drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                Image image = new Image();
                image.Source = drawingImage;

                //Align the image with the top of the bounds of the text geometry
                Canvas.SetLeft(image, g.Bounds.Left);
                Canvas.SetTop(image, g.Bounds.Top);

                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
            }
        }
    }
}
