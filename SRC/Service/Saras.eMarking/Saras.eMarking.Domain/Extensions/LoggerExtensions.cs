using System;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Saras.eMarking.Domain.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception?> _userLoggedIn =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(6, nameof(UserLoggedIn)),
            "{Loginname} logged in.");

        private static readonly Action<ILogger, string, Exception?> _userSignedUp =
       LoggerMessage.Define<string>(
           LogLevel.Information,
           new EventId(7, nameof(UserSignedUp)),
           "{Loginname} signed up.");

        private static readonly Action<ILogger, string, Exception?> _userChangedPassword =
      LoggerMessage.Define<string>(
          LogLevel.Information,
          new EventId(8, nameof(UserChangedPassword)),
          "{Loginname} changed their password.");

        private static readonly Action<ILogger, string, Exception?> _userForgotPassword =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(9, nameof(UserForgotPassword)),
                "{Loginname} forgot their password.");

        private static readonly Action<ILogger, string, Exception?> _userResetPassword =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(10, nameof(UserResetPassword)),
                "{Loginname} reset their password.");

        public static void UserLoggedIn(this ILogger logger, string loginname)
        => _userLoggedIn(logger, loginname, null);
        public static void UserSignedUp(this ILogger logger, string loginname)
        => _userSignedUp(logger, loginname, null);
        public static void UserChangedPassword(this ILogger logger, string loginname)
            => _userChangedPassword(logger, loginname, null);
        public static void UserForgotPassword(this ILogger logger, string loginname)
            => _userForgotPassword(logger, loginname, null);
        public static void UserResetPassword(this ILogger logger, string loginname)
            => _userResetPassword(logger, loginname, null);
    }
}
