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


        Regex todoLineRegex = new Regex(@"\/\/\s*TODO\b");
        //Regex todoLineRegex = new Regex(@"\bTODO\b");
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

            //var index = textViewLines.GetIndexOfTextLine(line);

                //    var match = todoLineRegex.Match(text);
                //    if (match.Success)
            foreach (var tag in this._createTagAggregator.GetTags(line.Extent))
            {
                foreach (var span in tag.Span.GetSpans(_view.TextSnapshot))
                {
                    //int matchStart = line.Start.Position + span.Index;
                    //var span = new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(matchStart, matchStart + match.Length));
                    SetBoundary(textViewLines, span);

                    string text = null;
                    if (TryGetText(_view, line, out text))
                    {
                        var top = _view.ViewportTop + 30;
                        var left = _view.ViewportRight - 30 - 162;

                        text = text.Trim();
                        var match = todoLineRegex.Match(text);
                        CreateNote(text.Substring(match.Index + match.Length), left, top);
                    }
                }
            }
        }

        // jared parson: https://gist.github.com/4320643

        /// <summary>
        /// This will get the text of the ITextView line as it appears in the actual user editable 
        /// document. 
        /// </summary>
        public static bool TryGetText(IWpfTextView textView, ITextViewLine textViewLine, out string text)
        {
            var extent = textViewLine.Extent;
            var bufferGraph = textView.BufferGraph;
            try
            {
                var collection = bufferGraph.MapDownToSnapshot(extent, SpanTrackingMode.EdgeInclusive, textView.TextSnapshot);
                var span = new SnapshotSpan(collection[0].Start, collection[collection.Count - 1].End);
                //text = span.ToString();
                text = span.GetText();
                return true;
            }
            catch
            {
                text = null;
                return false;
            }
        }

        // TODO namespace and project name settlement.
        private void CreateNote(string message, double left, double top)
        {
            /*var note = new ViewportNotification();
            note.DataContext = new
            {
                ReminderMessage = message
            };

            note.Width = 162;
            note.Height = 100;

            //Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(note, left);
            Canvas.SetTop(note, top);

            top += note.Height + 15;

            //add the image to the adornment layer and make it relative to the viewport
            _layer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, note, null);
            */
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
