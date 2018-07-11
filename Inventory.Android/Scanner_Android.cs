using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace Inventory.Droid
{
    public class Scanner_Android : IScanner
    {
        private Context _context = null;
        private bool _bRegistered = false;
        private DataWedgeReceiver _broadcastReceiver = null;
        private static string ACTION_DATAWEDGE_FROM_6_2 = "com.symbol.datawedge.api.ACTION";
        private static string EXTRA_CREATE_PROFILE = "com.symbol.datawedge.api.CREATE_PROFILE";
        private static string EXTRA_SET_CONFIG = "com.symbol.datawedge.api.SET_CONFIG";
        private static string EXTRA_PROFILE_NAME = "Inventory DEMO";

        public Scanner_Android()
        {
            _context = Application.Context;

            _broadcastReceiver = new DataWedgeReceiver();

            _broadcastReceiver.scanDataReceived += (s, scanData) =>
            {
                OnScanDataCollected?.Invoke(this, scanData);
            };

            CreateProfile();
        }

        public event EventHandler<StatusEventArgs> OnScanDataCollected;
        public event EventHandler<string> OnStatusChanged;


        public void Disable()
        {
            if ((null != _broadcastReceiver) && (null != _context) && _bRegistered)
            {
                // Unregister the broadcast receiver
                _context.UnregisterReceiver(_broadcastReceiver);
                _bRegistered = false;
            }

            DisableProfile();
        }

        public void Enable()
        {
            _context = Application.Context;

            if ((null != _broadcastReceiver) && (null != _context))
            {
                // Register the broadcast receiver
                IntentFilter filter = new IntentFilter(DataWedgeReceiver.IntentAction);
                filter.AddCategory(DataWedgeReceiver.IntentCategory);
                _context.RegisterReceiver(_broadcastReceiver, filter);
                _bRegistered = true;
            }

            EnableProfile();
        }

        public void Read()
        {
            // We can use this to activate a Soft triggered barcode scanning decoding
            throw new NotImplementedException();
        }

        public void SetConfig(IScannerConfig a_config)
        {

            ZebraScannerConfig config = (ZebraScannerConfig)a_config;

            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", EXTRA_PROFILE_NAME);
            profileConfig.PutString("PROFILE_ENABLED", _bRegistered ? "true" : "false"); //  Seems these are all strings
            profileConfig.PutString("CONFIG_MODE", "UPDATE");
            Bundle barcodeConfig = new Bundle();
            barcodeConfig.PutString("PLUGIN_NAME", "BARCODE");
            barcodeConfig.PutString("RESET_CONFIG", "false"); //  This is the default but never hurts to specify
            Bundle barcodeProps = new Bundle();
            barcodeProps.PutString("scanner_input_enabled", "true");
            barcodeProps.PutString("scanner_selection", "auto"); //  Could also specify a number here, the id returned from ENUMERATE_SCANNERS.
                                                                 //  Do NOT use "Auto" here (with a capital 'A'), it must be lower case.
            barcodeProps.PutString("decoder_ean8", config.IsEAN8 ? "true" : "false");
            barcodeProps.PutString("decoder_ean13", config.IsEAN13 ? "true" : "false");
            barcodeProps.PutString("decoder_code39", config.IsCode39 ? "true" : "false");
            barcodeProps.PutString("decoder_code128", config.IsCode128 ? "true" : "false");
            barcodeProps.PutString("decoder_upca", config.IsUPCA ? "true" : "false");
            barcodeProps.PutString("decoder_upce0", config.IsUPCE0 ? "true" : "false");
            barcodeProps.PutString("decoder_upce1", config.IsUPCE1 ? "true" : "false");
            barcodeProps.PutString("decoder_d2of5", config.IsD2of5 ? "true" : "false");
            barcodeProps.PutString("decoder_i2of5", config.IsI2of5 ? "true" : "false");
            barcodeProps.PutString("decoder_aztec", config.IsAztec ? "true" : "false");
            barcodeProps.PutString("decoder_pdf417", config.IsPDF417 ? "true" : "false");
            barcodeProps.PutString("decoder_qrcode", config.IsQRCode ? "true" : "false");

            barcodeConfig.PutBundle("PARAM_LIST", barcodeProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", barcodeConfig);
            Bundle appConfig = new Bundle();
            appConfig.PutString("PACKAGE_NAME", Android.App.Application.Context.PackageName);      //  Associate the profile with this app
            appConfig.PutStringArray("ACTIVITY_LIST", new String[] { "*" });
            profileConfig.PutParcelableArray("APP_LIST", new Bundle[] { appConfig });
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);

        }

        private void EnableProfile()
        {
            //  Now configure that created profile to apply to our application
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", EXTRA_PROFILE_NAME);
            profileConfig.PutString("PROFILE_ENABLED", "true"); //  Seems these are all strings
            profileConfig.PutString("CONFIG_MODE", "UPDATE");
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);
        }

        private void DisableProfile()
        {
            //  Now configure that created profile to apply to our application
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", EXTRA_PROFILE_NAME);
            profileConfig.PutString("PROFILE_ENABLED", "false"); //  Seems these are all strings
            profileConfig.PutString("CONFIG_MODE", "UPDATE");
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);
        }

        private void CreateProfile()
        {
            String profileName = EXTRA_PROFILE_NAME;
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_CREATE_PROFILE, profileName);

            //  Now configure that created profile to apply to our application
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", EXTRA_PROFILE_NAME);
            profileConfig.PutString("PROFILE_ENABLED", "true"); //  Seems these are all strings
            profileConfig.PutString("CONFIG_MODE", "UPDATE");
            Bundle barcodeConfig = new Bundle();
            barcodeConfig.PutString("PLUGIN_NAME", "BARCODE");
            barcodeConfig.PutString("RESET_CONFIG", "true"); //  This is the default but never hurts to specify
            Bundle barcodeProps = new Bundle();
            barcodeConfig.PutBundle("PARAM_LIST", barcodeProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", barcodeConfig);
            Bundle appConfig = new Bundle();
            appConfig.PutString("PACKAGE_NAME", Android.App.Application.Context.PackageName);      //  Associate the profile with this app
            appConfig.PutStringArray("ACTIVITY_LIST", new String[] { "*" });
            profileConfig.PutParcelableArray("APP_LIST", new Bundle[] { appConfig });
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);
            //  You can only configure one plugin at a time, we have done the barcode input, now do the intent output
            profileConfig.Remove("PLUGIN_CONFIG");
            Bundle intentConfig = new Bundle();
            intentConfig.PutString("PLUGIN_NAME", "INTENT");
            intentConfig.PutString("RESET_CONFIG", "true");
            Bundle intentProps = new Bundle();
            intentProps.PutString("intent_output_enabled", "true");
            intentProps.PutString("intent_action", DataWedgeReceiver.IntentAction);
            intentProps.PutString("intent_delivery", "2");
            intentConfig.PutBundle("PARAM_LIST", intentProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", intentConfig);
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);
        }

        private void SendDataWedgeIntentWithExtra(String action, String extraKey, Bundle extras)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extras);
            _context.SendBroadcast(dwIntent);
        }

        private void SendDataWedgeIntentWithExtra(String action, String extraKey, String extraValue)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extraValue);
            _context.SendBroadcast(dwIntent);
        }
    }
}
