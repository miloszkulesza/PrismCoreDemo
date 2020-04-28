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
            new Task { Id = 1, EmployeeId = 1, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = new DateTime(2020, 4, 27), IsSucceeded = null },
            new Task { Id = 2, EmployeeId = 3, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = new DateTime(2020, 4, 24), IsSucceeded = true },
            new Task { Id = 3, EmployeeId = 2, Description = "Zrobić tak żeby było dobrze", Name = "Zadanie", TaskDate = new DateTime(2020, 4, 20), IsSucceeded = false }
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

        public void TaskFail(Task task)
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == task.Id)
                    Tasks[i].IsSucceeded = false;
            }
            eventAggregator.GetEvent<TaskFailedEvent>().Publish(task);
        }

        public void TaskSuccess(Task task)
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == task.Id)
                    Tasks[i].IsSucceeded = true;
            }
            eventAggregator.GetEvent<TaskSucceededEvent>().Publish(task);
        }
    }
}
