using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aydocs.NotchWin.Utils
{
    public class TaskManager
    {
        private static TaskManager _instance;
        private string taskDataPath;
        private List<TaskItem> tasks = new List<TaskItem>();

        [JsonObject]
        public class TaskItem
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("completed")]
            public bool Completed { get; set; }

            [JsonProperty("dueDate")]
            public string DueDate { get; set; }

            [JsonProperty("createdDate")]
            public long CreatedTicks { get; set; }

            public TaskItem()
            {
                Id = Guid.NewGuid().ToString();
                CreatedTicks = DateTime.Now.Ticks;
            }

            public TaskItem(string text)
            {
                Id = Guid.NewGuid().ToString();
                Text = text;
                Completed = false;
                DueDate = "";
                CreatedTicks = DateTime.Now.Ticks;
            }
        }

        public static TaskManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TaskManager();
                }
                return _instance;
            }
        }

        public TaskManager()
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NotchWin");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            taskDataPath = Path.Combine(appDataPath, "TaskData.json");
            LoadTasks();
        }

        public void LoadTasks()
        {
            try
            {
                if (File.Exists(taskDataPath))
                {
                    string json = File.ReadAllText(taskDataPath);
                    tasks = JsonConvert.DeserializeObject<List<TaskItem>>(json) ?? new List<TaskItem>();
                }
            }
            catch
            {
                tasks = new List<TaskItem>();
            }
        }

        public void SaveTasks()
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(taskDataPath, json);
            }
            catch
            {
                // Silently fail if we can't save
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            return new List<TaskItem>(tasks);
        }

        public List<TaskItem> GetActiveTasks()
        {
            return tasks.Where(t => !t.Completed).ToList();
        }

        public List<TaskItem> GetCompletedTasks()
        {
            return tasks.Where(t => t.Completed).ToList();
        }

        public void AddTask(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var newTask = new TaskItem(text);
            tasks.Add(newTask);
            SaveTasks();
        }

        public void RemoveTask(string taskId)
        {
            tasks.RemoveAll(t => t.Id == taskId);
            SaveTasks();
        }

        public void ToggleTask(string taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.Completed = !task.Completed;
                SaveTasks();
            }
        }

        public void UpdateTask(string taskId, string newText)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.Text = newText;
                SaveTasks();
            }
        }

        public int GetTaskCount()
        {
            return tasks.Count;
        }

        public int GetActiveTaskCount()
        {
            return tasks.Count(t => !t.Completed);
        }
    }
}
