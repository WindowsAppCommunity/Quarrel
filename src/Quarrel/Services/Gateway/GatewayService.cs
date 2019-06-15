using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using DiscordAPI.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Gateway.DownstreamEvents;
using Quarrel.Messages.Gateway;
using DiscordAPI.Models;
using DiscordAPI.API;
using DiscordAPI.API.Gateway;
using DiscordAPI.Authentication;

namespace Quarrel.Services.Gateway
{
    public class GatewayService : IGatewayService
    {
        public DiscordAPI.Gateway.Gateway Gateway { get; private set; }

        public async void InitializeGateway([NotNull] string accessToken)
        {
            BasicRestFactory restFactory = new BasicRestFactory();
            IGatewayConfigService gatewayService = restFactory.GetGatewayConfigService();

            GatewayConfig gatewayConfig = await gatewayService.GetGatewayConfig();
            IAuthenticator authenticator = new DiscordAuthenticator(accessToken);

            Gateway = new DiscordAPI.Gateway.Gateway(gatewayConfig, authenticator);

            Gateway.Ready += Gateway_Ready;
        }

        #region Events

        private void Gateway_Ready(object sender, GatewayEventArgs<Ready> e)
        {
            Messenger.Default.Send(new GatewayReadyMessage(e.EventData));
        }

        #endregion
    }
}
