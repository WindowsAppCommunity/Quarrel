using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation.Metadata;
using Windows.Media.SpeechSynthesis;
using Windows.Phone.Devices.Notification;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Quarrel.Controls;
using Quarrel.Managers;
using DiscordAPI.SharedModels;
using Quarrel.SimpleClasses;
using Microsoft.Toolkit.Uwp.UI.Animations;
using DiscordAPI.API.User.Models;
using Quarrel.LocalModels;
using ContactManager = Quarrel.Managers.ContactManager;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactPanePage : Page
    {
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var contactManager = new Managers.ContactManager();
            ContactPanelActivatedEventArgs panelArgs = (ContactPanelActivatedEventArgs)e.Parameter;
            string userID = await contactManager.ContactIdToRemoteId(panelArgs.Contact.Id);
            string DmChannelID = LocalState.DMs
                              ?.FirstOrDefault(dm =>
                                  dm.Value?.Type == 1 && dm.Value.Users.FirstOrDefault()?.Id == userID).Value?.Id ??
                          (await RESTCalls.CreateDM(new CreateDM
                              { Recipients = new List<string> { userID }.AsEnumerable() })).Id;
            MessageBody.MyPeopleChannelId = DmChannelID;
        }
        public ContactPanePage()
        {
            this.InitializeComponent();
        }
    }
}
