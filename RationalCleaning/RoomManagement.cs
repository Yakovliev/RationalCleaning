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
using RationalCleaning.DatabaseManagement;
using RationalCleaning.RoomImage;

namespace RationalCleaning
{
    [Activity(Label = "@string/room_management", Theme = "@style/AppTheme")]
    public class RoomManagement : AppCompatActivity
    {
        #region Висувний список та Action Bar
        private SupportToolbar mToolBar;
        private SupportActionBar mActionBar;
        private DrawerLayout mDrawerLayout;
        private NavigationView mNavigationView;
        #endregion

        private int currentPosition = 1;     // За допомогою цього поля ми виділяємо пункт у висувному списку, який відповідає даній активності

        Android.Support.V7.Widget.RecyclerView listOfTasksRecycler;
        Android.Support.V7.Widget.RecyclerView roomRecycler;

        //private string roomTitle;
        private int roomId;

        private int[] taskIds;
        private int[] tasksTimeOfCleaning;
        private string[] tasksTimeOfCleaningString;
        private int[] tasksCleanness;
        private string[] taskTitles;

        private string[] timeOfRoomCleaning;
        private int[] timeOfRoomCleaningInteger;
        private string[] roomCleanness;
        private int[] roomCleannessInteger;

        public const string TASK_ID = "taskId";

        TextDialogFragment createRoomDialogFragment;

        private int[] roomIds;
        private string[] roomTitles;
        private int[] roomImageIds;
        private int[] isRoom; //Значення цієї штуки використовується для відображення потрібного меню


        private int[] spinnerPosition; //Визначаємо позицію кімнати у впорядкованому списку. Формат масиву, бо потім це будемо передавати в адаптер RecyclerView
        private string[] actionText;
        private bool[] isSecondActionButtonIsVisible;

        private IMenuItem menuItem;

        private int roomImageCounter = -1;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.room_management);

            actionText = new string[2];
            actionText[0] = GetString(Resource.String.next_image_text);
            actionText[1] = GetString(Resource.String.save_text);

            roomId = Intent.GetIntExtra(IntentManagement.IntentManagement.ROOM_ID, -1);

            RoomDatabaseManagement roomDatabaseManagement = new RoomDatabaseManagement();
            roomDatabaseManagement.CalculateRoomCleannessAndTimeOfRoomCleaning(this);
            roomIds = roomDatabaseManagement.GetRoomIds();
            roomTitles = roomDatabaseManagement.GetRoomTitles();
            timeOfRoomCleaningInteger = roomDatabaseManagement.GetTimeOfRoomCleaning();
            roomCleannessInteger = roomDatabaseManagement.GetRoomCleanness();
            roomImageIds = roomDatabaseManagement.GetRoomImageIds();
            isRoom = roomDatabaseManagement.GetIsRoom();
            
            timeOfRoomCleaning = new string[timeOfRoomCleaningInteger.Length];
            roomCleanness = new string[roomCleannessInteger.Length];

            spinnerPosition = new int[1];
            spinnerPosition[0] = 0;

            isSecondActionButtonIsVisible = new bool[1];
            isSecondActionButtonIsVisible[0] = false;

            if (roomId == -1)
            {
                roomId = roomIds[0];
            }

            for (int i = 0; i < roomIds.Length; i++)
            {
                if (roomId == roomIds[i])
                {
                    break;
                }
                else
                {
                    spinnerPosition[0]++;
                }
            }

            for (int i = 0; i < roomIds.Length; i++)
            {
                timeOfRoomCleaning[i] = timeOfRoomCleaningInteger[i].ToString() + " " + GetString(Resource.String.minute_text);
                roomCleanness[i] = roomCleannessInteger[i].ToString() + "%";
            }

