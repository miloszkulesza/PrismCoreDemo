# PrismCoreDemo
WPF modular demo app in .NET 10 using Prism 9 and Unity.
There are 3 modules - Employees, Tasks, Calendar.

## Build and tests

Run on Windows with the .NET 10 SDK:

```powershell
dotnet build PrismCoreDemo.sln
dotnet test PrismCoreDemo.Tests/PrismCoreDemo.Tests.csproj
dotnet run --project PrismCoreDemo/PrismCoreDemo.csproj
```

## Responsibilities and screen lifetime

- `Core`: models, validation rules and repository contracts, without Prism or WPF dependencies.
- `ApplicationCore`: notifications about changes to employees and tasks, using Prism Events.
- `Infrastructure`: in-memory repositories. They validate writes and expose read-only snapshots.
- `Presentation`: workspace coordination, Ribbon adapter, dialogs, shared XAML resources and base ViewModels.
- Feature modules: views and ViewModels for employees, tasks and the calendar.
- `PrismCoreDemo`: the shell and dependency registration.

The existing Ribbon is the only tab navigator. Its contextual groups (Employees, Tasks and Calendar) contain the tabs of open screens; modules declare their group through RibbonTab.ContextualTabGroupHeader. Employee and task editors open a separate screen for each record, and each Add action opens a separate new draft. Editing a record already open activates its screen and preserves its draft. Lists and the calendar have one instance each. Selecting a Ribbon tab activates its form; closing the active screen selects another screen in the same contextual group first. Each form owns its state and shares its DataContext with its Ribbon tab. Closing a dirty editor asks before discarding changes. Saving validates through both the command and repository and closes the editor without a discard prompt.

Employees with assigned tasks cannot be deleted. Reassign or delete those tasks first. Without employees, the task editor explains the missing requirement and disables saving. Lists and calendar dates refresh after data changes; dates are compared by their day, regardless of the time component.

Regression tests cover empty collections, validation, protected repository state, employee deletion and renaming, calendar date changes, editor drafts and real Prism/Unity navigation with Ribbon synchronization. The integration test creates a hidden WPF presentation source and runs on an STA thread.

This remains an in-memory demo: data is reset when the application starts.

![Main window](https://i.imgur.com/4xwCdV6.png)
![Employees list](https://i.imgur.com/zA8lOun.png)
![Employees edit](https://i.imgur.com/H3E64Tj.png)
![Tasks list](https://i.imgur.com/PwLWvCd.png)
![Task edit](https://i.imgur.com/VLN5bTJ.png)
![Tasks shedule](https://i.imgur.com/i76kBtw.png)
