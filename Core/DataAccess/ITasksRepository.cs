using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DataAccess
{
    public interface ITasksRepository
    {
        IReadOnlyList<Task> Tasks { get; }
        void Save(Task task);
        void Delete(Task task);
        void TaskSuccess(Task task);
        void TaskFail(Task task);
    }
}
