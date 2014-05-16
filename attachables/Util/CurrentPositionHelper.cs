using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TextManager.Interop;

namespace ninlabs.attachables.Util
{
    class CurrentPositionHelper
    {
        static CurrentPositionHelper()
        {
            m_applicationObject = Package.GetGlobalService(typeof(DTE)) as DTE;
        }
        static DTE m_applicationObject;

        public static void NavigateTo(string FilePath, int SpanStart = 0)
        {
            //IVsUIShell service = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            //Guid pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            //object pvaIn = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[] { FilePath });
            //service.PostExecCommand(ref pguidCmdGroup, 0xde, 2, ref pvaIn);

            uint ItemID;
            IVsWindowFrame WindowFrame;
            IVsTextView TextView;
            IVsUIHierarchy Hierarchy;
            VsShellUtilities.OpenDocument(ServiceProvider.GlobalProvider, FilePath, Guid.Empty, out Hierarchy, out ItemID, out WindowFrame, out TextView);

            if (TextView != null)
            {
                TextView.CenterLines(SpanStart, 1);
                //TextView.EnsureSpanVisible(new TextSpan() { iStartLine = Error.Line, iEndLine = Error.Line + 1, iStartIndex = Error.Column, iEndIndex = Error.Column + 1 });
                TextView.SetCaretPos(SpanStart, 0);
            }
        }


