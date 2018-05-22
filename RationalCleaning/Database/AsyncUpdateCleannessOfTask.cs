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
    public class AsyncUpdateCleannessOfTask : AsyncTask<int, int, bool>
    {
        Context context;

        private int taskId;
        private int cleanness;

        private int year = DateTime.Now.Year;
        private int month = DateTime.Now.Month - 1; // From 0 to 11
        private int dayOfMonth = DateTime.Now.Day;
        private int dateDefault = 1;

        private int yearOfChange = DateTime.Now.Year;
        private int monthOfChange = DateTime.Now.Month - 1;
        private int dayOfMonthOfChange = DateTime.Now.Day;

        public AsyncUpdateCleannessOfTask(Context context, int taskId, int cleanness)
        {
            this.context = context;

            this.taskId = taskId;
            this.cleanness = cleanness;
          
        }

        //Сюди будемо передавати одиницю, якщо хочемо здійснити запис в базу даних
        protected override bool RunInBackground(params int[] @params)
        {
            int param = @params[0];

            if (param == 1)
            {
                SQLiteOpenHelper updateCleannessOfTask = new RationalCleaningDatabaseHelper(context);

                try
                {
                    SQLiteDatabase db = updateCleannessOfTask.WritableDatabase;

                    ContentValues taskValues = new ContentValues();

                    if (cleanness == 0 || cleanness == 1)
                    {
                        taskValues.Put("CLEANNESS", cleanness);
                        taskValues.Put("YEAR", year);
                        taskValues.Put("MONTH", month);
                        taskValues.Put("DAY_OF_MONTH", dayOfMonth);
                        taskValues.Put("DATE_DEFAULT", dateDefault);
                        taskValues.Put("YEAR_OF_CHANGE", yearOfChange);
                        taskValues.Put("MONTH_OF_CHANGE", monthOfChange);
                        taskValues.Put("DAY_OF_MONTH_OF_CHANGE", dayOfMonthOfChange);
                    }

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
                Toast toast = Toast.MakeText(context, "Database unavailable", ToastLength.Short);
                toast.Show();
            }
        }
    }
}