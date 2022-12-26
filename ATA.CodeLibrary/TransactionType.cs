using System;

namespace ATA.ObjectModel
{
    public enum TransactionType : int
    {
        GroupCreated = 1,
        GroupModified = 2,
        GroupDeleted = 3,
        UserCreated = 4,
        UserModified = 5,
        UserDeleted = 6,
        UserGroup_UserAdded = 7,
        UserGroup_UserRemoved = 8,
        UserEmail_Changed = 9,
        UserCompany_Changed = 10,
        A4A_Portal = 11,
        IsActiveContact = 12,
        LastLogin = 13,
        EmailSent = 14
    }
}
