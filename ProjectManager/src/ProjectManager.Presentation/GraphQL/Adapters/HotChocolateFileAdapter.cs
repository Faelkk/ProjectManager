using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Types;
using ProjectManager.Application.Common.Interfaces;
using System.IO;

namespace ProjectManager.API.GraphQL.Adapters
{
    public class HotChocolateFileAdapter : IAppFile
    {
        private readonly IFile _file;

        public HotChocolateFileAdapter(IFile file)
        {
            _file = file;
        }

        public string Name => _file.Name;
        public long Length => _file.Length ?? 0;
        public Stream OpenReadStream() => _file.OpenReadStream();
    }
}
