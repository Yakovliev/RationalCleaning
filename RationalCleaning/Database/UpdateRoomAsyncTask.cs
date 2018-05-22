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
    public class UpdateRoomAsyncTask : AsyncTask<int, int, bool>
    {
        Activity myActivity;

        private string roomTitle;
        private int roomId;
        private int roomImageId;

        public UpdateRoomAsyncTask(Activity activity, int roomId, string roomTitle, int roomImageId)
        {
            myActivity = activity;
            this.roomTitle = roomTitle;
            this.roomId = roomId;
            this.roomImageId = roomImageId;
        }

        //Сюди будемо передавати одиницю, якщо хочемо здійснити запис в базу даних
        protected override bool RunInBackground(params int[] @params)
        {
            int param = @params[0];

            if (param == 1)
            {
                SQLiteOpenHelper updateRoomDatabaseHelper = new RationalCleaningDatabaseHelper(myActivity);

                try
                {
                    SQLiteDatabase db = updateRoomDatabaseHelper.WritableDatabase;

                    ContentValues roomValues = new ContentValues();
                    roomValues.Put("TITLE", roomTitle);
                    roomValues.Put("IMAGE_ID", roomImageId);

                    //Якщо провтикать, то вийде оновити title розділів, у яких title не повинен змінюватися (розділи із умовою isRoom == 0 (true))
                    //По хорошому, щоб ніде не провтичить, варто було б десь тут захист поставити. Сподіваюся найближчим часом я упорюсь і зроблю це. Інакше чекай новий фейлів
                    db.Update("ROOM_TABLE", roomValues, "_id = ?", new string[] { roomId.ToString() });

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