﻿using TsinghuaNet.Helper;

namespace TsinghuaNet.CrossPlatform.ViewModels
{
    public class ChangeCredentialViewModel : NetObservableBase
    {
        public ChangeCredentialViewModel()
        {
            Username = Credential.Username;
            Password = Credential.Password;
        }

        private string username;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string password;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public void Confirm()
        {
            Credential.Username = Username;
            Credential.Password = Password;
        }
    }
}