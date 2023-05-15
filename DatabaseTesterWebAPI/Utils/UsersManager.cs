using DatabaseTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.Utils
{
    public class UsersManager
    {
        public static User GetDummyUser()
        {
            return new User()
            {
                FirstName = DataUtils.GetRandomString(10),
                LastName = DataUtils.GetRandomString(15),
                UserName = DataUtils.GetRandomString(15),
                Email = $"{DataUtils.GetRandomString(15)}.test.com",
                Password = DataUtils.GetRandomString(16),
                Address = new Address()
                {
                    StreetName = DataUtils.GetRandomString(10),
                    ApartmentNumber = DataUtils.GetRandomString(5),
                    BuildingNumber = DataUtils.GetRandomString(5),
                    City = DataUtils.GetRandomString(12),
                    PostalCode = DataUtils.GetRandomString(6),
                    Country = DataUtils.GetRandomString(15)
                }
            };
        }

        public static List<User> GetUsers(int usersCount)
        {
            var users = new List<User>();
            for (int i = 1; i <= usersCount; i++)
            {
                var user = GetDummyUser();
                users.Add(user);
            }
            return users;
        }
    }
}
