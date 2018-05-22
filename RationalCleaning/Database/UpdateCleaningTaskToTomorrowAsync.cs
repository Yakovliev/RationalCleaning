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
    public class UpdateCleaningTaskToTomorrowAsync : AsyncTask<int, int, bool>
    {
        Activity myActivity;

        private int taskId;

        private int year;
        private int month; // From 0 to 11
        private int dayOfMonth;

        private int yearOfChange;
        private int monthOfChange; // From 0 to 11
        private int dayOfMonthOfChange;

        public UpdateCleaningTaskToTomorrowAsync(Activity activity, int taskId)
        {
            myActivity = activity;

            this.taskId = taskId;

            DateTime dateTime = GetTomorrowDate();

            year = dateTime.Year;
            month = dateTime.Month - 1;
            dayOfMonth = dateTime.Day;

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
                    taskValues.Put("YEAR", year);
                    taskValues.Put("MONTH", month);
                    taskValues.Put("DAY_OF_MONTH", dayOfMonth);
                    taskValues.Put("CLEANNESS", 0);
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

        private DateTime GetTomorrowDate()
        {
            DateTime dateTime = DateTime.Now;

            int year = dateTime.Year;
            int month = dateTime.Month - 1;
            int dayOfMonth = dateTime.Day;

            if (month == 0)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if ((year % 4) == 0 && month == 1)
            {
                if (dayOfMonth + 1 <= 29)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 29;
                }
            }
            else if (month == 1)
            {
                if (dayOfMonth + 1 <= 28)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 28;
                }
            }
            else if (month == 2)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 3)
            {
                if (dayOfMonth + 1 <= 30)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 30;
                }
            }
            else if (month == 4)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 5)
            {
                if (dayOfMonth + 1 <= 30)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 30;
                }
            }
            else if (month == 6)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 7)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 8)
            {
                if (dayOfMonth + 1 <= 30)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 30;
                }
            }
            else if (month == 9)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 10)
            {
                if (dayOfMonth + 1 <= 30)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 30;
                }
            }
            else if (month == 11)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month = 0;
                    dayOfMonth = dayOfMonth + 1 - 31;
                    year++;
                }
            }

            dateTime = new DateTime(year, month + 1, dayOfMonth);

            return dateTime;

        }
    }
}