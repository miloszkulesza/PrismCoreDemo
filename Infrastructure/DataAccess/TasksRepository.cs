using Infrastructure.Events;
using Infrastructure.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.DataAccess
{
    public class TasksRepository : ITasksRepository
    {
        private readonly IEventAggregator eventAggregator;

        public TasksRepository(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public IList<Task> Tasks { get; } = new List<Task>
        {
            new Task { EmployeeId = 1, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = DateTime.Now, IsSucceeded = null },
            new Task { EmployeeId = 3, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = DateTime.Now, IsSucceeded = true },
            new Task { EmployeeId = 2, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = DateTime.Now, IsSucceeded = false }
        };

        public void Delete(Task task)
        {
            Tasks.Remove(task);
            eventAggregator.GetEvent<TaskDeletedEvent>().Publish(task);
        }

        public void Save(Task task)
        {
            if (Tasks.Any(x => x.Id == task.Id))
            {
                for (int i = 0; i < Tasks.Count; i++)
                {
                    if (Tasks[i].Id == task.Id)
                        Tasks[i] = task;
                }
                eventAggregator.GetEvent<TaskUpdatedEvent>().Publish(task);
            }
            else
            {
                task.Id = Tasks.LastOrDefault().Id + 1;
                Tasks.Add(task);
                eventAggregator.GetEvent<TaskAddedEvent>().Publish(task);
            }
        }
    }
}
