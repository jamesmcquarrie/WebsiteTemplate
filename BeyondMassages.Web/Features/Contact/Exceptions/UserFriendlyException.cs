﻿namespace BeyondMassages.Web.Features.Contact.Exceptions;

[Serializable]
public class UserFriendlyException : Exception
{
    public UserFriendlyException(string message) : base(message) { }
}