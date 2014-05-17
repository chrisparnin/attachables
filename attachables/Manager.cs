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

namespace ninlabs.attachables
{
    public delegate void RemindersUpdateEvent(object sender, EventArgs args);

    public class ReminderManager
    {
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
                    NotificationType = r.NotificationType,
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

        public void AttachReminder(string message, string path, string sourcePath, int lineStart)
        {
            SaveReminder(new Reminder()
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
                        NotificationType = reminder.NotificationType,
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
                    dbReminder.NotificationType = reminder.NotificationType;
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
                    CurrentPositionHelper.NavigateTo(reminder.SourcePath, reminder.LineStart);
                }
                else if (parts.Length == 2 && parts[0] == "file")
                {
                    CurrentPositionHelper.NavigateTo(parts[1]);
                }
            }
        }
    }
}
