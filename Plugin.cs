using System.Diagnostics;
using BepInEx;
using BepInEx.Configuration;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace AreYouBored
{
	[BepInPlugin("ngbatz.areyoubored", "AreYouBored", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		bool init;
		public static ConfigEntry<bool> AllGames;
		public static ConfigEntry<string> SteamInstallPath;
		public static ConfigEntry<string> AllowedAppIds;
		public static System.Collections.Generic.List<string> appid = new System.Collections.Generic.List<string>();
		public static float timer;
		public static ControllerInputPoller ip = ControllerInputPoller.instance;
		public static bool ahh = ip.leftControllerPrimaryButton && ip.rightControllerPrimaryButton &&  ip.leftControllerSecondaryButton && ip.rightControllerSecondaryButton && ip.leftControllerGripFloat > 0.5 &&  ip.rightControllerGripFloat > 0.5f && ip.rightControllerIndexFloat > 0.5 && ip.leftControllerIndexFloat > 0.5;

        void Start()
        {
	        SteamInstallPath = Config.Bind("General",    
		        "SteamInstallationPath", 
		        "C:\\Program Files (x86)\\Steam",
		        "This is where steam is installed. I don't know if it can be installed elsewhere but in case it can i added this config entry and i also added it for different drives. If you want to use games in a different drive use \"D:\\FunGames\\steamapps\" where \"steamapps\" is where you would usually find your games in the common folder inside that.");
	        
	        
	        AllGames = Config.Bind("General", 
		        "AllGames",
		        true,
		        "Whether or not to use all games in your steam library that are installed where the steam location is. Currently does nothing as im lazy next update it will work");

	        AllowedAppIds = Config.Bind("General",
		        "AllowedAppIds",
		        "480,1533390",
				"What Steam App ids are allowed for the game to run when AllGames is false for example 1533390 is Gorilla Tag's. Make sure they are comma seperated. Currently does nothing as im lazy next update it will work");
			
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
		}

		async void OnGameInitialized()
		{
			if (Directory.Exists(SteamInstallPath.Value))
			{
				if (Directory.Exists(Path.Combine(SteamInstallPath.Value, "steamapps")))
				{
					await GetAllGames(Path.Combine(SteamInstallPath.Value, "steamapps"));
				} else {
					await GetAllGames(SteamInstallPath.Value);
				}
				init = true;
			}
			else
			{
				Logger.Log("Steam path isn't correct/Steam isn't installed!");
				init = false;
			}
		}
		private Task GetAllGames(string steampath)
		{
			string[] a = Directory.GetFiles(steampath, "appmanifest_*.acf");
			foreach (var e in a)
			{
				var cleanedString = Path.GetFileNameWithoutExtension(e).Replace("appmanifest_", "");
				appid.Add(cleanedString);
			}
			return Task.CompletedTask;
		}

		void Update()
		{
			if (ahh && init)
			{
				timer += Time.deltaTime;
				if (timer >= 8f)
				{
					LaunchGame();
				}
			}
			else
			{
				timer = 0f;
			}
		}


		void LaunchGame()
		{
			var rangamid = appid[new Random().Next(appid.Count)];
			if (rangamid == null) return;
			Process.Start(new ProcessStartInfo
			{
				FileName = $"steam://rungameid/{rangamid}",
				UseShellExecute = true,
			});
			Application.Quit();
		}
	}
}