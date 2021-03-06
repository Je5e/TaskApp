using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task.SharedCode;
using Task.SharedCode.Bl;

namespace Task.Android.ApplicationLayer
{
    class TodoItemListAdapter : BaseAdapter<TodoItem>
    {
        readonly Activity context = null;
        readonly IList<TodoItem> tasks = new List<TodoItem>();

		public TodoItemListAdapter(Activity context, IList<TodoItem> tasks) : base()
		{
			this.context = context;
			this.tasks = tasks;
		}

		public override TodoItem this[int position]
		{
			get { return tasks[position]; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override int Count
		{
			get { return tasks.Count; }
		}

		public override global::Android.Views.View GetView(int position,
            View convertView,
            ViewGroup parent)
		{
			// Get our object for position
			var item = tasks[position];

            //Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
            // gives us some performance gains by not always inflating a new view
            // will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()

            //var view = (convertView ??
            //        context.LayoutInflater.Inflate(
            //        Resource.Layout.TaskListItem,
            //        parent,
            //        false)) as LinearLayout;
            //// Find references to each subview in the list item's view
            //var txtName = view.FindViewById<TextView>(Resource.Id.NameText);
            //var txtDescription = view.FindViewById<TextView>(Resource.Id.NotesText);
            ////Assign item's values to the various subviews
            //txtName.SetText(item.Name, TextView.BufferType.Normal);
            //txtDescription.SetText(item.Notes, TextView.BufferType.Normal);

            // TODO: use this code to populate the row, and remove the above view
            var view = (convertView ??
				context.LayoutInflater.Inflate(
					global::Android.Resource.Layout.SimpleListItemChecked,
					parent,
					false)) as CheckedTextView;
			view.SetText(item.Name == "" ? "<new task>" : item.Name, TextView.BufferType.Normal);
			view.Checked = item.Done;


            //Finally return the view
            return view;
		}
	}

}
