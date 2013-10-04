using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Reflection;
using System.Diagnostics;
using System;

namespace ninlabs.attachables.UI
{
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class ViewportNoteFactory : IWpfTextViewCreationListener
    {
        /// <summary>
        /// Defines the adornment layer for the scarlet adornment. This layer is ordered 
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("ProspectiveNote")]
        [Order(After = PredefinedAdornmentLayers.Selection)]
        [TextViewRole(PredefinedTextViewRoles.Document)]
        public AdornmentLayerDefinition editorAdornmentLayer = null;

        public ViewportNoteFactory()
        {
            //try
            //{
            //    var ass = Assembly.GetExecutingAssembly();
            //    Assembly.LoadFrom(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ass.Location), "Microsoft.Expression.Drawing.dll"));
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine(ex.Message);
            //}
        }

        /// <summary>
        /// Instantiates a ProspectiveNote manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            new ViewportNote(textView);
        }
    }
}
