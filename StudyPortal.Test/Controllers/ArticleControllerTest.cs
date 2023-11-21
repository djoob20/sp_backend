using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyPortal.API.Controllers;
using StudyPortal.API.Models;
using StudyPortal.API.Services;
using StudyPortal.Test.Fixtures;

namespace StudyPortal.Test.Controllers
{
    public class ArticleControllerTest
    {
  

        //################## TEST GET METHOD ###################
        [Fact]
        public async Task Test_GetAllArticles_OnSuccess_Should_Return_StatusCode_200()
        {
            //Arrange
            var articleServiceMock = new Mock<ISubjectsService<Article>>();
            articleServiceMock.Setup( service => service.GetAsync() ).ReturnsAsync( SubjectsFixture.GetTestArticles() );

            var controller = new ArticleController( articleServiceMock.Object);

            //Act
            var result = await controller.GetAllArticles();

            //Verify
            result.Should().BeOfType<OkObjectResult>();
            var objectResult = (OkObjectResult) result;
            objectResult.StatusCode.Should().Be( 200 );
        }
    }
}

