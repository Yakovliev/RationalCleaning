using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using RationalCleaning.CleaningTasks;

namespace RationalCleaning.RecyclerView
{
    public class TaskAdapterExtended : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        //Подія, що виникає при клікі по cardview
        public event EventHandler<int> ItemClick;

        public event EventHandler<int> ActionTextViewClick;


        public List<TaskCardSimple> TaskCardSimpleList { get; set; }

        public TaskAdapterExtended(List<TaskCardSimple> taskCardSimpleList)
        {
            TaskCardSimpleList = taskCardSimpleList;
        }

        public override int ItemCount
        {
            get { return TaskCardSimpleList.Count; }
        }

        public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            CardView cv = (CardView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.card_task_extended, parent, false);

            ViewHolderCardTaskExtended viewHolder = new ViewHolderCardTaskExtended(cv, OnClick, OnActionTextViewClick);
            return viewHolder;
        }

        public override void OnBindViewHolder(Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
        {
            ViewHolderCardTaskExtended viewHolderCardTask = holder as ViewHolderCardTaskExtended;

            viewHolderCardTask.MainTextView.Text = TaskCardSimpleList[position].TaskTitle;
            viewHolderCardTask.FirstImageView.SetImageResource(TaskCardSimpleList[position].TimeImageId);
            viewHolderCardTask.FirstTextView.Text = TaskCardSimpleList[position].TimeOfCleaning;
            viewHolderCardTask.SecondImageView.SetImageResource(TaskCardSimpleList[position].CleannessImageId);
            viewHolderCardTask.SecondTextView.Text = TaskCardSimpleList[position].Cleanness;
            viewHolderCardTask.LineView.SetBackgroundColor(new Android.Graphics.Color(224, 224, 224));
            viewHolderCardTask.ThirdTextView.Text = TaskCardSimpleList[position].RoomTitle;
            viewHolderCardTask.ForthTextView.Text = TaskCardSimpleList[position].ActionTextViewText;
            viewHolderCardTask.ActionTextView.Text = TaskCardSimpleList[position].ActionTextViewText2;

        }


        void OnClick(int position)
        {
            if (ItemClick != null)
            {
                ItemClick(this, position);
            }
        }

        void OnActionTextViewClick(int position)
        {
            if (ItemClick != null)
            {
                ActionTextViewClick(this, position);
            }
        }

    }
}