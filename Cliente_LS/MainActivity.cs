using Android.App;
using Android.Widget;
using System.Json;
using Android.OS;
using Android.Webkit;
using Xamarin.Auth;
using System;
using System.Threading.Tasks;

namespace Cliente_LS
{
    [Activity(Label = "Cliente_LS", MainLauncher = true)]
    public class MainActivity : Activity
    {
        #region Metodo Login Facebook
        void LoginToFacebook(bool allowCancel)
        {
            var auth = new OAuth2Authenticator(
                clientId: "1782644685315915",
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            auth.AllowCancel = allowCancel;

            // If authorization succeeds or is canceled, .Completed will be fired.
            auth.Completed += (s, ee) => {
                if (!ee.IsAuthenticated)
                {
                    var builder = new AlertDialog.Builder(this);
                    builder.SetMessage("Not Authenticated");
                    builder.SetPositiveButton("Ok", (o, e) => { });
                    builder.Create().Show();
                    return;
                }

                //Now that we're logged in, make a OAuth2 request to get the user's info.
                var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, ee.Account);
                request.GetResponseAsync().ContinueWith(t =>
                {
                    var builder = new AlertDialog.Builder(this);
                    if (t.IsFaulted)
                    {
                        builder.SetTitle("Error");
                        builder.SetMessage(t.Exception.Flatten().InnerException.ToString());
                    }
                    else if (t.IsCanceled)
                    {
                        builder.SetTitle("Task Canceled");
                    }
                    else
                    {
                        var obj = JsonValue.Parse(t.Result.GetResponseText());
                        builder.SetTitle("Logged in");
                        builder.SetMessage("Name: " + obj["name"]);
                        SetContentView(Resource.Layout.Principal);
                        try
                        {
                            FindViewById<TextView>(Resource.Id.txt_nombre).Text = obj["name"];
                        }
                        catch (Exception ex)
                        {
                            builder.SetTitle("Error");
                            builder.SetMessage("Error: " + ex.Message);
                        }
                    }

                    builder.SetPositiveButton("Ok", (o, e) => { });
                    builder.Create().Show();
                }, UIScheduler);
            };

            var intent = auth.GetUI(this);
            StartActivity(intent);
        }
        #endregion 

        private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            var facebook = FindViewById<Button>(Resource.Id.FacebookButtonNoCancel);
            facebook.Click += delegate { LoginToFacebook(false); };
            

        }

    }
}

