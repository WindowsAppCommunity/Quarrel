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
            await UpdateNotificationBGTask();
            await RegisterBG("Main", "InvisibleBackgroundTask", new ToastNotificationActionTrigger(), false);
        }
        private static async Task RegisterBG(string taskName, string name, IBackgroundTrigger trigger, bool internetRequired)
        {
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            if (access == BackgroundAccessStatus.DeniedBySystemPolicy || access == BackgroundAccessStatus.DeniedByUser)
                return;
            var taskRegistered = false;
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
                builder.TaskEntryPoint = name + "." + taskName;
                if(internetRequired)
                    builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                builder.SetTrigger(trigger);
                builder.Register();
            }
        }
        public static async Task UpdateNotificationBGTask()
        {
            //Unregister task
            string taskName = "MainClass";
            string name = "DiscordBackgroundTask1";
            foreach (var task in BackgroundTaskRegistration.AllTasks)
                if (task.Value.Name == taskName)
                    task.Value.Unregister(true);
            //Re-register task
            if (Storage.Settings.BackgroundTaskTime != 0)
            {
                var trigger = new TimeTrigger(Convert.ToUInt32(Storage.Settings.BackgroundTaskTime*5), false);
                var access = await BackgroundExecutionManager.RequestAccessAsync();
                if (access == BackgroundAccessStatus.DeniedBySystemPolicy || access == BackgroundAccessStatus.DeniedByUser)
                    return;
                var builder = new BackgroundTaskBuilder();
                builder.Name = taskName;
                builder.TaskEntryPoint = name + "." + taskName;
                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                builder.SetTrigger(trigger);
                builder.Register();
            }
        }
    }
}
