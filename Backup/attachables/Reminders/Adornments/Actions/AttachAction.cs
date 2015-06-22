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
using Microsoft.VisualStudio.Text.Editor;

namespace ninlabs.attachables.Reminders.Adornments.Actions
{
    class AttachAction : ISmartTagAction
    {
        private ITrackingSpan m_span;
        private string m_upper;
        private string m_display;
        private ITextSnapshot m_snapshot;
        private bool m_enabled = true;
        private string m_path;
        TodoTagger m_tagger;
        ITextView m_view;

        private static BitmapImage m_attachIcon;

        static AttachAction()
        {
            try
            {
                m_attachIcon = new BitmapImage();
                m_attachIcon.BeginInit();
                m_attachIcon.UriSource = new Uri("pack://application:,,,/attachables;component/Resources/attach.png", UriKind.Absolute);
                m_attachIcon.CacheOption = BitmapCacheOption.OnLoad;
                m_attachIcon.EndInit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public AttachAction(ITrackingSpan span, ITextView view, TodoTagger tagger, string display, string path, string filePath)
        {
            m_span = span;
            m_snapshot = span.TextBuffer.CurrentSnapshot;
            m_upper = span.GetText(m_snapshot).ToUpper();
            m_display = display;
            m_path = path;
            m_tagger = tagger;
            m_view = view;
            this.FileLocation = filePath;
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

                    var reminder = AttachablesPackage.Manager.AttachReminder(text.Trim(), m_path, FileLocation, line.LineNumber);

                    ITrackingPoint triggerPoint = m_snapshot.CreateTrackingPoint(
                        m_span.GetStartPoint(m_snapshot), PointTrackingMode.Positive);

                    AttachablesPackage.Manager.TrackReminder(triggerPoint, m_view, reminder);

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
            get { return m_attachIcon; }
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
