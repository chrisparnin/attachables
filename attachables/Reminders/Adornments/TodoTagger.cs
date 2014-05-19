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
using ninlabs.attachables.Reminders.Adornments;

namespace TodoArdornment
{
    public class TodoTagger : ITagger<TodoGlyphTag>
    {
        public event EventHandler<Microsoft.VisualStudio.Text.SnapshotSpanEventArgs> TagsChanged;
        public static Regex todoLineRegex = new Regex(@"\/\/\s*TODO\s*(BY)?\b", RegexOptions.IgnoreCase);

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
                    var hasDueBy = false;
                    if (match.Groups.Count == 2 && match.Groups[1].Value.ToLower() == "by" )
                    {
                        hasDueBy = true;
                    }

                    ITextViewLine line = null;
                    DateTime? dueDate = null;
                    string friendly = null;
                    try
                    {
                        line = _textView.Caret.ContainingTextViewLine;
                        if (hasDueBy)
                        {
                            var dateSpan = new SnapshotSpan(span.Snapshot, new Span(point.Position + match.Length, span.Length - match.Length));
                            var str = dateSpan.GetText();

                            DateMatcher matcher = new DateMatcher();
                            dueDate = matcher.FromString(str, out friendly);
                            
                            // Bail if there isn't a good date found (should give visual feedback to dev).
                            if (!dueDate.HasValue)
                                continue;
                        }
                    }
                    catch( Exception ex )
                    {
                        continue;
                    }
                    var actions = new ReadOnlyCollection<SmartTagActionSet>(new SmartTagActionSet[]{}.ToList());
                    if (line != null &&
                        _textView.Caret.ContainingTextViewLine.ContainsBufferPosition(span.Start))
                    {
                         actions = GetSmartTagActions(spanNew, dueDate, friendly);
                    }
                    yield return new TagSpan<TodoGlyphTag>(spanNew, 
                        new TodoGlyphTag(SmartTagType.Ephemeral, actions)
                    );
                }
            }
        }

        private ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(SnapshotSpan span, DateTime? dueDate, string friendly)
        {
            ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);

            var filePath = CurrentFilePathRaw();

            var attachActionList = new List<ISmartTagAction>();
            attachActionList.Add(new AttachAction(trackingSpan, this, "Attach here", CurrentFilePath(), filePath));
            attachActionList.Add(new AttachAction(trackingSpan, this, "Attach everywhere", "", filePath));

            var whenActionList = new List<ISmartTagAction>();
            if( dueDate.HasValue )
            {
                string actionTitle = "Due on " + dueDate.Value.ToShortDateString();
                if (friendly != null )
                {
                    actionTitle = string.Format("Due on {0} ({1})", friendly, dueDate.Value.ToShortDateString());
                }
                whenActionList.Add(new WhenAction(trackingSpan, this, actionTitle, TimeSpan.FromDays(1), filePath));
            }
            whenActionList.Add(new WhenAction(trackingSpan, this, "Show next day", TimeSpan.FromDays(1), filePath));
            whenActionList.Add(new WhenAction(trackingSpan, this, "Show next week", TimeSpan.FromDays(7), filePath));



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

        private string CurrentFilePathRaw()
        {
            try
            {
                return CurrentPositionHelper.GetCurrentFile();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}
