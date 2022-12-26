using System;
using System.Collections.Generic;
using System.Text;

namespace ATA.Authentication
{
    internal sealed class Resources
    {
        public class Messages
        {
            public const string ConnectionStringNameNotProvided = "You must specify the connectionStringName in the web.config file for the ATAProfileProvider.";
            public const string ConnectionStringNotFound = "The connection string with the specified name was not found.";
            public const string ProviderApplicationNameTooLong = "The application name is too long, maximum length is 256 characters.";
            public const string ProviderCannotDecodeHashedPasswords = "The provider cannot recover hashed passwords.";
        }
    }
}
