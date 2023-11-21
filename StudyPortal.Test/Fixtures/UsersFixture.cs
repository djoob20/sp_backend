using System;
using StudyPortal.API.Models;

namespace StudyPortal.Test.Fixtures
{
    public static class UsersFixture
    {
        public static List<User> GetTestUsers()
        {
            return new List<User>()
            {
                new (
                    id:"Dummy UId1",
                    firstname:"Dummy Fname",
                    lastname:"Dummy Lname",
                    email: "Dummy email",
                    password:"Dummy password",
                    role:"user"
                    ),

                new (
                    id:"Dummy UId2",
                    firstname:"Dummy Fname",
                    lastname:"Dummy Lname",
                    email: "Dummy email",
                    password:"Dummy password",
                    role:"user"
                    ),

                new (
                    id:"Dummy UId3",
                    firstname:"Dummy Fname",
                    lastname:"Dummy Lname",
                    email: "Dummy email",
                    password:"Dummy password",
                    role:"user"
                    )
            };

        }

    }
}

