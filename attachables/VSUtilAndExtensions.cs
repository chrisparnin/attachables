using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ninlabs.attachables
{
    public static class VSUtilAndExtensions
    {
        public static ITextDocument GetTextDocument(this ITextBuffer TextBuffer)
        {
            ITextDocument textDoc;
            var rc = TextBuffer.Properties.TryGetProperty<ITextDocument>(
              typeof(ITextDocument), out textDoc);
            if (rc == true)
                return textDoc;
            else
                return null;
        }

        private static string GetFileNameFromTextView(this IVsTextView vTextView)
        {
            IVsTextLines buffer;
            vTextView.GetBuffer(out buffer);
            IVsUserData userData = buffer as IVsUserData;
            Guid monikerGuid = typeof(IVsUserData).GUID;
            object pathAsObject;
            userData.GetData(ref monikerGuid, out pathAsObject);
            return (string)pathAsObject;
        }

    }
}
