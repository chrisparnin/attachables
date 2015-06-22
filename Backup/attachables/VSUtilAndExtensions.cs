using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ninlabs.attachables
{
    public static class VSUtilAndExtensions
    {
        public static ITextDocument GetTextDocument(this ITextBuffer TextBuffer)
        {
            ITextDocument textDoc;
            var rc = TextBuffer.Properties.TryGetProperty<ITextDocument>(
              typeof(ITextDocument), out textDoc);
            if (rc == true)
                return textDoc;
            else
                return null;
        }

        private static string GetFileNameFromTextView(this IVsTextView vTextView)
        {
            IVsTextLines buffer;
            vTextView.GetBuffer(out buffer);
            IVsUserData userData = buffer as IVsUserData;
            Guid monikerGuid = typeof(IVsUserData).GUID;
            object pathAsObject;
            userData.GetData(ref monikerGuid, out pathAsObject);
            return (string)pathAsObject;
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
