using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Diagnostics;
using ninlabs.attachables;
using ninlabs.attachables.Reminders.Adornments.Actions;
using ninlabs.attachables.Util;

namespace TodoArdornment
{
    public class TodoTagger : ITagger<TodoGlyphTag>
    {
        public event EventHandler<Microsoft.VisualStudio.Text.SnapshotSpanEventArgs> TagsChanged;
        public static Regex todoLineRegex = new Regex(@"\/\/\s*TODO\b");
        ITextView _textView;

        internal TodoTagger(ITextView textView)
        {
            _textView = textView;
            _textView.LayoutChanged += OnLayoutChanged;
        }

        internal void RaiseTagsChanged(SnapshotSpan span)
        {
            if (TagsChanged != null)
            {
                TagsChanged(this, new SnapshotSpanEventArgs(span));
            }
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (var span in e.NewOrReformattedSpans)
            {
                if (TagsChanged != null)
                {
                    TagsChanged(this, new SnapshotSpanEventArgs(span));
                }
            }
        }

        public IEnumerable<ITagSpan<TodoGlyphTag>> GetTags(Microsoft.VisualStudio.Text.NormalizedSnapshotSpanCollection spans)
        {
            foreach (var span in spans)
            {
                String text = span.GetText();
                var match = todoLineRegex.Match(text);
                if (match.Success)
                {
                    var point = span.Start.Add(match.Index);
                    var spanNew = new SnapshotSpan(span.Snapshot, new Span(point.Position, match.Length));

                    ITextViewLine line = null;
                    try
                    {
                        line = _textView.Caret.ContainingTextViewLine;
                    }
                    catch( Exception ex )
                    {
                        continue;
                    }
                    var actions = new ReadOnlyCollection<SmartTagActionSet>(new SmartTagActionSet[]{}.ToList());
                    if (line != null &&
                        _textView.Caret.ContainingTextViewLine.ContainsBufferPosition(span.Start))
                    {
                         actions = GetSmartTagActions(spanNew);
                    }
                    yield return new TagSpan<TodoGlyphTag>(spanNew, 
                        new TodoGlyphTag(SmartTagType.Ephemeral, actions)
                    );
                }
            }
        }

        private ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(SnapshotSpan span)
        {
            ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);

            var attachActionList = new List<ISmartTagAction>();
            attachActionList.Add(new AttachAction(trackingSpan, this, "Attach here", CurrentFilePath()));
            attachActionList.Add(new AttachAction(trackingSpan, this, "Attach everywhere", ""));

            var whenActionList = new List<ISmartTagAction>();
            whenActionList.Add(new WhenAction(trackingSpan, this, "Show next day", TimeSpan.FromDays(1)));
            whenActionList.Add(new WhenAction(trackingSpan, this, "Show next week", TimeSpan.FromDays(7)));

            // list of action sets...
            var actionSetList = new List<SmartTagActionSet>();
            actionSetList.Add(new SmartTagActionSet(attachActionList.AsReadOnly()));
            actionSetList.Add(new SmartTagActionSet(whenActionList.AsReadOnly()));

            return actionSetList.AsReadOnly();
        }

        private string CurrentFilePath()
        {
            try
            {
                return "file;" + CurrentPositionHelper.GetCurrentFile();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        // Based on twitter convo with Jared Parson: https://gist.github.com/4320643

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
                text = span.GetText();
                return true;
            }
            catch
            {
                text = null;
                return false;
            }
        }
    }
}
