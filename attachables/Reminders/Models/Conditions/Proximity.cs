using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ninlabs.attachables.Util;
using Microsoft.VisualStudio.Text.Editor;

namespace ninlabs.attachables.Models.Conditions
{
    [DataContract]
    public class Proximity : AbstractCondition
    {
        public Proximity()
        {
            Type = ConditionType.Proximity;
        }

        [DataMember]
        public string Path { get; set; }

        public override string ToString()
        {
            return "Proximity near " + Path;
        }

        public override bool IsApplicable(Reminder reminder, IWpfTextView view)
        {
            if (string.IsNullOrWhiteSpace(Path))
            {
                return true;
            }

            if (!Path.Contains(';'))
                return false;

            string kind = Path.Split(';')[0];
            string data = Path.Split(';')[1];
            if (kind == "project")
            {
                return data == CurrentPositionHelper.GetCurrentProject();
            }
            if (kind == "namespace")
            {
                return data == CurrentPositionHelper.GetCurrentNamespace();
            }
            if (kind == "file")
            {
                //return data == CurrentPositionHelper.GetCurrentFile();
                var textDocument = view.TextBuffer.GetTextDocument();
                return textDocument != null && textDocument.FilePath == data;
            }
            if (kind == "method")
            {
                bool atMethod;
                return data == CurrentPositionHelper.GetCurrentMethod(out atMethod);
            }

            // Current Context Path?
            //using (var db = new SessionsContext())
            //{
            //    var current = db.LastActivity.ToList().SingleOrDefault();
            //    if (current != null)
            //    {
            //        var path = current.LastFile;
            //    }
            //}
            return false;
        }

        public static bool DoesPathContainLocation(string mainPath, string current)
        {
            // File and entity comparison
            if (mainPath.Contains(";") && current.Contains(";"))
            {
                if (CompareAtoms(mainPath.Split('/'), current.Split('/')))
                {
                    return CompareAtoms(mainPath.Split(';')[1].Split('.'),
                                         current.Split(';')[1].Split('.'));
                }
                return false;
            }
            // File comparison
            else if (!mainPath.Contains(";") && !current.Contains(";"))
            {
                return CompareAtoms(mainPath.Split('/'), current.Split('/'));
            }
            // Main path only needs file match
            else if (!mainPath.Contains(";") && current.Contains(";"))
            {
                return CompareAtoms(mainPath.Split('/'), current.Split(';')[0].Split('/'));
            }
            // Main path has entity but none in current location.
            else
            {
                return false;
            }
        }

        public static bool CompareAtoms(string[] a, string[] b)
        {
            if (a.Length > b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
