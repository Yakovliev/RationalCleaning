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
    public class CaptionedImagesAdapterFull : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        //Заголовки cardview, яким підписується рисунок
        private string[] captions;

        //id рисунків, які фігурують в cardview
        private int[] imageIds;

        private string[] descriptions;

        //Подія, що виникає при клікі по cardview
        public event EventHandler<int> ItemClick;

        public CaptionedImagesAdapterFull(string[] captions, int[] imageIds, string[] descriptions)
        {
            this.captions = captions;
            this.imageIds = imageIds;
            this.descriptions = descriptions;
        }

        public override int ItemCount
        {
            get { return captions.Length; }
        }


        public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            CardView cv = (CardView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.card_captioned_image, parent, false);

            ViewHolder viewHolder = new ViewHolder(cv, OnClick);
            return viewHolder;
        }

        public override void OnBindViewHolder(Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
        {
            ViewHolder viewHolder = holder as ViewHolder;

            CardView cardView = viewHolder.CardView;
            ImageView imageView = cardView.FindViewById<ImageView>(Resource.Id.info_image);
            Drawable drawable = cardView.Resources.GetDrawable(imageIds[position]);
            imageView.SetImageDrawable(drawable);
            imageView.ContentDescription = captions[position];
            TextView mainTextView = cardView.FindViewById<TextView>(Resource.Id.main_text);
            mainTextView.Text = captions[position];
            TextView infoTextView = cardView.FindViewById<TextView>(Resource.Id.info_text);
            infoTextView.Text = descriptions[position];
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