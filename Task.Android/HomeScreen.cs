using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System.Collections.Generic;
using Task.Android.ApplicationLayer;
using Task.SharedCode;
using Task.SharedCode.Bl;

namespace Task.Android.Screens
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class HomeScreen : Activity

    {
        TodoItemListAdapter taskList;
        IList<TodoItem> tasks;
        Button addTaskButton;
        ListView taskListView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.HomeScreen);

            //Find our controls
            taskListView = FindViewById<ListView>(Resource.Id.TaskList);
            taskListView.FastScrollEnabled = true;
            addTaskButton = FindViewById<Button>(Resource.Id.AddButton);

            // wire up add task button handler
            if (addTaskButton != null)
            {
                addTaskButton.Click += (sender, e) =>
                {
                    StartActivity(typeof(TodoItemScreen));
                };
            }

            // wire up task click handler
            if (taskListView != null)
            {
                taskListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
                {
                    var taskDetails = new Intent(this, typeof(TodoItemScreen));
                    taskDetails.PutExtra("TaskID", tasks[e.Position].Id);
                    StartActivity(taskDetails);
                };

            }

        }
        protected override void OnResume()
        {
            base.OnResume();

            tasks = TodoItemManager.GetTasks();

            // create our adapter
            taskList = new TodoItemListAdapter(this, tasks);

            //Hook up our adapter to our ListView
            
            taskListView.Adapter = taskList;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] global::Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}