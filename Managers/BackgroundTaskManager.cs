using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Discord_UWP.Managers
{
    public static class BackgroundTaskManager
    {
        public static async void TryRegisterBackgroundTask()
        {
            var access = await BackgroundExecutionManager.RequestAccessAsync();

            //abort if access isn't granted
            if (access == BackgroundAccessStatus.DeniedBySystemPolicy || access == BackgroundAccessStatus.DeniedByUser)
                return;

            var taskRegistered = false;
            var exampleTaskName = "MainClass";

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == exampleTaskName)
                {
                    taskRegistered = true;
                    break;
                }
            }
            if (!taskRegistered)
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = exampleTaskName;
                builder.TaskEntryPoint = "DiscordBackgroundTask1.MainClass";
                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                TimeTrigger fifteenMinutes = new TimeTrigger(15, false);
                builder.SetTrigger(fifteenMinutes);
                builder.Register();
            }
        }
    }
}
