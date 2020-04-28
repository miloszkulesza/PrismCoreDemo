using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DataAccess
{
    public interface ITasksRepository
    {
        IList<Task> Tasks { get; }
        void Save(Task task);
        void Delete(Task task);
        void TaskSuccess(Task task);
        void TaskFail(Task task);
    }
}
