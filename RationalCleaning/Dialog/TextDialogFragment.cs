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
using Android.Database;
using Android.Database.Sqlite;
using Android.Support.V7.Widget;
using Android.Text.Format;
using Java.Util;
using Android.Graphics;
using Android.Views.InputMethods;


namespace RationalCleaning.Dialog
{
    public class TextDialogFragment : DialogFragment
    {
        View view;
        TextView positiveButton;
        TextView negativeButton;

        public string PrevText { get; private set; }
        public string CurrentText { get; private set; }
        public string Header { get; private set; }


        public delegate void PositiveButtonIsClickedHandler(object sender, EventArgsForTextDialogFragment e);
        public event PositiveButtonIsClickedHandler PositiveButtonIsClicked;

        LinedEditText linedEditText;

        private TextView headerTextView;

        public TextDialogFragment(string header, string prevText, PositiveButtonIsClickedHandler positiveButtonIsClicked)
        {
            PrevText = prevText;
            PositiveButtonIsClicked = positiveButtonIsClicked;
            Header = header;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle(Resource.String.title_text);

            view = inflater.Inflate(Resource.Layout.text_dialog_fragment, container, false);

            headerTextView = view.FindViewById<TextView>(Resource.Id.header);
            headerTextView.Text = Header;

            positiveButton = view.FindViewById<TextView>(Resource.Id.PositiveButton);
            negativeButton = view.FindViewById<TextView>(Resource.Id.NegativeButton);
            linedEditText = view.FindViewById<LinedEditText>(Resource.Id.linedEditText);

            string nominalMotivation = GetString(Resource.String.room_title);

            if (nominalMotivation != PrevText)
            {
                linedEditText.Text = PrevText;
            }

            negativeButton.SetBackgroundColor(Color.White);
            positiveButton.SetBackgroundColor(Color.White);
            negativeButton.Text = GetString(Resource.String.negative_button_lined_text_dialog);
            positiveButton.Text = GetString(Resource.String.positive_button_lined_text_dialog);

            negativeButton.Click += NegativeButton_Click;
            positiveButton.Click += PositiveButton_Click;

            linedEditText.SetSelection(linedEditText.Text.Length); //Ставимо курсор в кінець

            //Виводимо клавіатуру при відкритті діалогового вікна
            linedEditText.RequestFocus();
            Dialog.Window.SetSoftInputMode(SoftInput.StateVisible);


            return view;
        }

        private void PositiveButton_Click(object sender, EventArgs e)
        {
            CurrentText = linedEditText.Text;

            EventArgsForTextDialogFragment eventArgsForMotivationDialogFragment = new EventArgsForTextDialogFragment(CurrentText);
            if (PositiveButtonIsClicked != null)
            {
                PositiveButtonIsClicked(sender, eventArgsForMotivationDialogFragment);
            }
            Dismiss();
        }

        private void NegativeButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

    }
}