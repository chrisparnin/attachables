using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ninlabs.attachables.Models;
using ninlabs.attachables.Models.Conditions;
using ninlabs.attachables.Storage;
using ninlabs.attachables.Util;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ninlabs.attachables.Reminders.Models.Conditions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace ninlabs.attachables
{
    public delegate void RemindersUpdateEvent(object sender, EventArgs args);

    public class ReminderManager
    {
        public ErrorListProvider ErrorProvider { get; set; }
        internal ReminderManager(IServiceProvider Provider) 
        {
            ErrorProvider = new ErrorListProvider(Provider);
            ErrorProvider.ProviderName = "attachables";
            ErrorProvider.ProviderGuid = new Guid(GuidList.guidAttachablesPkgString);
        }

        public event RemindersUpdateEvent RemindersUpdated;

        public List<Reminder> GetReminders()
        {
            using (var db = new RemindersContext())
            {
                if (db.Reminders == null || db.Reminders.Count() == 0)
                    return new List<Reminder>();

                var list = db.Reminders.ToList();
                return list.Select(r => new Reminder()
                {
                    Id = r.Id,
                    NotificationType = (NotificationType) r.NotificationType,
                    ReminderMessage = r.ReminderMessage,
                    Condition = r.ConditionAsString.Deserialize<AbstractCondition>(),
                    IsCompleted = r.IsCompleted,
                    CreatedOn = r.CreatedOn,
                    SnoozeUntil = r.SnoozeUntil,
                    SourcePath = r.SourcePath,
                    LineStart = r.LineStart,
                    CompletedOn = r.CompletedOn
                }).OrderBy(r => r.IsCompleted).ToList();
            }
        }
        // TODO By Friday - Let reminders be editable.

        // TODO By Today Example expired reminder.

        public void CompleteReminder(Reminder reminder)
        {
            reminder.IsCompleted = true;
            reminder.CompletedOn = DateTime.Now;
            AttachablesPackage.Manager.SaveReminder(reminder);
        }


        // TODO weird behavior sometimes with done being clicked, but subsquent actions not working...better refresh?
        
        // TODO when to check todo by ?  Startup...hook into before build?
        
        // TODO Edit TODO reminder text.



        // If supporting Todo by, how to turn off?
        // Option 1: Detect changes in deleted code.  Match against existing.
        // Option 2: Need tag/autocomplete thing to turn on more often/hover instead of change.
        // Option 3: Ability to turn off from Error List Provider.

        // Option 3 works at the moment.

        public Reminder AttachReminder(string message, string path, string sourcePath, int lineStart)
        {
            var reminder = new Reminder()
            {
                Condition = new Proximity()
                {
                    Path = path
                },
                CreatedOn = DateTime.Now,
                NotificationType = NotificationType.Viewport,
                ReminderMessage = message,
                SnoozeUntil = null,
                SourcePath = sourcePath,
                LineStart = lineStart,
                CompletedOn = null
            };
            SaveReminder(reminder);

            return reminder;
            //TodoBy(message, "Today", sourcePath, lineStart);
        }

        // Touch Points
        // TODO Idea: Apply To (fix it to other candidates).

        public void DueByReminder(string message, DateTime dueBy, string friendlyDate, string sourcePath, int lineStart)
        {
            SaveReminder(new Reminder()
            {
                Condition = new DueBy()
                {
                    DueByDate = dueBy,
                    FriendlyDueDate = friendlyDate
                },
                CreatedOn = DateTime.Now,
                NotificationType = NotificationType.BuildError,
                ReminderMessage = message,
                
                SourcePath = sourcePath,
                LineStart = lineStart,
                CompletedOn = null
            });
        }


        public void WhenDateShowReminder(string message, DateTime triggerBy, string sourcePath, int lineStart)
        {
            SaveReminder(new Reminder()
            {
                Condition = new Time()
                {
                    TriggerBy = triggerBy
                },
                CreatedOn = DateTime.Now,
                NotificationType = NotificationType.Viewport,
                ReminderMessage = message,
                SourcePath = sourcePath,
                LineStart = lineStart,
                CompletedOn = null
            });
        }

        public void SaveReminder(Reminder reminder)
        {
            using (var db = new RemindersContext())
            {
                var dbReminder = db.Reminders.Where(r => r.Id == reminder.Id).SingleOrDefault();
                if (dbReminder == null)
                {
                    dbReminder = new ReminderContract()
                    {
                        ConditionAsString = reminder.Condition.Serialize(),
                        CreatedOn = reminder.CreatedOn,
                        NotificationType = (int)reminder.NotificationType,
                        ReminderMessage = reminder.ReminderMessage,
                        IsCompleted = reminder.IsCompleted,
                        SnoozeUntil = reminder.SnoozeUntil,
                        SourcePath = reminder.SourcePath,
                        LineStart = reminder.LineStart,
                        CompletedOn = reminder.CompletedOn
                    };
                    var updated = db.Reminders.Add(dbReminder);
                    db.SaveChanges();
                    reminder.Id = updated.Id;
                }
                else
                {
                    dbReminder.ConditionAsString = reminder.Condition.Serialize();
                    dbReminder.NotificationType = (int)reminder.NotificationType;
                    dbReminder.ReminderMessage = reminder.ReminderMessage;
                    dbReminder.IsCompleted = reminder.IsCompleted;
                    dbReminder.SnoozeUntil = reminder.SnoozeUntil;
                    dbReminder.SourcePath = reminder.SourcePath;
                    dbReminder.LineStart = reminder.LineStart;
                    dbReminder.CompletedOn = reminder.CompletedOn;

                    db.SaveChanges();
                }

                if (RemindersUpdated != null)
                {
                    RemindersUpdated(this, EventArgs.Empty);
                }
            }
        }

        public void ExportReminders(string exportToPath)
        {
            var reminders = GetReminders();
            var package = new ReminderExportPackage()
            {
                Reminders = reminders,
                Version = "V5/3/2013"
            };

            File.WriteAllText(exportToPath, Util.SerializationExtensions.Serialize(package));
        }

        internal void ImportReminders(string ReminderPackagePath)
        {
            var content = File.ReadAllText(ReminderPackagePath);
            var package = Util.SerializationExtensions.Deserialize<ReminderExportPackage>(content);

            var existingReminders = GetReminders();

            foreach (var reminder in package.Reminders)
            {
                if (existingReminders.Where(r => r.ReminderMessage == reminder.ReminderMessage).Count() == 0)
                {
                    SaveReminder(reminder);
                }
            }
        }

        internal void SnoozeReminder(Reminder reminder)
        {
            reminder.SnoozeUntil = DateTime.Now.AddHours(8);
            SaveReminder(reminder);
        }

        internal void GotoReminder(Reminder reminder)
        {
            if (reminder.Condition is Proximity )
            {
                var prox = reminder.Condition as Proximity;
                //Console.WriteLine(prox.Path);
                var parts = prox.Path.Split(';');
                // Path = "" for Everywhere.

                if (reminder.SourcePath != null)
                {
                    var point = UpdatedPoint(reminder);
                    if (point == null)
                    {
                        CurrentPositionHelper.NavigateTo(reminder.SourcePath, reminder.LineStart);
                    }
                    else 
                    {
                        CurrentPositionHelper.NavigateTo(reminder.SourcePath, point.Value.GetContainingLine().LineNumber);                    
                    }
                }
                else if (parts.Length == 2 && parts[0] == "file")
                {
                    CurrentPositionHelper.NavigateTo(parts[1]);
                }
            }
        }

        internal Reminder FindReminderByProperties(string todoNote, string filePath, bool completed)
        {
            return GetReminders()
                .Where(r => r.ReminderMessage == todoNote && r.SourcePath == filePath
                       && r.IsCompleted == completed)
                .FirstOrDefault();
        }

        internal void CheckTodoBy()
        {
            var remindersDue = GetReminders()
                .Where( r => r.NotificationType == NotificationType.BuildError )
                //.Where( r => r.Condition.Type == ConditionType.Time )
                .Where( r => r.IsCompleted == false )
                .Where( r => r.Condition.IsApplicable(r, null ));

            var ivsSolution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
            ErrorProvider.Tasks.Clear();	// clear previously created

            foreach (var reminder in remindersDue)
            {
                var condition = reminder.Condition as DueBy;
                TodoBy(ivsSolution, reminder.ReminderMessage, 
                    condition.DueByDate.ToShortDateString(),
                    condition.FriendlyDueDate,
                    reminder.SourcePath, reminder.LineStart);
            }

            ErrorProvider.Show(); 		// make sure it is visible
        }

        Dictionary<long, Tuple<ITrackingPoint,ITextView>> trackingMap = new Dictionary<long, Tuple<ITrackingPoint,ITextView>>();
        public void TrackReminder(ITrackingPoint point, ITextView view, Reminder reminder)
        {
            trackingMap[reminder.Id] = new Tuple<ITrackingPoint,ITextView>(point,view);
        }

        public SnapshotPoint? UpdatedPoint(Reminder reminder)
        {
            if (trackingMap.ContainsKey(reminder.Id))
            {
                var trackingPoint = trackingMap[reminder.Id].Item1;
                var updatedPoint = trackingPoint.GetPoint(trackingMap[reminder.Id].Item2.TextSnapshot);

                return updatedPoint;
            }
            return null;
        }

        public void TodoBy(IVsSolution ivsSolution, string message, string date, string friendly, string file, int line)
        {
            //Get first project IVsHierarchy item (needed to link the task with a project)
            IVsHierarchy hierarchyItem;
            ivsSolution.GetProjectOfUniqueName(file, out hierarchyItem);

            var errorMessage = message + " not completed by " + date;
            if (friendly != null)
            {
                errorMessage = string.Format("{0} not completed by {1} ({2})",
                    message, friendly, date);
            }
            var newError = new ErrorTask()
            {
                ErrorCategory = TaskErrorCategory.Error,
                Category = TaskCategory.BuildCompile,
                Text = errorMessage,
                Document = file,
                Line = line,
                Column = 6,
                HierarchyItem = hierarchyItem,
                CanDelete = true
            };

            newError.Navigate += (sender, e) =>
            {
                //there are two Bugs in the errorListProvider.Navigate method:
                //    Line number needs adjusting
                //    Column is not shown
                newError.Line++;
                ErrorProvider.Navigate(newError, new Guid(EnvDTE.Constants.vsViewKindCode));
                newError.Line--;
            };

            ErrorProvider.Tasks.Add(newError);	// add item
        }

    }
}
