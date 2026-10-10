using Core.DataAccess;
using Core.Models;
using ApplicationCore.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.DataAccess
{
    public class TasksRepository : ITasksRepository
    {
        private readonly IEventAggregator events;
        private readonly List<Task> tasks = new()
        {
            new Task { Id = 1, EmployeeId = 1, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = new DateTime(2020, 4, 27) },
            new Task { Id = 2, EmployeeId = 3, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = new DateTime(2020, 4, 24), IsSucceeded = true },
            new Task { Id = 3, EmployeeId = 2, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = new DateTime(2020, 4, 20), IsSucceeded = false }
        };
        private int nextId = 4;
        public IReadOnlyList<Task> Tasks => tasks.Select(x => new Task(x)).ToList().AsReadOnly();
        public TasksRepository(IEventAggregator events) => this.events = events;

        public void Delete(Task task)
        {
            ArgumentNullException.ThrowIfNull(task);
            var existing = tasks.FirstOrDefault(x => x.Id == task.Id);
            if (existing == null) return;
            tasks.Remove(existing);
            events.GetEvent<TaskDeletedEvent>().Publish(new Task(existing));
        }

        public void Save(Task task)
        {
            ArgumentNullException.ThrowIfNull(task);
            if (!task.IsValid() || task.EmployeeId <= 0 || task.TaskDate == default)
                throw new ArgumentException("Wprowadź poprawne dane zadania, datę i pracownika.", nameof(task));
            var index = tasks.FindIndex(x => x.Id == task.Id);
            if (index >= 0)
            {
                tasks[index] = new Task(task);
                events.GetEvent<TaskUpdatedEvent>().Publish(new Task(task));
            }
            else
            {
                task.Id = nextId++;
                tasks.Add(new Task(task));
                events.GetEvent<TaskAddedEvent>().Publish(new Task(task));
            }
        }

        public void TaskSuccess(Task task) => SetStatus(task, true);
        public void TaskFail(Task task) => SetStatus(task, false);
        private void SetStatus(Task task, bool succeeded)
        {
            ArgumentNullException.ThrowIfNull(task);
            var existing = tasks.FirstOrDefault(x => x.Id == task.Id);
            if (existing == null) return;
            existing.IsSucceeded = succeeded;
            if (succeeded) events.GetEvent<TaskSucceededEvent>().Publish(new Task(existing));
            else events.GetEvent<TaskFailedEvent>().Publish(new Task(existing));
        }
    }
}