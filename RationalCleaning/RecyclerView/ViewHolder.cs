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
    public class ViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {


        public CardView CardView { get; private set; }
        public ViewHolder(CardView cardView, Action<int> listener)
            : base(cardView)
        {
            CardView = cardView;

            cardView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}