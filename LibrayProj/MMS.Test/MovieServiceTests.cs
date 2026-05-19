
using System;
using System.Linq;
using Xunit;

using MMS.Data.Services;
using MMS.Data.Entities;

namespace MMS.Test;
   
// ==================== BookService Tests =============================
[Collection("Sequential")]
public class BookServiceTests
{
    private readonly IBookService svc;

    public BookServiceTests()
    {
        // general arrangement
        svc = new BookServiceDb();
        
        // ensure data source is empty before each test
        svc.Initialise();
    }

    // ========================== TBC Recipe Tests  =========================
    
    
}

// ==================== UserService Tests =============================
[Collection("Sequential")]
public class UserServiceTests
{
    private readonly IUserService svc;

    public UserServiceTests()
    {
        // general arrangement
        svc = new UserServiceDb();
        
        // ensure data source is empty before each test
        svc.Initialise();
    }

    // ========================== User Tests =========================

    [Fact] // --- Register Valid User test
    public void User_Register_WhenValid_ShouldReturnUser()
    {
        // arrange 
        var reg = svc.Register("XXX", "xxx@email.com", "admin", Role.admin);
        
        // act
        var user = svc.GetUserByEmail(reg.Email);
        
        // assert
        Assert.NotNull(reg);
        Assert.NotNull(user);
    } 

    [Fact] // --- Register Duplicate Test
    public void User_Register_WhenDuplicateEmail_ShouldReturnNull()
    {
        // arrange 
        var s1 = svc.Register("XXX", "xxx@email.com", "admin", Role.admin);
        
        // act
        var s2 = svc.Register("XXX", "xxx@email.com", "admin", Role.admin);

        // assert
        Assert.NotNull(s1);
        Assert.Null(s2);
    } 

    [Fact] // --- Authenticate Invalid Test
    public void User_Authenticate_WhenInValidCredentials_ShouldReturnNull()
    {
        // arrange 
        var s1 = svc.Register("XXX", "xxx@email.com", "admin", Role.admin);
    
        // act
        var user = svc.Authenticate("xxx@email.com", "guest");
        // assert
        Assert.Null(user);

    } 

    [Fact] // --- Authenticate Valid Test
    public void User_Authenticate_WhenValidCredentials_ShouldReturnUser()
    {
        // arrange 
        var s1 = svc.Register("XXX", "xxx@email.com", "admin", Role.admin);
    
        // act
        var user = svc.Authenticate("xxx@email.com", "admin");
        
        // assert
        Assert.NotNull(user);
    } 
 
}