        public static string GetCurrentProject()
        {
            IntPtr hierarchyPtr, selectionContainerPtr;
            Object prjItemObject  = null;
            IVsMultiItemSelect mis;
            uint prjItemId;

            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
            monitorSelection.GetCurrentSelection(out hierarchyPtr, out prjItemId, out mis, out selectionContainerPtr);

            if (hierarchyPtr == IntPtr.Zero)
                return null;

            IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
            if (selectedHierarchy != null)
            {
                try
                {
                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(prjItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out prjItemObject));
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            ProjectItem selectedPrjItem = prjItemObject as ProjectItem;
            if (selectedPrjItem != null && selectedPrjItem.ContainingProject != null)
            {
                return selectedPrjItem.ContainingProject.Name;
            }
            return null;
        }

        public static string GetCurrentNamespace()
        {
            CodeElement element = null;
            try
            {
                if (m_applicationObject.ActiveDocument != null && m_applicationObject.ActiveDocument.ProjectItem != null)
                {
                    if (m_applicationObject.ActiveDocument.ProjectItem.FileCodeModel != null)
                    {
                        element = m_applicationObject.ActiveDocument.ProjectItem.FileCodeModel.CodeElementFromPoint(GetEditPointFromActivePoint(), vsCMElement.vsCMElementNamespace);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                element = null;
            }

            if (element != null)
            {
                return element.FullName;
            }
            return null;
        }

        public static string GetCurrentFile()
        {
            try
            {
                return m_applicationObject.ActiveDocument.FullName;
            }
            catch
            {
                return null;
            }
        }

        public static string GetCurrentClass()
        {
            CodeElement element = null;
            try
            {
                element = GetCodeElementFromActivePoint();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                element = null;
            }

            if (element != null)
            {
                if (element.Kind == vsCMElement.vsCMElementFunction)
                {
                    return element.FullName.Remove(element.FullName.LastIndexOf('.'));
                }
                return element.FullName;
            }
            return null;
        }

        public static string GetCurrentMethod(out bool couldGetMethod)
        {
            CodeElement element = null;
            try
            {
                element = GetCodeElementFromActivePoint();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                element = null;
            }

            couldGetMethod = false;
            if (element != null)
            {
                if (element.Kind == vsCMElement.vsCMElementFunction)
                {
                    couldGetMethod = true;
                    return element.FullName;
                }
                return element.FullName;
            }
            return null;
        }

        static protected EditPoint GetActiveEditPoint()
        {
            TextSelection selection = GetActiveSelection();
            if (selection == null)
                return null;
            EditPoint selPoint = selection.ActivePoint.CreateEditPoint();
            selPoint.StartOfLine();
            selPoint.WordRight(1);
            return selPoint;
        }

        /// <summary>
        /// Returns an adjusted CodeElement. Walks comments 'down' to the next real element.
        /// From Rick Strahl's Weblog
        /// </summary>
        /// <returns></returns>
        static protected CodeElement GetCodeElementFromActivePoint()
        {
            EditPoint selPoint = GetActiveEditPoint();
            if (selPoint == null)
                return null;

            selPoint.StartOfLine();

            while (true)
            {
                if (selPoint.AtEndOfDocument)
                    break;
                string BlockText = selPoint.GetText(selPoint.LineLength).Trim();
                // *** Skip over any XML Doc comments
                if (BlockText.StartsWith("//"))
                {
                    selPoint.LineDown(1);
                    selPoint.StartOfLine();
                }
                else
                    break;
            }
            // *** Make sure the cursor is placed inside of the definition always
            // *** Especially required for single line methods/fields/events etc.
            selPoint.EndOfLine();
            selPoint.CharLeft(1);  // Force into the text

            return GetActiveCodeElement();
        }

        /// <summary>
        /// Returns an adjusted CodeElement. Walks comments 'down' to the next real element.
        /// From Rick Strahl's Weblog
        /// </summary>
        /// <returns></returns>
        static protected EditPoint GetEditPointFromActivePoint()
        {
            EditPoint selPoint = GetActiveEditPoint();
            if (selPoint == null)
                return null;

            selPoint.StartOfLine();

            while (true)
            {
                if (selPoint.AtEndOfDocument)
                    break;
                string BlockText = selPoint.GetText(selPoint.LineLength).Trim();
                // *** Skip over any XML Doc comments
                if (BlockText.StartsWith("//"))
                {
                    selPoint.LineDown(1);
                    selPoint.StartOfLine();
                }
                else
                    break;
            }
            // *** Make sure the cursor is placed inside of the definition always
            // *** Especially required for single line methods/fields/events etc.
            selPoint.EndOfLine();
            selPoint.CharLeft(1);  // Force into the text

            return selPoint;
        }


        static protected CodeElement GetActiveCodeElement()
        {
            EditPoint pt = GetActiveEditPoint();
            if (pt == null)
                return null;
            return SmartGetActiveCodeElement(pt);
        }

        static protected string GetSelectedText(EditPoint selPoint)
        {
            return selPoint.GetText(selPoint.LineLength).Trim();
        }

        static protected CodeElement SmartGetActiveCodeElement(EditPoint selPoint)
        {
            var list = new vsCMElement[]
            {
				vsCMElement.vsCMElementFunction,
				vsCMElement.vsCMElementClass
            };

            CodeElement element = null;
            foreach (vsCMElement elem in list)
            {
                element = GetActiveCodeElement(selPoint, elem);
                if (element != null)
                    break;
            }
            //            if (element == null)
            //            {
            //                //if( GetSelectedText( selPoint ).IndexOf(";") >= 0 )
            //                {
            //                    while (GetActiveCodeElement(selPoint, 0) == null)
            //                    {
            //                        selPoint.LineDown(1);
            //                        if (selPoint.AtEndOfDocument)
            //                            break;
            //                    }
            //                    element = GetActiveCodeElement(selPoint, 0);
            //                }
            //            }
            return element;
        }
        static protected CodeElement GetActiveCodeElement(EditPoint selPoint, vsCMElement scope)
        {
            // get the element under the cursor
            CodeElement element = null;
            try
            {
                if (m_applicationObject.ActiveDocument.ProjectItem.FileCodeModel != null)
                {
                    element = m_applicationObject.ActiveDocument.ProjectItem.FileCodeModel.CodeElementFromPoint(selPoint, scope);
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                element = null;
            }
            return element;
        }

        static protected TextSelection GetActiveSelection()
        {
        //    return (TextSelection)m_applicationObject.ActiveWindow.Selection;
            return m_applicationObject.ActiveDocument.Selection as TextSelection;
        }
    }
}
