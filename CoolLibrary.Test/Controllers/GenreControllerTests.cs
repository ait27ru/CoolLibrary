using CoolLibrary.Models;
using CoolLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CoolLibrary.Controllers.Tests
{
    public class GenreControllerTests
    {
        private readonly Mock<IGenreRepository> _mockGenreRepository;
        private readonly GenreController _controller;
        public GenreControllerTests()
        {
            _mockGenreRepository = new Mock<IGenreRepository>();
            _controller = new GenreController(_mockGenreRepository.Object);
        }

        [Fact]
        public void Index_HasAuthorizeAttribute_WithAdminRole()
        {
            // Arrange
            var controllerType = typeof(GenreController);

            // Act
            var authorizeAttribute = (AuthorizeAttribute?)controllerType
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .FirstOrDefault();

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal(AppGlobals.AdminRole, authorizeAttribute.Roles);
        }

        [Fact()]
        public async Task Index_ReturnsViewResult_WithListOfGenres()
        {
            // Arrange
            _mockGenreRepository.Setup(repo => repo.GetAllAsync(null, null, null, true))
                .ReturnsAsync(GetTestGenres())
                .Verifiable();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Genre>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact()]
        public void Create_ReturnsViewResult_WithEmptyModel()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public void CreatePost_HasHttpPostAttribute()
        {
            // Arrange
            var methodInfo = _controller.GetType().GetMethod("Create", [typeof(Genre)]);

            // Act
            var attributes = methodInfo?.GetCustomAttributes(typeof(HttpPostAttribute), false);

            // Assert
            Assert.True(attributes?.Any(), "Create action does not have HttpPost attribute");
        }

        [Fact()]
        public async Task CreatePost_ReturnsViewWithSameGenre_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Id", "Required");
            var genre = GetTestGenres().First();

            // Act
            var result = await _controller.Create(genre);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Genre>(viewResult.Model);
            Assert.NotNull(model);
            Assert.Equal(1, model.Id);
            Assert.Equal(1, model.DisplayOrder);
            Assert.Equal("Test1", model.Name);
            _mockGenreRepository.Verify(repo => repo.AddAsync(It.IsAny<Genre>()), Times.Never);
            _mockGenreRepository.Verify(repo => repo.SaveAsync(), Times.Never);
        }

        [Fact()]
        public async Task CreatePost_ReturnsRedirectionAndAddsGenre_WhenModelStateIsValid()
        {
            // Arrange
            _mockGenreRepository.Setup(repo => repo.AddAsync(It.IsAny<Genre>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            _mockGenreRepository.Setup(repo => repo.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            var genre = GetTestGenres().First();

            // Act
            var result = await _controller.Create(genre);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockGenreRepository.Verify();
        }

        [Fact()]
        public async Task Edit_ReturnsNotFound_WhenGenreIdIsNull()
        {
            // Arrange
            int? genreId = null;

            // Act
            var result = await _controller.Edit(genreId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGenreRepository.Verify(repo => repo.FindAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact()]
        public async Task Edit_ReturnsNotFound_WhenGenreIdIsZero()
        {
            // Arrange
            int? genreId = 0;

            // Act
            var result = await _controller.Edit(genreId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGenreRepository.Verify(repo => repo.FindAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact()]
        public async Task Edit_ReturnsNotFound_WhenGenreIsNotFoundInDb()
        {
            // Arrange
            Genre? genre = null;
            int genreId = 5;
            _mockGenreRepository.Setup(repo => repo.FindAsync(genreId)).ReturnsAsync(genre);

            // Act
            var result = await _controller.Edit(genreId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGenreRepository.Verify(repo => repo.FindAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact()]
        public async Task Edit_ReturnsViewResult_WhenGenreIsFoundInDb()
        {
            // Arrange
            int genreId = 5;
            _mockGenreRepository.Setup(repo => repo.FindAsync(genreId)).ReturnsAsync(GetTestGenres().First());

            // Act
            var result = await _controller.Edit(genreId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Genre>(viewResult.Model);
            Assert.NotNull(model);
            Assert.Equal(1, model.Id);
            Assert.Equal(1, model.DisplayOrder);
            Assert.Equal("Test1", model.Name);
            _mockGenreRepository.Verify(repo => repo.FindAsync(5), Times.Once);
        }

        [Fact()]
        public async Task DeletePost_ReturnsRedirectionAndDeletesGenre_WhenGenreIsInDb()
        {
            // Arrange
            var genre = GetTestGenres().First();

            _mockGenreRepository.Setup(repo => repo.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(genre)
                .Verifiable();

            _mockGenreRepository.Setup(repo => repo.Remove(genre)).Verifiable();

            _mockGenreRepository.Setup(repo => repo.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();


            // Act
            var result = await _controller.DeletePost(5);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockGenreRepository.Verify();
        }


        private List<Genre> GetTestGenres()
        {
            var genres = new List<Genre>
            {
                new Genre
                {
                    Id = 1,
                    DisplayOrder = 1,
                    Name = "Test1",
                },
                new Genre
                {
                    Id = 2,
                    DisplayOrder = 2,
                    Name = "Test2",
                }
            };
            return genres;
        }
    }
}