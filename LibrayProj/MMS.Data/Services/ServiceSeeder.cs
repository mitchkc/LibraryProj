using MMS.Data.Entities;
namespace MMS.Data.Services;

public static class ServiceSeeder
{

    // default seeder using Db versions of services
    public static void Seed()
    {
        IUserService usvc = new UserServiceDb();
        IBookService rsvc = new BookServiceDb();
       
        usvc.Initialise();

        SeedUsers(usvc);
        SeedBooks(rsvc);
    }

    // use this method FIRST to seed the database with dummy test data using an IUserService
    private static void SeedUsers(IUserService svc)
    {
        // Note: do not call initialise here

        svc.Register("admin","admin@mail.com","password",Role.admin);
       
    }
    
    // use this method SECOND to seed the database with dummy test data using an IRecipeService
    private static void SeedBooks(IBookService svc)
    {        
        // Note: do not call initialise here

        // add relevant book seed data 


    }
}

