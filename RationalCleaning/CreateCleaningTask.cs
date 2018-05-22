using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Content.Res;
using Java.Util;
using Android.Database;
using Android.Database.Sqlite;
using RationalCleaning.ActionBar;
using RationalCleaning.RecyclerView;
using RationalCleaning.Dialog;
using RationalCleaning.Database;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.Design.Widget;
using Android.Text.Format;
using RationalCleaning.IntentManagement;
using RationalCleaning.DatabaseManagement;

namespace RationalCleaning
{
    [Activity(Label = "@string/new_task_text", Theme = "@style/AppTheme")]
    public class CreateCleaningTask : AppCompatActivity
    {
        #region Висувний список та Action Bar
        private SupportToolbar mToolBar;
        private SupportActionBar mActionBar;
        private DrawerLayout mDrawerLayout;
        private NavigationView mNavigationView;
        #endregion

        private int roomId; //Коли ми створюємо задачу переходячи до створення із головної сторінки (наприклад), а не з активності конкретної кімнати, то roomId == -1
        private int taskId; //taskId == -1 означає (якщо це true), що задача ще не створена

        private int[] roomIds;
        private string[] roomTitles;
        //private int[] roomImageIds;

        private int countOfCleaningTask; //Ця змінна необхідна для відслідковування того, скільки cleaning task створено для конкретного розділа-кімнати

        private DateTime dateTime;

        private string taskTitle;

        private int year = -1;
        private int month = -1;
        private int dayOfMonth = -1;
        private int dateDefault = 1;
        private int hour = -1;
        private int minute = -1;
        private int periodicity = 7;
        private int timeOfCleaning = 5;

        //Допоміжні поля, щоб слідкувати за змінами dateDefault
        private int dateDefaultOriginal;

        private string[] captions;
        private int[] imagesId;
        private string[] descriptions;

        private SQLiteDatabase db;
        private ICursor roomCursor;
        private ICursor taskCursor;

        private Android.Support.V7.Widget.RecyclerView taskRecycler;

        private TextDialogFragment createTaskDialogFragment;

        private Calendar dateAndTime = Calendar.GetInstance(Java.Util.TimeZone.Default);
        private TimePickerDialog timePickerDialog;
        private DatePickerDialog datePickerDialog;

        private Spinner roomSpinner;
        private TextView timeTextView;
        private TextView dateTextView;

        private EditText periodicityEditText;

        private EditText timeOfCleaningEditText;

        private Button saveButton;

        private CaptionedImagesAdapterFull adapter;

        //Це поле створюємо для відслідковування того, чи був taskTitle змінений користувачем. Якщо так, то ми так його і залишаємо, коли змінюємо в roomSpinner
        //кімнату. Якщо taskTitle не змінювався, то при перемиканні кімнати в roomSpinner ми змінюємо taskTitle відштовхуючись від кількості уже існуючих завдань
        private bool isTaskTitleChanged = false; //Це поле знадобиться тоді, коли буде йти опрацювання OnSavedInstanceState

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.create_cleaning_task);

            View view1 = FindViewById<View>(Resource.Id.line_view1);
            view1.SetBackgroundColor(new Android.Graphics.Color(224, 224, 224));

            View view2 = FindViewById<View>(Resource.Id.line_view2);
            view2.SetBackgroundColor(new Android.Graphics.Color(224, 224, 224));

            View view3 = FindViewById<View>(Resource.Id.line_view3);
            view3.SetBackgroundColor(new Android.Graphics.Color(224, 224, 224));

            View view4 = FindViewById<View>(Resource.Id.line_view4);
            view4.SetBackgroundColor(new Android.Graphics.Color(224, 224, 224));

            View view5 = FindViewById<View>(Resource.Id.line_view5);
            view5.SetBackgroundColor(new Android.Graphics.Color(224, 224, 224));

            timeTextView = FindViewById<TextView>(Resource.Id.time_textview);
            dateTextView = FindViewById<TextView>(Resource.Id.date_textview);

            timeTextView.Click += TimeTextView_Click;
            dateTextView.Click += DateTextView_Click;

