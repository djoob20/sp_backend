using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyPortal.API.Controllers;
using StudyPortal.API.Services;
using StudyPortal.API.Models;
using StudyPortal.Test.Fixtures;

namespace StudyPortal.Test.Controllers
{
    public class CourseControllerTest
    {
        //################## TEST GET METHOD ###################
        [Fact]
        public async Task Test_GetCourses_OnSuccess_Should_Return_StatusCode_200()
        {
            //Arrange
            var courseServiceMock = new Mock<ISubjectsService<Course>>();
            courseServiceMock.Setup(service => service.GetAsync()).ReturnsAsync(SubjectsFixture.GetTestCourses());

            var controller = new CourseController(courseServiceMock.Object);

            //Act
            var result = await controller.GetAllCourses();

            //Verify
            result.Should().BeOfType<OkObjectResult>();
            var objectResult = (OkObjectResult) result;
            objectResult.StatusCode.Should().Be( 200 );
        }

        [Fact]
        public async Task Test_GetCourses_OnNotFound_Should_Return_StatusCode_404()
        {
            //Arrange
            var courseServiceMock = new Mock<ISubjectsService<Course>>();
            courseServiceMock.Setup( service => service.GetAsync() ).ReturnsAsync( new List<Course>() );

            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.GetAllCourses();

            //Verify
            result.Should().BeOfType<NotFoundResult>();
            var objectResult = (NotFoundResult) result;
            objectResult.StatusCode.Should().Be( 404 );
        }

        [Fact]
        public async Task Get_OnSuccess_Invokes_UsersService_Returns_Exactly_Once()
        {
            //Arrange
            var courseServiceMock = new Mock<ISubjectsService<Course>>();
            courseServiceMock.Setup( service => service.GetAsync() ).ReturnsAsync( new List<Course>() );

            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.GetAllCourses();

            //Verify
            courseServiceMock.Verify( s => s.GetAsync(), Times.Once );
        }


        //################## TEST GET METHOD WITH ID ###################
        [Fact]
        public async Task Test_GetCourseById_OnSuccess_Should_Return_One_Course()
        {
            //Arrange
            string? courseId = SubjectsFixture.GetTestCourses().First().Id;
            var expectedCourse = SubjectsFixture.GetTestCourses().First();
            var courseServiceMock = new Mock<ISubjectsService<Course>>();
            courseServiceMock.Setup( service => service.GetAsync( courseId == null? "": courseId ) ).ReturnsAsync( expectedCourse );

            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.GetCourseById( courseId == null ? "" : courseId );

            //Verify
            result.Should().BeOfType<OkObjectResult>();
            var objectResult = (OkObjectResult) result;
            objectResult.StatusCode.Should().Be( 200 );

        }

        [Fact]
        public async Task Test_GetCourseById_OnNotFound_Should_Return_StatusCode_404()
        {
            //Arrange
            var courseServiceMock = new Mock<ISubjectsService<Course>>();

            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.GetCourseById("dummyId");

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
            var courseServiceMock = new Mock<ISubjectsService<Course>>();

            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.CreateCourse( SubjectsFixture.GetTestCourses().First() );

            //Verify
            result.Should().BeOfType<CreatedAtActionResult>();
            var objectResult = (CreatedAtActionResult) result;
            objectResult.StatusCode.Should().Be( 201 );
        }

        //################## TEST UPDATE METHOD ###################

        [Fact]
        public async Task Test_UpdateCourse_OnSuccess_Returns_StatusCode_204()
        {
            //Arrange
            string? courseId = SubjectsFixture.GetTestCourses().First().Id;
            var courseToUpdate = SubjectsFixture.GetTestCourses().First();
            var courseServiceMock = new Mock<ISubjectsService<Course>>();

            courseServiceMock.Setup( service => service.GetAsync( courseId == null ? "" : courseId ) ).ReturnsAsync( courseToUpdate );

            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.UpdateCourse( courseId == null ? "" : courseId, SubjectsFixture.GetTestCourses().First() );

            //Verify
            result.Should().BeOfType<NoContentResult>();
            var objectResult = (NoContentResult) result;
            objectResult.StatusCode.Should().Be( 204 );
        }

        [Fact]
        public async Task Test_UpdateCourse_NotFound_Returns_StatusCode_404()
        {
            //Arrange
            string? courseId = SubjectsFixture.GetTestCourses().First().Id;
            var courseServiceMock = new Mock<ISubjectsService<Course>>();


            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.UpdateCourse( courseId == null ? "" : courseId, SubjectsFixture.GetTestCourses().First() );

            //Verify
            result.Should().BeOfType<NotFoundObjectResult>();
            var objectResult = (NotFoundObjectResult) result;
            objectResult.StatusCode.Should().Be( 404 );
        }
        [Fact]
        public async Task Test_UpdateCourse_WithBadId_Returns_StatusCode_400()
        {
            //Arrange
            string? courseId = "bad-id";
            var courseServiceMock = new Mock<ISubjectsService<Course>>();

            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.UpdateCourse( courseId, SubjectsFixture.GetTestCourses().First() );

            //Verify
            result.Should().BeOfType<BadRequestObjectResult>();
            var objectResult = (BadRequestObjectResult) result;
            objectResult.StatusCode.Should().Be( 400 );
        }

        //################## TEST DELETE METHOD ###################
        [Fact]
        public async Task Test_DeleteCourse_OnSuccess_Returns_StatusCode_204()
        {
            //Arrange
            string? courseId = SubjectsFixture.GetTestCourses().First().Id;
            var courseToDelete = SubjectsFixture.GetTestCourses().First();
            var courseServiceMock = new Mock<ISubjectsService<Course>>();

            courseServiceMock.Setup( service => service.GetAsync( courseId == null ? "" : courseId ) ).ReturnsAsync( courseToDelete );

            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.DeleteCourse( courseId == null ? "" : courseId );

            //Verify
            result.Should().BeOfType<NoContentResult>();
            var objectResult = (NoContentResult) result;
            objectResult.StatusCode.Should().Be( 204 );
        }

        [Fact]
        public async Task Test_DeleteCourse_OnNotFound_Returns_StatusCode_404()
        {
            //Arrange
            var courseServiceMock = new Mock<ISubjectsService<Course>>();
            var controller = new CourseController( courseServiceMock.Object );

            //Act
            var result = await controller.DeleteCourse( "dummy_id" );

            //Verify
            result.Should().BeOfType<NotFoundObjectResult>();
            var objectResult = (NotFoundObjectResult) result;
            objectResult.StatusCode.Should().Be( 404 );
        }

        
    }
}

