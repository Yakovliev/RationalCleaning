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
    public class CaptionedImagesWithTwoActionsAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {

        //Заголовки cardview, яким підписується рисунок
        private string[] captions;

        //id рисунків, які фігурують в cardview
        private int[] imageIds;

        private string[] actionText;

        Context context;

        //Подія, що виникає при клікі по cardview
        public event EventHandler<int> ItemClick;

        public event EventHandler Action1_Click;
        public event EventHandler Action2_Click;




        public CaptionedImagesWithTwoActionsAdapter(Context context, string[] captions, int[] imageIds, string[] actionText)
        {
            this.captions = captions;
            this.imageIds = imageIds;
            this.context = context;

            this.actionText = new string[2];
            this.actionText = actionText;
        }

        public override int ItemCount
        {
            get { return captions.Length; }
        }


        public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            CardView cv = (CardView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.card_captioned_image_with_two_actions, parent, false);

            ViewHolder viewHolder = new ViewHolder(cv, OnClick);
            return viewHolder;
        }

        public override void OnBindViewHolder(Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
        {
            ViewHolder viewHolder = holder as ViewHolder;

            CardView cardView = viewHolder.CardView;
            ImageView mainImageView = cardView.FindViewById<ImageView>(Resource.Id.info_image);
            Drawable drawable = cardView.Resources.GetDrawable(imageIds[position]);
            mainImageView.SetImageDrawable(drawable);
            mainImageView.ContentDescription = captions[position];
            TextView titleTextView = cardView.FindViewById<TextView>(Resource.Id.room_title_textview);
            titleTextView.Text = captions[position];
            
            
            //ImageView iconImage = cardView.FindViewById<ImageView>(Resource.Id.info_icon);
            //iconImage.SetImageResource(Resource.Drawable.ic_edit);
            //iconImage.ContentDescription = captions[position];
            //iconImage.RequestLayout();
            //iconImage.LayoutParameters.Height = Convert.ToInt16(titleTextView.TextSize);
            //iconImage.SetColorFilter(new Android.Graphics.Color(context.GetColor(Resource.Color.colorAction)));
            
            TextView actionTextView1 = cardView.FindViewById<TextView>(Resource.Id.action1_textview);
            actionTextView1.Text = actionText[0];
            actionTextView1.Click += Action1_Click;

            TextView actionTextView2 = cardView.FindViewById<TextView>(Resource.Id.action2_textview);
            actionTextView2.Text = actionText[1];
            actionTextView2.Click += Action2_Click;

            View view = cardView.FindViewById<View>(Resource.Id.line_view);
            view.SetBackgroundColor(new Android.Graphics.Color(224, 224, 224));

            TextView infoTextView = cardView.FindViewById<TextView>(Resource.Id.info_text);
            infoTextView.Text = context.GetString(Resource.String.info_text_for_room_title);
            infoTextView.SetTextColor(new Android.Graphics.Color(189, 189, 189));

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