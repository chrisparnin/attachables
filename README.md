attachables
===========

`attachables` allows you to attach reminders where and when you need them in your code editor.  It is currently available as a [Visual Studio extension](http://visualstudiogallery.msdn.microsoft.com/850937ba-ff0b-43cb-badd-4e273b508c32).

### Why use attachables?

Prospective memory (pm) helps us remember to perform an action in the future. We often use physical locations and cues, to remember daily activities like taking pills, or buying groceries. In digital spaces, there is often no equivalent device.

Research has have found developers often use ad-hoc tactics, such as inserting *intentional compile errors* to in order to introduce reminders in their code.  There should be a better way!

### How to use attachables

![viewport](https://raw.github.com/chrisparnin/attachables/master/doc/viewport.png)

To use, you simply can create a reminder note in the editor `// TODO Explore other analyzer`, then choose a way you want to be reminded from the dropdown menu.

Reminders can be attached to the Editor viewport, local to the current file, or displayed everywhere.  Attached reminders will fade during the programming session unless they are re-engaged by hovering over them.

![attach](https://raw.github.com/chrisparnin/attachables/master/doc/attach.png)

TODO notes can also have **due dates**.  Simply use the following format:
`// TODO BY <a date> message`, where date is a day of the week, today, tomorrow, next day, next week, or a specific date such as 5/31/14 (date format is quite flexible).  Day of week and date format should work in your locale.

![DueOn](https://raw.github.com/chrisparnin/attachables/master/doc/DueOn.png)


#### TODO menu actions: 

- **Attach here:** A reminder is attached to the corner of the editor viewport and only displayed when you are at this file.
- **Attach everywhere:** A reminder is attached to the corner of the editor viewport in all code windows.  You really want to remember it!
- **Due on:** A reminder that is note taken care of by the specified date will be displayed as a *compile error*!
- **Mark Complete:** A reminder that has a due by date can be marked complete from the menu.
- **Show next day:** A reminder will be displayed the next day in all code windows.
- **Show next week:** A reminder will be displayed the next week in all code windows.  Probably not a big deal.

Example context menu displayed that allows you to mark reminders with a due date complete.

![MarkComplete](https://raw.github.com/chrisparnin/attachables/master/doc/MarkComplete.png)

#### Attachable actions: 

Additional actions are available if you hover over a reminder.

- **Goto:** Navigate to source of TODO note.
- **Snooze:** Hide a reminder for the next 8 hours.
- **Done:** Complete and remove reminder from display.

![menu](https://raw.github.com/chrisparnin/attachables/master/doc/attachablesmenu.png)

If you have an expired todo note, you can use the context menu to *cancel*, *mark done*, or *snooze* the reminder.

![Cancel](https://raw.github.com/chrisparnin/attachables/master/doc/Cancel.png)

### Installing attachables

- Download .vsix and double click to install from [Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/850937ba-ff0b-43cb-badd-4e273b508c32), or search for "attachables" in Online Gallery in "Visual Studio's Tools > Extensions and Updates" tool menu.

- To install from source, first install the [Visual Studio SDK 2012](http://www.microsoft.com/en-us/download/details.aspx?id=30668) or [2010](http://www.microsoft.com/en-us/download/details.aspx?id=21835) in order to build project.  Then install the resulting .vsix file.

### Other platforms

`attachables` is also available in Chrome!  See https://github.com/chrisparnin/pm

### Future Features

- **Condition:** The reminder is displayed when a condition is true: When the code is a buildable state, you are debugging, or even an external condition such as a team member has committed code you are waiting on. 
- **Cloud Sync:** Reminders are synced to a reminder app, such as remember the milk.