            roomRecycler = FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.room_recycler);

            SetAdapterForRoomRecycler();
            LinearLayoutManager roomLinearLayoutManager = new LinearLayoutManager(this);
            roomRecycler.SetLayoutManager(roomLinearLayoutManager);

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

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += Fab_Click;

            UpdateTasks();

            listOfTasksRecycler = FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.list_of_tasks_recycler);

            SetAdapterForTaskRecycler();
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            listOfTasksRecycler.SetLayoutManager(linearLayoutManager);
        }

        private void UpdateTasks()
        {
            CleaningTaskDatabaseManagement cleaningTaskDatabaseManagement = new CleaningTaskDatabaseManagement(this);
            cleaningTaskDatabaseManagement.GetRoomTasksFromDatabase(roomIds[spinnerPosition[0]]);
            taskIds = cleaningTaskDatabaseManagement.GetTaskIds();
            tasksTimeOfCleaning = cleaningTaskDatabaseManagement.GetTasksTimeOfCleaning();
            tasksCleanness = cleaningTaskDatabaseManagement.GetTasksCleanness();
            taskTitles = cleaningTaskDatabaseManagement.GetTaskTitles();

            tasksTimeOfCleaningString = new string[tasksTimeOfCleaning.Length];

            for (int i = 0; i < tasksTimeOfCleaningString.Length; i++)
            {
                tasksTimeOfCleaningString[i] = tasksTimeOfCleaning[i].ToString() + " " + GetString(Resource.String.minute_text);
            }
        }

        private void SetAdapterForRoomRecycler()
        {
            CaptionedImagesWithSpinner roomAdapter = new CaptionedImagesWithSpinner(this, spinnerPosition, roomTitles, roomImageIds,
                actionText, isSecondActionButtonIsVisible, timeOfRoomCleaning, roomCleanness);
            roomAdapter.ItemClick += RoomAdapter_ItemClick;
            roomAdapter.SpinnerItemSelectionChanged += RoomAdapter_SpinnerItemSelectionChanged;
            roomAdapter.Action1_Click += RoomAdapter_Action1_Click;
            roomAdapter.Action2_Click += RoomAdapter_Action2_Click;
            roomRecycler.SetAdapter(roomAdapter);
        }

        private void SetAdapterForTaskRecycler()
        {
            TaskInListAdapter adapter = new TaskInListAdapter(taskTitles, tasksTimeOfCleaningString);
            adapter.ItemClick += OnItemClick;
            listOfTasksRecycler.SetAdapter(adapter);
        }

        private void RoomAdapter_Action1_Click(object sender, EventArgs e)
        {
            if (roomImageCounter == -1)
            {
                roomImageCounter = RoomImageClass.GetRoomImageCounter(roomImageIds[spinnerPosition[0]]);
            }

            roomImageCounter++;

            roomImageIds[spinnerPosition[0]] = RoomImageClass.GetRoomImageResource(roomImageCounter);
            isSecondActionButtonIsVisible[0] = true;
            SetAdapterForRoomRecycler();

        }

        private void RoomAdapter_Action2_Click(object sender, EventArgs e)
        {
            roomImageCounter = -1;
            isSecondActionButtonIsVisible[0] = false;

            new UpdateRoomAsyncTask(this, roomIds[spinnerPosition[0]], roomTitles[spinnerPosition[0]], roomImageIds[spinnerPosition[0]]).Execute(1);

            Toast toast = Toast.MakeText(this, GetString(Resource.String.room_updated), ToastLength.Short);
            toast.Show();

            SetAdapterForRoomRecycler();

        }

        private void RoomAdapter_SpinnerItemSelectionChanged(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (spinnerPosition[0] != e.Position)  //Якогось фіга при створенні roomAdapter.SpinnerItemSelectionChanged += RoomAdapter_SpinnerItemSelectionChanged; запускається RoomAdapter_SpinnerItemSelectionChanged, що без даної умови по суті призведе до нескінченного циклу
            {
                spinnerPosition[0] = e.Position;
                isSecondActionButtonIsVisible[0] = false;
                SetAdapterForRoomRecycler();
                roomImageCounter = -1;

                UpdateTasks();
                SetAdapterForTaskRecycler();
            }
        }

        private void RoomAdapter_ItemClick(object sender, int e)
        {
            if (isRoom[spinnerPosition[0]] != 0)
            {
                createRoomDialogFragment = new TextDialogFragment("UpdateRoomTitle", roomTitles[spinnerPosition[0]], CreateRoomDialogFragment_PositiveButtonIsClicked);
                createRoomDialogFragment.Show(FragmentManager, "createRoomDialogFragment");
            }
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, -1, roomIds[spinnerPosition[0]]);
        }

        private void OnItemClick(object sender, int position)
        {
            IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, taskIds[position], roomIds[spinnerPosition[0]]);
        }


        private void CreateRoomDialogFragment_PositiveButtonIsClicked(object sender, EventArgsForTextDialogFragment e)
        {
            roomTitles[spinnerPosition[0]] = e.Name;
            isSecondActionButtonIsVisible[0] = true;
            SetAdapterForRoomRecycler();
        }

        private void DeleteAlertDialogNegativeButtonListener(object sender, DialogClickEventArgs e)
        {
            //Закриваємо вікно alert dialog
        }

        private void DeleteAlertDialogPositiveButtonListener(object sender, DialogClickEventArgs e)
        {
            if (isRoom[spinnerPosition[0]] != 0)
            {
                new DeleteRoomAsyncTask(this, roomIds[spinnerPosition[0]]).Execute(1);

                IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(ListOfRooms));

                Toast toast = Toast.MakeText(this, GetString(Resource.String.room_deleted), ToastLength.Short);
                toast.Show();
            }
       
        }

        #region Висувний список та Action Bar
        //Задаємо Action Bar

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
                        mDrawerLayout.CloseDrawers();
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
                        IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, -1, -1);
                        break;
                    default:
                        mDrawerLayout.CloseDrawers();
                        break;
                }

            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_room_management, menu);

            menuItem = menu.FindItem(Resource.Id.delete_room);

            if (isRoom[spinnerPosition[0]] == 0)
            {
                menuItem.SetVisible(false);
                this.InvalidateOptionsMenu();
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

                    mNavigationView.Menu.FindItem(Resource.Id.nav_management).SetCheckable(true);
                    mNavigationView.Menu.FindItem(Resource.Id.nav_management).SetChecked(true);

                    return true;
                case Resource.Id.create_cleaning_task:
                    IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, -1, roomId);
                    return true;
                case Resource.Id.delete_room:
                    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);

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

        #endregion
    }
}