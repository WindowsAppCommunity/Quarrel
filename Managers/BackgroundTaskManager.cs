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
            /*
            //Register time triggered background task
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            if (access == BackgroundAccessStatus.DeniedBySystemPolicy || access == BackgroundAccessStatus.DeniedByUser)
                return;
            var taskRegistered = false;
            var taskName = "MainClass";
            foreach (var task in BackgroundTaskRegistration.AllTasks)
                if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(taskName)))
                {
                    taskRegistered = true;
                    break;
                }
            if (!taskRegistered)
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = taskName;
                builder.TaskEntryPoint = "DiscordBackgroundTask1.MainClass";
                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                TimeTrigger timetrigger = new TimeTrigger(25, false);
                builder.SetTrigger(timetrigger);
                builder.Register();
            }
            
            //Register invisible background task, to handle notification clicks
            const string taskName2 = "InvisibleBackgroundTask";
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(taskName2)))
                return; //Now we can just return, because we don't need to execute the rest of the code
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
            BackgroundTaskBuilder builder2 = new BackgroundTaskBuilder()
            {
                Name = taskName2,
            };

            // Assign the toast action trigger
            builder2.SetTrigger(new ToastNotificationActionTrigger());

            // And register the task
            builder2.Register();*/
        }
    }
}
