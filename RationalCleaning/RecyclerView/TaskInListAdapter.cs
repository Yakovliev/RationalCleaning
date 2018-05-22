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
using Android.Graphics.Drawables;

namespace RationalCleaning.RecyclerView
{
    public class TaskInListAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        //Назви кімнат
        private string[] roomTitles;

        //Час прибирання кімнати
        private string[] timeOfRoomCleaning;

        //Подія, що виникає при клікі по cardview
        public event EventHandler<int> ItemClick;

        public TaskInListAdapter(string[] roomTitles, string[] timeOfRoomCleaning)
        {
            this.roomTitles = roomTitles;
            this.timeOfRoomCleaning = timeOfRoomCleaning;
        }

        public override int ItemCount
        {
            get { return roomTitles.Length; }
        }


        public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            CardView cv = (CardView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.card_task_in_list, parent, false);

            ViewHolder viewHolder = new ViewHolder(cv, OnClick);
            return viewHolder;
        }

        public override void OnBindViewHolder(Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
        {
            ViewHolder viewHolder = holder as ViewHolder;
            CardView cardView = viewHolder.CardView;

            TextView roomTitleTextView = cardView.FindViewById<TextView>(Resource.Id.room_title_text_view);
            roomTitleTextView.Text = roomTitles[position];

            ImageView timeImageView = cardView.FindViewById<ImageView>(Resource.Id.time_image_view);
            timeImageView.SetImageResource(Resource.Drawable.stopwatch);
            timeImageView.ContentDescription = timeOfRoomCleaning[position];

            TextView timeTextView = cardView.FindViewById<TextView>(Resource.Id.time_text_view);
            timeTextView.Text = timeOfRoomCleaning[position];

        }

        void OnClick(int position)
        {
            if (ItemClick != null)
            {
                ItemClick(this, position);
            }
        }

    }
}