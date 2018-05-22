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
    class CreateCleaningTaskAsyncTask : AsyncTask<int, int, bool>
    {
        Activity myActivity;

        private string taskTitle;
        private int roomId;
        private int year;
        private int month; // From 0 to 11
        private int dayOfMonth;
        private int dateDefault;
        private int hour;
        private int minute;
        private int periodicity;
        private int timeOfCleaning;

        private int yearOfChange;
        private int monthOfChange;
        private int dayOfMonthOfChange;

        public CreateCleaningTaskAsyncTask(Activity activity, string taskTitle, int roomId, int year, int month, int dayOfMonth, int dateDefault, 
            int hour, int minute, int periodicity, int timeOfCleaning)
        {
            myActivity = activity;
            this.taskTitle = taskTitle;
            this.roomId = roomId;
            this.year = year;
            this.month = month;
            this.dayOfMonth = dayOfMonth;
            this.dateDefault = dateDefault;
            this.hour = hour;
            this.minute = minute;
            this.periodicity = periodicity;
            this.timeOfCleaning = timeOfCleaning;

            //При створенні таску необхідно допоміжним параметрам YEAR_OF_CHANGE, MONTH_OF_CHANGE, DAY_OF_MONTH_OF_CHANGE
            //присвоїти значення вчорашнього дня відносно дати створення таску (або не вчорашнього. головне, 
            //щоб ця дата знаходилася в минулому). Це допоможе зручніше працювати в TasksForToday (активності)
            if (DateTime.Now.Day > 1)
            {
                yearOfChange = DateTime.Now.Year;
                monthOfChange = DateTime.Now.Month - 1; //пам'ятаємо, що в андроїді нумерація місяців від 0 до 11, коли в стандартному функціоналі С# - від 1 до 12.
                dayOfMonthOfChange = DateTime.Now.Day - 1; //Робимо дату меншою
            }
            else
            {
                if (DateTime.Now.Month > 1)
                {
                    yearOfChange = DateTime.Now.Year;
                    monthOfChange = DateTime.Now.Month - 2;
                    dayOfMonthOfChange = DateTime.Now.Day; //Неважливо, що тут за день. Все одно дата вийде минулою по відношенню до сьогоднішьої
                }
                else
                {
                    yearOfChange = DateTime.Now.Year - 1; //Інші параметри окрім року й не важливі. Адже дата й так знаходитимиться в минулому по відношенню до сьогоднішньої дати
                    monthOfChange = DateTime.Now.Month - 1;
                    dayOfMonthOfChange = DateTime.Now.Day;
                }
            }
        }

        //Сюди будемо передавати одиницю, якщо хочемо здійснити запис в базу даних
        protected override bool RunInBackground(params int[] @params)
        {
            int param = @params[0];

            if (param == 1)
            {
                SQLiteOpenHelper createCleaningTaskDatabaseHelper = new RationalCleaningDatabaseHelper(myActivity);

                try
                {
                    SQLiteDatabase db = createCleaningTaskDatabaseHelper.WritableDatabase;

                    ContentValues taskValues = new ContentValues();
                    taskValues.Put("TITLE", taskTitle);
                    taskValues.Put("ROOM_ID", roomId);
                    taskValues.Put("YEAR", year);
                    taskValues.Put("MONTH", month);
                    taskValues.Put("DAY_OF_MONTH", dayOfMonth);
                    taskValues.Put("DATE_DEFAULT", dateDefault);
                    taskValues.Put("HOUR", hour);
                    taskValues.Put("MINUTE", minute);
                    taskValues.Put("PERIODICITY", periodicity);
                    taskValues.Put("TIME_OF_CLEANING", timeOfCleaning);
                    taskValues.Put("CLEANNESS", 1);                   
                    taskValues.Put("YEAR_OF_CHANGE", yearOfChange);
                    taskValues.Put("MONTH_OF_CHANGE", monthOfChange);
                    taskValues.Put("DAY_OF_MONTH_OF_CHANGE", dayOfMonthOfChange);


                    db.Insert("CLEANING_TASK_TABLE", null, taskValues);

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