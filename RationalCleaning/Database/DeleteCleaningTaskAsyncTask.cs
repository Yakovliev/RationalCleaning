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
    public class DeleteCleaningTaskAsyncTask : AsyncTask<int, int, bool>
    {
        Activity myActivity;

        private int taskId;

        public DeleteCleaningTaskAsyncTask(Activity activity, int taskId)
        {
            myActivity = activity;
            this.taskId = taskId;
        }

        //Сюди будемо передавати одиницю, якщо хочемо здійснити запис в базу даних
        protected override bool RunInBackground(params int[] @params)
        {
            int param = @params[0];

            if (param == 1)
            {
                SQLiteOpenHelper deleteCleaningTaskDatabaseHelper = new RationalCleaningDatabaseHelper(myActivity);

                try
                {
                    SQLiteDatabase db = deleteCleaningTaskDatabaseHelper.WritableDatabase;

                    db.Delete("CLEANING_TASK_TABLE", "_id = ?", new string[] { taskId.ToString() });

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