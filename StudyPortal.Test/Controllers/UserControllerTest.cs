using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyPortal.API.Controllers;
using StudyPortal.API.Models;
using StudyPortal.API.Services;
using StudyPortal.Test.Fixtures;

namespace StudyPortal.Test.Controllers
{
    public class UserControllerTest
    {
        //################## TEST GET METHOD ###################
        [Fact]
        public async Task Test_GetAllUsers_OnSuccess_Should_Return_StatusCode_200()
        {
            //Arrange
            var userService = new Mock<IUserService>();
            userService.Setup( s => s.GetAsync() ).ReturnsAsync( UsersFixture.GetTestUsers() );
            var controller = new UserController( userService.Object );

            //Act
            var result = await controller.GetAllUsers();
            //Verify

            result.Should().BeOfType<OkObjectResult>();
            var okObjResult = (OkObjectResult) result;
            okObjResult.StatusCode.Should().Be( 200 );



        }

        [Fact]
        public async Task Test_GetAllUsers_OnNotFound_Should_Return_StatusCode_404()
        {
            //Arrange
            var userService = new Mock<IUserService>();
            userService.Setup( s => s.GetAsync() ).ReturnsAsync( new List<User> () );
            var controller = new UserController( userService.Object );

            //Act
            var result = await controller.GetAllUsers();

            //Verify
            result.Should().BeOfType<NotFoundResult>();
            var okObjResult = (NotFoundResult) result;
            okObjResult.StatusCode.Should().Be( 404 );

        }

        //################## TEST GET METHOD WITH ID ###################
        [Fact]
        public async Task Test_GetUserById_OnSuccess_Should_Return_One_Course()
        {
            //Arrange
            var userId = UsersFixture.GetTestUsers().ElementAt( 0 ).Id;
            var userService = new Mock<IUserService>();
            userService.Setup( s => s.GetAsync( userId ) ).ReturnsAsync( UsersFixture.GetTestUsers().ElementAt( 0 ) );
            var controller = new UserController( userService.Object );

            //Act
            var result = await controller.GetUserById( userId );

            //verify
            result.Should().BeOfType<OkObjectResult>();
            var okObjResult = (OkObjectResult) result;
            okObjResult.StatusCode.Should().Be( 200 );
        }

        [Fact]
        public async Task Test_GetUserById_OnNotFound_Should_Return_StatusCode_404()
        {
            //Arrange
            var userService = new Mock<IUserService>();

            var controller = new UserController( userService.Object );

            //Act
            var result = await controller.GetUserById( "dummy id" );

            //Verify
            result.Should().BeOfType<NotFoundObjectResult>();
            var objectResult = (NotFoundObjectResult) result;
            objectResult.StatusCode.Should().Be( 404 );

        }

        //################## TEST POST METHOD ###################
        [Fact]
        public async Task Test_CreateCourse_OnSuccess_Returns_StatusCode_201()
        {
            //Arrange
            var userService = new Mock<IUserService>();

            var controller = new UserController( userService.Object );

            //Act
            var result = await controller.CreateUser( UsersFixture.GetTestUsers().First());

            //Verify
            result.Should().BeOfType<CreatedAtActionResult>();
            var objectResult = (CreatedAtActionResult) result;
            objectResult.StatusCode.Should().Be( 201 );
        }
    }
}

