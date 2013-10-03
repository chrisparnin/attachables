using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;

namespace TodoArdornment
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [Order(Before = "default")]
    [TagType(typeof(TodoGlyphTag))]
    internal class TodoTagProvider : IViewTaggerProvider
    {
        Dictionary<Microsoft.VisualStudio.Text.Editor.ITextView, TodoTagger> taggers = new Dictionary<Microsoft.VisualStudio.Text.Editor.ITextView, TodoTagger>();

        public ITagger<T> CreateTagger<T>(Microsoft.VisualStudio.Text.Editor.ITextView textView, Microsoft.VisualStudio.Text.ITextBuffer buffer) where T : ITag
        {
            if (buffer == null || textView == null)
            {
                return null;
            }

            //make sure we are tagging only the top buffer 
            if (buffer == textView.TextBuffer)
            {
                if (!taggers.ContainsKey(textView))
                {
                    taggers[textView] = new TodoTagger(textView);
                }
                return taggers[textView] as ITagger<T>;
            }
            else return null;
        }
    }
}
