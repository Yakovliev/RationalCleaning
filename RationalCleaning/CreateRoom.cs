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


namespace RationalCleaning
{
    [Activity(Label = "@string/create_room_text", Theme = "@style/AppTheme")]
    public class CreateRoom : AppCompatActivity
    {
        #region Висувний список та Action Bar
        private SupportToolbar mToolBar;
        private SupportActionBar mActionBar;
        private DrawerLayout mDrawerLayout;
        private NavigationView mNavigationView;
        #endregion

        Android.Support.V7.Widget.RecyclerView createRoomRecycler;

        private string[] captions; //Заголовки
        private int[] imagesId; //id рисунків
        private string[] actionText;

        TextDialogFragment createRoomDialogFragment;

        private string roomTitle;

        int roomImageCounter = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.create_room);

            roomTitle = GetString(Resource.String.room_title);
            roomTitle += (DatabaseManagement.RoomDatabaseManagement.CountOfRooms(this) + 1).ToString();

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

            
            createRoomRecycler = FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.create_room_recycler);


            captions = new string[1];
            captions[0] = roomTitle;
            imagesId = new int[1];
            imagesId[0] = Resource.Drawable.roomImage1;
            actionText = new string[2];
            actionText[0] = GetString(Resource.String.next_image_text);
            actionText[1] = GetString(Resource.String.save_text);

            SetAdapterForRoomRecycler();
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            createRoomRecycler.SetLayoutManager(linearLayoutManager);
            
        }

        private void Adapter_Action1_Click(object sender, EventArgs e)
        {
            roomImageCounter++;

            imagesId[0] = RoomImage.RoomImageClass.GetRoomImageResource(roomImageCounter);

            CaptionedImagesWithTwoActionsAdapter adapter = new CaptionedImagesWithTwoActionsAdapter(this, captions, imagesId, actionText);
            adapter.ItemClick += OnItemClick;

            adapter.Action1_Click += Adapter_Action1_Click;
            adapter.Action2_Click += Adapter_Action2_Click;

            createRoomRecycler.SetAdapter(adapter);


        }

        private void Adapter_Action2_Click(object sender, EventArgs e)
        {
            new CreateRoomAsyncTask(this, roomTitle, imagesId[0]).Execute(1);
            IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(ListOfRooms));

            Toast toast = Toast.MakeText(this, GetString(Resource.String.room_saved_text), ToastLength.Short);
            toast.Show();
        }

        private void OnItemClick(object sender, int positon)
        {

            switch (positon)
            {
                case 0:
                    createRoomDialogFragment = new TextDialogFragment(GetString(Resource.String.name_of_room_text) ,roomTitle, CreateRoomDialogFragment_PositiveButtonIsClicked);
                    createRoomDialogFragment.Show(FragmentManager, "createRoomDialogFragment");
                    
                    break;
                default:
                    break;
            }
        }

        private void CreateRoomDialogFragment_PositiveButtonIsClicked(object sender, EventArgsForTextDialogFragment e)
        {
            roomTitle = e.Name;
            captions[0] = roomTitle;

            SetAdapterForRoomRecycler();
        }

        private void SetAdapterForRoomRecycler()
        {
            CaptionedImagesWithTwoActionsAdapter adapter = new CaptionedImagesWithTwoActionsAdapter(this, captions, imagesId, actionText);
            adapter.ItemClick += OnItemClick;

            adapter.Action1_Click += Adapter_Action1_Click;
            adapter.Action2_Click += Adapter_Action2_Click;

            createRoomRecycler.SetAdapter(adapter);
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
                        mDrawerLayout.CloseDrawers();
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
            MenuInflater.Inflate(Resource.Menu.menu_create, menu);

            return true;
        }

        //Реагуємо на кліки по Action Bar та висувного списку
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);

                    mNavigationView.Menu.FindItem(Resource.Id.nav_add_room).SetCheckable(true);
                    mNavigationView.Menu.FindItem(Resource.Id.nav_add_room).SetChecked(true);

                    return true;
                case Resource.Id.save_item:
                    new CreateRoomAsyncTask(this, roomTitle, imagesId[0]).Execute(1);
                    IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(ListOfRooms));

                    Toast toast = Toast.MakeText(this, GetString(Resource.String.room_saved_text), ToastLength.Short);
                    toast.Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }


        #endregion

    }
}