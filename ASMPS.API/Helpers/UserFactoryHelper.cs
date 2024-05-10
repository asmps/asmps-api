using ASMPS.Models;

namespace ASMPS.API.Helpers;

public static class UserFactoryHelper
{
    public static User CreateUser(UserRoles role)
    {
        switch (role)
        {
            case UserRoles.Student:
                return new Student();
            case UserRoles.Teacher:
                return new Teacher();
            case UserRoles.Deanery:
                return new Deanery();
            default:
                throw new ArgumentException("Invalid role specified");
        }
    }
}