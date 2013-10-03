using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ninlabs.attachables.Models;
using ninlabs.attachables.Models.Conditions;
using ninlabs.attachables.Storage;

namespace ninlabs.attachables
{
    public class ReminderManager
    {
        public List<Reminder> GetReminders()
        {
            //using (var db = new RemindersContext())
            //{
            //    var list = db.Reminders.ToList();
            //    return list.Select(r => new Reminder()
            //    {
            //        Id = r.Id,
            //        NotificationType = (NotificationType)Enum.ToObject(typeof(NotificationType), (ulong)r.NotificationType),
            //        ReminderMessage = r.Message,
            //        Condition = r.Condition.Deserialize<AbstractCondition>(),
            //        IsCompleted = r.IsCompleted,
            //        CreatedOn = r.CreatedOn
            //    }).OrderBy(r => r.IsCompleted).ToList();
            //}
            return new Reminder[] 
            { 
                new Reminder()
                {
                    Condition = new Proximity()
                    {
                        Path = "",
                    },
                    NotificationType = NotificationType.Viewport,
                    ReminderMessage = "Always something to remind me"
                }
            }.ToList();
        }

        public void SaveReminder(Reminder reminder)
        {
            //using (var db = new RemindersContext())
            //{
            //    var dbReminder = db.Reminders.Where(r => r.Id == reminder.Id).SingleOrDefault();
            //    if (dbReminder == null)
            //    {
            //        dbReminder = new SmartReminderEntity()
            //        {
            //            Condition = reminder.Condition.Serialize(),
            //            CreatedOn = reminder.CreatedOn,
            //            ExpireBy = reminder.CreatedOn.AddDays(90),
            //            TriggerBy = null,
            //            NotificationType = (int)reminder.NotificationType,
            //            Message = reminder.ReminderMessage,
            //            IsCompleted = reminder.IsCompleted,
            //        };
            //        var updated = db.Reminders.Add(dbReminder);
            //        db.SaveChanges();
            //        reminder.Id = updated.Id;
            //    }
            //    else
            //    {
            //        dbReminder.Condition = reminder.Condition.Serialize();
            //        dbReminder.ExpireBy = DateTime.Now.AddDays(90);
            //        dbReminder.NotificationType = (int)reminder.NotificationType;
            //        dbReminder.Message = reminder.ReminderMessage;
            //        dbReminder.IsCompleted = reminder.IsCompleted;

            //        db.SaveChanges();
            //    }
            //}
        }

        public void ExportReminders(string exportToPath)
        {
            var reminders = GetReminders();
            var package = new ReminderExportPackage()
            {
                Reminders = reminders,
                Version = "V4/24/2012-A"
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
    }
}
