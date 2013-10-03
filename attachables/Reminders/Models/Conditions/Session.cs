using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ninlabs.attachables.Models.Conditions
{
    [DataContract]
    public class Session : AbstractCondition
    {
        public Session()
        {
            Type = ConditionType.Session;
        }

        public override string ToString()
        {
            return "On next session";
        }

        public override bool IsApplicable(Reminder reminder)
        {
            var now = DateTime.Now;
            //using (var db = new SessionsContext())
            //{
            //    var current = db.Sessions.Where(s => !s.Complete && s.Start > reminder.CreatedOn);
            //    if (current.Any())
            //        return true;
            //    var sessions = db.Sessions
            //        .Where(s => s.Complete && s.End.Value > reminder.CreatedOn);
            //    return sessions.Any();
            //}
            return true;
        }
    }
}
