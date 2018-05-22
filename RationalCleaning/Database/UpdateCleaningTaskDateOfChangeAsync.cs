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
    public class UpdateCleaningTaskDateOfChangeAsync : AsyncTask<int, int, bool>
    {
        Activity myActivity;

        private int taskId;

        //private int year;
        //private int month; // From 0 to 11
        //private int dayOfMonth;

        private int yearOfChange;
        private int monthOfChange; // From 0 to 11
        private int dayOfMonthOfChange;

        public UpdateCleaningTaskDateOfChangeAsync(Activity activity, int taskId)
        {
            myActivity = activity;

            this.taskId = taskId;

            DateTime dateTime = DateTime.Now;

            //year = dateTime.Year;
            //month = dateTime.Month - 1;
            //dayOfMonth = dateTime.Day;

            yearOfChange = dateTime.Year;
            monthOfChange = dateTime.Month - 1;
            dayOfMonthOfChange = dateTime.Day;

        }

        //Сюди будемо передавати одиницю, якщо хочемо здійснити запис в базу даних
        protected override bool RunInBackground(params int[] @params)
        {
            int param = @params[0];

            if (param == 1)
            {
                SQLiteOpenHelper updateCleaningTaskDatabaseHelper = new RationalCleaningDatabaseHelper(myActivity);

                try
                {
                    SQLiteDatabase db = updateCleaningTaskDatabaseHelper.WritableDatabase;

                    ContentValues taskValues = new ContentValues();
                    //taskValues.Put("YEAR", year);
                    //taskValues.Put("MONTH", month);
                    //taskValues.Put("DAY_OF_MONTH", dayOfMonth);
                    taskValues.Put("YEAR_OF_CHANGE", yearOfChange);
                    taskValues.Put("MONTH_OF_CHANGE", monthOfChange);
                    taskValues.Put("DAY_OF_MONTH_OF_CHANGE", dayOfMonthOfChange);


                    db.Update("CLEANING_TASK_TABLE", taskValues, "_id = ?", new string[] { taskId.ToString() });

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