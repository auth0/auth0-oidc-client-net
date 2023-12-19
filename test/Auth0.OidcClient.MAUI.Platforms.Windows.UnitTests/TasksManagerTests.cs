using Auth0.OidcClient.Platforms.Windows;

namespace Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests
{
    public class TasksManagerTests
    {
        [Fact]
        public void Should_add_and_resume_task()
        {
            var taskId = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<Uri>();

            TasksManager.Default.Add(taskId, tcs);

            var callbackUri = new Uri("myapp://callback");

            TasksManager.Default.ResumeTask(callbackUri, taskId);

            Assert.True(tcs.Task.IsCompleted);
            Assert.Equal(tcs.Task.Result, callbackUri);
        }

        [Fact]
        public void Should_not_resume_task_when_removed()
        {
            var taskId = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<Uri>();

            TasksManager.Default.Add(taskId, tcs);
            TasksManager.Default.Remove(taskId);

            var callbackUri = new Uri("myapp://callback");

            TasksManager.Default.ResumeTask(callbackUri, "123");

            Assert.False(tcs.Task.IsCompleted);
        }
    }
}