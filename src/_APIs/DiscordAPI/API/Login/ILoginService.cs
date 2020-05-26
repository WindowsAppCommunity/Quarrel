// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Login.Models;
using Refit;
using System.Threading.Tasks;

namespace DiscordAPI.API.Login
{
    /// <summary>
    /// A service for login REST calls.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Logs in via REST.
        /// </summary>
        /// <param name="loginRequest">The login request.</param>
        /// <returns>A login result.</returns>
        [Post("/auth/login")]
        Task<LoginResult> Login([Body] LoginRequest loginRequest);

        /// <summary>
        /// Logs in with MFA via REST.
        /// </summary>
        /// <param name="loginRequest">The login request.</param>
        /// <returns>A login result.</returns>
        [Post("/auth/mfa/totp")]
        Task<LoginResult> LoginMFA([Body] LoginMFARequest loginRequest);

        /// <summary>
        /// Logs in with SMS via REST.
        /// </summary>
        /// <param name="loginRequest">The login request.</param>
        /// <returns>A login result.</returns>
        [Post("/auth/mfa/sms")]
        Task<LoginResult> LoginSMS([Body] LoginMFARequest loginRequest);

        /// <summary>
        /// Send SMS via REST.
        /// </summary>
        /// <param name="loginRequest">Sms request.</param>
        /// <returns>An sms result.</returns>
        [Post("/auth/mfa/sms/send")]
        Task<SmsResult> SendSMS([Body] SmsRequest loginRequest);
    }
}
