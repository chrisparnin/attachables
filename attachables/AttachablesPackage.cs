using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using ninlabs.attachables.Storage;
using System.Data.Entity.Migrations;
using EnvDTE80;

namespace ninlabs.attachables
{
    [Guid("2AAE0C73-D8A3-49C0-8070-630DC0F2BE16")]
    public abstract class WindowKinds
    {
        public const string vsWindowKindKindStartPage = "{387CB18D-6153-4156-9257-9AC3F9207BBE}";
        public const string vsWindowKindCommunityWindow = "{96DB1F3B-0E7A-4406-B73E-C6F0A2C67B97}";
        public const string vsWindowKindDeviceExplorer = "{B65E9355-A4C7-4855-96BB-1D3EC8514E8F}";
        public const string vsWindowKindBookmarks = "{A0C5197D-0AC7-4B63-97CD-8872A789D233}";
        public const string vsWindowKindApplicationBrowser = "{399832EA-70A8-4AE7-9B99-3C0850DAD152}";
        public const string vsWindowKindFavorites = "{57DC5D59-11C2-4955-A7B4-D7699D677E93}";
        public const string vsWindowKindErrorList = "{D78612C7-9962-4B83-95D9-268046DAD23A}";
        public const string vsWindowKindHelpSearch = "{46C87F81-5A06-43A8-9E25-85D33BAC49F8}";
        public const string vsWindowKindHelpIndex = "{73F6DD58-437E-11D3-B88E-00C04F79F802}";
        public const string vsWindowKindHelpContents = "{4A791147-19E4-11D3-B86B-00C04F79F802}";
        public const string vsWindowKindCallBrowser = "{5415EA3A-D813-4948-B51E-562082CE0887}";
        public const string vsWindowKindCodeDefinition = "{588470CC-84F8-4A57-9AC4-86BCA0625FF4}";
        public const string vsWindowKindImmediate = "{28836128-FC2C-11D2-A433-00C04F72D18A}";
    }

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(MyToolWindow))]
    [Guid(GuidList.guidAttachablesPkgString)]
    //[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    //[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]
    public sealed class AttachablesPackage : Package, IVsSolutionEvents
    {
        private uint m_solutionCookie = 0;

        public AttachablesPackage()
        {
            // TODO Have the menu bring up list of reminders.
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

            // Start database migration.
            //var migratorConfig = new Migrations.Configuration();
            //migratorConfig.AutomaticMigrationsEnabled = true;
            //var dbMigrator = new DbMigrator(migratorConfig);
            //dbMigrator.Update("0");
        }

        public static ReminderManager Manager
        {
            get;
            set;
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(MyToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }


        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidAttachablesCmdSet, (int)PkgCmdIDList.cmdidRemindersCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidAttachablesCmdSet, (int)PkgCmdIDList.cmdidRemindersWindow);
                MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
                mcs.AddCommand( menuToolWin );

                // Create the menu option in the error list window
                
                // cancel
                CommandID cancelCommand = new CommandID(GuidList.guidAttachablesCmdSet, (int)PkgCmdIDList.cmdidTodoByCancel);
                OleMenuCommand cancelMenuItem = new OleMenuCommand(this.CancelErrorItem, cancelCommand);
                cancelMenuItem.BeforeQueryStatus += new System.EventHandler(this.errorMenuItem_BeforeQueryStatus);
                mcs.AddCommand(cancelMenuItem);

                // mark done
                CommandID completeListCommand = new CommandID(GuidList.guidAttachablesCmdSet, (int)PkgCmdIDList.cmdidTodoByMarkDone);
                OleMenuCommand completeMenuItem = new OleMenuCommand(this.MarkErrorItemDone, completeListCommand);
                completeMenuItem.BeforeQueryStatus += new System.EventHandler(this.errorMenuItem_BeforeQueryStatus);
                mcs.AddCommand(completeMenuItem);

                // snooze
                CommandID snoozeListCommand = new CommandID(GuidList.guidAttachablesCmdSet, (int)PkgCmdIDList.cmdidTodoBySnooze);
                OleMenuCommand snoozeMenuItem = new OleMenuCommand(this.SnoozeErrorItem, snoozeListCommand);
                snoozeMenuItem.BeforeQueryStatus += new System.EventHandler(this.errorMenuItem_BeforeQueryStatus);
                mcs.AddCommand(snoozeMenuItem);

            }

            IVsSolution solution = (IVsSolution)GetService(typeof(SVsSolution));
            ErrorHandler.ThrowOnFailure(solution.AdviseSolutionEvents(this, out m_solutionCookie));
        }

        private void CancelErrorItem(object sender, EventArgs e)
        {
            EnvDTE.Window window = this.dte.Windows.Item(WindowKinds.vsWindowKindErrorList);
            ErrorList myErrorList = (EnvDTE80.ErrorList)window.Object;
            object[] objer = (object[])myErrorList.SelectedItems;
            foreach (object item in objer)
            {
                var errorItem = item as ErrorItem;
                if (errorItem != null)
                {
                    var errorTask = FindErrorTask(errorItem, AttachablesPackage.Manager.ErrorProvider);
                    if (errorTask != null)
                    {
                        AttachablesPackage.Manager.ErrorProvider.Tasks.Remove(errorTask);

                        var searchText = errorTask.Text.Substring(0, errorTask.Text.IndexOf(" not completed by "));
                        var reminder = AttachablesPackage.Manager.FindReminderByProperties(searchText, errorTask.Document, false);
                        if (reminder != null)
                        {
                            reminder.NotificationType = Models.NotificationType.None;
                            AttachablesPackage.Manager.SaveReminder(reminder);
                        }
                    }
                }
            }
        }

        private void MarkErrorItemDone(object sender, EventArgs e)
        {
            EnvDTE.Window window = this.dte.Windows.Item(WindowKinds.vsWindowKindErrorList);
            ErrorList myErrorList = (ErrorList)window.Object;
            object[] objer = (object[])myErrorList.SelectedItems;
            foreach (object item in objer)
            {
                var errorItem = item as ErrorItem;
                if (errorItem != null)
                {
                    var errorTask = FindErrorTask(errorItem, AttachablesPackage.Manager.ErrorProvider);
                    if (errorTask != null)
                    {
                        var searchText = errorTask.Text.Substring(0, errorTask.Text.IndexOf(" not completed by "));
                        var reminder = AttachablesPackage.Manager.FindReminderByProperties(searchText, errorTask.Document, false);
                        if (reminder != null)
                        {
                            AttachablesPackage.Manager.CompleteReminder(reminder);
                        }

                        AttachablesPackage.Manager.ErrorProvider.Tasks.Remove(errorTask);
                    }
                }
            }
        }

        private void SnoozeErrorItem(object sender, EventArgs e)
        {
            EnvDTE.Window window = this.dte.Windows.Item(WindowKinds.vsWindowKindErrorList);
            ErrorList myErrorList = (ErrorList)window.Object;
            object[] objer = (object[])myErrorList.SelectedItems;
            foreach (object item in objer)
            {
                var errorItem = item as ErrorItem;
                if (errorItem != null)
                {
                    var errorTask = FindErrorTask(errorItem, AttachablesPackage.Manager.ErrorProvider);
                    if (errorTask != null)
                    {
                        var searchText = errorTask.Text.Substring(0, errorTask.Text.IndexOf(" not completed by ") );
                        var reminder = AttachablesPackage.Manager.FindReminderByProperties(searchText, errorTask.Document, false);
                        if (reminder != null)
                        {
                            AttachablesPackage.Manager.SnoozeReminder(reminder);
                        }

                        AttachablesPackage.Manager.ErrorProvider.Tasks.Remove(errorTask);
                    }
                }
            }
        }

        private ErrorTask FindErrorTask(ErrorItem item, ErrorListProvider errorListMenu)
        {
            foreach (ErrorTask errorTask in errorListMenu.Tasks)
            {
                if (errorTask.Text == item.Description &&
                    errorTask.Line == item.Line - 1 &&
                    errorTask.Document == item.FileName)
                    return errorTask;
            }
            return null;
        }

        #endregion

        EnvDTE.DTE dte;
        EnvDTE.BuildEvents buildEvents;
        private void InitializeWithSolutionAndDTEReady()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            dte = (EnvDTE.DTE)this.GetService(typeof(EnvDTE.DTE));
            if (dte != null && dte.Solution != null)
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                path = System.IO.Path.Combine(path, "attachables", System.IO.Path.GetFileNameWithoutExtension(dte.Solution.FullName));
            }
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            
            
            RemindersContext.ConfigureDatabase(path);
            Manager = new ReminderManager(this);

            // Check due reminders once during startup.
            Manager.CheckTodoBy();

            // Need to always have reference to avoid COM releasing.
            buildEvents = dte.Events.BuildEvents;

            buildEvents.OnBuildBegin += (scope, action) =>
            {
                // Check due reminders when building.
                Manager.CheckTodoBy();
            };

            buildEvents.OnBuildDone += (scope, action) =>
            {
            };


            buildEvents.OnBuildProjConfigBegin += (project, config, platform, solutionConfig) =>
            {
            };


            buildEvents.OnBuildProjConfigDone += (project, config, platform, solutionConfig, success) =>
            {
            };


        }

        private void errorMenuItem_BeforeQueryStatus(object sender, System.EventArgs e)
        {
            OleMenuCommand menuItem = sender as OleMenuCommand;
            
            EnvDTE.Window window = this.dte.Windows.Item(WindowKinds.vsWindowKindErrorList);
            ErrorList myErrorList = (EnvDTE80.ErrorList)window.Object;

            // If there is any error selected, enable menu command and return.
            object[] objer = (object[])myErrorList.SelectedItems;
            foreach (object item in objer)
            {
                var errorItem = item as ErrorItem;
                if (errorItem != null)
                {
                    var errorTask = FindErrorTask(errorItem, AttachablesPackage.Manager.ErrorProvider);
                    if (errorTask != null)
                    {
                        menuItem.Visible = true;
                        menuItem.Enabled = true;
                        return;
                    }
                }
            }
            // else disable
            menuItem.Enabled = false;
            menuItem.Visible = false;
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "attachables",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }


        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            InitializeWithSolutionAndDTEReady();
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }
    }
}
