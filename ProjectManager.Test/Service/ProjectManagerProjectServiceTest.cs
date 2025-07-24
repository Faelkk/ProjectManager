using Moq;

namespace ProjectManager.Test.Application.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _projectRepoMock;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectService = new ProjectService(_projectRepoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProjects()
        {
            var projects = new List<Project> { new Project { Name = "Test Project" } };
            _projectRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(projects);

            var result = await _projectService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Project", ((List<Project>)result)[0].Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProject()
        {
            var project = new Project { Id = "1", Name = "My Project" };
            _projectRepoMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(project);

            var result = await _projectService.GetByIdAsync("1");

            Assert.NotNull(result);
            Assert.Equal("My Project", result?.Name);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedProject()
        {
            var project = new Project { Name = "New Project" };
            _projectRepoMock.Setup(r => r.CreateAsync(project)).ReturnsAsync(project);

            var result = await _projectService.CreateAsync(project);

            Assert.NotNull(result);
            Assert.Equal("New Project", result.Name);
            _projectRepoMock.Verify(r => r.CreateAsync(project), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedProject()
        {
            var project = new Project { Id = "1", Name = "Updated Project" };
            _projectRepoMock.Setup(r => r.UpdateAsync(project)).ReturnsAsync(project);

            var result = await _projectService.UpdateAsync(project);

            Assert.NotNull(result);
            Assert.Equal("Updated Project", result.Name);
            _projectRepoMock.Verify(r => r.UpdateAsync(project), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryDeleteOnce()
        {
            var projectId = "1";
            _projectRepoMock.Setup(r => r.DeleteAsync(projectId)).Returns(Task.CompletedTask);

            await _projectService.DeleteAsync(projectId);

            _projectRepoMock.Verify(r => r.DeleteAsync(projectId), Times.Once);
        }
    }
}
