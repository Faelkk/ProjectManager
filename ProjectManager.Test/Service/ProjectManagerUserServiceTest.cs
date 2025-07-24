using Moq;

public class ProjectManagerUserServiceTest
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();

            _userService = new UserService(
                _userRepoMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUsers()
        {
            // Arrange
            var users = new List<User> { new() { Email = "test@test.com" } };
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("test@test.com", result.First().Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var user = new User { Id = "123", Email = "user@test.com" };
            _userRepoMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync("123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user@test.com", result.Email);
        }

        [Fact]
        public async Task CreateAsync_WithValidUser_ShouldReturnToken()
        {
            // Arrange
            var user = new User { Email = "new@user.com", Password = "123" };

            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null); // usuário ainda não existe

            _passwordHasherMock
                .Setup(h => h.HashPassword(user, user.Password))
                .Returns("hashed123");

            _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(user);

            _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("mocked-token");

            // Act
            var token = await _userService.CreateAsync(user);

            // Assert
            Assert.Equal("mocked-token", token);
            _userRepoMock.Verify(
                r => r.CreateAsync(It.Is<User>(u => u.Password == "hashed123")),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAsync_WhenEmailExists_ShouldThrow()
        {
            // Arrange
            var existingUser = new User { Email = "exists@user.com" };
            _userRepoMock
                .Setup(r => r.GetByEmailAsync(existingUser.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.CreateAsync(existingUser)
            );
        }

        [Fact]
        public async Task LoginAsync_WithCorrectCredentials_ShouldReturnToken()
        {
            var user = new User { Email = "test@test.com", Password = "hashed" };
            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _passwordHasherMock
                .Setup(p => p.VerifyPassword(user, user.Password, "plain"))
                .Returns(true);
            _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

            var token = await _userService.LoginAsync(user.Email, "plain");

            Assert.Equal("jwt-token", token);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ShouldThrow()
        {
            var user = new User { Email = "test@test.com", Password = "hashed" };
            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _passwordHasherMock
                .Setup(p => p.VerifyPassword(user, user.Password, "wrong"))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.LoginAsync(user.Email, "wrong")
            );
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedUser()
        {
            // Arrange
            var user = new User
            {
                Id = "abc123",
                Username = "updatedUser",
                Email = "updated@test.com",
                Password = "newpass",
                Role = "User",
            };

            _userRepoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(user);

            // Act
            var result = await _userService.UpdateAsync(user);

            // Assert
            Assert.Equal("updatedUser", result.Username);
            Assert.Equal("updated@test.com", result.Email);
            _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryDeleteOnce()
        {
            // Arrange
            var userId = "abc123";

            _userRepoMock.Setup(r => r.DeleteAsync(userId)).Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteAsync(userId);

            // Assert
            _userRepoMock.Verify(r => r.DeleteAsync(userId), Times.Once);
        }
    }
}
