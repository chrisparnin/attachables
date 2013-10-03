attachables
===========

`attachables` is a Visual Studio exstension that allows you to attach reminders where and when you need them. 

### Why use attachables?

Prospective memory (pm) helps us remember to perform an action in the future. We often use physical locations and cues, to remember daily activities like taking pills, or buying groceries. In digital spaces, there is often no equivalent device.

Research has have found developers often use ad-hoc tactics, such as inserting intentional compile errors to in order to introduce reminders in their code.  There should be a better way!

### How to use attachables

To use, you simply can create a reminder note in the editor '// TODO Rename namespace to something better', then choose a way you want to be reminded from the dropdown menu.

![attach](https://raw.github.com/chrisparnin/attachables/master/doc/attach.png)

- **Attach:** The reminder is attached in the corner of the editor viewport.  Display can be affected by your proximity to initial place of creation: e.g same project, same namespace, etc.
- **When:** The reminder is not displayed until a future time, e.g. next day, next week.

![viewport](https://raw.github.com/chrisparnin/attachables/master/doc/viewport.png)

Other actions: 

- **Snooze:** Hide a reminder for the next 8 hours.
- **Done:** Complete and remove reminder.

### Installing attachables

- Will be available in the Visual Studio gallery shortly. 

- To install from source, first install the [Visual Studio SDK 2012](http://www.microsoft.com/en-us/download/details.aspx?id=30668) or [2010](http://www.microsoft.com/en-us/download/details.aspx?id=21835) in order to build project.  Then install the resulting .VSIX file.

### Other platforms

`attachables` is also available in chrome!  See https://github.com/chrisparnin/pm

### Future Features

- **Condition:** The reminder is displayed when a condition is true: When the code is a buildable state, you are debugging, or even an external condition such as a team member has committed code you are waiting on. 
- **Cloud Sync:** Reminders are synced to a reminder app, such as remember the milk.