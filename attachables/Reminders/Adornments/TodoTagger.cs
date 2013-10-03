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

namespace TodoArdornment
{
    public class TodoTagger : ITagger<TodoGlyphTag>
    {
        public event EventHandler<Microsoft.VisualStudio.Text.SnapshotSpanEventArgs> TagsChanged;
        Regex todoLineRegex = new Regex(@"\/\/\s*TODO\b");
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
            List<SmartTagActionSet> actionSetList = new List<SmartTagActionSet>();
            List<ISmartTagAction> actionList = new List<ISmartTagAction>();

            ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);
            actionList.Add(new AttachAction(trackingSpan, this));
            SmartTagActionSet actionSet = new SmartTagActionSet(actionList.AsReadOnly());
            actionSetList.Add(actionSet);
            return actionSetList.AsReadOnly();
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

    internal class AttachAction : ISmartTagAction
    {
        private ITrackingSpan m_span;
        private string m_upper;
        private string m_display;
        private ITextSnapshot m_snapshot;
        private bool m_enabled = true;
        TodoTagger m_tagger;

        public AttachAction(ITrackingSpan span, TodoTagger tagger)
        {
            m_span = span;
            m_snapshot = span.TextBuffer.CurrentSnapshot;
            m_upper = span.GetText(m_snapshot).ToUpper();
            m_display = "Attach";
            m_tagger = tagger;
        }

        public void Invoke()
        {
            //m_span.TextBuffer.Replace(m_span.GetSpan(m_snapshot), m_upper);
            m_enabled = false;
            this.m_tagger.RaiseTagsChanged(m_span.GetSpan(m_snapshot));
        }

        public string DisplayText
        {
            get { return m_display; }
        }
        public ImageSource Icon
        {
            get { return null; }
        }
        public bool IsEnabled
        {
            get { return m_enabled; }
        }

        public ISmartTagSource Source
        {
            get;
            private set;
        }

        public ReadOnlyCollection<SmartTagActionSet> ActionSets
        {
            get { return null; }
        }
    }
}
