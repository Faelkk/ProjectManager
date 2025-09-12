namespace ProjectManager.Application.Common.Interfaces
{
    public interface IAppFile
    {
        string Name { get; }
        long Length { get; }
        Stream OpenReadStream();
    }
}