            taskRecycler = FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.task_reсycler);
            roomSpinner = FindViewById<Spinner>(Resource.Id.room_spinner);

            periodicityEditText = FindViewById<EditText>(Resource.Id.periodicity_edit_text);

            periodicityEditText.Text = periodicity.ToString();

            timeOfCleaningEditText = FindViewById<EditText>(Resource.Id.time_of_cleaning_edit_text);

            timeOfCleaningEditText.Text = timeOfCleaning.ToString();


            roomId = Intent.GetIntExtra(IntentManagement.IntentManagement.ROOM_ID, -1);
            taskId = Intent.GetIntExtra(RoomManagement.TASK_ID, -2); //-2 за дефолтом присвоюємо тому, що -1 уже забито під ще не створену задачу. коли вилізе -2 ми відразу побачимо помилку

            captions = new string[1];
            imagesId = new int[1];
            descriptions = new string[1];

            descriptions[0] = GetString(Resource.String.click_to_edit_text);

            RoomDatabaseManagement roomDatabaseManagement = new RoomDatabaseManagement();
            roomDatabaseManagement.FindRightOrderOfRoomTitles(this);
            roomIds = roomDatabaseManagement.GetRoomIds();
            roomTitles = roomDatabaseManagement.GetRoomTitles();
            //roomImageIds = roomDatabaseManagement.GetRoomImageIds();

            roomSpinner.ItemSelected += RoomSpinner_ItemSelected;


            if (taskId == -1) //taskId == -1 означає (якщо це true), що задача ще не створена
            {
                if (roomId == -1) //Коли ми створюємо задачу переходячи до створення із головної сторінки (наприклад), а не з активності конкретної кімнати
                {
                    var spinnerAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, roomTitles);
                    roomSpinner.Adapter = spinnerAdapter;
                    roomId = roomIds[0];
                }
                else
                {
                    var spinnerAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, roomTitles);
                    roomSpinner.Adapter = spinnerAdapter;

                    int spinnerCounter = 0;
                    for (int i = 0; i < roomTitles.Length; i++)
                    {
                        if (roomId == roomIds[i])
                        {
                            spinnerCounter = i;
                            break;
                        }
                    }

                    roomSpinner.SetSelection(spinnerCounter);
                }

                imagesId[0] = DatabaseManagement.RoomDatabaseManagement.GetRoomImageId(this, roomId);

                countOfCleaningTask = DatabaseManagement.CleaningTaskDatabaseManagement.CountOfCleaningTask(this, roomId);

                taskTitle = GetString(Resource.String.task_title) + (countOfCleaningTask + 1).ToString();
                captions[0] = taskTitle;
            }
            else
            {
                var spinnerAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, roomTitles);
                roomSpinner.Adapter = spinnerAdapter;

                int spinnerCounter = 0;
                for (int i = 0; i < roomTitles.Length; i++)
                {
                    if (roomId == roomIds[i])
                    {
                        spinnerCounter = i;
                        break;
                    }
                }

                roomSpinner.SetSelection(spinnerCounter);

                SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(this);
                db = rationalCleaningDatabaseHelper.ReadableDatabase;
                taskCursor = db.Query("CLEANING_TASK_TABLE",
                   new string[] { "TITLE", "YEAR", "MONTH", "DAY_OF_MONTH", "DATE_DEFAULT", "HOUR", "MINUTE", "PERIODICITY", "TIME_OF_CLEANING" },
                   "_id = ?", new string[] { taskId.ToString() }, null, null, null);

                if (taskCursor.MoveToFirst())
                {
                    taskTitle = taskCursor.GetString(0);
                    year = taskCursor.GetInt(1);
                    month = taskCursor.GetInt(2);
                    dayOfMonth = taskCursor.GetInt(3);
                    dateDefault = taskCursor.GetInt(4);
                    hour = taskCursor.GetInt(5);
                    minute = taskCursor.GetInt(6);
                    periodicity = taskCursor.GetInt(7);
                    timeOfCleaning = taskCursor.GetInt(8);
                    dateDefaultOriginal = dateDefault;
                }

                taskCursor.Close();
                db.Close();

                captions[0] = taskTitle;

                if (hour != -1)
                {
                    TimeToFormat(hour, minute);
                }

                if (dateDefault == 0)
                {
                    DateToFormat(year, month, dayOfMonth);
                }

                periodicityEditText.Text = periodicity.ToString();
                timeOfCleaningEditText.Text = timeOfCleaning.ToString();

                imagesId[0] = DatabaseManagement.RoomDatabaseManagement.GetRoomImageId(this, roomId);
            }


            #region Висувний список та Action Bar
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(mToolBar);

            mActionBar = SupportActionBar;
            mActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            mActionBar.SetHomeButtonEnabled(true);
            mActionBar.SetDisplayHomeAsUpEnabled(true);

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mNavigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            if (mNavigationView != null)
            {
                SetUpDrawerContent(mNavigationView);
            }
            #endregion


            UpdateCaptionedImagesAdapter();
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            taskRecycler.SetLayoutManager(linearLayoutManager);
            taskRecycler.NestedScrollingEnabled = false;

            
            saveButton = FindViewById<Button>(Resource.Id.save_button);
            saveButton.Click += SaveButton_Click;
        }

        private void DateTextView_Click(object sender, EventArgs e)
        {
            if (dateDefault == 0)
            {
                datePickerDialog = new DatePickerDialog(this, DateListener, year, month, dayOfMonth);
                datePickerDialog.Show();
            }
            else
            {
                datePickerDialog = new DatePickerDialog(this, DateListener, DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day);
                datePickerDialog.Show();
            }
        }

        private void TimeTextView_Click(object sender, EventArgs e)
        {
            if (hour == -1 && minute == -1)
            {
                timePickerDialog = new TimePickerDialog(this, TimeListener, DateTime.Now.Hour, DateTime.Now.Minute, DateFormat.Is24HourFormat(this));
                timePickerDialog.Show();
            }
            else
            {
                timePickerDialog = new TimePickerDialog(this, TimeListener, hour, minute, DateFormat.Is24HourFormat(this));
                timePickerDialog.Show();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveCleaningTask();
        }

        private void SaveCleaningTask()
        {
            if (taskId == -1)
            {
                periodicity = Convert.ToInt16(periodicityEditText.Text);
                timeOfCleaning = Convert.ToInt16(timeOfCleaningEditText.Text);

                if (dateDefault == 1)
                {
                    year = DateTime.Now.Year;
                    month = DateTime.Now.Month - 1;  //тут номер місяця лежить в межах від 1 до 12. віднімаємо одиницю, бо за базис беремо Java, де номер місяця лежить в межах від 0 до 11
                    dayOfMonth = DateTime.Now.Day;
                }

                new CreateCleaningTaskAsyncTask(this, taskTitle, roomId, year, month, dayOfMonth, dateDefault, hour, minute, periodicity, timeOfCleaning).Execute(1);

                IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(ListOfRooms));

                Toast toast = Toast.MakeText(this, GetString(Resource.String.task_saved_text), ToastLength.Short);
                toast.Show();
            }
            else
            {
                periodicity = Convert.ToInt16(periodicityEditText.Text);
                timeOfCleaning = Convert.ToInt16(timeOfCleaningEditText.Text);

                if (dateDefault == 1)
                {
                    if (dateDefaultOriginal == 1)
                    {
                        //Це означає, що ми не бажаємо змінювати дату, яка задана за замовчуванням і могла вже декілька разів змінюватися функціоналом додатку при виконанні таску.
                        //Асинхронна задача, в яку ми передамо це значення розпізнає, що dateDefault == 1 і year
                        year = -1;
                        month = DateTime.Now.Month - 1;  //тут номер місяця лежить в межах від 1 до 12. віднімаємо одиницю, бо за базис беремо Java, де номер місяця лежить в межах від 0 до 11
                        dayOfMonth = DateTime.Now.Day;
                    }
                    else if (dateDefaultOriginal == 0)
                    {
                        year = DateTime.Now.Year;
                        month = DateTime.Now.Month - 1;  //тут номер місяця лежить в межах від 1 до 12. віднімаємо одиницю, бо за базис беремо Java, де номер місяця лежить в межах від 0 до 11
                        dayOfMonth = DateTime.Now.Day;
                    }

                    
                }

                new UpdateCleaningTaskAsyncTask(this, taskId, taskTitle, roomId, year, month, dayOfMonth, dateDefault, hour, minute, periodicity, timeOfCleaning).Execute(1);

                IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(ListOfRooms));

                Toast toast = Toast.MakeText(this, GetString(Resource.String.task_updated_text), ToastLength.Short);
                toast.Show();
            }
        }

        private void RoomSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            int position = e.Position;
            roomId = roomIds[position];
            imagesId[0] = DatabaseManagement.RoomDatabaseManagement.GetRoomImageId(this, roomId);

            if (taskId == -1 && taskTitle == GetString(Resource.String.task_title) + (countOfCleaningTask + 1).ToString())
            {
                countOfCleaningTask = DatabaseManagement.CleaningTaskDatabaseManagement.CountOfCleaningTask(this, roomId);

                taskTitle = GetString(Resource.String.task_title) + (countOfCleaningTask + 1).ToString();
                captions[0] = taskTitle;
                UpdateCaptionedImagesAdapter();
            }
            else
            {
                UpdateCaptionedImagesAdapter();
            }

        }

        private void DeleteAlertDialogNegativeButtonListener(object sender, DialogClickEventArgs e)
        {
            //Закриваємо вікно alert dialog
        }

        private void DeleteAlertDialogPositiveButtonListener(object sender, DialogClickEventArgs e)
        {
            new DeleteCleaningTaskAsyncTask(this, taskId).Execute(1);

            Intent intent = new Intent(this, typeof(RoomManagement));
            if (roomId != -1)
            {
                intent.PutExtra(IntentManagement.IntentManagement.ROOM_ID, roomId);
            }
            StartActivity(intent);

            Toast toast = Toast.MakeText(this, GetString(Resource.String.task_deleted_text), ToastLength.Short);
            toast.Show();

        }

        private void OnItemClick(object sender, int positon)
        {

            switch (positon)
            {
                case 0:
                    createTaskDialogFragment = new TextDialogFragment(GetString(Resource.String.description_of_task_text), taskTitle, CreateTaskDialogFragment_PositiveButtonIsClicked);
                    createTaskDialogFragment.Show(FragmentManager, "createRoomDialogFragment");
                    break;
                default:
                    break;
            }
        }

        private void DateListener(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            year = e.Year;
            month = e.Month;
            dayOfMonth = e.DayOfMonth;

            dateDefault = 0;

            DateToFormat(year, month, dayOfMonth);
        }

        private void TimeListener(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hour = e.HourOfDay;
            minute = e.Minute;
            TimeToFormat(hour, minute);
        }

        private void CreateTaskDialogFragment_PositiveButtonIsClicked(object sender, EventArgsForTextDialogFragment e)
        {
            if (taskTitle != e.Name)
            {
                isTaskTitleChanged = true;
                taskTitle = e.Name;
                captions[0] = taskTitle;
                UpdateCaptionedImagesAdapter();
            }
        }

        #region Висувний список та Action Bar
        //Задаємо Action Bar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (taskId == -1)
            {
                MenuInflater.Inflate(Resource.Menu.menu_create_cleaning_task, menu);
            }
            else
            {
                MenuInflater.Inflate(Resource.Menu.menu_update_cleaning_task, menu);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        //Реагуємо на кліки по Action Bar та висувного списку
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            

            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);

                    mNavigationView.Menu.FindItem(Resource.Id.nav_create_cleaning_task).SetCheckable(true);
                    mNavigationView.Menu.FindItem(Resource.Id.nav_create_cleaning_task).SetChecked(true);
                    return true;

                case Resource.Id.time_to_default_item:
                    hour = -1;
                    minute = -1;

                    timeTextView.Text = GetString(Resource.String.default_time_text);

                    Toast toast1 = Toast.MakeText(this, GetString(Resource.String.toast_default_time_text), ToastLength.Short);
                    toast1.Show();
                    return true;

                case Resource.Id.date_to_default_item:
                    year = -1;
                    month = -1;
                    dayOfMonth = -1;
                    dateDefault = 1;

                    dateTextView.Text = GetString(Resource.String.default_date_text);

                    Toast toast2 = Toast.MakeText(this, GetString(Resource.String.toast_default_date_text), ToastLength.Short);
                    toast2.Show();
                    return true;
                case Resource.Id.save_item:
                    SaveCleaningTask();
                    return true;
                case Resource.Id.delete_item:
                    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);

                    //builder.SetTitle(Resource.String.delete_alert_dialog_title);
                    builder.SetMessage(Resource.String.delete_alert_dialog_message);
                    builder.SetPositiveButton(Resource.String.positive_button_lined_text_dialog, DeleteAlertDialogPositiveButtonListener);
                    builder.SetNegativeButton(Resource.String.negative_button_lined_text_dialog, DeleteAlertDialogNegativeButtonListener);
                    builder.Create();

                    builder.Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }

        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(MainActivity));
                        break;
                    case Resource.Id.nav_management:
                        IntentManagement.IntentManagement.RoomManagementIntent(this, -1);
                        break;
                    case Resource.Id.nav_list_of_rooms:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(ListOfRooms));
                        break;
                    case Resource.Id.nav_tasks_for_today:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(TasksForToday));
                        break;
                    case Resource.Id.nav_tasks_for_next_day:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(TasksForNextDay));
                        break;
                    case Resource.Id.nav_list_of_tasks:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(FullListOfTasks));
                        break;

                    case Resource.Id.nav_add_room:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(CreateRoom));
                        break;
                    case Resource.Id.create_cleaning_task:
                        mDrawerLayout.CloseDrawers();
                        break;

                    default:
                        mDrawerLayout.CloseDrawers();
                        break;
                }

            };
        }

        #endregion

        private void UpdateCaptionedImagesAdapter()
        {
            adapter = new CaptionedImagesAdapterFull(captions, imagesId, descriptions);
            adapter.ItemClick += OnItemClick;
            taskRecycler.SetAdapter(adapter);
        }

        private void TimeToFormat(int hour, int minute)
        {
            Java.Util.Formatter timeFormatter = new Java.Util.Formatter();
            timeFormatter.Format("%02d:%02d", hour, minute);
            timeTextView.Text = timeFormatter.ToString();
        }

        private void DateToFormat(int year, int month, int dayOfMonth)
        {
            Java.Util.Formatter dateFormatter = new Java.Util.Formatter();
            dateFormatter.Format("%d/%02d/%02d", year, month + 1, dayOfMonth);
            dateTextView.Text = dateFormatter.ToString();
        }
    }
}