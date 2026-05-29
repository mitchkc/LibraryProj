
using Bogus;
using LMS.Data.Entities;

namespace LMS.Data.Services
{
    public static class Seeder
    {
        // use this class to seed the database with dummy test data using an IUserService 
        public static void Seed(IUserService svc)
        {
            // seeder destroys and recreates the database - NOT to be called in production!!!
            svc.Initialise();

            // add users
            svc.AddUser("Admin", "Istrator", "adminPWRD", Role.admin, new DateOnly(2021,1,1), "admin@mail.com", "1 Address, BT10", "Male", new DateTime(1980,1,1), "07711111111");
            svc.AddUser("Staff", "Ffats", "stafPWRD", Role.staff, new DateOnly(2022,2,2), "staff@mail.com", "2 Address, BT20", "Female", new DateTime(1990,2,2), "07722222222");
            svc.AddUser("Member", "Rebmem", "memberPWRD", Role.member, new DateOnly(2023,3,3), "member@mail.com", "3 Address, BT30", "Female", new DateTime(1993,3,3), "07733333333"); 

            // Add books

            // Add book loans attached to member




            // optionally add some fake users
            // var faker = new Faker();
            // for(int i=1; i<=20; i++)
            // {
            //     var s = svc.AddUser(
            //         faker.Name.FullName(),
            //         faker.Internet.Email(),
            //         "password",
            //         Role.guest
            //     );
            // }
        }
    }

}