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

namespace RationalCleaning.Database
{
    class CreateRoomAsyncTask : AsyncTask<int, int, bool>
    {
        Activity myActivity;

        private string roomTitle;
        private int imageId;


        public CreateRoomAsyncTask(Activity activity, string roomTitle, int imageId)
        {
            myActivity = activity;
            this.roomTitle = roomTitle;
            this.imageId = imageId;

        }

        //Сюди будемо передавати одиницю, якщо хочемо здійснити запис в базу даних
        protected override bool RunInBackground(params int[] @params)
        {
            int param = @params[0];

            if (param == 1)
            {
                SQLiteOpenHelper createRoomDatabaseHelper = new RationalCleaningDatabaseHelper(myActivity);

                try
                {
                    SQLiteDatabase db = createRoomDatabaseHelper.WritableDatabase;

                    ContentValues roomValues = new ContentValues();
                    roomValues.Put("TITLE", roomTitle);
                    roomValues.Put("IS_ROOM", 1);
                    roomValues.Put("IMAGE_ID", imageId);

                    db.Insert("ROOM_TABLE", null, roomValues);

                    db.Close();

                    return true;

                }
                catch (SQLException)
                {
                    return false;
                }
            }

            return false;
        }

        protected override void OnPostExecute(bool result)
        {
            base.OnPostExecute(result);
            if (!result)
            {
                Toast toast = Toast.MakeText(myActivity, "Database unavailable", ToastLength.Short);
                toast.Show();
            }
        }
    }
}