using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using System.Reflection;
using ninlabs.attachables.Models;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;

namespace ninlabs.attachables.UI
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    class ViewportNote
    {
        //private ViewportNotification _note;
        private IWpfTextView _view;
        private IAdornmentLayer _adornmentLayer;

        Dictionary<long, UserControl> _notes = new Dictionary<long, UserControl>();

        private const int NoteWidth = 362;
        private const int NoteHeight = 50;

        public ViewportNote(IWpfTextView view)
        {
            if (view != null)
            {
                _view = view;

                //Grab a reference to the adornment layer that this adornment should be added to
                _adornmentLayer = view.GetAdornmentLayer("ProspectiveNote");

                _view.ViewportHeightChanged += delegate { this.onSizeChange(); };
                _view.ViewportWidthChanged += delegate { this.onSizeChange(); };
                _view.GotAggregateFocus += delegate { this.onSizeChange(); };

                if (AttachablesPackage.Manager != null)
                {
                    AttachablesPackage.Manager.RemindersUpdated += Manager_RemindersUpdated;
                }
            }
            Trace.WriteLine( string.Format("attachables: {0} {1}", view, AttachablesPackage.Manager) );
        }

        private void Manager_RemindersUpdated(object sender, EventArgs args)
        {
            onSizeChange();
        }

        public void onSizeChange()
        {
            //clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();
            _notes.Clear();

            if (AttachablesPackage.Manager == null)
                return;

            var top = _view.ViewportTop + 30;
            var left = _view.ViewportRight - 30 - NoteWidth;
            foreach (var r in AttachablesPackage.Manager.GetReminders()
                .ToList()
                .Where(r => r.NotificationType == NotificationType.Viewport)
                .Where(r => !r.IsCompleted)
                .Where(r => r.SnoozeUntil == null || DateTime.Now >= r.SnoozeUntil.Value )
                .Where(r => r.Condition.IsApplicable(r)))
            {
                var note = new ViewportNotification();
                note.DataContext = new ViewportNotificationViewModel(this)
                {
                    ReminderMessage = r.ReminderMessage,
                    Reminder = r
                };

                note.Width = NoteWidth;
                note.Height = NoteHeight;
                //note.Opacity = .50;
                // TODO: Help I need a reminder.

                //Place the image in the top right hand corner of the Viewport
                Canvas.SetLeft(note, left);
                Canvas.SetTop(note, top);

                top += note.Height + 3;

                //add the image to the adornment layer and make it relative to the viewport
                _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, note, null);

                _notes.Add(r.Id, note);
            }
        }

        public void RemoveAdornment(long id)
        {
            _adornmentLayer.RemoveAdornment(_notes[id]);

            this.onSizeChange();
        }
    }
}
