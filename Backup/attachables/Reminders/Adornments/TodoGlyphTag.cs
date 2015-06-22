using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Collections.ObjectModel;
using System.Windows;

namespace TodoArdornment
{
    public class TodoGlyphTag : SmartTag
    {
        public TodoGlyphTag(SmartTagType smartTagType, ReadOnlyCollection<SmartTagActionSet> actionSets)
            : base(smartTagType, actionSets)
        {

        }
        internal void Execute(Point position, FrameworkElement frameworkElement)
        {
            //MessageBox.Show("hello: " + position.X + ":" + position.Y);
            // Smart Tag, Intellisense.
        }
    }
}
