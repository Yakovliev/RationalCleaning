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
    public class SpinnerViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public CardView CardView { get; private set; }
        public Spinner Spinner { get; set; }

        public SpinnerViewHolder(CardView cardView, Action<int> listener, Action<object, AdapterView.ItemSelectedEventArgs> spinnerItemSelected)
            : base(cardView)
        {
            CardView = cardView;

            cardView.Click += (sender, e) => listener(base.LayoutPosition);

            Spinner = cardView.FindViewById<Spinner>(Resource.Id.room_spinner);
            Spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerItemSelected);
        }
    }
}