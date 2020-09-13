using System.Collections.Generic;
using Grimity.Data;

namespace Features.Tasks {
public class WorkerData {
    public readonly HashSet<BaseTask> DeclinedTasks;
    public Optional<BaseTask> Task;

    public WorkerData(Optional<BaseTask> task) {
        Task = task;
        DeclinedTasks = new HashSet<BaseTask>();
    }
}
}