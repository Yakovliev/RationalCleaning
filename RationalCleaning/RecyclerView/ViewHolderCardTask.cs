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

namespace RationalCleaning.RecyclerView
{
    public class ViewHolderCardTask : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public TextView MainTextView { get; private set; }
        public ImageView FirstImageView { get; private set; }
        public TextView FirstTextView { get; private set; }
        public ImageView SecondImageView { get; private set; }
        public TextView SecondTextView { get; private set; }
        public TextView ThirdTextView { get; private set; }
        public Switch MainSwitch { get; private set; }
        public View LineView { get; private set; }
        public TextView ActionTextView { get; private set; }


        public ViewHolderCardTask(CardView cardView, Action<int> listener, Action<int> mainSwitchListener, Action<int> actionTextViewListener)
            : base(cardView)
        {
            MainTextView = cardView.FindViewById<TextView>(Resource.Id.main_text_view);
            FirstImageView = cardView.FindViewById<ImageView>(Resource.Id.first_image_view);
            FirstTextView = cardView.FindViewById<TextView>(Resource.Id.first_text_view);
            SecondImageView = cardView.FindViewById<ImageView>(Resource.Id.second_image_view);
            SecondTextView = cardView.FindViewById<TextView>(Resource.Id.second_text_view);
            ThirdTextView = cardView.FindViewById<TextView>(Resource.Id.third_text_view);
            MainSwitch = cardView.FindViewById<Switch>(Resource.Id.main_switch);
            LineView = cardView.FindViewById<View>(Resource.Id.line_view);
            ActionTextView = cardView.FindViewById<TextView>(Resource.Id.action_text_view);


            cardView.Click += (sender, e) => listener(base.LayoutPosition);
            MainSwitch.Click += (sender, e) => mainSwitchListener(base.LayoutPosition);
            ActionTextView.Click += (sender, e) => actionTextViewListener(base.LayoutPosition);

        }
    }
}