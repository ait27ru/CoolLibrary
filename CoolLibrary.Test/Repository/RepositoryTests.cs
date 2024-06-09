using CoolLibrary.Data;
using CoolLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace CoolLibrary.Repository.Tests
{
    public class RepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<Genre>> _mockDbSet;
        private readonly Repository<Genre> _repository;
        private readonly Mock<DatabaseFacade> _mockDbFacade;

        public RepositoryTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockDbFacade = new Mock<DatabaseFacade>(_mockContext.Object);
            _mockDbSet = new Mock<DbSet<Genre>>();
            _mockContext.Setup(m => m.Set<Genre>()).Returns(_mockDbSet.Object);
            _mockContext.Setup(m => m.Database).Returns(_mockDbFacade.Object);
            _repository = new Repository<Genre>(_mockContext.Object);
        }

        [Fact]
        public async Task BeginTransactionAsync_ShouldBeginTransaction()
        {
            // Arrange
            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockContext.Setup(m => m.Database.BeginTransactionAsync(default)).ReturnsAsync(mockTransaction.Object);

            // Act
            var result = await _repository.BeginTransactionAsync();

            // Assert
            _mockContext.Verify(m => m.Database.BeginTransactionAsync(default), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity()
        {
            // Arrange
            var testGenre = GetTestGenres().First();

            // Act
            await _repository.AddAsync(testGenre);

            // Assert
            _mockDbSet.Verify(m => m.AddAsync(testGenre, default), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldFindEntityById()
        {
            // Arrange
            var testGenre = GetTestGenres().First();
            _mockDbSet.Setup(m => m.FindAsync(1)).ReturnsAsync(testGenre);

            // Act
            var result = await _repository.FindAsync(1);

            // Assert
            _mockDbSet.Verify(m => m.FindAsync(1), Times.Once);
            Assert.Equal(testGenre, result);
        }


        [Fact]
        public void Remove_ShouldRemoveEntity()
        {
            // Arrange
            var genre = GetTestGenres().First();

            // Act
            _repository.Remove(genre);

            // Assert
            _mockDbSet.Verify(m => m.Remove(genre), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ShouldSaveChanges()
        {
            // Act
            await _repository.SaveAsync();

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
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