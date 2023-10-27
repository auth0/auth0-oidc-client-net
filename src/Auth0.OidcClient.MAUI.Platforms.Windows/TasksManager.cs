using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests")]
namespace Auth0.OidcClient.Platforms.Windows
{
    interface ITasksManager
    {
        void Remove(string taskId);
        void Add(string taskId, TaskCompletionSource<Uri> tcs);
        void ResumeTask(Uri callbackUri, string taskId);
    }

    internal sealed class TasksManager : ITasksManager
    {
        public static readonly TasksManager Default = new TasksManager();

        private readonly Dictionary<string, TaskCompletionSource<Uri>> _tasks = new Dictionary<string, TaskCompletionSource<Uri>>();
        private TasksManager() { }

        public void Remove(string taskId)
        {
            if (_tasks.ContainsKey(taskId))
            {
                _tasks.Remove(taskId);
            }
        }

        public void Add(string taskId, TaskCompletionSource<Uri> tcs)
        {
            _tasks.Add(taskId, tcs);
        }

        public void ResumeTask(Uri callbackUri, string taskId)
        {
            if (taskId != null && _tasks.ContainsKey(taskId))
            {
                var task = _tasks[taskId];
                _tasks.Remove(taskId);
                task.TrySetResult(callbackUri);
            }
        }

    }
}