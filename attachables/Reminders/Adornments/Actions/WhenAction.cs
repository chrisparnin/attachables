using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using TodoArdornment;

namespace ninlabs.attachables.Reminders.Adornments.Actions
{
    class WhenAction : ISmartTagAction
    {
        private ITrackingSpan m_span;
        private string m_upper;
        private string m_display;
        private ITextSnapshot m_snapshot;
        private bool m_enabled = true;
        private TimeSpan m_delayBy;
        TodoTagger m_tagger;

        private static BitmapImage m_timeIcon;

        static WhenAction()
        {
            try
            {
                m_timeIcon = new BitmapImage();
                m_timeIcon.BeginInit();
                m_timeIcon.UriSource = new Uri("pack://application:,,,/attachables;component/Resources/time.png", UriKind.Absolute);
                m_timeIcon.CacheOption = BitmapCacheOption.OnLoad;
                m_timeIcon.EndInit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public WhenAction(ITrackingSpan span, TodoTagger tagger, string display, TimeSpan delayBy, string filePath)
        {
            m_span = span;
            m_snapshot = span.TextBuffer.CurrentSnapshot;
            m_upper = span.GetText(m_snapshot).ToUpper();
            m_display = display;
            m_delayBy = delayBy;
            m_tagger = tagger;
            FileLocation = filePath;
        }

        public void Invoke()
        {
            if (AttachablesPackage.Manager != null)
            {
                try
                {
                    var line = m_span.GetEndPoint(m_snapshot).GetContainingLine();
                    var text = line.Extent.GetText();

                    

                    text = text.Trim();
                    var match = TodoTagger.todoLineRegex.Match(text);
                    text = text.Substring(match.Index + match.Length);


                    AttachablesPackage.Manager.WhenDateShowReminder(text.Trim(), DateTime.Now + m_delayBy, FileLocation, line.LineNumber);

                    m_enabled = false;
                    this.m_tagger.RaiseTagsChanged(m_span.GetSpan(m_snapshot));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public string FileLocation
        {
            get;
            set;
        }

        public string DisplayText
        {
            get { return m_display; }
        }
        public ImageSource Icon
        {
            get { return m_timeIcon; }
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
