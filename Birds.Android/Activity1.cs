using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Telephony.Data;
using Android.Views;
using Birds.src;
using Java.Security;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.Json;

namespace Birds.Android
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class Activity1 : AndroidGameActivity
    {
        private Game1 _game;
        private View _view;
        public string KeyStoreFile { get; private set; }
        public string KeyAlias { get; private set; }
        public string KeyPass { get; private set; }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = new Game1();
            _view = _game.Services.GetService(typeof(View)) as View;

            SetContentView(_view);
            _game.Run();
        }

        /*private void LoadAppSettings()
        {
            // The path to the configuration file in the assets folder
            var configFilePath = "secrets.json";
            var assetManager = Assets;

            using (var stream = assetManager.Open(configFilePath))
            using (var reader = new StreamReader(stream))
            {
                var jsonString = reader.ReadToEnd();
                var config = JsonConvert.DeserializeObject<AppSettings>(jsonString);

                // Assign values from the parsed JSON
                KeyStoreFile = config.AndroidKeystore.KeyStoreFile;
                KeyAlias = config.AndroidKeystore.KeyAlias;
                KeyPass = config.AndroidKeystore.KeyPass;
            }
        }
        d*/
    }
}
