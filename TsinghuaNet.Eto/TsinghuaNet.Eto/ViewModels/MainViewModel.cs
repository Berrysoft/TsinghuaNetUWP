﻿using System;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using PropertyChanged;
using TsinghuaNet.Models;
using TsinghuaNet.ViewModels;

namespace TsinghuaNet.Eto.ViewModels
{
    public class MainViewModel : MainViewModelBase
    {
        public MainViewModel() : base()
        {
            Status = new NetPingStatus();
            ReceivedResponse += Model_ReceivedResponse;
            timer = new UITimer(OnlineTimerTick);
            timer.Interval = 1;
            if (Settings.AutoLogin)
                Login();
            else
                Refresh();
        }

        [DoNotNotify]
        public new NetSettings Settings
        {
            get => (NetSettings)base.Settings;
            set => base.Settings = value;
        }

        public override void LoadSettings()
        {
            Settings = SettingsHelper.Helper.ReadSettings<NetSettings>() ?? new NetSettings();
            Credential.Username = Settings.Username ?? string.Empty;
            Credential.Password = Encoding.UTF8.GetString(Convert.FromBase64String(Settings.Password ?? string.Empty));
        }

        public override void SaveSettings()
        {
            if (Settings.DeleteSettingsOnExit)
            {
                SettingsHelper.Helper.DeleteSettings();
            }
            else
            {
                Settings.Username = Credential.Username;
                Settings.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Credential.Password));
                SettingsHelper.Helper.WriteSettings(Settings);
            }
        }

        private readonly UITimer timer;
        private void OnlineTimerTick(object sender, EventArgs e)
        {
            OnlineTime += TimeSpan.FromSeconds(1);
        }

        protected override async Task<LogResponse> RefreshAsync(IConnect helper)
        {
            var res = await base.RefreshAsync(helper);
            timer.Stop();
            OnlineTime = OnlineUser.OnlineTime;
            if (Settings.UseTimer && !string.IsNullOrEmpty(OnlineUser.Username))
                timer.Start();
            if (Settings.EnableFluxLimit && OnlineUser.Flux > Settings.FluxLimit)
                res = new LogResponse(false, $"流量已使用超过{Settings.FluxLimit}");
            return res;
        }

        private void Model_ReceivedResponse(object sender, LogResponse res) => Response = res.Message;
    }
}
