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
    public class CaptionedImagesWithSpinner : Android.Support.V7.Widget.RecyclerView.Adapter
    {

        //Заголовки cardview, які відображаються в spinner (для кожної карточки один і той же набір)
        private string[] captions;

        //id рисунків, які фігурують в cardview
        private int[] imageIds;

        private string[] actionText;

        //Позначає, чи відображається кнопка (точніше, TextView), яка відповідає за другу активну дію.
        private bool[] isSecondActionButtonVisible;

        private int[] spinnerPosition;

        private string[] timeOfRoomCleaning;
        private string[] roomCleanness;


        Context context;

        //Подія, що виникає при клікі по cardview
        public event EventHandler<int> ItemClick;

        public event EventHandler Action1_Click;
        public event EventHandler Action2_Click;
        public event EventHandler<AdapterView.ItemSelectedEventArgs> SpinnerItemSelectionChanged;




        public CaptionedImagesWithSpinner(Context context, int[] spinnerPosition, string[] captions, int[] imageIds, string[] actionText, 
            bool[] isSecondActionButtonVisible, string[] timeOfRoomCleaning, string[] roomCleanness)
        {
            this.captions = captions;
            this.imageIds = imageIds;
            this.context = context;

            this.actionText = new string[2];
            this.actionText = actionText;

            this.isSecondActionButtonVisible = isSecondActionButtonVisible;

            this.spinnerPosition = spinnerPosition;

            this.timeOfRoomCleaning = timeOfRoomCleaning;
            this.roomCleanness = roomCleanness;
        }

        /// <summary>
        /// Кількість карточок розраховуємо за кількістю переданих картинок
        /// </summary>
        public override int ItemCount
        {
            get { return isSecondActionButtonVisible.Length; }
        }


        public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            CardView cv = (CardView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.card_captioned_images_with_spinner, parent, false);

            SpinnerViewHolder viewHolder = new SpinnerViewHolder(cv, OnClick, spinner_ItemSelected);
            return viewHolder;
        }

        public override void OnBindViewHolder(Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
        {
            SpinnerViewHolder viewHolder = holder as SpinnerViewHolder;

            CardView cardView = viewHolder.CardView;
            ImageView mainImageView = cardView.FindViewById<ImageView>(Resource.Id.info_image);
            mainImageView.SetImageResource(imageIds[spinnerPosition[position]]);
            mainImageView.ContentDescription = captions[spinnerPosition[position]];

            Spinner roomSpinner = viewHolder.Spinner;
            var spinnerAdapter = new ArrayAdapter<string>(context, Resource.Layout.custom_xml_spinner_layout, captions);
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            roomSpinner.Adapter = spinnerAdapter;
            roomSpinner.SetSelection(spinnerPosition[position]);
            //roomSpinner.ItemSelected += RoomSpinner_ItemSelected;


            TextView actionTextView1 = cardView.FindViewById<TextView>(Resource.Id.action1_textview);
            actionTextView1.Text = actionText[0];
            actionTextView1.Click += Action1_Click;

            TextView actionTextView2 = cardView.FindViewById<TextView>(Resource.Id.action2_textview);
            actionTextView2.Text = actionText[1];
            actionTextView2.Click += Action2_Click;

            if (isSecondActionButtonVisible[position])
            {
                actionTextView2.Visibility = ViewStates.Visible;
            }
            else
            {
                actionTextView2.Visibility = ViewStates.Invisible;
            }

            View view = cardView.FindViewById<View>(Resource.Id.line_view);
            view.SetBackgroundColor(new Android.Graphics.Color(224, 224, 224));

            TextView infoTextView = cardView.FindViewById<TextView>(Resource.Id.info_text);
            infoTextView.Text = context.GetString(Resource.String.info_text_for_change_room_title);
            infoTextView.SetTextColor(new Android.Graphics.Color(189, 189, 189));

            ImageView timeImageView = cardView.FindViewById<ImageView>(Resource.Id.time_image_view);
            timeImageView.SetImageResource(Resource.Drawable.stopwatch);

            ImageView cleannessImageView = cardView.FindViewById<ImageView>(Resource.Id.cleanness_image_view);
            cleannessImageView.SetImageResource(Resource.Drawable.performance);


            TextView timeTextView = cardView.FindViewById<TextView>(Resource.Id.time_text_view);
            TextView cleannessTextView = cardView.FindViewById<TextView>(Resource.Id.cleanness_text_view);

            timeTextView.Text = timeOfRoomCleaning[spinnerPosition[position]];
            cleannessTextView.Text = roomCleanness[spinnerPosition[position]];
        }


        void OnClick(int position)
        {
            if (ItemClick != null)
            {
                ItemClick(this, position);
            }
        }

        void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (SpinnerItemSelectionChanged != null)
            {
                SpinnerItemSelectionChanged(sender, e);
            }
        }
    }
}