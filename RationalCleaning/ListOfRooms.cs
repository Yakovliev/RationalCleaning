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

namespace RationalCleaning
{
    [Activity(Label = "@string/list_of_rooms_text", Theme = "@style/AppTheme")]
    public class ListOfRooms : AppCompatActivity
    {
        #region Висувний список та Action Bar
        private SupportToolbar mToolBar;
        private SupportActionBar mActionBar;
        private DrawerLayout mDrawerLayout;
        private NavigationView mNavigationView;
        #endregion

        private int currentPosition = 2;     // За допомогою цього поля ми виділяємо пункт у висувному списку, який відповідає даній активності

        //Назви кімнат
        private string[] roomTitles;

        //Час прибирання кімнати
        private string[] timeOfRoomCleaning;
        private int[] timeOfRoomCleaningInteger;

        //Відсоток виконання прибирання кімнати
        private string[] roomCleanness;
        private int[] roomCleannessInteger;

        Android.Support.V7.Widget.RecyclerView listOfRoomsRecycler;


        private SQLiteDatabase db;
        private ICursor roomCursor;
        private ICursor taskCursor;

        private int[] roomIds;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.list_of_rooms);

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
            //fab.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.LightCyan);

            fab.Click += Fab_Click;

            try
            {
                RoomDatabaseManagement roomDatabaseManagement = new RoomDatabaseManagement();
                roomDatabaseManagement.FindRightOrderOfRoomTitles(this);
                roomIds = roomDatabaseManagement.GetRoomIds();
                roomTitles = roomDatabaseManagement.GetRoomTitles();

                timeOfRoomCleaning = new string[roomTitles.Length];
                roomCleanness = new string[roomTitles.Length];

                RoomDatabaseManagement roomDatabaseManagement2 = new RoomDatabaseManagement();
                roomDatabaseManagement2.CalculateRoomCleannessAndTimeOfRoomCleaning(this);
                timeOfRoomCleaningInteger = roomDatabaseManagement2.GetTimeOfRoomCleaning();
                roomCleannessInteger = roomDatabaseManagement2.GetRoomCleanness();

                for (int i = 0; i < timeOfRoomCleaning.Length; i++)
                {
                    timeOfRoomCleaning[i] = timeOfRoomCleaningInteger[i].ToString() + " " + GetString(Resource.String.minute_text);
                    roomCleanness[i] = roomCleannessInteger[i].ToString() + "%";
                }


                listOfRoomsRecycler = FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.list_of_rooms_recycler);

                RoomInListAdapter adapter = new RoomInListAdapter(roomTitles, timeOfRoomCleaning, roomCleanness);
                adapter.ItemClick += OnItemClick;
                listOfRoomsRecycler.SetAdapter(adapter);
                LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
                listOfRoomsRecycler.SetLayoutManager(linearLayoutManager);
                listOfRoomsRecycler.NestedScrollingEnabled = false;

            }
            catch (Exception)
            {

            }
            
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(CreateRoom));
        }

        private void OnItemClick(object sender, int position)
        {
            IntentManagement.IntentManagement.RoomManagementIntent(this, roomIds[position]);
        }

        #region Висувний список та Action Bar


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
                        mDrawerLayout.CloseDrawers();
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

        //Задаємо Action Bar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            return true;
        }

        //Реагуємо на кліки по Action Bar та висувного списку
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);

                    mNavigationView.Menu.FindItem(Resource.Id.nav_list_of_rooms).SetCheckable(true);
                    mNavigationView.Menu.FindItem(Resource.Id.nav_list_of_rooms).SetChecked(true);

                    return true;
                case Resource.Id.create_room:
                    Intent intent = new Intent(this, typeof(CreateRoom));
                    StartActivity(intent);
                    return true;
                case Resource.Id.create_cleaning_task:
                    IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, -1, -1);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }

        #endregion

    }
}