using aydocs.NotchWin.Resources;
using aydocs.NotchWin.UI.UIElements;
using aydocs.NotchWin.Utils;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aydocs.NotchWin.UI.Widgets.Big
{
    class RegisterTasksWidget : IRegisterableWidget
    {
        public bool IsSmallWidget => false;
        public string WidgetName => "Tasks & Reminders";

        public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        {
            return new TasksWidget(parent, position, alignment);
        }
    }

    class RegisterTasksWidgetSettings : IRegisterableSetting
    {
        public string SettingID => "taskswidget";
        public string SettingTitle => "Tasks";

        public static TasksWidgetSave saveData;

        public struct TasksWidgetSave
        {
            public bool showCompleted;
            public int maxVisibleTasks;
        }

        public void LoadSettings()
        {
            if (SaveManager.Contains(SettingID))
            {
                saveData = JsonConvert.DeserializeObject<TasksWidgetSave>((string)SaveManager.Get(SettingID));
            }
            else
            {
                saveData = new TasksWidgetSave()
                {
                    showCompleted = false,
                    maxVisibleTasks = 5
                };
            }
        }

        public void SaveSettings()
        {
            SaveManager.Add(SettingID, JsonConvert.SerializeObject(saveData));
        }

        public List<UIObject> SettingsObjects()
        {
            var objects = new List<UIObject>();

            var showCompletedCheckbox = new DWCheckbox(null, "Show completed tasks", new Vec2(25, 0), new Vec2(25, 25), null, UIAlignment.TopLeft);
            showCompletedCheckbox.IsChecked = saveData.showCompleted;
            showCompletedCheckbox.clickCallback += () => saveData.showCompleted = showCompletedCheckbox.IsChecked;
            objects.Add(showCompletedCheckbox);

            return objects;
        }
    }

    public class TasksWidget : WidgetBase
    {
        private DWText taskCountText;
        private DWText noTasksText;
        private List<DWText> taskTextElements = new List<DWText>();
        private List<DWCheckbox> taskCheckboxes = new List<DWCheckbox>();

        private float updateTimer = 0f;
        private const float UPDATE_INTERVAL = 1.0f;

        public TasksWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
        {
            roundRadius = 12f;
            isSmallWidget = false;

            // Task count display (collapsed state)
            taskCountText = new DWText(this, "0 Tasks", new Vec2(0, 0), UIAlignment.Center);
            taskCountText.TextSize = 14;
            taskCountText.Font = Res.SFProBold;
            taskCountText.Color = Theme.TextMain;
            AddLocalObject(taskCountText);

            // No tasks text (expanded state)
            noTasksText = new DWText(this, "No active tasks", new Vec2(0, 0), UIAlignment.Center);
            noTasksText.TextSize = 12;
            noTasksText.Font = Res.SFProRegular;
            noTasksText.Color = Theme.TextSecond;
            noTasksText.SilentSetActive(false);
            AddLocalObject(noTasksText);

            RefreshTasks();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            updateTimer += deltaTime;
            if (updateTimer >= UPDATE_INTERVAL)
            {
                RefreshTasks();
                updateTimer = 0f;
            }
        }

        private void RefreshTasks()
        {
            TaskManager taskManager = TaskManager.Instance;
            List<TaskManager.TaskItem> activeTasks = taskManager.GetActiveTasks();
            int taskCount = taskManager.GetActiveTaskCount();

            taskCountText.Text = taskCount == 1 ? "1 Task" : taskCount + " Tasks";

            if (activeTasks.Count == 0)
            {
                noTasksText.SetActive(true);
                taskCountText.SetActive(false);
            }
            else
            {
                taskCountText.SetActive(true);
                noTasksText.SetActive(false);
            }
        }

        protected override float GetWidgetWidth()
        {
            return 350;
        }

        protected override float GetWidgetHeight()
        {
            return 100;
        }
    }
}
