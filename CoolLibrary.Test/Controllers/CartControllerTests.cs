using CoolLibrary.Models;
using CoolLibrary.Models.ViewModels;
using CoolLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System.Security.Claims;

namespace CoolLibrary.Controllers.Tests
{
    public class CartControllerTests
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IBorrowingRepository> _mockBorrowingRepository;
        private readonly Mock<IApplicationUserRepository> _mockApplicationUserRepository;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockBorrowingRepository = new Mock<IBorrowingRepository>();
            _mockApplicationUserRepository = new Mock<IApplicationUserRepository>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockEmailSender = new Mock<IEmailSender>();

            _controller = new CartController(
                _mockBookRepository.Object,
                _mockBorrowingRepository.Object,
                _mockApplicationUserRepository.Object,
                _mockWebHostEnvironment.Object,
                _mockEmailSender.Object
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "test@test.com"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var sessionMock = new Mock<ISession>();
            //sessionMock.Setup(s => s.Get(It.IsAny<string>())).Returns((byte[])null);
            _controller.ControllerContext.HttpContext.Session = sessionMock.Object;
        }

        [Fact]
        public async Task SummaryPost_ShouldCommitTransaction_OnSuccessfulUpdate()
        {
            // Arrange
            var bookList = new List<Book>
            {
                new Book { Id = 1, Title = "Test Book", Author = "Author", PublishedYear = 2020, Quantity = 5 }
            };
            var bookUserVM = new BookUserVM
            {
                BookList = bookList,
                ApplicationUser = new ApplicationUser { Id = "1", Email = "test@test.com" }
            };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockBorrowingRepository.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockBookRepository.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(bookList[0]);

            _mockWebHostEnvironment.Setup(w => w.WebRootPath).Returns(AppContext.BaseDirectory);

            // Act
            var result = await _controller.SummaryPost(bookUserVM);

            // Assert
            _mockBorrowingRepository.Verify(r => r.BeginTransactionAsync(), Times.Once);
            _mockBorrowingRepository.Verify(r => r.AddAsync(It.IsAny<Borrowing>()), Times.Once);
            _mockBorrowingRepository.Verify(r => r.SaveAsync(), Times.Once);
            mockTransaction.Verify(t => t.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task SummaryPost_ShouldRollbackTransaction_OnError()
        {
            // Arrange
            var bookList = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", PublishedYear = 2000, Author = "Author 1", Quantity = 1 },
                new Book { Id = 2, Title = "Book 2", PublishedYear = 2001, Author = "Author 2", Quantity = 1 }
            };

            var bookUserVM = new BookUserVM
            {
                ApplicationUser = new ApplicationUser { Id = "test-user-id", Email = "test@example.com" },
                BookList = bookList
            };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockBorrowingRepository.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockBookRepository.Setup(r => r.FindAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Test Exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.SummaryPost(bookUserVM));

            mockTransaction.Verify(t => t.RollbackAsync(default), Times.Once);
        }
    }
}