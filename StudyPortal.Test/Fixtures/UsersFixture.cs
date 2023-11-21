using StudyPortal.API.Models;

namespace StudyPortal.Test.Fixtures;

public static class UsersFixture
{
    public static List<User> GetTestUsers()
    {
        return new List<User>
        {
            new(
                "Dummy UId1",
                "Dummy Fname",
                "Dummy Lname",
                "Dummy email",
                "Dummy password",
                "user"
            ),

            new(
                "Dummy UId2",
                "Dummy Fname",
                "Dummy Lname",
                "Dummy email",
                "Dummy password",
                "user"
            ),

            new(
                "Dummy UId3",
                "Dummy Fname",
                "Dummy Lname",
                "Dummy email",
                "Dummy password",
                "user"
            )
        };
    }
}